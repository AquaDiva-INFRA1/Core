using BExIS.Aam.Entities.Mapping;
using BExIS.Aam.Services;
using BExIS.Dlm.Services.Data;
using BExIS.Modules.Asm.UI.Models;
using BExIS.Security.Services.Subjects;
using F23.StringSimilarity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Vaiona.Logging;
using Vaiona.Utils.Cfg;

namespace BExIS.Modules.Asm.UI.Controllers
{
    public class SummaryAnalysisController : Controller
    {
        static string BaseAdress = WebConfigurationManager.AppSettings["BaseAdress"];

        List<string> domains = new List<string>() { "Sites", "BioGeoChemichals", "Cycles", "Matter Cycles",
            "Signals", "Phages", "Surface Inputs", "Gases", "Tree Matter", "Groundwater BioGeoChem", "Viruses", "Pathways" };
        static String projectTerminolgies = Path.Combine(AppConfiguration.GetModuleWorkspacePath("ASM"), "Project-terminologies.csv");

        public static Dictionary<string, List<string>> dict_ = new Dictionary<string, List<string>>();

        #region classification
        public ActionResult Summary(long id)
        {
            return PartialView("classify", id);
        }
        public async System.Threading.Tasks.Task<ActionResult> classify(string dataset)
        {
            string username = this.ControllerContext.HttpContext.User.Identity.Name;
            Dictionary<string, string> dict_data = new Dictionary<string, string>();
            dict_data.Add("username", username );
            dict_data.Add("data", dataset);

            try
            {
                List<string> nodes = new List<string>();
                List<List<int>> paths = new List<List<int>>();
                List<string> terminologies = new List<string>();
                List<string> predictions = new List<string>();
                List<string> keywords = new List<string>();
                List<Input> classification_results = new List<Input>();

                using (var client = new HttpClient())
                {
                    string url = BaseAdress + "/api/Summary/getSummary";
                    client.BaseAddress = new Uri(url);

                    var json = JsonConvert.SerializeObject(dict_data, Newtonsoft.Json.Formatting.Indented);
                    var stringContent = new StringContent(json);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var responseTask = await client.PostAsync(url, stringContent);
                    string result = await responseTask.Content.ReadAsStringAsync();

                    // parsing results - matches to the ontologies
                    dynamic json_class = ((dynamic)Newtonsoft.Json.JsonConvert.DeserializeObject(result));

                    int index = 0;
                    foreach (JProperty xx in json_class)
                    {
                        Input inp = JsonConvert.DeserializeObject<Input>(JObject.Parse(xx.Value.ToString())["input"].ToString().Replace("[]", "\"\""));
                        inp.onto_match = JsonConvert.DeserializeObject<List<string>>(JObject.Parse(xx.Value.ToString())["onto_match"].ToString());
                        inp.db_match = JsonConvert.DeserializeObject<List<string>>(JObject.Parse(xx.Value.ToString())["db_match"].ToString().Replace("[]", "\"\""));
                        inp.db_no_node = JsonConvert.DeserializeObject<List<string>>(JObject.Parse(xx.Value.ToString())["db_no_node"].ToString());
                        inp.onto_no_node = JsonConvert.DeserializeObject<List<string>>(JObject.Parse(xx.Value.ToString())["onto_no_node"].ToString());
                        inp.onto_no_path = JsonConvert.DeserializeObject<List<string>>(JObject.Parse(xx.Value.ToString())["onto_no_path"].ToString());
                        inp.onto_target_file = JsonConvert.DeserializeObject<List<string>>(JObject.Parse(xx.Value.ToString())["onto_target_file"].ToString());
                        inp.predicted_class = JObject.Parse(xx.Value.ToString())["predicted_class"].ToString().Replace("[]", "\" \"");
                        inp.db_no_path = JsonConvert.DeserializeObject<List<string>>(JObject.Parse(xx.Value.ToString())["db_no_path"].ToString().Replace("[]", "\"\""));

                        classification_results.Add(inp);
                        foreach (string ss in inp.predicted_class.Split(';'))
                        {
                            if ((ss != "\" \"") && (!predictions.Contains(domains[Int32.Parse(ss)])))
                            {
                                predictions.Add(domains[Int32.Parse(ss)]);
                            }
                        }
                        index++;
                    }

                    DatasetManager dm = new DatasetManager();
                    DataTable table = dm.GetLatestDatasetVersionTuples(Int64.Parse(dataset), 0, 0, true);
                    dm.Dispose();

                    foreach (string pred in predictions)
                    {
                        using (var reader = new StreamReader(projectTerminolgies))
                        {
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                List<string> tmp = line.Split(',').ToList<string>();
                                if (tmp.Count > 1)
                                {
                                    if (tmp[1].Trim() == pred.Trim())
                                    {
                                        foreach (string keyword in tmp[2].Replace("  ", " ").Split('"'))
                                        {
                                            foreach (DataColumn dc in table.Columns)
                                            {
                                                if ((!keywords.Contains(keyword.Replace("\"", "").Trim())) &&
                                                    (similarity(dc.Caption.Trim(), keyword.Replace("\"", "").Trim()) > 0.5))
                                                {
                                                    keywords.Add(keyword.Replace("\"", "").Trim());
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                    Aam_UriManager aam_manag = new Aam_UriManager();
                    foreach (Input inp in classification_results)
                    {
                        if (inp.db_match != null)
                        {
                            foreach (string s in inp.db_match)
                            {
                                List<int> path = new List<int>();
                                foreach (string el in s.Replace("[", "").Replace("]", "").Replace("'", "").Replace("\"", "").Replace(" ", "").Split(','))
                                {
                                    if (el.Contains("http"))
                                    {
                                        Aam_Uri aam_uri = aam_manag.get_Aam_Uri_by_uri(el);
                                        string label = aam_uri != null ? aam_uri.label : el;
                                        if (nodes.FindAll(x => x == label).Count == 0)
                                        {
                                            nodes.Add(label);
                                        }
                                        int index_in_list = nodes.FindIndex(x => x == label);
                                        path.Add(index_in_list);
                                    }
                                }
                                if (path.Count() > 0) paths.Add(path);
                            }
                        }
                        if (inp.onto_match != null)
                        {
                            foreach (string s in inp.onto_match)
                            {
                                List<int> path = new List<int>();
                                foreach (string el in s.Replace("[", "").Replace("]", "").Replace("'", "").Replace("\"", "").Replace(" ", "").Split(','))
                                {
                                    if (el.Contains("http"))
                                    {
                                        Aam_Uri aam_uri = aam_manag.get_Aam_Uri_by_uri(el);
                                        string label = aam_uri != null ? aam_uri.label : el;
                                        if (nodes.FindAll(x => x == label).Count == 0)
                                        {
                                            nodes.Add(label);
                                        }
                                        int index_in_list = nodes.FindIndex(x => x == label);
                                        path.Add(index_in_list);
                                    }
                                }
                                if (path.Count() > 0) paths.Add(path);
                            }
                        }
                    }
                    var json_ = new JavaScriptSerializer().Serialize(
                        new
                        {
                            nodes = nodes,
                            links = paths,
                            id=dataset,
                            keywords = keywords,
                            class_results = classification_results
                        }
                        );
                    return Json(json_, JsonRequestBehavior.AllowGet);

                    //ViewData["id"] = dataset;
                    //ViewData["label"] = "";
                    //ViewData["keywords"] = keywords;
                    //classification.keywords = keywords;
                    //classification.class_results = classification_results;
                }
            }
            catch (Exception e)
            {
                LoggerFactory.GetFileLogger().LogCustom(e.Message);
                LoggerFactory.GetFileLogger().LogCustom(e.StackTrace);
            }
            return Json(new JavaScriptSerializer().Serialize(new
                        {
                            nodes = "[]",
                            links = "[]",
                            id = dataset,
                            keywords = "[]",
                            class_results = "[]"
                        }
                        ), JsonRequestBehavior.AllowGet);
        }
        private double similarity(string a, string b)
        {
            List<double> similarities = new List<double>();
            double output = 0.0;

            var l = new NormalizedLevenshtein();
            similarities.Add(l.Similarity(a, b));
            var jw = new JaroWinkler();
            similarities.Add(jw.Similarity(a, b));
            var jac = new Jaccard();
            similarities.Add(jac.Similarity(a, b));

            foreach (double sim in similarities)
            {
                output += sim;
            }

            return output / similarities.Count;
        }
        #endregion

        #region categorical analysis
        public async System.Threading.Tasks.Task<ActionResult> CategoralAnalysisAsync(long id)
        {
            string result = "";
            using (var client = new HttpClient())
            {
                string username = this.ControllerContext.HttpContext.User.Identity.Name;
                Dictionary<string, string> dict_data = new Dictionary<string, string>();
                dict_data.Add("username", username);
                dict_data.Add("data", id.ToString());

                string url = BaseAdress + "/api/Summary/getCategrocialAnalysis";
                client.BaseAddress = new Uri(url);

                var json = JsonConvert.SerializeObject(dict_data, Newtonsoft.Json.Formatting.Indented);
                var stringContent = new StringContent(json);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var responseTask = await client.PostAsync(url, stringContent);
                result = await responseTask.Content.ReadAsStringAsync();
            }
            return PartialView("classify", JObject.Parse(result));
        }
        #endregion

        #region sampling summary
        public async System.Threading.Tasks.Task<ActionResult> Filter_ApplyAsync(
            string welllocation = "", string year = "", string filtersize = "", 
            string GroupName = "", string NameFIlter = "",
            String Season_dict = "", string column = "-1", string row = "-1", Boolean flag = false)
        {
            string row_ = "";
            dict_ = dict_.OrderByDescending(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            if (row != "-1")
                row_ = dict_.ElementAt(dict_.Count - Int32.Parse(row) - 1).Key;
            string col = "";
            if (column != "-1")
                col = dict_.ElementAt(dict_.Count - 1).Value[Int32.Parse(column) - 1];
            string param = "?year=" + year + "&filtersize=" + filtersize + "&GroupName=" + GroupName + "&Season_dict=" + Season_dict + "&column=" + col + "&row=" + row_;
            if (welllocation != "")
                param = param + "&welllocation=" + parse_Json_location(welllocation);
            if (NameFIlter != "")
                param = param + "&PIName=" + NameFIlter;

            string result = "";
            using (var client = new HttpClient())
            {
                string username = this.ControllerContext.HttpContext.User.Identity.Name;
                Dictionary<string, string> dict_data = new Dictionary<string, string>();
                dict_data.Add("username", username);
                dict_data.Add("data", param);

                string url = BaseAdress + "/api/Summary/getSamplingSummary";
                client.BaseAddress = new Uri(url);

                var json = JsonConvert.SerializeObject(dict_data, Newtonsoft.Json.Formatting.Indented);
                var stringContent = new StringContent(json);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var responseTask = await client.PostAsync(url, stringContent);
                result = await responseTask.Content.ReadAsStringAsync();
            }
            return PartialView("classify", JObject.Parse(result));
        }

        private String parse_Json_location(String location_coordinates)
        {
            String Gps_coordinates_for_wells = Path.Combine(AppConfiguration.GetModuleWorkspacePath("DDM"), "Interactive Search", "D03_well coordinates_20180525.json");
            //"LatLng(51.080258, 10.42626)"
            using (StreamReader r = new StreamReader(Gps_coordinates_for_wells))
            {
                string json = r.ReadToEnd();
                List<coordinates_GPS> items = JsonConvert.DeserializeObject<List<coordinates_GPS>>(json);
                if (location_coordinates.Length > 0)
                {
                    string lon = location_coordinates.Substring(location_coordinates.IndexOf('(') + 1, location_coordinates.IndexOf(',') - location_coordinates.IndexOf('(') - 1);
                    string lat = location_coordinates.Substring(location_coordinates.IndexOf(", ") + 2, location_coordinates.IndexOf(')') - location_coordinates.IndexOf(',') - 2);

                    foreach (coordinates_GPS item in items)
                    {
                        try
                        {
                            if ((item.Lat.ToString().IndexOf(lon.Substring(0, lon.Length - 1)) > -1) && (item.Lon.ToString().IndexOf(lat.Substring(0, lat.Length - 1)) > -1))
                            {
                                return item.Well_name;
                            }
                        }
                        catch (NullReferenceException e)
                        {
                            LoggerFactory.GetFileLogger().LogCustom(e.Message);
                            LoggerFactory.GetFileLogger().LogCustom(e.StackTrace);
                        }

                    }
                }
                else
                {
                    return json;
                }
            }
            return "";
        }
        #endregion
    }
}