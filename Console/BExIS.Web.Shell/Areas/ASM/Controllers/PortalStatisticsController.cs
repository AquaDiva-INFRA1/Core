using System;
using System.Collections.Generic;
using System.Diagnostics;
using BExIS.Dlm.Services.Data;
using Npgsql;
using System.Configuration;
using BExIS.Dlm.Services.DataStructure;
using BExIS.Dlm.Entities.DataStructure;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json.Linq;
using BExIS.Modules.Asm.UI.Models;
using BExIS.Modules.Rpm.UI.Models;
using System.Web.Configuration;
using System.IO;
using Vaiona.Utils.Cfg;
using System.Xml;
using Vaiona.Web.Mvc;
using System.Web.Mvc;
using BExIS.Dlm.Entities.Data;
using System.Linq;
using BExIS.Aam.Services;
using BExIS.Aam.Entities.Mapping;
using Vaiona.Persistence.Api;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BExIS.Modules.ASM.UI.Controllers
{
    public class PortalStatisticsController : Controller
    {
        static string DatastructAPI = "https://addp.uni-jena.de/api/structures/";
        static List<Variable_analytics> VA_list;
        static String temp_file = Path.Combine(AppConfiguration.GetModuleWorkspacePath("ASM"), "Analytics_temp.txt");
        static List<string> project_list_names = new List<string> { "A01", "A02", "A03", "A04", "A05", "A06", "B01", "B02", "B03", "B04", "B05", "C03", "C05", "D01", "D02", "D03", "D04" };

        static String DebugFilePath = Path.Combine(AppConfiguration.GetModuleWorkspacePath("ASM"), "debug.txt");

        string BaseAdress = WebConfigurationManager.AppSettings["BaseAdress"];



        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(10);

                string url = BaseAdress + "/api/Statistics/get";
                client.BaseAddress = new Uri(url);
                client.Timeout = TimeSpan.FromMinutes(10);
                var responseTask = client.GetAsync("");
                responseTask.Wait();
                //To store result of web api response.   
                Stream result = await responseTask.Result.Content.ReadAsStreamAsync();
                string jsonstring = new StreamReader(result).ReadToEnd();
                JObject Json_res = JObject.Parse(jsonstring);
                ViewData["Json_res"] = Json_res;
                return View("Index");
            }
        }

        public async System.Threading.Tasks.Task<ActionResult> reset()
        {
            using (var client = new HttpClient())
            {
                string url = this.ControllerContext.HttpContext.Request.Url.Scheme + "://" + this.ControllerContext.HttpContext.Request.Url.Authority + "/api/Statistics/reset";
                client.BaseAddress = new Uri(url);
                var responseTask = client.GetAsync("");
                responseTask.Wait();
                //To store result of web api response.   
                Stream result = await responseTask.Result.Content.ReadAsStreamAsync();
                string jsonstring = new StreamReader(result).ReadToEnd();
                JObject Json_res = JObject.Parse(jsonstring);
                //ViewData["Json_res"] = Json_res;
                return RedirectToAction("Index");
            }
        }

        /*
        public ActionResult Index()
        {
            DatasetManager DM = new DatasetManager();
            List <long> ds_ids = DM.GetDatasetLatestIds();
            DataStructureManager DStructM = new DataStructureManager();
            List <DataStructure> dataStructs = (List<DataStructure>) DStructM.AllTypesDataStructureRepo.Get();
            String temp_file = Path.Combine(AppConfiguration.GetModuleWorkspacePath("ASM"), "Analytics_temp.txt");
            JObject stats_obj = JObject.Parse(System.IO.File.ReadAllText(temp_file));
            ViewData["datasetCount"] = stats_obj["dataset_count"].ToString();
            ViewData["Datapoints"] = stats_obj["datapoints"].ToString();

            Data_container_analytics datacontaineranalytics = new Data_container_analytics();
            ViewData["datacontaineranalytics"] = datacontaineranalytics;

            VA_list = new List<Variable_analytics>();

            foreach (long id in ds_ids)
            {
                if (DM.GetDataset(id).Status == DatasetStatus.CheckedIn)
                {
                    List<string> variable_id = new List<string>();
                    List<string> variable_label = new List<string>();
                    List<string> dataType = new List<string>();
                    List<string> unit = new List<string>();
                    List<string> variable_concept_entity = new List<string>();
                    List<string> variable_concept_caracteristic = new List<string>();

                    #region metadata extraction
                    //get the owner of the dataset
                    DatasetManager dm = new DatasetManager();
                    XmlDocument xmlDoc = dm.GetDatasetLatestMetadataVersion(id);
                    XmlNode root = xmlDoc.DocumentElement;
                    string idMetadata = root.Attributes["id"].Value;

                    string datastructure_id = DM.GetDataset(id).DataStructure.Id.ToString();
                    string owner = "";
                    string project = "";

                    if (idMetadata == "1")
                    {
                        XmlNodeList nodeList_givenName = xmlDoc.SelectNodes("/Metadata/Creator/PersonEML/Givenname/Name");
                        XmlNodeList nodeList_Surname = xmlDoc.SelectNodes("/Metadata/Creator/PersonEML/Surname/Name");
                        owner = nodeList_givenName[0].InnerText + " " + nodeList_Surname[0].InnerText;
                        XmlNodeList nodeList_Title = xmlDoc.SelectNodes("/Metadata/Project/ProjectEML/Title/Title");
                        XmlNodeList nodeList_Personnelgivenname = xmlDoc.SelectNodes("/Metadata/Project/ProjectEML/Personnelgivenname/Name");
                        XmlNodeList nodeList_Personnelsurname = xmlDoc.SelectNodes("/Metadata/Project/ProjectEML/Personnelsurname/Name");
                        project = nodeList_Title[0].InnerText + "/" + nodeList_Personnelgivenname[0].InnerText + " " + nodeList_Personnelsurname[0].InnerText;
                    }
                    else if (idMetadata == "2")
                    {
                        XmlNodeList nodeList_givenName = xmlDoc.SelectNodes("/Metadata/Owner/Owner/FullName/Name");
                        owner = nodeList_givenName[0].InnerText;
                        XmlNodeList nodeList_Title = xmlDoc.SelectNodes("/Metadata/Owner/Owner/Role/Role");
                        XmlNodeList nodeList_SourceInstitutionID = xmlDoc.SelectNodes("/Metadata/Unit/Unit/SourceInstitutionID/Id");
                        XmlNodeList nodeList_SourceID = xmlDoc.SelectNodes("/Metadata/Unit/Unit/SourceID/Id");
                        XmlNodeList nodeList_UnitID = xmlDoc.SelectNodes("/Metadata/Unit/Unit/UnitID/Id");
                        project = nodeList_Title[0].InnerText + "/" + nodeList_SourceInstitutionID[0].InnerText + " - " + nodeList_SourceID[0].InnerText + " - " + nodeList_UnitID[0].InnerText;
                    }
                    else if (idMetadata == "3")
                    {
                        XmlNodeList nodeList_givenName = xmlDoc.SelectNodes("/Metadata/Metadata/MetadataType/Owners/OwnersType/Owner/Contact/Person/PersonName/FullName/FullNameType");
                        foreach (XmlElement node in nodeList_givenName)
                        {
                            owner = node.InnerText + " - " + owner;
                        }
                        //owner = nodeList_givenName[0].InnerText;
                        XmlNodeList nodeList_Title = xmlDoc.SelectNodes("/Metadata/Metadata/MetadataType/Owners/OwnersType/Owner/Contact/Organisation/Organisation/Name/Label/Representation/RepresentationType/Text/TextType");
                        XmlNodeList nodeList_SourceInstitutionID = xmlDoc.SelectNodes("/Metadata/Metadata/MetadataType/Owners/OwnersType/Owner/Contact/Organisation/Organisation/OrgUnits/OrgUnitsType/OrgUnit/OrgUnitType");
                        project = nodeList_Title[0].InnerText + "/" + nodeList_SourceInstitutionID[0].InnerText;
                    }

                    foreach (string proj in project_list_names)
                    {
                        if (project.Contains(proj))
                            project = proj;
                    }
                    #endregion

                    //Construct a HttpClient for the search-Server
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(DatastructAPI);
                    //Set the searchTerm as query-String
                    String param = HttpUtility.UrlEncode(datastructure_id);
                 
                    try
                    {
                        HttpResponseMessage response = client.GetAsync(param).Result;  
                        if (response.IsSuccessStatusCode)
                        {
                            JObject json_ds_struct = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                            JArray json_variables = JArray.Parse(json_ds_struct["Variables"].ToString());

                            Aam_Dataset_column_annotationManager aam_manager = new Aam_Dataset_column_annotationManager();
                            List<Aam_Dataset_column_annotation> aam_list = aam_manager.get_all_dataset_column_annotation();

                            foreach (JObject json_variable in json_variables)
                            {
                                variable_id.Add(json_variable["Id"].ToString());
                                variable_label.Add(json_variable["Label"].ToString());
                                dataType.Add(json_variable["DataType"].ToString());
                                unit.Add(json_variable["Unit"].ToString());
                                Aam_Dataset_column_annotation annot = 
                                    aam_list.Where(x => x.Dataset.Id == id && x.variable_id.Id == Int64.Parse(json_variable["Id"].ToString())).FirstOrDefault();
                                if (annot.entity_id.URI.ToString() == "")
                                {
                                    variable_concept_entity.Add(annot.entity_id.label.ToString() + " - No Annotation");
                                }
                                else
                                {
                                    variable_concept_entity.Add(annot.entity_id.label.ToString() + " - " + annot.entity_id.URI.ToString());
                                }

                                if (annot.characteristic_id.URI.ToString() == "")
                                {
                                    variable_concept_caracteristic.Add(annot.characteristic_id.label.ToString() + " - No Annotation");
                                }
                                else
                                {
                                    variable_concept_caracteristic.Add(annot.characteristic_id.label.ToString() + " - " + annot.characteristic_id.URI.ToString());
                                }
                            }
                            // create a new instance of variable analytics
                            Variable_analytics VA = new Variable_analytics(id, datastructure_id, owner, project, variable_id, variable_label, variable_concept_entity, variable_concept_caracteristic, dataType, unit);
                            VA_list.Add(VA);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString());
                    }
                 }
            }
            
            Data_container_analytics datacontaineranalytics = new Data_container_analytics();
            
            ViewData["datacontaineranalytics"] = datacontaineranalytics;
            DataStructureManager dataStructureManager = new DataStructureManager();
            List<StructuredDataStructure> structureRepo = DStructM.GetUnitOfWork().GetReadOnlyRepository<StructuredDataStructure>().Get().ToList<StructuredDataStructure>();
            int counting = 0;
            foreach(StructuredDataStructure x in structureRepo){
                counting = counting + x.Variables.Count;
            }
            ViewData["var_counts"] = counting;
            ViewData["VA_list"] = VA_list;

            //debugging file
            using (StreamWriter sw = System.IO.File.AppendText(DebugFilePath))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-ddThh:mm:ssTZD") + " : Analytic scalled: portal statistics - VA_list " + VA_list.ToString());
            }

            return View("Index");
        }
        */
        public ActionResult Download_Report()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("dataset_id , datastructure_id ,owner, project,Semantic Coverage, variable_id,variable_label,variable_concept_entity,variable_concept_caracteristic,dataType,unit,data points");

            DatasetManager dm = new DatasetManager();
            
            foreach (var va in VA_list)
            {
                Dataset ds = dm.DatasetRepo.Query().ToList().FirstOrDefault(p => p.Id == Int64.Parse(va.dataset_id.ToString()));
                long noColumns = ds.DataStructure.Self is StructuredDataStructure ? (ds.DataStructure.Self as StructuredDataStructure).Variables.Count() : 0L;
                long noRows = ds.DataStructure.Self is StructuredDataStructure ? dm.GetDatasetLatestVersionEffectiveTupleCount(ds) : 0; // It would save time to calc the row count for all the datasets at once!
                long somme =  noRows * noColumns;

                if (va.variable_id.Count > 0)
                {
                    int Concepts_count = 0;
                    int Caracteristics_count = 0;


                    foreach (String s in va.variable_concept_entity)
                    {
                        if (s != "No Annotation")
                        {
                            Concepts_count = Concepts_count + 1;
                        }
                    }
                    foreach (String s in va.variable_concept_caracteristic)
                    {
                        if (s != "No Annotation")
                        {
                            Caracteristics_count = Caracteristics_count + 1;
                        }
                    }

                    for (int kk = 0; kk < va.variable_id.Count; kk++)
                    {
                        sb.AppendLine(
                            va.dataset_id.ToString().Replace("\n", String.Empty).Trim() + "," +
                            va.datastructure_id.ToString().Replace("\n", String.Empty).Trim() + "," +
                            va.owner.Replace(",", "-").Replace("\n", String.Empty).Trim() + "," +
                            va.project.Replace(",", "-").Replace("\n", String.Empty).Trim() + "," +
                            Concepts_count.ToString().Replace("\n", String.Empty).Trim() + "," +
                            va.variable_id.Count.ToString().Replace("\n", String.Empty).Trim() + "," +
                            va.variable_id[kk].ToString().Replace("\n", String.Empty).Trim() + "," +
                            va.variable_label[kk].ToString().Replace("\n", String.Empty).Trim() + "," +
                            va.variable_concept_entity[kk].ToString().Replace("\n", String.Empty).Trim() + "," +
                            va.variable_concept_caracteristic[kk].ToString().Replace("\n", String.Empty).Trim() + "," +
                            va.unit[kk].ToString().Replace("\n", String.Empty).Trim() + "," +
                            va.dataType[kk].ToString().Replace("\n", String.Empty).Trim() + "," +
                            somme);
                    }
                }
            }
            // save the string builder sb and ask for download 

            Console.WriteLine(sb.ToString());
            System.IO.File.WriteAllText(
                System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "REPORT.csv"),
                sb.ToString());
            
            return File(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "REPORT.csv"), "text/csv", "REPORT.csv");
        }

        public Boolean refresh_stats()
        {
            Task.Run(() => this.refresh_stats_async());
            return true;
        } 

        private async Task<Boolean> refresh_stats_async()
        {
            DatasetManager dm = new DatasetManager();
            List<Dataset> datasets = new List<Dataset>();
            List<long> datasetIds = new List<long>();
            datasets = dm.DatasetRepo.Query().OrderBy(p => p.Id).ToList();
            datasetIds = datasets.Select(p => p.Id).ToList();
            long somme = 0;
            foreach (Dataset ds in datasets)
            {
                long noColumns = ds.DataStructure.Self is StructuredDataStructure ? (ds.DataStructure.Self as StructuredDataStructure).Variables.Count : 0L;
                long noRows = ds.DataStructure.Self is StructuredDataStructure ? dm.GetDatasetLatestVersionEffectiveTupleCount(ds) : 0; // It would save time to calc the row count for all the datasets at once!
                if (ds.Status == DatasetStatus.CheckedIn)
                {
                    somme = somme + (noRows * noColumns);
                }
            }
            dm.Dispose();
            string json = "{\"dataset_count\" : " + datasets.Count + " , \"datapoints\" : " + somme + " }";
            using (StreamWriter sw = System.IO.File.CreateText(temp_file))
            {
                sw.WriteLine(json);
            }
            return true;
            //The ASP.NET Session doesn't allow you to perform parallel requests from the same session.
            //Having a RESTful API relying on session is a very bad design and IMHO should be rearchitectured so that it doesn't rely on state.
        }
        public ActionResult DataAttributeStruct_list(List<DataAttributeStruct> DataAttributeStruct_)
        {
            ViewData["DataAttributeStruct"] = DataAttributeStruct_;
            return PartialView();
        }
        public ActionResult EditUnitModel_list(List<EditUnitModel> EditUnitModel_)
        {
            ViewData["EditUnitModel_"] = EditUnitModel_;
            return PartialView();
        }
        public ActionResult DataTypeModel_list(List<DataType> DataTypeModel_)
        {
            ViewData["DataTypeModel_"] = DataTypeModel_;
            return PartialView();
        }
    }
    
}