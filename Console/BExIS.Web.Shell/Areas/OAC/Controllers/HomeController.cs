using System;
using BExIS.Modules.OAC.UI.Models;
using System.Collections.Generic;
using System.IO;
using BExIS.Xml.Helpers;
using BExIS.Xml.Helpers.Mapping;
using System.Xml;
using BExIS.Modules.Dcm.UI.Controllers;
using BExIS.Modules.Dcm.UI.Models;
using BExIS.Dcm.CreateDatasetWizard;
using BExIS.Dcm.Wizard;
using System.Web.Routing;
using System.Net;
using System.Xml.Linq;
using System.Web.Script.Serialization;
using System.Linq;
using Newtonsoft.Json.Linq;
using BExIS.Dlm.Services.Data;
using BExIS.Dlm.Services.DataStructure;
using BExIS.Dlm.Entities.Data;
using BExIS.Dlm.Entities.DataStructure;
using BExIS.Dlm.Services.Administration;
using BExIS.Dlm.Services.MetadataStructure;
using BExIS.Dlm.Entities.Administration;
using BExIS.Dlm.Entities.MetadataStructure;
using BExIS.Security.Services.Authorization;
using BExIS.Security.Entities.Subjects;
using BExIS.Security.Entities.Authorization;
using BExIS.Security.Services.Subjects;
using System.Diagnostics;
using BExIS.IO.Transform.Input;
using System.Web.Configuration;
using BExIS.IO;
using Newtonsoft.Json;
using Vaiona.Persistence.Api;
using Vaiona.Utils.Cfg;
using System.Web.Mvc;

using Vaiona.Web.Mvc.Modularity;
using Vaiona.Logging;
using BExIS.Security.Services.Utilities;
using System.Configuration;
using System.Text;
using BExIS.IO.Transform.Validation.Exceptions;
using Vaiona.Entities.Common;

namespace BExIS.Modules.OAC.UI.Controllers
{
    /// <summary> 
    /// this module and controller allows users to import metadata from APIs, by following the steps:
    /// 1. ask for Metadata Schema + Data Schema + Accession
    /// 2. import data
    /// 3. map the data using XML
    /// 4. redirect the user to the import page
    /// </summary>
    public class HomeController : Controller
    {
        static SelectedImportOptionsModel model;
        //static string EBI_study_accession = "https://www.ebi.ac.uk/ena/data/warehouse/filereport?accession=PRJEB25133&result=read_run&fields=study_accession,sample_accession,secondary_sample_accession,experiment_accession,run_accession,tax_id,scientific_name,instrument_model,library_layout,fastq_ftp,fastq_galaxy,submitted_ftp,submitted_galaxy,sra_ftp,sra_galaxy,cram_index_ftp,cram_index_galaxy&download=txt"
        static string EBI_study_accession = "https://www.ebi.ac.uk/ena/data/warehouse/filereport?result=read_run&fields=sample_accession,study_accession&accession=";
        static string EBI_sample_Accession = "https://www.ebi.ac.uk/biosamples/api/samples/";
        static string EBI_study_metadata = "https://www.ebi.ac.uk/ena/data/view/";
        static string temp_file_to_save_json_as_csv = Path.GetFullPath(WebConfigurationManager.AppSettings["output_Folder"]);
        public enum DataSource : long
        {
            BioGPS = 1,
            EBI = 2, NCBI = 3 // the same in our examples
        }



        public ActionResult Index()
        {
            CreateDatasetController HelperController = new CreateDatasetController();

            model = new SelectedImportOptionsModel()
            {
                MetadataStructureViewList = HelperController.LoadMetadataStructureViewList(),
                DataStructureViewList = HelperController.LoadDataStructureViewList(),
                DataSourceViewList = GetDataSourceList(),
                Accessions = new Dictionary<string, string>()
            };

            HelperController.Dispose();

            return View(model);
        }

        public ActionResult FetchDataFromPortal()
        {
            if (model != null)
                if(model.Accessions.Count > 0 )
                    if (Request.Params["Identifier"] == model.Identifier)
                        return View("Index", model);

            CreateDatasetController HelperController = new CreateDatasetController();

            model = new SelectedImportOptionsModel()
            {
                MetadataStructureViewList = HelperController.LoadMetadataStructureViewList(),
                DataStructureViewList = HelperController.LoadDataStructureViewList(),
                DataSourceViewList = GetDataSourceList(),
                Accessions = new Dictionary<string, string>()
            };

            HelperController.Dispose();

            try
            {
                string Identifier = Request.Params["Identifier"];
                long MetadataStructureId = long.Parse(Request.Params["SelectedMetadataStructureId"]);
                long DataStructureId = Request.Params["SelectedDataStructureId"] == null ? GetDefaultUnstructuredDataStructureId() : long.Parse(Request.Params["SelectedDataStructureId"]);
                long DataSourceId = long.Parse(Request.Params["SelectedDataSourceId"]);

                model.Identifier = Identifier;
                model.SelectedDataSourceId = DataSourceId;
                model.SelectedDataStructureId = DataStructureId;
                model.SelectedMetadataStructureId = MetadataStructureId;
                
                #region find out the correct URL for the metadata download

                string Url = null;

                // what is the download url for the specific source?; if not found, an exception is thrown and the user is informed
                switch ((DataSource)DataSourceId)
                {
                    case DataSource.EBI:
                    case DataSource.NCBI: // the same in our examples
                        //Url = "https://www.ebi.ac.uk/biosamples/api/samples/" + Identifier;
                        Url = EBI_study_accession + Identifier.Replace("\"", "");
                        break;
                    case DataSource.BioGPS:
                        Url = "http://biogps.org/dataset/" + Identifier + "/values/?format=xml";
                        break;
                    default:
                        throw new Exception("Missing implementation for enum " + Enum.GetName(typeof(DataSource), DataSourceId));
                }

                if (Url == null) throw new Exception("ID could not be matched with an API.");

                #endregion

                #region parse through accessions numbers // study number or accession number on the main url have the same return type 
                string Accessions_List = new WebClient().DownloadString(Url).Trim();
                List<string> All_Accessions = Accessions_List.Split('\n').ToList<string>().GetRange(1, Accessions_List.Split('\n').ToList<string>().Count() - 1);
                model.project = All_Accessions[0].Split('	')[1].ToString();
                Session["All_Accessions"] = All_Accessions;
                List<string> Accessions = new List<string>();
                int index = 0;
                foreach (string s in All_Accessions)
                {
                    string sample_Url = "https://www.ebi.ac.uk/biosamples/api/samples/" + s.Split('	')[1];

                    #region download the metadata
                    // download the metadata
                    string DownloadedData = new WebClient().DownloadString(sample_Url).Trim();

                    #endregion
                    model.Accessions.Add(index.ToString() + " " + s, DownloadedData);
                    index++;
                }
                #endregion

                return View("Index", model);

                //return LoadMetadataForm(Mapped, MetadataStructureId, DataStructureId);
            }
            catch (WebException e)
            {
                String msg = "";
                HttpWebResponse errorResponse = e.Response as HttpWebResponse;
                if (errorResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    msg = "Sample was not found!";
                }
                else
                {
                    msg = e.Message;
                }

                #region show the error message

                model.Error = "An error occurred: " + msg;

                return View("Index", model);

                #endregion
            }
            catch (Exception e)
            {
                #region show the error message

                model.Error = "An error occurred: " + e.Message;

                return View("Index", model);

                #endregion
            }
        }

        public ActionResult LoadSamplesViewMetadata(string sample, string project)
        {
            string x = "";
            //model.Accessions.TryGetValue(sample + "	" + project, out x);
            x = model.Accessions.FirstOrDefault( xx => xx.Key.Contains (sample)).Value;
            EBIresponseModel EBIresponseModel = new EBIresponseModel(JObject.Parse(x));
            return PartialView("View" , EBIresponseModel);
        }

        public Int64 Submit(string acc)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (string s in acc.Split(',').ToList())
            {
                KeyValuePair<string, string> xx = model.Accessions.FirstOrDefault(x => x.Key.Contains(s));
                dict.Add(xx.Key, xx.Value);
            }
            Dataset ds = AddProjectsdataset(dict);
            return ds.Id;
        }

        public Dataset AddProjectsdataset(Dictionary<string, string> xx)
        {
            DataStructureManager dsm = new DataStructureManager();
            DatasetManager dm = new DatasetManager();
            XmlDocument MetadataDoc = new XmlDocument();
            Dataset ds = new Dataset() ;
            XmlDatasetHelper xmlDatasetHelper = new XmlDatasetHelper();
            DataStructure dataStruct = (DataStructure)dsm.AllTypesDataStructureRepo.Get().FirstOrDefault(x => x.Name == "none");
            StructuredDataStructure dataStruct_ = (StructuredDataStructure)dsm.StructuredDataStructureRepo.Get().FirstOrDefault(x => x.Name.ToLower() == "sequence data");
            MetadataStructureManager msm = new MetadataStructureManager();
            ResearchPlanManager rpm = new ResearchPlanManager();
            EntityPermissionManager entityPermissionManager = new EntityPermissionManager();
            List<Error> temp = new List<Error>();
            string temp_file_path = "";
            try
            {
                #region create empty dataset to be filled

                ResearchPlan rp = rpm.Repo.Get().First();
                MetadataStructure metadataStructure = msm.Repo.Get().FirstOrDefault(x => x.Name.ToLower() == "basic abcd");
                ds = dm.CreateEmptyDataset(dataStruct_, rp, metadataStructure);
                if (dm.IsDatasetCheckedOutFor(ds.Id, GetUsernameOrDefault()) || dm.CheckOutDataset(ds.Id, GetUsernameOrDefault()))
                {
                    DatasetVersion dsv = dm.GetDatasetWorkingCopy(ds.Id);
                    long METADATASTRUCTURE_ID = metadataStructure.Id;
                    XmlMetadataWriter xmlMetadatWriter = new XmlMetadataWriter(XmlNodeMode.xPath);
                    XDocument metadataX = xmlMetadatWriter.CreateMetadataXml(METADATASTRUCTURE_ID);
                    XmlDocument metadataXml = XmlMetadataWriter.ToXmlDocument(metadataX);
                    dsv.Metadata = metadataXml;
                    dm.EditDatasetVersion(dsv, null, null, null);
                    dm.CheckInDataset(ds.Id, "Metadata Imported", GetUsernameOrDefault());
                }


                    #endregion
                }
            catch (Exception ex)
            {
                throw (ex);
            }

            try
            {
                // add security
                if (GetUsernameOrDefault() != "DEFAULT")
                {
                    foreach (RightType rightType in Enum.GetValues(typeof(RightType)).Cast<RightType>())
                    {
                        //The user gets full permissions
                        // add security
                        if (GetUsernameOrDefault() != "DEFAULT")
                        {
                            entityPermissionManager.Create<User>(GetUsernameOrDefault(), "Dataset", typeof(Dataset), ds.Id, Enum.GetValues(typeof(RightType)).Cast<RightType>().ToList());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }


            string json = "";
            try {
                #region save primary data in csv format and temporary csv file
                var json_array_data = new List<string[]>();
                temp_file_path = temp_file_to_save_json_as_csv + ds.Id + ".csv";

                string data_csv = new EBIresponseModel().Initialise_header(temp_file_path);
                json_array_data.Add(data_csv.Split(','));

                foreach (KeyValuePair<string, string> kvp in xx)
                {
                    EBIresponseModel EBIresponseModel = new EBIresponseModel(JObject.Parse(kvp.Value));
                    data_csv = data_csv + EBIresponseModel.ConvertTocsv(EBIresponseModel, temp_file_path);
                    json_array_data.Add(EBIresponseModel.ConvertTocsv(EBIresponseModel, "").Split(','));
                }
                json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(json_array_data);
            }
            #endregion
            catch (Exception ex)
            {
                throw (ex);
            }

            #region creating data tuples 

            DataTuple[] rows = null;
            int packageSize = 10000;
            int counter = 0;
            counter++;
            List<string> selectedDataAreaJsonArray = new List<string>() { "[1 , 0 ," + xx.Count.ToString() + "," + (typeof(EBIresponseModel).GetProperties().Count() - 1).ToString() + "]" };
            string selectedHeaderAreaJsonArray = string.Join(" ,", new List<string>() { "[0, 0, 0, " + (typeof(EBIresponseModel).GetProperties().Count() - 1).ToString() + "]" });
            List<int[]> areaDataValuesList = new List<int[]>();
            foreach (string area in selectedDataAreaJsonArray)
            {
                areaDataValuesList.Add(JsonConvert.DeserializeObject<int[]>(area));
            }
            int[] areaHeaderValues = JsonConvert.DeserializeObject<int[]>(selectedHeaderAreaJsonArray);
            Orientation orientation = Orientation.columnwise;
            String worksheetUri = temp_file_path;
            int batchSize = (new Object()).GetUnitOfWork().PersistenceManager.PreferredPushSize;
            int batchnr = 1;

            //try {
                dm.CheckOutDataset(ds.Id, GetUsernameOrDefault());
                DatasetVersion workingCopy = dm.GetDatasetWorkingCopy(ds.Id);

                foreach (int[] areaDataValues in areaDataValuesList)
                {
                    //First batch starts at the start of the current data area
                    int currentBatchStartRow = areaDataValues[0] + 1;
                    while (currentBatchStartRow <= areaDataValues[2] + 1) //While the end of the current data area has not yet been reached
                    {
                        //End row is start row plus batch size
                        int currentBatchEndRow = currentBatchStartRow + batchSize;

                        //Set the indices for the reader
                        EasyUploadFileReaderInfo fri = new EasyUploadFileReaderInfo
                            {
                                DataStartRow = currentBatchStartRow,
                                //End row is either at the end of the batch or the end of the marked area
                                //DataEndRow = (currentBatchEndRow > areaDataValues[2] + 1) ? areaDataValues[2] + 1 : currentBatchEndRow,
                                DataEndRow = Math.Min(currentBatchEndRow, areaDataValues[2] + 1),
                                //Column indices as marked in a previous step
                                DataStartColumn = areaDataValues[1] + 1,
                                DataEndColumn = areaDataValues[3] + 1,

                                //Header area as marked in a previous step
                                VariablesStartRow = areaHeaderValues[0] + 1,
                                VariablesStartColumn = areaHeaderValues[1] + 1,
                                VariablesEndRow = areaHeaderValues[2] + 1,
                                VariablesEndColumn = areaHeaderValues[3] + 1,

                                Offset = areaDataValues[1],
                                Orientation = orientation
                            };


                        #region csv / txt parsing to get data tuples and variables
                        AsciiFileReaderInfo afri = new AsciiFileReaderInfo();
                        afri.Seperator = TextSeperator.comma;

                        List<String[]> Json_table_ = JsonConvert.DeserializeObject<List<String[]>>
                            (json);

                        AsciiReaderEasyUpload reader_ = new AsciiReaderEasyUpload(dataStruct_, afri);
                        FileStream Stream = reader_.Open(temp_file_path);
                        rows = reader_.ReadFile(Stream, System.IO.Path.GetFileName(temp_file_path), Json_table_, afri, dataStruct_, ds.Id, packageSize, fri);
                        Stream.Close();
                        if (rows != null)
                        {
                            dm.EditDatasetVersion(workingCopy, rows.ToList(), null, null);
                        }

                        //Close the Stream so the next ExcelReader can open it again
                        Stream.Close();

                        //Debug information
                        int lines = (areaDataValues[2] + 1) - (areaDataValues[0] + 1);
                        int batches = lines / batchSize;
                        batchnr++;

                        //Next batch starts after the current one
                        currentBatchStartRow = currentBatchEndRow + 1;

                        #endregion csv parsing to get data tuples and variables 
                    }
                }
            workingCopy.ModificationInfo = new EntityAuditInfo()
            {
                Performer = GetUsernameOrDefault(),
                Comment = "Data",
                ActionType = AuditActionType.Create
            };

            dm.CheckInDataset(ds.Id, "Import data from OMIC archives  ", GetUsernameOrDefault());

            //}
            //catch (Exception e)
            //{
            //    throw (e);
            //}
            #endregion

            //System.IO.File.Delete(temp_file_path);
            //dm.CheckInDataset(ds.Id, "Import data from OMIC archives ", GetUsernameOrDefault());

            dsm.Dispose();
            dm.Dispose();
            entityPermissionManager.Dispose();

            return ds;
        }


        public void add_Accessions_as_primary_Data(Dataset ds, DataStructure dataStruct, DataStructureManager dsm,string acc )
        {
            // add primary data top the dataset
            long id = ds.Id;
            long iddsd = dataStruct.Id;
            StructuredDataStructure sds = dsm.StructuredDataStructureRepo.Get(iddsd);
            try
            {
                // tweak code from FinishUpload() from EasyUploadSummaryController under DCM module

                //dsm.StructuredDataStructureRepo.LoadIfNot(sds.Variables);
                string sample_metadata = new WebClient().DownloadString(EBI_sample_Accession + acc).Trim();
                EBIresponseModel EBIresponseModel = new EBIresponseModel(JObject.Parse(sample_metadata));
                string filepath = temp_file_to_save_json_as_csv + "tmp" + ds.Id + ".csv";
                EBIresponseModel.ConvertTocsv(EBIresponseModel, filepath);
                AsciiFileReaderInfo fri = new AsciiFileReaderInfo();
                fri.Seperator = TextSeperator.comma;
                AsciiReader reader = new AsciiReader(sds, fri);
                Stream Stream = reader.Open(temp_file_to_save_json_as_csv + "tmp" + ds.Id + ".csv");
                reader.ValidateFile(Stream, Path.GetFileName(filepath) , ds.Id );
                Stream.Close();
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
            }
        }


        public ActionResult LoadMetadataForm(XmlDocument metadata, long MetadataStructureId, long DataStructureId)
        {
            // generate the CreateTaskManager Instance and add all important values for the form
            CreateTaskmanager taskManager = new CreateTaskmanager();

            // set all needed informations to the BUS
            var metadataX = XDocument.Load(new XmlNodeReader(metadata));
            taskManager.AddToBus(CreateTaskmanager.METADATA_XML, metadataX);
            taskManager.AddToBus(CreateTaskmanager.METADATASTRUCTURE_ID, MetadataStructureId);
            taskManager.AddToBus(CreateTaskmanager.DATASTRUCTURE_ID, DataStructureId);
            taskManager.AddToBus(CreateTaskmanager.RESEARCHPLAN_ID, 1);

            #region set function actions of COPY, RESET, CANCEL, SUBMIT
            ActionInfo copyAction = new ActionInfo()
            {
                ActionName = "Copy",
                ControllerName = "CreateDataset",
                AreaName = "DCM"
            };

            ActionInfo resetAction = new ActionInfo()
            {
                ActionName = "Reset",
                ControllerName = "Form",
                AreaName = "DCM"
            };

            ActionInfo cancelAction = new ActionInfo()
            {
                ActionName = "Cancel",
                ControllerName = "Form",
                AreaName = "DCM"
            };

            ActionInfo submitAction = new ActionInfo()
            {
                ActionName = "Submit",
                ControllerName = "CreateDataset",
                AreaName = "DCM"
            };

            //add actions to the taskmanager
            taskManager.Actions.Add(CreateTaskmanager.CANCEL_ACTION, cancelAction);
            taskManager.Actions.Add(CreateTaskmanager.COPY_ACTION, copyAction);
            taskManager.Actions.Add(CreateTaskmanager.RESET_ACTION, resetAction);
            taskManager.Actions.Add(CreateTaskmanager.SUBMIT_ACTION, submitAction);

            #endregion

            //set taskmanager to session 
            Session["CreateDatasetTaskmanager"] = taskManager;

            // call the editor
            return RedirectToAction("StartMetadataEditor", "Form", new RouteValueDictionary { { "area", "DCM" } });

        }

        public XmlDocument ConvertOmicsToBExIS(long metadataStructureId, XmlDocument metadataForImport, DataSource source)
        {

            string sourceName = GetSourceName(source);

            CreateDatasetController HelperController = new CreateDatasetController();
            string metadataStructureName = HelperController.LoadMetadataStructureViewList().Find(x => x.Id == metadataStructureId).Title;
            HelperController.Dispose();
            // create path to mapping file, MappingFile_extern_biogps.xsd_to_intern_bfgghng.xml
            var path_mappingFile = Path.Combine(AppConfiguration.GetModuleWorkspacePath("DIM"), "MappingFile_extern_" + sourceName + "21_to_intern_" + metadataStructureName + ".xml");
            // use maybe XmlMetadataImportHelper.GetMappingFileName(metadataStructureId, TransmissionType.mappingFileImport, mappingFileName)

            // if not existing, then an error will be thrown...
            if (!System.IO.File.Exists(path_mappingFile))
            {
                throw new Exception("Missing mapping file from " + metadataStructureName + " to " + sourceName + ".");
            }

            // XML mapper + mapping file
            var xmlMapperManager = new XmlMapperManager(TransactionDirection.ExternToIntern);
            xmlMapperManager.Load(path_mappingFile, "");

            // generate internal metadata without internal attributes
            var metadataResult = xmlMapperManager.Generate(metadataForImport, 1, true);

            // throw new Exception(XmlToString(metadataResult));

            // generate internal template metadata xml with needed attribtes
            var xmlMetadataWriter = new XmlMetadataWriter(XmlNodeMode.xPath);
            var metadataXml = xmlMetadataWriter.CreateMetadataXml(
                metadataStructureId,
                XmlUtility.ToXDocument(metadataResult)
            );

            // shall contain the attributes
            var metadataXmlTemplate = XmlMetadataWriter.ToXmlDocument(metadataXml);

            // set attributes FROM metadataXmlTemplate TO metadataResult
            var completeMetadata = XmlMetadataImportHelper.FillInXmlValues(metadataResult, metadataXmlTemplate);

            return completeMetadata;

        }


        ////////////////////////////////////




        public long GetDefaultUnstructuredDataStructureId()
        {
            var x = new Dlm.Services.DataStructure.DataStructureManager();
            var y = x.UnStructuredDataStructureRepo.Get();
            return y[0].Id;

        }

        public String XmlToString(XmlDocument xml)
        {
            StringWriter stringWriter = new StringWriter();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);

            xml.WriteTo(xmlTextWriter);

            return stringWriter.ToString();
        }

        public void ConvertXMLItemKeys(XmlDocument doc, XmlNode parent)
        {
            foreach(XmlNode node in parent.ChildNodes)
            {
                if(node.Name == "item" && node.Attributes["key"] != null)
                {
                    var NewChild = doc.CreateNode(System.Xml.XmlNodeType.Element, "", node.Attributes["key"].Value, "");
                    foreach(XmlNode child in node.ChildNodes)
                    {
                        NewChild.AppendChild(child);
                    }
                    parent.AppendChild(NewChild);
                } else
                {
                    ConvertXMLItemKeys(doc, node);
                }
            }
        }

        public String GetSourceName(DataSource source)
        {
            switch (source)
            {// if a different name is needed, switch it
                case DataSource.NCBI:
                    return "EBI";
                default:
                    return Enum.GetName(typeof(DataSource), source);
            }
        }

        public ActionResult QueryAvailableMappings()
        {

            DataSource source = (DataSource)long.Parse(Request.Params["SelectedDataSourceId"]);
            string sourceName = GetSourceName(source);

            CreateDatasetController HelperController = new CreateDatasetController();
            var msList = HelperController.LoadMetadataStructureViewList();
            var list = new List<object>();
            foreach (var entry in msList)
            {
                var path_mappingFile = Path.Combine(AppConfiguration.GetModuleWorkspacePath("DIM"), "MappingFile_extern_" + sourceName + "_to_intern_" + entry.Title + ".xml");
                list.Add(Json(new { title = entry.Title, id = entry.Id, hasFile = System.IO.File.Exists(path_mappingFile) }));
            }

            JavaScriptSerializer js = new JavaScriptSerializer();
            HelperController.Dispose();
            return Content(js.Serialize(list));

        }

        public XmlDocument JsonStringToXML(string json)
        {
            XmlDocument doc = (XmlDocument) Newtonsoft.Json.JsonConvert.DeserializeXmlNode(json);
            return doc;
        }





        public List<ListViewItem> GetDataSourceList()
        {
            List<ListViewItem> temp = new List<ListViewItem>();

            Type type = typeof(DataSource);

            foreach (DataSource source in Enum.GetValues(type))
            {
                temp.Add(new ListViewItem((long)source, Enum.GetName(type, source)));
            }

            return temp;
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
    }

}
