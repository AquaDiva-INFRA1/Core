using System;
using BExIS.Modules.OAC.UI.Models;
using System.Collections.Generic;
using System.IO;
using BExIS.Xml.Helpers;
using BExIS.Xml.Helpers.Mapping;
using System.Xml;
using BExIS.Modules.Dcm.UI.Controllers;
using BExIS.Modules.Dcm.UI.Models;
using BExIS.Web.Shell.Models;
using BExIS.Web.Shell.Helpers;
using BExIS.Dcm.CreateDatasetWizard;
using BExIS.Dcm.Wizard;
using System.Web.Routing;
using Vaiona.Utils.Cfg;
using System.Net;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Web.Script.Serialization;

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

        #region source enums

        // a list of all sources;
        // the explicit link needs to be added further down
        public enum DataSource : long
        {
            BioGPS = 1,
            EBI = 2, NCBI = 3 // the same in our examples
        }

        #endregion

        #region main processing

        public long GetDefaultUnstructuredDataStructureId()
        {
            var x = new Dlm.Services.DataStructure.DataStructureManager();
            var y = x.UnStructuredDataStructureRepo.Get();
            return y[0].Id;

        }

        /// <summary>
        ///     processes the request, downloads the data, and then redirects & shows the DIM page for creation of a new dataset
        /// </summary>
        /// <returns>the page created</returns>
        public ActionResult TransformMetadata()
        {
            try
            {
                string Identifier = Request.Params["Identifier"];
                long MetadataStructureId = long.Parse(Request.Params["SelectedMetadataStructureId"]);
                long DataStructureId = Request.Params["SelectedDataStructureId"] == null ? GetDefaultUnstructuredDataStructureId() : long.Parse(Request.Params["SelectedDataStructureId"]);
                long DataSourceId = long.Parse(Request.Params["SelectedDataSourceId"]);

                #region find out the correct URL for the metadata download

                string Url = null;

                // what is the download url for the specific source?; if not found, an exception is thrown and the user is informed
                switch ((DataSource)DataSourceId)
                {
                    case DataSource.EBI:
                    case DataSource.NCBI: // the same in our examples
                        Url = "https://www.ebi.ac.uk/biosamples/api/samples/" + Identifier;
                        break;
                    case DataSource.BioGPS:
                        Url = "http://biogps.org/dataset/" + Identifier + "/values/?format=xml";
                        break;
                    default:
                        throw new Exception("Missing implementation for enum " + Enum.GetName(typeof(DataSource), DataSourceId));
                }

                if (Url == null) throw new Exception("ID could not be matched with an API.");

                #endregion

                #region download the metadata

                // download the metadata
                string DownloadedData = new WebClient().DownloadString(Url).Trim();

                #endregion

                #region convert it to xml
                XmlDocument Metadata;

                if (DownloadedData.StartsWith("{") || DownloadedData.StartsWith("[")) // it's json
                {
                    Metadata = JsonStringToXML("{\"root\":" + DownloadedData + "}"); // the root element is only allowed to have one property
                }
                else // it's xml
                {
                    Metadata = new XmlDocument();
                    Metadata.LoadXml(DownloadedData);
                }
                #endregion

                ConvertXMLItemKeys(Metadata, Metadata);

                #region map the data

                XmlDocument Mapped = ConvertOmicsToBExIS(MetadataStructureId, Metadata, (DataSource)DataSourceId);

                #endregion

                return LoadMetadataForm(Mapped, MetadataStructureId, DataStructureId);

            }
            catch(WebException e)
            {
                String msg = "";
                HttpWebResponse errorResponse = e.Response as HttpWebResponse;
                if (errorResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    msg = "Sample was not found!";
                } else
                {
                    msg = e.Message;
                }

                #region show the error message

                CreateDatasetController HelperController = new CreateDatasetController();
                SelectedImportOptionsModel model = new SelectedImportOptionsModel()
                {
                    MetadataStructureViewList = HelperController.LoadMetadataStructureViewList(),
                    DataStructureViewList = HelperController.LoadDataStructureViewList(),
                    DataSourceViewList = GetDataSourceList(),
                    Error = "An error occurred: " + msg
                };

                return View("Index", model);

                #endregion
            }
            catch (Exception e)
            {
                #region show the error message

                CreateDatasetController HelperController = new CreateDatasetController();
                SelectedImportOptionsModel model = new SelectedImportOptionsModel()
                {
                    MetadataStructureViewList = HelperController.LoadMetadataStructureViewList(),
                    DataStructureViewList = HelperController.LoadDataStructureViewList(),
                    DataSourceViewList = GetDataSourceList(),
                    Error = "An error occurred: " + e.Message
                };

                return View("Index", model);

                #endregion
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

        // for debugging, can be removed later
        public String XmlToString(XmlDocument xml)
        {
            StringWriter stringWriter = new StringWriter();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);

            xml.WriteTo(xmlTextWriter);

            return stringWriter.ToString();
        }

        #endregion

        #region mapping

        /// <summary>
        /// converts &lt;item key="name"&gt;&lt;/item&gt; to &lt;name&gt;&lt;/item&gt;
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="parent"></param>
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

            return Content(js.Serialize(list));

        }

        public XmlDocument ConvertOmicsToBExIS(long metadataStructureId, XmlDocument metadataForImport, DataSource source)
        {

            string sourceName = GetSourceName(source);

            CreateDatasetController HelperController = new CreateDatasetController();
            string metadataStructureName = HelperController.LoadMetadataStructureViewList().Find(x => x.Id == metadataStructureId).Title;

            // create path to mapping file, MappingFile_extern_biogps.xsd_to_intern_bfgghng.xml
            var path_mappingFile = Path.Combine(AppConfiguration.GetModuleWorkspacePath("DIM"), "MappingFile_extern_" + sourceName + "_to_intern_" + metadataStructureName + ".xml");
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

        /// <summary>
        ///     converts JSON strings to XML documents for use in the XML based functions
        ///     uses Newtonsoft, JsonConvert.DeserializeXmlNode
        /// </summary>
        /// <param name="json">The JSON, that should be converted to XML</param>
        /// <returns>the XML created from the JSON string</returns>
        public XmlDocument JsonStringToXML(string json)
        {
            XmlDocument doc = (XmlDocument) JsonConvert.DeserializeXmlNode(json);
            return doc;
        }

        #endregion

        #region form page

        /// <summary>
        ///     shows the page, where the user enters all needed information
        /// </summary>
        /// <returns>the page created</returns>
        public ActionResult Index()
        {

            CreateDatasetController HelperController = new CreateDatasetController();

            SelectedImportOptionsModel model = new SelectedImportOptionsModel()
            {
                MetadataStructureViewList = HelperController.LoadMetadataStructureViewList(),
                DataStructureViewList = HelperController.LoadDataStructureViewList(),
                DataSourceViewList = GetDataSourceList()
            };

            return View(model);
        }

        #endregion

        #region utilities

        /// <summary>
        ///     get a list of all sources, needed for the selection of the source
        /// </summary>
        /// <returns>the list of all available sources</returns>
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

        #endregion
    }

}
