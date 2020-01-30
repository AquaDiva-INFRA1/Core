using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BExIS.Dlm.Services.Data;
using System.Configuration;
using BExIS.Dlm.Services.DataStructure;
using BExIS.Dlm.Entities.DataStructure;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Xml;
using System.Net;
using System.IO;
using BExIS.Modules.OAC.UI.Models;
using BExIS.Dlm.Services.Administration;
using BExIS.Xml.Helpers;
using BExIS.Dlm.Entities.Administration;
using BExIS.Dlm.Services.MetadataStructure;
using BExIS.Dlm.Entities.MetadataStructure;
using BExIS.Security.Services.Authorization;
using BExIS.Security.Entities.Subjects;
using BExIS.Dlm.Entities.Data;
using BExIS.Security.Entities.Authorization;
using BExIS.Security.Services.Subjects;
using System.Xml.Linq;
using Vaiona.Web.Mvc.Modularity;
using System.Web.Routing;
using Vaiona.Logging;
using BExIS.Security.Services.Utilities;
using Vaiona.Utils.Cfg;
using BExIS.Xml.Helpers.Mapping;
using System.Diagnostics;

namespace BExIS.Modules.OAC.UI.Controllers
{
    public class SequenceDataUploadController : Controller
    {
        static string JsonResult = "";
        static EBIresponseModel EBIresponseModel = new EBIresponseModel();
        static string ebiSampleAPIurl = "https://www.ebi.ac.uk/biosamples/api/samples/";
        public ActionResult Index(string sampleID)
        {
            if (sampleID != null)
            {
                sampleID = sampleID.Replace("\"", "");
                var request = (HttpWebRequest)WebRequest.Create(ebiSampleAPIurl + sampleID);

                var response = (HttpWebResponse)request.GetResponse();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                JObject json = JObject.Parse(responseString);
                JsonResult = responseString.ToString();

                EBIresponseModel = new EBIresponseModel(json);
                return View("Index", EBIresponseModel);
            }

            return View("Index");
        }

        public ActionResult getMetadataFromSampleID(string sampleID)
        {
            sampleID = sampleID.Replace("\"", "");
            var request = (HttpWebRequest)WebRequest.Create(ebiSampleAPIurl + sampleID);

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            JObject json = JObject.Parse(responseString);

            EBIresponseModel = new EBIresponseModel(json);

            var oMycustomclassname = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseString);

            return View(EBIresponseModel);
        }

        public string importMetadataFromSample()
        {
            DataStructureManager dsm = new DataStructureManager();
            DatasetManager dm = new DatasetManager();
            XmlDocument MetadataDoc = new XmlDocument();
            Dataset ds = new Dataset();
            XmlDatasetHelper xmlDatasetHelper = new XmlDatasetHelper();

            try
            {
                //UnStructuredDataStructure ds_struct = (UnStructuredDataStructure) dsm.UnStructuredDataStructureRepo.Get().FirstOrDefault(x => x.Name == "none");
                DataStructure dataStruct = (DataStructure)dsm.AllTypesDataStructureRepo.Get().FirstOrDefault(x => x.Name == "none");

                ResearchPlanManager rpm = new ResearchPlanManager();
                ResearchPlan rp = rpm.Repo.Get().First();


                xmlDatasetHelper = new XmlDatasetHelper();
                MetadataStructureManager msm = new MetadataStructureManager();
                MetadataStructure metadataStructure = msm.Repo.Get().FirstOrDefault(x => x.Name.ToLower() == "ebi");

                //XmlDocument doc = new XmlDocument();
                //XmlNode Node = doc.CreateElement("root");
                //XmlNode Node_ = doc.CreateElement("accession");
                //Node_.InnerText = EBIresponseModel.accession;
                //Node.AppendChild(Node_);
                //doc.AppendChild(Node);
                //XmlDocument MetadataDoc = LoadFromXml_external(doc, metadataStructure);
                MetadataDoc = LoadFromXml_external(EBIresponseModel.ConvertToXML(JsonResult), metadataStructure);
                //XmlDocument MetadataDoc = EBIresponseModel.ConvertToXML(JsonResult);

                ds = dm.CreateEmptyDataset(dataStruct, rp, metadataStructure);
            }
            catch (Exception ex)
            {
                return ("error in metadata structure : " + ex.ToString());
            }

            try
            {
                if (GetUsernameOrDefault() != "DEFAULT")
                {
                    EntityPermissionManager entityPermissionManager = new EntityPermissionManager();
                    //Full permissions for the user
                    entityPermissionManager.Create<User>(GetUsernameOrDefault(), "Dataset", typeof(Dataset), ds.Id, Enum.GetValues(typeof(RightType)).Cast<RightType>().ToList());

                    #region Aquadiva: permissions for PIs
                    UserPiManager upm = new UserPiManager();

                    //Get PIs of the current user
                    List<User> piList = upm.GetPisFromUserByName(GetUsernameOrDefault()).ToList();
                    foreach (User pi in piList)
                    {
                        //Full permissions for the pis
                        entityPermissionManager.Create<User>(pi.Name, "Dataset", typeof(Dataset), ds.Id, Enum.GetValues(typeof(RightType)).Cast<RightType>().ToList());

                        //Get all users with the same pi
                        List<User> piMembers = upm.GetAllPiMembers(pi.Id).ToList();
                        //Give view and download rights to the members
                        foreach (User piMember in piMembers)
                        {
                            entityPermissionManager.Create<User>(piMember.Name, "Dataset", typeof(Dataset), ds.Id, new List<RightType> {
                                        RightType.Read,
                                        RightType.Download
                                    });
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                return ("error in Entity Permission Manager structure : " + ex.ToString());
            }

            try
            {

                if (dm.IsDatasetCheckedOutFor(ds.Id, GetUsernameOrDefault()) || dm.CheckOutDataset(ds.Id, GetUsernameOrDefault()))
                {
                    DatasetVersion workingCopy = dm.GetDatasetWorkingCopy(ds.Id);

                    if (MetadataDoc.OuterXml != null)
                    {
                        XDocument xMetadata = XDocument.Parse(MetadataDoc.OuterXml);
                        workingCopy.Metadata = Xml.Helpers.XmlWriter.ToXmlDocument(xMetadata);
                    }

                    //set status

                    if (workingCopy.StateInfo == null) workingCopy.StateInfo = new Vaiona.Entities.Common.EntityStateInfo();

                    workingCopy.StateInfo.State = DatasetStateInfo.Valid.ToString();

                    string title = xmlDatasetHelper.GetInformationFromVersion(workingCopy.Id, NameAttributeValues.title);
                    if (string.IsNullOrEmpty(title)) title = "No Title available.";

                    dm.EditDatasetVersion(workingCopy, null, null, null);
                    dm.CheckInDataset(ds.Id, "Metadata was submited.", GetUsernameOrDefault(), ViewCreationBehavior.None);
                    try
                    {
                        var x = this.Run("DDM", "SearchIndex", "ReIndexSingle", new RouteValueDictionary() { { "id", ds.Id } });
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }

                    LoggerFactory.LogData(ds.Id.ToString(), typeof(Dataset).Name, Vaiona.Entities.Logging.CrudState.Created);

                    var es = new EmailService();
                    es.Send(MessageHelper.GetCreateDatasetHeader(),
                        MessageHelper.GetCreateDatasetMessage(ds.Id, title, GetUsernameOrDefault()),
                        ConfigurationManager.AppSettings["SystemEmail"]
                        );
                }
                return ds.Id.ToString();

            }
            catch (Exception ex)
            {
                return ("error in re indexing : " + ex.ToString());
            }

        }

        public string GetUsernameOrDefault()
        {
            string username = string.Empty;
            try
            {
                username = HttpContext.User.Identity.Name;
            }
            catch { }

            return !string.IsNullOrWhiteSpace(username) ? username : "DEFAULT";
        }


        public XmlDocument LoadFromXml_external(XmlDocument metadataForImport, MetadataStructure MetadataStructure)
        {
            if ((metadataForImport != null) && (MetadataStructure != null))
            {
                // metadataStructure ID
                var metadataStructureId = MetadataStructure.Id;
                var metadataStructrueName = MetadataStructure.Name;

                // loadMapping file
                var path_mappingFile = Path.Combine(AppConfiguration.GetModuleWorkspacePath("DIM"), XmlMetadataImportHelper.GetMappingFileName(metadataStructureId, TransmissionType.mappingFileImport, metadataStructrueName));

                // XML mapper + mapping file
                var xmlMapperManager = new XmlMapperManager(TransactionDirection.ExternToIntern);
                xmlMapperManager.Load(path_mappingFile, "ebi");

                // generate intern metadata without internal attributes
                var metadataResult = xmlMapperManager.Generate(metadataForImport, 1, true);

                // generate intern template metadata xml with needed attribtes
                var xmlMetadatWriter = new XmlMetadataWriter(BExIS.Xml.Helpers.XmlNodeMode.xPath);
                var metadataXml = xmlMetadatWriter.CreateMetadataXml(metadataStructureId,
                    XmlUtility.ToXDocument(metadataResult));

                var metadataXmlTemplate = XmlMetadataWriter.ToXmlDocument(metadataXml);

                // set attributes FROM metadataXmlTemplate TO metadataResult
                var completeMetadata = XmlMetadataImportHelper.FillInXmlValues(metadataResult,
                    metadataXmlTemplate);

                return completeMetadata;


            }
            return null;
        }


    }

}