using System;
using System.Collections.Generic;
using BExIS.Dlm.Services.Data;
using BExIS.Dlm.Entities.DataStructure;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using BExIS.Modules.Asm.UI.Models;
using BExIS.Modules.Rpm.UI.Models;
using System.Web.Configuration;
using System.IO;
using Vaiona.Utils.Cfg;
using System.Web.Mvc;
using BExIS.Dlm.Entities.Data;
using System.Linq;
using System.Threading.Tasks;
using Vaiona.Logging;

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
                string url = BaseAdress + "/api/Statistics/reset";
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
                    LoggerFactory.GetFileLogger().LogCustom(ds.Id + " --> " + noColumns + " --> " + noRows + " --> " + noRows* noColumns);
                    somme = somme + (noRows * noColumns);
                }
            }
            LoggerFactory.GetFileLogger().LogCustom("Sum total : " + somme );
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


        /// //////////////////////////////////////////////////////

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
                long somme = noRows * noColumns;

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
        /*
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
        */
    }
    
}