using BExIS.Aam.Entities.Mapping;
using BExIS.Aam.Services;
using BExIS.Dlm.Services.Data;
using BExIS.Modules.Asm.UI.Models;
using F23.StringSimilarity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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

        static String projectTerminolgies = Path.Combine(AppConfiguration.GetModuleWorkspacePath("ASM"), "Project-terminologies.csv");

        public static Dictionary<string, List<string>> dict_ = new Dictionary<string, List<string>>();

        static String dataset_pnk = Path.Combine(AppConfiguration.GetModuleWorkspacePath("ASM"), "PNK dataset_links.csv");
        static String well_coordinates = Path.Combine(AppConfiguration.GetModuleWorkspacePath("DDM"), "Interactive Search", "D03_well coordinates_20180525.json");

        public ActionResult Summary(long id)
        {
            return PartialView("Summary", id);
        }

        #region classification
        public ActionResult SemanticSummary(long id)
        {
            return PartialView("classify", id);
        }
        public async System.Threading.Tasks.Task<ActionResult> classify(string dataset, Boolean semantic_flag)
        {
            string username = this.ControllerContext.HttpContext.User.Identity.Name;
            Dictionary<string, string> dict_data = new Dictionary<string, string>();
            dict_data.Add("username", username);
            dict_data.Add("data", dataset);
            dict_data.Add("semantic_flag", semantic_flag.ToString());

            try
            {
                List<string> nodes = new List<string>();
                List<List<int>> paths = new List<List<int>>();

                List<string> terminologies = new List<string>();
                List<string> predictions = new List<string>();
                List<string> keywords = new List<string>();

                List<Predict_results> classification_results = new List<Predict_results>();

                DatasetManager dm = new DatasetManager();
                DataTable table = dm.GetLatestDatasetVersionTuples(Int64.Parse(dataset), 0, 0, true);

                List<string> projectTerminolgies_lines = System.IO.File.ReadAllLines(projectTerminolgies)
                                           .Skip(1)
                                           .Select(v => v.Trim())
                                           .ToList();

                using (var client = new HttpClient())
                {
                    string url = BaseAdress + "/api/Summary/getSummary";
                    client.BaseAddress = new Uri(url);

                    var json = JsonConvert.SerializeObject(dict_data, Newtonsoft.Json.Formatting.Indented);
                    var stringContent = new StringContent(json);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var responseTask = await client.PostAsync(url, stringContent);
                    string result = await responseTask.Content.ReadAsStringAsync();

                    JObject json_class = JObject.Parse((string)JToken.Parse(result));
                    foreach(JProperty xx in (JToken)json_class)
                    {
                        using (Aam_UriManager aam_manag_ = new Aam_UriManager())
                        {
                            List<string> labels = new List<string>();
                            foreach (string key in new List<string> { "db_match", "onto_match" } )
                            {
                                foreach (string match in json_class[xx.Name][key])
                                {
                                    List<string> uris = new List<string>();
                                    foreach (string uri in match.Replace("[", "").Replace("]", "").Replace("'", "").Replace("\"", "").Replace(" ", "").Split(','))
                                    {
                                        uris.Add((aam_manag_.get_Aam_Uri_by_uri(uri) != null) ? (aam_manag_.get_Aam_Uri_by_uri(uri).label) : uri);
                                    }
                                    labels.Add(string.Join(",", uris));
                                }
                                json_class[xx.Name][key] = JsonConvert.SerializeObject(labels, Newtonsoft.Json.Formatting.Indented);
                            }
                        }
                    }

                    JsonResult _jr_ = Json(new { data = json_class.ToString(), terminologies = JsonConvert.SerializeObject(projectTerminolgies_lines) });
                    _jr_.MaxJsonLength = Int32.MaxValue;
                    return _jr_;
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

        public ActionResult get_datasets_from_annot(string annot)
        {
            List<long> datasets = new List<long>();
            List<string> URIs = new List<string>();
            Aam_Dataset_column_annotationManager aam = new Aam_Dataset_column_annotationManager();
            Aam_UriManager aam_manag = new Aam_UriManager();
            List<Aam_Dataset_column_annotation> annots = aam.get_all_dataset_column_annotation();
            if (!annot.Contains("http"))
            {
                foreach (Aam_Uri uri in aam_manag.get_all_Aam_Uri().FindAll(x => x.label == annot))
                    URIs.Add(uri.URI);
            }
            foreach (string uri in URIs)
            {
                foreach (Aam_Dataset_column_annotation aa in annots.FindAll(x => x.entity_id.URI == uri.Trim()))
                {
                    if (!datasets.Contains(aa.Dataset.Id))
                        datasets.Add(aa.Dataset.Id);
                }
                foreach (Aam_Dataset_column_annotation aa in annots.FindAll(x => x.characteristic_id.URI == uri.Trim()))
                {
                    if (!datasets.Contains(aa.Dataset.Id))
                        datasets.Add(aa.Dataset.Id);
                }
            }

            ViewData["label"] = "No Label for URI";
            if (!annot.Contains("http"))
            {
                ViewData["label"] = annot;
            }
            else
            {
                try
                {
                    if (annots.Find(x => x.entity_id.URI == annot.Trim()).entity_id.label != "")
                        ViewData["label"] = annots.Find(x => x.entity_id.URI == annot.Trim()).entity_id.label;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                try
                {
                    if (annots.Find(x => x.characteristic_id.URI == annot.Trim()).characteristic_id.label != "")
                        ViewData["label"] = annots.Find(x => x.characteristic_id.URI == annot.Trim()).characteristic_id.label;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            string sJSONResponse = JsonConvert.SerializeObject(datasets);
            Dictionary<string, string> json = new Dictionary<string, string>();
            json.Add((string)ViewData["label"], sJSONResponse);
            return Json(JsonConvert.SerializeObject(json), JsonRequestBehavior.AllowGet);
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
            string bexis_analysis_dataset = "";
            Dataset_analysis analytics;
            using (var client = new HttpClient())
            {
                string username = this.ControllerContext.HttpContext.User.Identity.Name;
                Dictionary<string, string> dict_data = new Dictionary<string, string>();
                dict_data.Add("username", username);
                dict_data.Add("data", id.ToString());

                string url = BaseAdress + "/Api/Summary/getCategrocialAnalysis";
                client.BaseAddress = new Uri(url);

                var json = JsonConvert.SerializeObject(dict_data, Newtonsoft.Json.Formatting.Indented);
                var stringContent = new StringContent(json);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var responseTask = await client.PostAsync(url, stringContent);
                result = await responseTask.Content.ReadAsStringAsync();
                //result = "\"{  \\\"categorical\\\": [],   \\\"classification\\\": [    {      \\\"Attribute Name\\\": \\\"time\\\",       \\\"Average Categorization Probability\\\": 100.0,       \\\"Category\\\": \\\"Non Categorical\\\",       \\\"drawable\\\": \\\"False\\\"    },     {      \\\"Attribute Name\\\": \\\"Blank 1\\\",       \\\"Average Categorization Probability\\\": 100.0,       \\\"Category\\\": \\\"Non Categorical\\\",       \\\"drawable\\\": \\\"True\\\"    },     {      \\\"Attribute Name\\\": \\\"Blank 1 volumetric dilution\\\",       \\\"Average Categorization Probability\\\": 1.0416666666666665,       \\\"Category\\\": \\\"Non Categorical\\\",       \\\"drawable\\\": \\\"True\\\"    },     {      \\\"Attribute Name\\\": \\\"Blank 2\\\",       \\\"Average Categorization Probability\\\": 100.0,       \\\"Category\\\": \\\"Non Categorical\\\",       \\\"drawable\\\": \\\"True\\\"    },     {      \\\"Attribute Name\\\": \\\"Blank 2 volumetric dilution\\\",       \\\"Average Categorization Probability\\\": 1.0416666666666665,       \\\"Category\\\": \\\"Non Categorical\\\",       \\\"drawable\\\": \\\"True\\\"    },     {      \\\"Attribute Name\\\": \\\"quartz 1\\\",       \\\"Average Categorization Probability\\\": 100.0,       \\\"Category\\\": \\\"Non Categorical\\\",       \\\"drawable\\\": \\\"True\\\"    },     {      \\\"Attribute Name\\\": \\\"quartz 1 volumetric dilution\\\",       \\\"Average Categorization Probability\\\": 1.0416666666666665,       \\\"Category\\\": \\\"Non Categorical\\\",       \\\"drawable\\\": \\\"True\\\"    },     {      \\\"Attribute Name\\\": \\\"quartz 2\\\",       \\\"Average Categorization Probability\\\": 100.0,       \\\"Category\\\": \\\"Non Categorical\\\",       \\\"drawable\\\": \\\"True\\\"    },     {      \\\"Attribute Name\\\": \\\"quartz 2 volumetric dilution\\\",       \\\"Average Categorization Probability\\\": 1.0416666666666665,       \\\"Category\\\": \\\"Non Categorical\\\",       \\\"drawable\\\": \\\"True\\\"    },     {      \\\"Attribute Name\\\": \\\"quartz-illite 1\\\",       \\\"Average Categorization Probability\\\": 100.0,       \\\"Category\\\": \\\"Non Categorical\\\",       \\\"drawable\\\": \\\"True\\\"    },     {      \\\"Attribute Name\\\": \\\"quartz-illite 1 volumetric dilution\\\",       \\\"Average Categorization Probability\\\": 1.0416666666666665,       \\\"Category\\\": \\\"Non Categorical\\\",       \\\"drawable\\\": \\\"True\\\"    },     {      \\\"Attribute Name\\\": \\\"quartz-illite 2\\\",       \\\"Average Categorization Probability\\\": 100.0,       \\\"Category\\\": \\\"Non Categorical\\\",       \\\"drawable\\\": \\\"True\\\"    },     {      \\\"Attribute Name\\\": \\\"quartz-illite 2 volumetric dilution\\\",       \\\"Average Categorization Probability\\\": 1.0416666666666665,       \\\"Category\\\": \\\"Non Categorical\\\",       \\\"drawable\\\": \\\"True\\\"    },     {      \\\"Attribute Name\\\": \\\"quartz-goethite 1\\\",       \\\"Average Categorization Probability\\\": 100.0,       \\\"Category\\\": \\\"Non Categorical\\\",       \\\"drawable\\\": \\\"True\\\"    },     {      \\\"Attribute Name\\\": \\\"quartz-goethite 1 volumetric dilution\\\",       \\\"Average Categorization Probability\\\": 1.0416666666666665,       \\\"Category\\\": \\\"Non Categorical\\\",       \\\"drawable\\\": \\\"True\\\"    },     {      \\\"Attribute Name\\\": \\\"quartz-goethite 2\\\",       \\\"Average Categorization Probability\\\": 100.0,       \\\"Category\\\": \\\"Non Categorical\\\",       \\\"drawable\\\": \\\"True\\\"    },     {      \\\"Attribute Name\\\": \\\"quartz-goethite 2 volumetric dilution\\\",       \\\"Average Categorization Probability\\\": 1.0416666666666665,       \\\"Category\\\": \\\"Non Categorical\\\",       \\\"drawable\\\": \\\"True\\\"    }  ],   \\\"non_categorical\\\": [    {      \\\"counts\\\": [        0,         0,         1,         0,         0,         0,         0,         84,         8      ],       \\\"name\\\": \\\"Blank 1\\\",       \\\"values\\\": [        -0.0,         0.1,         0.21,         0.31,         0.42,         0.52,         0.63,         0.73,         0.84,         0.95      ]    },     {      \\\"counts\\\": [        0,         0,         0,         0,         0,         0,         0,         0,         0      ],       \\\"name\\\": \\\"Blank 1 volumetric dilution\\\",       \\\"values\\\": [        0.79,         0.79,         0.79,         0.79,         0.79,         0.79,         0.79,         0.79,         0.79,         0.79      ]    },     {      \\\"counts\\\": [        0,         0,         0,         0,         0,         0,         0,         1,         92      ],       \\\"name\\\": \\\"Blank 2\\\",       \\\"values\\\": [        -0.0,         0.08,         0.17,         0.26,         0.35,         0.44,         0.52,         0.61,         0.7,         0.79      ]    },     {      \\\"counts\\\": [        0,         0,         0,         0,         0,         0,         0,         0,         0      ],       \\\"name\\\": \\\"Blank 2 volumetric dilution\\\",       \\\"values\\\": [        0.79,         0.79,         0.79,         0.79,         0.79,         0.79,         0.79,         0.79,         0.79,         0.79      ]    },     {      \\\"counts\\\": [        3,         0,         0,         0,         0,         0,         0,         91,         2      ],       \\\"name\\\": \\\"quartz 1\\\",       \\\"values\\\": [        -0.01,         0.09,         0.19,         0.29,         0.38,         0.48,         0.58,         0.67,         0.77,         0.87      ]    },     {      \\\"counts\\\": [        0,         0,         0,         0,         0,         0,         0,         0,         0      ],       \\\"name\\\": \\\"quartz 1 volumetric dilution\\\",       \\\"values\\\": [        0.84,         0.84,         0.84,         0.84,         0.84,         0.84,         0.84,         0.84,         0.84,         0.84      ]    },     {      \\\"counts\\\": [        0,         0,         0,         0,         0,         0,         0,         92,         1      ],       \\\"name\\\": \\\"quartz 2\\\",       \\\"values\\\": [        -0.0,         0.09,         0.19,         0.29,         0.39,         0.49,         0.59,         0.69,         0.79,         0.89      ]    },     {      \\\"counts\\\": [        0,         0,         0,         0,         0,         0,         0,         0,         0      ],       \\\"name\\\": \\\"quartz 2 volumetric dilution\\\",       \\\"values\\\": [        0.85,         0.85,         0.85,         0.85,         0.85,         0.85,         0.85,         0.85,         0.85,         0.85      ]    },     {      \\\"counts\\\": [        0,         0,         0,         0,         0,         85,         6,         2,         0      ],       \\\"name\\\": \\\"quartz-illite 1\\\",       \\\"values\\\": [        -0.0,         0.1,         0.2,         0.3,         0.41,         0.51,         0.61,         0.72,         0.82,         0.92      ]    },     {      \\\"counts\\\": [        0,         0,         0,         0,         0,         0,         0,         0,         0      ],       \\\"name\\\": \\\"quartz-illite 1 volumetric dilution\\\",       \\\"values\\\": [        0.84,         0.84,         0.84,         0.84,         0.84,         0.84,         0.84,         0.84,         0.84,         0.84      ]    },     {      \\\"counts\\\": [        3,         0,         0,         0,         0,         84,         6,         2,         1      ],       \\\"name\\\": \\\"quartz-illite 2\\\",       \\\"values\\\": [        -0.01,         0.1,         0.22,         0.33,         0.44,         0.55,         0.66,         0.77,         0.88,         0.99      ]    },     {      \\\"counts\\\": [        0,         0,         0,         0,         0,         0,         0,         0,         0      ],       \\\"name\\\": \\\"quartz-illite 2 volumetric dilution\\\",       \\\"values\\\": [        0.84,         0.84,         0.84,         0.84,         0.84,         0.84,         0.84,         0.84,         0.84,         0.84      ]    },     {      \\\"counts\\\": [        0,         0,         0,         0,         0,         0,         0,         27,         66      ],       \\\"name\\\": \\\"quartz-goethite 1\\\",       \\\"values\\\": [        -0.0,         0.09,         0.19,         0.29,         0.38,         0.48,         0.58,         0.68,         0.77,         0.87      ]    },     {      \\\"counts\\\": [        0,         0,         0,         0,         0,         0,         0,         0,         0      ],       \\\"name\\\": \\\"quartz-goethite 1 volumetric dilution\\\",       \\\"values\\\": [        0.84,         0.84,         0.84,         0.84,         0.84,         0.84,         0.84,         0.84,         0.84,         0.84      ]    },     {      \\\"counts\\\": [        0,         0,         0,         0,         0,         0,         0,         35,         59      ],       \\\"name\\\": \\\"quartz-goethite 2\\\",       \\\"values\\\": [        -0.0,         0.09,         0.19,         0.29,         0.38,         0.48,         0.58,         0.67,         0.77,         0.87      ]    },     {      \\\"counts\\\": [        0,         0,         0,         0,         0,         0,         0,         0,         0      ],       \\\"name\\\": \\\"quartz-goethite 2 volumetric dilution\\\",       \\\"values\\\": [        0.84,         0.84,         0.84,         0.84,         0.84,         0.84,         0.84,         0.84,         0.84,         0.84      ]    }  ]}\"";
                bexis_analysis_dataset = (JObject.Parse(JToken.Parse(result).ToString())["dataset"]).ToString();
                analytics = new Dataset_analysis(JObject.Parse((string)JToken.Parse(result)), (int)id);
            }

            List<List<string>> values = new List<List<string>>();
            List<List<string>> labels = new List<List<string>>();
            foreach (var item in analytics.categorical)
            {
                var temp = item.values.Select(i => i.ToString()).ToList();
                labels.Add(new List<string> { item.name, "count" });
                values.Add(new List<string> { temp.ToString(), item.counts.ToString() });
            }
            foreach (var item in analytics.non_Categorical)
            {
                labels.Add(new List<string> { item.name, "count" });
                values.Add(new List<string> { item.values.ToString(), item.counts.ToString() });
            }
            List<List<string>> results = new List<List<string>>();
            foreach (var item in analytics.category_Classifications)
            {
                //results.Add(new List<string> { item.name, item.categorization_probability.ToString(), item.category });
                results.Add(new List<string> { item.name, item.categorization_probability.ToString() });
            }
            List<List<string>> headers = new List<List<string>>();
            //headers.Add(new List<string> { "Attribute Name", "Average Categorisation Probability", "Category" });
            headers.Add(new List<string> { "Attribute Name", "Average Categorisation Probability" });

            ViewData["id"] = analytics.id.ToString();
            ViewData["error"] = "";
            ViewData["values"] = values;
            ViewData["labels"] = labels;
            ViewData["headers"] = headers;
            ViewData["table"] = results;

            List<KeyValuePair<int, Object>> data_ranges = new List<KeyValuePair<int, Object>>();
            List<double> nullsCount = new List<double>();

            for (int index__ = 0; index__ < results.Count; index__++)
            {
                string var_name = results[index__][0];
                JToken obj = JToken.Parse(bexis_analysis_dataset).Where(x => x["variableName"].ToString() == var_name).FirstOrDefault();
                nullsCount.Add(JToken.Parse(bexis_analysis_dataset).Where(x => x["variableName"].ToString() == var_name).FirstOrDefault()["missingValues"].Count());
                string min = obj["min"].ToString();
                string max = obj["max"].ToString();
                var data_range = new
                {
                    min = min,
                    max = max
                };
                ;
                data_ranges.Add(new KeyValuePair<int, Object>(index__, JObject.Parse(new JavaScriptSerializer().Serialize(data_range))));
            }
            //var json = new JavaScriptSerializer().Serialize(data_ranges[0].Value)
            ViewData["data_ranges"] = data_ranges;
            ViewData["nullsCount"] = nullsCount;


            JObject graphData = new JObject();
            int k = 0;
            foreach (var item in analytics.categorical)
            {
                JArray Xarray = new JArray { item.name, "['" + string.Join("','", item.values) + "']" };
                JArray Yarray = new JArray { "count", "['" + string.Join("','", item.counts) + "']" };

                JArray jArray = new JArray { Xarray, Yarray };

                graphData[k.ToString()] = jArray;
                k = k + 1;
            }
            foreach (var item in analytics.non_Categorical)
            {
                JArray Xarray = new JArray { item.name, "['" + string.Join("','", item.values) + "']" };
                JArray Yarray = new JArray { "count", "['" + string.Join("','", item.counts) + "']" };

                string a = "\"['" + string.Join("','", item.values) + "']\"";
                string b = string.Join("','", item.values);

                JArray jArray = new JArray { Xarray, Yarray };

                graphData[k.ToString()] = jArray;
                k = k + 1;
            }

            string graphData_str = graphData.ToString();
            graphData_str = graphData_str.Replace("\n", string.Empty).Replace("\r", String.Empty).Replace("\t", String.Empty);
            ViewData["graphData"] = graphData_str;


            return PartialView("CategoralAnalysisAsync");
        }



        #endregion

        #region sampling summary
        public ActionResult Specialdatasetanalysis()
        {
            return PartialView("SpecialdatasetanalysisV1");
        }

        public async System.Threading.Tasks.Task<string> Filter_ApplyAsync(
            string welllocation = "", string year = "", string filtersize = "",
            string GroupName = "", string NameFIlter = "",
            String Season_dict = "", string column = "-1", string row = "-1", string flag = "false")
        {
            string row_ = "";
            //dict_ = dict_.OrderByDescending(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            if (row != "-1")
                row_ = dict_.ElementAt(Int32.Parse(row)).Key;
            string col = "";
            if (column != "-1")
                col = dict_.ElementAt(0).Value[Int32.Parse(column) - 1];
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
                var stringContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var responseTask = await client.PostAsync(url, stringContent);
                result = await responseTask.Content.ReadAsStringAsync();
            }

            SpecialDataset_analysis keys = new SpecialDataset_analysis(JObject.Parse((string)JToken.Parse(result)));
            if ((keys.key2.Count > 2) || (keys.key1.Count > 2))
                try
                {
                    if (column == "-1")
                    {
                        dict_ = new Dictionary<string, List<string>>();
                        foreach (var element in keys.key1)
                        {
                            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(element));
                            foreach (KeyValuePair<string, string> kvp in dict)
                            {
                                if (!dict_.ContainsKey(kvp.Key))
                                    dict_.Add(kvp.Key.Replace(',', ' '), new List<string>());
                                List<string> value = new List<string>();
                                dict_.TryGetValue(kvp.Key, out value);
                                value.Add(kvp.Value);
                                dict_[kvp.Key] = value;
                            }
                        }
                        dict_ = dict_.OrderByDescending(x => x.Key.Length).ToDictionary(x => x.Key, x => x.Value);
                        dict_.Remove("id");

                        string ss = JsonConvert.SerializeObject(dict_);
                        return ss;
                    }
                    else
                    {
                        Dictionary<string, List<string>> dict_PNKs = new Dictionary<string, List<string>>();
                        foreach (var element in keys.key2)
                        {
                            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(element));
                            //JObject obj = element.Value;
                            foreach (KeyValuePair<string, string> kvp in dict)
                            {
                                if (!dict_PNKs.ContainsKey(kvp.Key))
                                    dict_PNKs.Add(kvp.Key.Replace(',', ' '), new List<string>());
                                List<string> value = new List<string>();
                                dict_PNKs.TryGetValue(kvp.Key, out value);
                                value.Add(kvp.Value);
                                dict_PNKs[kvp.Key] = value;
                            }
                        }
                        //dict_PNKs = dict_PNKs.OrderByDescending(x => x.Key.Length).ToDictionary(x => x.Key, x => x.Value);
                        dict_PNKs.Remove("id");
                        string ss = JsonConvert.SerializeObject(dict_PNKs);
                        return ss;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return ("");
                }
            return ("");
        }

        public String parse_Json_location(String location_coordinates)
        {
            String Gps_coordinates_for_wells = Path.Combine(AppConfiguration.GetModuleWorkspacePath("DDM"), "Interactive Search", well_coordinates);
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

        public string get_datasets_from_pnk(string pnk)
        {
            List<string> datasets_ids = new List<string>();
            //dataset_pnk
            try
            {
                using (var reader = new StreamReader(dataset_pnk))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        List<string> tmp = line.Split(',').ToList<string>();
                        if (tmp.Count > 1)
                        {
                            if (tmp[0].ToLower().Trim() == pnk.ToLower().Trim())
                            {
                                for (int i = 1; i < tmp.Count; i++)
                                {
                                    if (tmp[i] != "") datasets_ids.Add(tmp[i]);
                                    i++;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The File could not be read:");
                Console.WriteLine(e.Message);
            }
            string output = new JavaScriptSerializer().Serialize(datasets_ids);
            return output;
        }

        #endregion

        public double similarity_(string a, string b)
        {

            return similarity(a, b);
        }
        public string get_label(string uri)
        {
            using (Aam_UriManager am_manag = new Aam_UriManager())
            {
                try
                {
                    return am_manag.get_Aam_Uri_by_uri(uri.Replace('\'', ' ').Trim())?.label;
                }
                catch (Exception exc)
                {
                    LoggerFactory.GetFileLogger().LogCustom(exc.Message + exc.StackTrace);
                    return "";
                }
            }
        }
    }
}