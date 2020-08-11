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

namespace BExIS.Modules.ASM.UI.Controllers
{
    public class PortalStatisticsController : Controller
    {
        static string DatastructAPI = "https://aquadiva-pub1.inf-bb.uni-jena.de/api/structures/";
        static List<Variable_analytics> VA_list;
        static String debugFile = Path.Combine(AppConfiguration.GetModuleWorkspacePath("ASM"), "debug.txt");
        static List<string> project_list_names = new List<string> { "A01", "A02", "A03", "A04", "A05", "A06", "B01", "B02", "B03", "B04", "B05", "C03", "C05", "D01", "D02", "D03", "D04" };

        static string Conx = ConfigurationManager.ConnectionStrings[1].ConnectionString;

        static string python_path = @WebConfigurationManager.AppSettings["python_path"].ToString();
        static string python_script = @WebConfigurationManager.AppSettings["python_script"].ToString();
        static string output_Folder = @WebConfigurationManager.AppSettings["output_Folder"].ToString();

        private static string datasets_root_folder = WebConfigurationManager.AppSettings["DataPath"];
        string[] allowed_extention = new string[] { ".csv", ".xlsx" ,".xls" };

        static List<string> lines = new List<string>();
        static String DebugFilePath = Path.Combine(AppConfiguration.GetModuleWorkspacePath("ASM"), "debug.txt");

        /* this action reveals a semantic coverage for our data portal and needs to be accessed via URL ... no button for it ...
        */
        public ActionResult Index()
        {
            DatasetManager DM = new DatasetManager();
            List <long> ds_ids = DM.GetDatasetLatestIds();
            DataStructureManager DStructM = new DataStructureManager();
            List <DataStructure> dataStructs = (List<DataStructure>) DStructM.AllTypesDataStructureRepo.Get();

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

                            foreach (JObject json_variable in json_variables)
                            {
                                variable_id.Add(json_variable["Id"].ToString());
                                variable_label.Add(json_variable["Label"].ToString());
                                dataType.Add(json_variable["DataType"].ToString());
                                unit.Add(json_variable["Unit"].ToString());
                            
                                string select = "SELECT datasets_id, variable_id, entity , characteristic FROM dataset_column_annotation WHERE datasets_id= \'" +
                                    id + "\' AND variable_id = \'" + json_variable["Id"].ToString() + "\'";
                                NpgsqlCommand MyCmd = null;
                                NpgsqlConnection MyCnx = null;
                                MyCnx = new NpgsqlConnection(Conx);
                                MyCnx.Open();
                                MyCmd = new NpgsqlCommand(select, MyCnx);

                                NpgsqlDataReader dr = MyCmd.ExecuteReader();
                                if (dr != null)
                                {
                                    while (dr.Read())
                                    {
                                        if ((dr["datasets_id"] != System.DBNull.Value) && ((dr["variable_id"] != System.DBNull.Value)))
                                        {
                                            if (dr["entity"].ToString() == "")
                                            {
                                                variable_concept_entity.Add("No Annotation");
                                            }
                                            else
                                            {
                                                variable_concept_entity.Add(dr["entity"].ToString());
                                            }

                                            if (dr["characteristic"].ToString() == "")
                                            {
                                                variable_concept_caracteristic.Add("No Annotation");
                                            }
                                            else
                                            {
                                                variable_concept_caracteristic.Add(dr["characteristic"].ToString());
                                            }
                                        }
                                    }
                                    if (dr.HasRows == false)
                                    {
                                        variable_concept_entity.Add("No Annotation");
                                        variable_concept_caracteristic.Add("No Annotation");
                                    }
                                }
                                MyCnx.Close();
                            }
                            // create a new instance of variable analytics
                            Variable_analytics VA = new Variable_analytics(id, datastructure_id, owner, project, variable_id, variable_label, variable_concept_entity, variable_concept_caracteristic, dataType, unit);
                            VA_list.Add(VA);
                        }
                    }
                    catch (Exception e)
                    {
                        using (StreamWriter sw = System.IO.File.AppendText(debugFile))
                        {
                            sw.WriteLine(DateTime.Now.ToString("yyyy-MM-ddThh:mm:ssTZD") + " : analysis summary called : dataset " + id + " has an error : " + e.ToString());
                        }
                        Debug.WriteLine(e.ToString());
                    }
                 }
            }
            
            Data_container_analytics datacontaineranalytics = new Data_container_analytics();

            ViewData["datacontaineranalytics"] = datacontaineranalytics;

            ViewData["VA_list"] = VA_list;

            //debugging file
            using (StreamWriter sw = System.IO.File.AppendText(DebugFilePath))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-ddThh:mm:ssTZD") + " : Analytic scalled: portal statistics - VA_list " + VA_list.ToString());
            }

            return View("Index");
        }
        
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
                    //sb.AppendLine(
                    //    va.dataset_id.ToString() + "," +
                    //    va.owner.Replace(",", "-") + "," +
                    //    va.project.Replace(",", "-") + "," +
                    //    Concepts_count.ToString() + "," +
                    //    va.variable_id.Count.ToString() + "," +
                    //    va.variable_id[0].ToString() + "," +
                    //    va.variable_label[0] + "," +
                    //    va.variable_concept_entity[0] + "," +
                    //    va.variable_concept_caracteristic[0] + "," +
                    //    va.dataType[0] + "," +
                    //    va.unit[0]);
                    //
                    //for (int kk = 1; kk < va.variable_id.Count; kk++)
                    //{
                    //    sb.AppendLine(
                    //        ", , , , , " +
                    //        va.variable_id[kk].ToString() + "," +
                    //        va.variable_label[kk].ToString() + "," +
                    //        va.variable_concept_entity[kk].ToString() + "," +
                    //        va.variable_concept_caracteristic[kk].ToString() + "," +
                    //        va.unit[kk].ToString() + "," +
                    //        va.dataType[kk].ToString());
                    //}
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