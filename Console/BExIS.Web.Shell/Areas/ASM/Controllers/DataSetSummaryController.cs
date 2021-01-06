using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Diagnostics;
using BExIS.Dlm.Services.Data;
using System.Linq;
using BExIS.Dlm.Entities.Data;
using System.IO;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using BExIS.IO.Transform.Output;
using BExIS.IO;
using Vaiona.Logging;
using BExIS.Modules.Asm.UI.Models;
using System.Configuration;
using System.Web.Configuration;
using Vaiona.Utils.Cfg;
using Newtonsoft.Json.Linq;
using BExIS.Security.Services.Subjects;
using System.Net;
using System.Text;
using System.Net.Http;
using System.Web;
using System.Net.Sockets;
using BExIS.Dlm.Entities.DataStructure;
using BExIS.Dlm.Services.DataStructure;
using System.Data;
using BExIS.Xml.Helpers;
using Npgsql;
using System.Xml;
using Vaiona.Web.Mvc;
using BExIS.Aam.Services;
using BExIS.Aam.Entities.Mapping;
using Deedle;
using CenterSpace.NMath.Core;
using CenterSpace.NMath.Stats;
using System.IO.Compression;
using Ionic.Zip;
using F23.StringSimilarity;

namespace BExIS.Modules.ASM.UI.Controllers
{
    public class DataSetSummaryController : Controller , IController
    {
        private XmlDatasetHelper xmlDatasetHelper = new XmlDatasetHelper();

        // GET: DataSummary
        private static String username = "hamdihamed";
        private static String password = "hamdi1992";
        private static string FTPAddress = WebConfigurationManager.AppSettings["FTPAddress"]; //"ftp://aquadiva-analysis2.inf-bb.uni-jena.de:21";
        private static string AnalAddress = WebConfigurationManager.AppSettings["AnalAddress"]; //"http://aquadiva-analysis2.inf-bb.uni-jena.de:8080";

        public static Dictionary<string, List<string>> dict_ = new Dictionary<string, List<string>>();

        static Dictionary<string, List<string>> project_list_names_ = new Dictionary<string, List<string>> {
            {"A01", new List<string> { "Wick", "Antonis Chatzinotas" } },
            {"A02", new List<string> { "Pohnert", "Gleixner" } },
            {"A03", new List<string> { "Kusel", "Martin Taubert", "Jurgen Popp" , "Petra Rosch" } },
            {"A04", new List<string> { "Martin von Bergen", "Jehmlich" } },
            {"A05", new List<string> { "Ulrich Brose", "Bjorn Rall" } },
            {"A06", new List<string> { "Manja Marz" } },
            {"B01", new List<string> { "Beate Michalzik", "Nicole van Dam" } },
            {"B02", new List<string> { "Anke Hildebrandt " } },
            {"B03", new List<string> { "Susan Trumbore", "Torsten Frosch" } },
            {"B04", new List<string> { "Sabine Attinger" } },
            {"B05", new List<string> { "Martina Herrmann" } },
            {"C03", new List<string> { "Totsche" } },
            {"C05", new List<string> { "Totsche", "Ulrich S. Schubert" } },
            {"D01", new List<string> { "Birgitta Konig-Ries", "Udo Hahn" } },
            {"D02", new List<string> { "Anke Hildebrandt", "Kusel" } },
            {"D03", new List<string> { "Totsche", "Kusel" } }
        };

        List<string> domains = new List<string>() { "Sites", "BioGeoChemichals", "Cycles", "Matter Cycles",
            "Signals", "Phages", "Surface Inputs", "Gases", "Tree Matter", "Groundwater BioGeoChem", "Viruses", "Pathways" };


        static string Conx = ConfigurationManager.ConnectionStrings[1].ConnectionString;

        static string python_path = Path.GetFullPath(WebConfigurationManager.AppSettings["python_path"]);
        static string python_script = Path.GetFullPath(WebConfigurationManager.AppSettings["python_script"]);
        static string output_Folder = Path.GetFullPath(WebConfigurationManager.AppSettings["output_Folder"]);
        static string datapath = Path.GetFullPath(WebConfigurationManager.AppSettings["DataPath"]);

        string[] allowed_extention = new string[] { ".csv", ".xlsx", ".xls" };

        static List<string> lines = new List<string>();
        static String debugFile = Path.Combine(AppConfiguration.GetModuleWorkspacePath("ASM"), "debug.txt");

        static String datasetsepcial = Path.Combine(AppConfiguration.GetModuleWorkspacePath("ASM"), "dataset361.csv");
        static String Gps_coordinates_for_wells = Path.Combine(AppConfiguration.GetModuleWorkspacePath("DDM"), "Interactive Search", "D03_well coordinates_20180525.json");
        static String dataset_pnk = Path.Combine(AppConfiguration.GetModuleWorkspacePath("ASM"), "PNK dataset_links.csv");

        static String projectTerminolgies = Path.Combine(AppConfiguration.GetModuleWorkspacePath("ASM"), "Project-terminologies.csv");

        static List<Input> classification_results = new List<Input>();

        public ActionResult Summary(long id)
        {
            return PartialView("Summary" ,  id );
        }
        public ActionResult CategoralAnalysis(long id)
        {
            //debugging file
            ViewData["id"] = id.ToString();
            using (StreamWriter sw = System.IO.File.AppendText(debugFile))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-ddThh:mm:ssTZD") + " : Data Summary scalled: CategoralAnalysis2 for dataset id : "+id );
            }

            if (id == 361)
            {
                return RedirectToAction("Specialdatasetanalysis");
            }
            ViewData["error"] = "";
            DatasetManager datasetManager = new DatasetManager();
            try
            {
                DatasetVersion datasetVersion = datasetManager.GetDatasetLatestVersion(id);
                AsciiWriter writer = new AsciiWriter(TextSeperator.comma);
                OutputDataManager ioOutputDataManager = new OutputDataManager();
                string title = id.ToString();
                string path = "";

                string message = string.Format("dataset {0} version {1} was downloaded as txt.", id,
                                                datasetVersion.Id);
                path = ioOutputDataManager.GenerateAsciiFile(id, "text/csv",true);
                
                LoggerFactory.LogCustom(message);

                string absolute_file_path = File(path, "text/csv", title + ".csv").FileName.ToString();

                Debug.WriteLine("Dataset id : " + id + "has path : " + absolute_file_path);
                string extension = Path.GetExtension(absolute_file_path);

                Frame<int, string> df = Frame.ReadCsv(absolute_file_path, separators: ";");

                if (allowed_extention.Contains(extension))
                {
                    String output_ = UploadFiletoAnalysis(absolute_file_path);
                    String output = output_.Split(new string[] { "\n\n\n" }, StringSplitOptions.None)[1];
                    //debugging file
                    using (StreamWriter sw = System.IO.File.AppendText(debugFile))
                    {
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-ddThh:mm:ssTZD") + " : Data Summary scalled: results of analysis for dataset id : " + output);
                    }

                    //string progToRun = python_script;
                    string outputFolder = output_Folder;
                    List <string> lines_ = output.Split('*').ToList();
                    lines = lines_[0].Split(Environment.NewLine.ToCharArray()).ToList();
                    int index = lines.IndexOf("Numerical");
                    lines = lines.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

                    List<List<string>> values = new List<List<string>>();
                    List<List<string>> labels = new List<List<string>>();

                    for (int k = 0; k < lines.Count; k++)
                    {
                        string x_label = lines[k];
                        string x_values = lines[k + 1];
                        string y_label = lines[k + 2];
                        string y_values = lines[k + 3];
                        k = k + 3;
                        List<string> bocket = new List<string>();
                        bocket.Add(x_label);
                        bocket.Add(y_label);
                        labels.Add(bocket);
                        bocket = new List<string>();
                        bocket.Add(x_values);
                        bocket.Add(y_values);
                        values.Add(bocket);
                        bocket = new List<string>();
                    }

                    var jsonSerialiser = new JavaScriptSerializer();
                    var json = jsonSerialiser.Serialize(lines);

                    var json_ = JsonConvert.SerializeObject(lines);

                    datasetManager.Dispose();

                    FileInfo myfileinf = new FileInfo(absolute_file_path);
                    try
                    {
                        //myfileinf.Delete();
                    }
                    catch (Exception exec)
                    {
                        Debug.WriteLine(exec.Message);
                    }

                    String table = output_.Split(new string[] { "\n\n\n" }, StringSplitOptions.None)[0];
                    List<string> result = table.Split(System.Environment.NewLine.ToCharArray()).ToList<string>();
                    result = result.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                    List<List<string>> results = new List<List<string>>();
                    List<List<string>> headers = new List<List<string>>();
                    int kk = 0;
                    foreach (string s in result)
                    {
                        if (kk != result.Count - 1)
                            results.Add(s.Split(';').ToList<string>());
                        else headers.Add(s.Split(';').ToList<string>());
                        kk++;
                    }

                    // get guessed system types : 
                    List<System.Type> column_types = df.ColumnTypes.ToList<System.Type>();
                    // get data ranges observations : 
                    List<List<KeyValuePair<int, object>>> data_ranges = new List<List<KeyValuePair<int, object>>>();
                    for (int index__ = 0; index__ < results.Count; index__++)
                    {
                        data_ranges.Add(new List<KeyValuePair<int, object>>());
                    }
                    List<KeyValuePair<string, ObjectSeries<int>>> observations_keys = df.ColumnsDense.Observations.ToList();
                    foreach(KeyValuePair<string, ObjectSeries<int>> obs_key in observations_keys)
                    {
                        int index_matching_from_results_toobservations = results.FindIndex(x => x[0] == obs_key.Key);
                        var k = obs_key.Value.Observations.OrderBy(x => x.Value.ToString()).ToList();
                        if ((index_matching_from_results_toobservations != -1) && (column_types[index_matching_from_results_toobservations].Name == "Decimal"))
                            data_ranges[index_matching_from_results_toobservations] = k;
                    }

                    //analyse nullls and Nans
                    DataTable dt = datasetManager.GetLatestDatasetVersionTuples(id);
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        DataColumn column = dt.Columns[i];
                        string var_name = column.Caption;
                        int conv = 0;
                        if (!Int32.TryParse(column.ColumnName.ToString().Replace("var", ""), out conv))
                        {
                            dt.Columns.Remove(column.ColumnName.ToString());
                            i = i - 1;
                        }
                        else
                        {
                            dt.Columns[column.ColumnName].ColumnName = var_name;
                        }
                    }
                    DataFrame data = new DataFrame(dt);
                    List<double> nullsCount = new List<double>();
                    for (int c = 0; c < data.Cols; c++)
                    {
                        int notNulls = StatsFunctions.NaNCount(data[data[c].Label.ToString()]);
                        nullsCount.Add((double)(data[data[c].Label.ToString()].Count - notNulls) / data[data[c].Label.ToString()].Count );
                    }


                    ViewData["headers"] = headers;
                    ViewData["table"] = results;

                    ViewData["values"] = values;
                    ViewData["labels"] = labels;

                    ViewData["column_types"] = column_types;
                    ViewData["data_ranges"] = data_ranges;
                    ViewData["nullsCount"] = nullsCount;

                    //ViewData["header"] = header;
                    //ViewData["data_lines"] = data_lines;
                    //debugging file
                    using (StreamWriter sw = System.IO.File.AppendText(debugFile))
                    {
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-ddThh:mm:ssTZD") + " : Data Summary scalled: results of analysis for dataset id : " + id+ " has finished");
                    }
                    return View("showDataSetAnalysis");
                }
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                //debugging file
                using (StreamWriter sw = System.IO.File.AppendText(debugFile))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-ddThh:mm:ssTZD") + " : Data Summary scalled: results of analysis for dataset id : " + id + " has an error : "+ex.Message);
                }
            }
            ViewData["values"] = new List<List<string>>();
            ViewData["labels"] = new List<List<string>>();
            ViewData["header"] = new List<string>();
            ViewData["data_lines"] = new List<List<string>>();
            ViewData["error"] = "No categoral information was extracted";
            ViewData["table"] = new List<List<string>>();
            

            return PartialView("showDataSetAnalysis");
        }
        
        public ActionResult Specialdatasetanalysis()
        {
            ViewData["project_list_names"] = project_list_names_;
            return PartialView("Specialdatasetanalysis");
        }

        //string should be under this form 42; 155 where dataset ids should be sepearated by ; semicolon
        
        public ActionResult classification(string ds , string flag = "" )
        {
            List<string> nodes = new List<string>();
            List<List<int>> paths = new List<List<int>>();
            List<string> terminologies = new List<string>();
            List<string> predictions = new List<string>();
            List<string> keywords = new List<string>();
            Classification result = new Classification();
            ViewData["id"] = ds;
            if (flag == "")
            {
                Aam_Dataset_column_annotationManager aam = new Aam_Dataset_column_annotationManager();
                List<Aam_Dataset_column_annotation> annots = aam.get_all_dataset_column_annotation();
                classification_results = new List<Input>();
                prepare_for_classification(datapath+ Path.DirectorySeparatorChar+"tmp_" +ds.Replace(';','_').Trim()+".txt", ds);
                string results = UploadFiletoAnalysis(datapath + Path.DirectorySeparatorChar + "tmp_" + ds.Replace(';', '_').Trim() + ".txt", "/predict?").ToString();
                dynamic json_class= ((dynamic)Newtonsoft.Json.JsonConvert.DeserializeObject(results));
            
                int index = 0;
                foreach (JProperty xx in json_class)
                {
                    Input inp = JsonConvert.DeserializeObject<Input>(JObject.Parse(xx.Value.ToString())["input"].ToString());
                    inp.onto_match = JsonConvert.DeserializeObject<List<string>>(JObject.Parse(xx.Value.ToString())["onto_match"].ToString());
                    inp.db_match = JsonConvert.DeserializeObject<List<string>>(JObject.Parse(xx.Value.ToString())["db_match"].ToString());
                    inp.db_no_node = JsonConvert.DeserializeObject<List<string>>(JObject.Parse(xx.Value.ToString())["db_no_node"].ToString());
                    inp.onto_no_node = JsonConvert.DeserializeObject<List<string>>(JObject.Parse(xx.Value.ToString())["onto_no_node"].ToString());
                    inp.onto_no_path = JsonConvert.DeserializeObject<List<string>>(JObject.Parse(xx.Value.ToString())["onto_no_path"].ToString());
                    inp.onto_target_file = JsonConvert.DeserializeObject<List<string>>(JObject.Parse(xx.Value.ToString())["onto_target_file"].ToString());
                    inp.predicted_class = JObject.Parse(xx.Value.ToString())["predicted_class"].ToString();
                    inp.db_no_path = JsonConvert.DeserializeObject<List<string>>(JObject.Parse(xx.Value.ToString())["db_no_path"].ToString());
                    
                    classification_results.Add( inp);
                    foreach (string ss in inp.predicted_class.Split(';'))
                    {
                        if ((ss != " ") && (!predictions.Contains(domains[Int32.Parse(ss)])))
                        {
                            predictions.Add(domains[Int32.Parse(ss)]);
                        }
                    }
                    index ++ ;
                }

                try
                {
                    DatasetManager dm = new DatasetManager();
                    DataTable table = dm.GetLatestDatasetVersionTuples(Int64.Parse(ds), 0, 0, true);
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
                                        foreach (string keyword in tmp[2].Replace("  "," ").Split('"'))
                                        {
                                            foreach (DataColumn dc in table.Columns)
                                            {
                                                if((!keywords.Contains(keyword.Replace("\"", "").Trim())) &&
                                                    (similarity(dc.Caption.Trim(),keyword.Replace("\"","").Trim()) > 0.5))
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
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine("The File could not be read:");
                    Console.WriteLine(e.Message);
                }
                ViewData["id"] = ds;
                ViewData["label"] = "";
                ViewData["keywords"] = keywords;
                result.keywords = keywords;
                result.class_results = classification_results;
                return PartialView(result);
            }
            Aam_UriManager aam_manag = new Aam_UriManager();
            foreach (Input inp in classification_results)
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
            var json = new JavaScriptSerializer().Serialize(new
            {
                nodes = nodes,
                links = paths
            });
            //ViewData["datasets"] = datasets;
            //ViewData["nodes"] = nodes;
            //return View("classification", classification_results);
            return Json(json, JsonRequestBehavior.AllowGet);
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

        [HttpPost]
        public String Filter_Apply(string welllocation = "", string year = "", string filtersize = "", string GroupName = "", string NameFIlter="", 
            String Season_dict="" , string column = "-1" , string row = "-1" , Boolean flag = false)
        {
            string row_ = "";
            dict_ = dict_.OrderByDescending(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            if (row != "-1")
                row_ = dict_.ElementAt(dict_.Count - Int32.Parse(row)-1).Key;
            string col = "";
            if (column != "-1")
                col = dict_.ElementAt(dict_.Count - 1).Value[Int32.Parse(column) - 1];
            string param ="?year=" + year + "&filtersize=" + filtersize + "&GroupName=" + GroupName + "&Season_dict=" + Season_dict + "&column=" + col + "&row=" + row_;
            if (welllocation != "")
                param = param + "&welllocation=" + parse_Json_location(welllocation);
            if (NameFIlter != "")
                param = param + "&PIName=" + NameFIlter;
            string results = UploadFiletoAnalysis(datasetsepcial, "/getvalue"+ param).ToString();
            if (column == "-1") results = results.Split('\n')[0];
            if (column != "-1") results = results.Split('\n')[1];
            if (results.Length > 10)
            {
                try
                {
                    results = results.Substring(3, results.Length - 8);
                    List<string> results_rows = results.Split(new string[] { "}, {" }, StringSplitOptions.None).ToList<string>();
                    Dictionary<string, List<string>> backup_dict_ = new Dictionary<string, List<string>>();
                    if (flag == true)
                        backup_dict_ = dict_;
                    dict_ = new Dictionary<string, List<string>>();
                    foreach (string s in results_rows)
                    {
                        var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>("{" + s + "}");
                        foreach (KeyValuePair<string, string> kvp in dict)
                        {
                            if (!dict_.ContainsKey(kvp.Key))
                            {
                                dict_.Add(kvp.Key.Replace(',', ' '), new List<string>());
                            }
                            List<string> value = new List<string>();
                            dict_.TryGetValue(kvp.Key, out value);
                            value.Add(kvp.Value);
                            dict_[kvp.Key] = value;
                        }
                    }
                    dict_ = dict_.OrderByDescending(x => x.Key.Length).ToDictionary(x => x.Key, x => x.Value);
                    string ss = JsonConvert.SerializeObject(dict_);
                    if (flag == true)
                        dict_ = backup_dict_;
                    ViewData["project_list_names"] = project_list_names_;
                    return ss;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    ViewData["project_list_names"] = project_list_names_;
                    return ("");
                }
            }
            ViewData["project_list_names"] = project_list_names_;
            return ("");
        }

        // this method parses the JSON file containing the well names and their coordinates to get the well name from the coordinates.
        // It is made due to the fact that the leaflet.js map view return only the coordinates and a reason to fetch the well name from coordinates is needed
        public String parse_Json_location(String location_coordinates)
        {
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
                            Debug.WriteLine(e.ToString());
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

        public string UploadFiletoAnalysis(string filePath , string api_action="" )
        {
            String filename = Path.GetFileName(filePath);

            string name = "";
            string email = "";
            try
            {
                name = HttpContext.User.Identity.Name;
                var userManager = new UserManager();
                var user = userManager.FindByNameAsync(name).Result;
                email = user.Email;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.ToString());
            }

            //Hamdi : this line should be deleted for seeting username and email 
            //Hamdi : the name is set to hamdi and email is set to hamdi.hamed1992@gmail.com
            //email = "hamdi.hamed1992@gmail.com";
            //name = "hamdi";

            //create the user folder
            WebRequest request_ = WebRequest.Create(FTPAddress + "/" + name);
            request_.Method = WebRequestMethods.Ftp.MakeDirectory;
            request_.Credentials = new NetworkCredential(username, password);
            try
            {
                using (var resp = (FtpWebResponse)request_.GetResponse())
                {
                    Console.WriteLine(resp.StatusCode);
                    resp.Close();
                }
            }
            catch (WebException e)
            {
                Debug.WriteLine(e.ToString());
            }

            // upload the file to analyse
            WebRequest request =  WebRequest.Create(FTPAddress + "/" + name + "/" + filename);
            request.Credentials = new NetworkCredential(username, password);
            request.Method = WebRequestMethods.Ftp.UploadFile;

            try
            {
                using (Stream fileStream = System.IO.File.OpenRead(filePath))
                using (Stream ftpStream = request.GetRequestStream())
                {
                    fileStream.CopyTo(ftpStream);
                    fileStream.Close();
                }
            }
            catch (WebException e)
            {
                Debug.WriteLine(e.ToString());
                Debug.WriteLine(((FtpWebResponse)e.Response).StatusDescription);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message.ToString());
            }


            //// run the analysis over server tool
            //string url = "http://aquadiva-analysis1.inf-bb.uni-jena.de:5000"+ api_action;
            //var request2 = (HttpWebRequest)WebRequest.Create(url);
            //request2.Method = "POST";
            //request2.ContentType = "application/x-www-form-urlencoded";
            //
            //string params_ = "file_path=" + filename + "&user_home_directory=" + name;
            //byte[] bytes = Encoding.ASCII.GetBytes(params_);
            //request2.ContentLength = bytes.Length;
            //try
            //{
            //    using (var reqStream = request2.GetRequestStream())
            //    {
            //        reqStream.Write(bytes, 0, bytes.Length);
            //        var response = (HttpWebResponse)request2.GetResponse();
            //        Debug.WriteLine("response ==> " + response.ToString());
            //        reqStream.Close();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Debug.WriteLine(ex.Message.ToString());
            //}



            //Construct a HttpClient for the search-Server
            HttpClient client = new HttpClient();
            if (api_action == "") api_action = "/?";
            else api_action = api_action + "&";
                client.BaseAddress = new Uri(AnalAddress +
                    api_action + "file_path=" + filename + "&user_home_directory=" + name);
            //Set the searchTerm as query-String
            StringBuilder paramBuilder = new StringBuilder();
            paramBuilder.Append(" ");
            String param = HttpUtility.UrlEncode(paramBuilder.ToString().Replace(" ", ""));
            client.Timeout = TimeSpan.FromMinutes(30);
            string output = "";
            try
            {
                HttpResponseMessage response =  client.GetAsync(param).Result;
                // Blocking call!
                if (response.IsSuccessStatusCode)
                {
                    // Get the response body. Blocking!
                    output = response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (SocketException exx)
            {
                Debug.WriteLine(exx.Message.ToString());
            }
            return output;
        }

        //this is using the AAM module - ids mostly instead of string content of variables without unit / data type 
        public void prepare_for_classification(string path, string datasetids)
        {
            //path = path + "/prepare_for_classification.csv";
            using (StreamWriter sw = new StreamWriter(System.IO.File.Open(path, System.IO.FileMode.Create)))
            {
                sw.WriteLine("datasetID;Datasetversion_id;variable_id;unit;type;entity_id;entity;charachteristic_id;charachteristic;standard_id;standard;dataset_title;owner;project;variable_id_from_table;variable_value");
            }

            DataStructureManager dsm = new DataStructureManager();
            DatasetManager dm = new DatasetManager();
            Aam_Dataset_column_annotationManager aam = new Aam_Dataset_column_annotationManager();
            List<Aam_Dataset_column_annotation> all_annot = aam.get_all_dataset_column_annotation();
            try
            {
                List<Int64> req_ds = datasetids.Split(';').ToList<string>().Select(Int64.Parse).ToList();
                if (req_ds.Count > 0) all_annot = (List<Aam_Dataset_column_annotation>)all_annot.FindAll(x => req_ds.Contains(x.Dataset.Id));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            foreach (Aam_Dataset_column_annotation annot in all_annot)
            {
                string ch = "";
                string title = xmlDatasetHelper.GetInformationFromVersion(annot.DatasetVersion.Id, NameAttributeValues.title) != "" ? xmlDatasetHelper.GetInformationFromVersion(annot.DatasetVersion.Id, NameAttributeValues.title) : "No title";
                ch = ch +
                    annot.Dataset.Id + ";" +
                    annot.DatasetVersion.Id + ";" +
                    annot.variable_id.Id + ";" +
                    annot.variable_id.Unit.Name + ";" +
                    annot.variable_id.DataAttribute.DataType.Name + ";" +
                    annot.entity_id.Id + ";" +
                    annot.entity_id.URI + ";" +
                    annot.characteristic_id.Id + ";" +
                    annot.characteristic_id.URI + ";" +
                    annot.standard_id.Id + ";" +
                    annot.standard_id.URI + ";" +
                    title.Replace(';', ' ') + ";" ;

                StructuredDataStructure sds = dsm.StructuredDataStructureRepo.Get(annot.Dataset.DataStructure.Id);
                DataStructure ds = dsm.AllTypesDataStructureRepo.Get(annot.Dataset.DataStructure.Id);

                

                if (ds.Self.GetType() == typeof(StructuredDataStructure))
                {
                    //ToDO Javad: 18.07.2017 -> replaced to the new API for fast retrieval of the latest version
                    //
                    //List<AbstractTuple> dataTuples = dm.GetDatasetVersionEffectiveTuples(dsv, 0, 100);
                    //DataTable table = SearchUIHelper.ConvertPrimaryDataToDatatable(dsv, dataTuples);
                    DataTable table = dm.GetLatestDatasetVersionTuples(annot.DatasetVersion.Dataset.Id, 0, 0, true);

                    //List<string> var_labels = new List<string>();
                    //List<string> var_ids = new List<string>();
                    //List<string> label_values = new List<string>();

                    //writing the values needed for the mining in a csv file

                    DatasetManager dsm_ = new DatasetManager();
                    XmlDocument xmlDoc = dsm_.GetDatasetLatestMetadataVersion(annot.Dataset.Id);
                    XmlNode root = xmlDoc.DocumentElement;
                    string idMetadata = root.Attributes["id"].Value;
                    string owner = "none";
                    string project = "none";

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
                    }
                    ch = ch + owner.Replace(';',' ') + " ; " + project.Replace(';', ' ') + " ; ";
                    
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        DataColumn column = table.Columns[i];
                        string col_name = column.ColumnName.ToString().Replace("var", "");
                        string var_label = column.Caption != "" ? column.Caption : "NO Label";
                        if (col_name == annot.variable_id.Id.ToString() )
                        {
                            string values = "";
                            int n = 0;
                            foreach (DataRow obj in table.Rows)
                            {
                                try
                                {
                                    string elem = obj.ItemArray[i].ToString();
                                    //label_values.Add(obj.ToString());
                                    //copy = ch2 + elem.ToString().Replace(';', ' ') + " ; ";
                                    if (elem.ToString().Replace(';', ' ').Trim().Length !=0)
                                    {
                                        if (!(values.Contains(elem.ToString().Replace(';', ' ').Trim()))) {
                                            values = values + elem.ToString().Replace(';', ' ').Trim() + "-";
                                            n = n + 1;
                                        }
                                    }
                                    //using (StreamWriter sw = new StreamWriter(System.IO.File.Open(path, System.IO.FileMode.Append)))
                                    //{
                                    //    sw.WriteLine(copy);
                                    //}
                                }
                                catch (Exception exc)
                                {
                                    Debug.WriteLine(exc.Message);
                                    //using (StreamWriter sw = new StreamWriter(System.IO.File.Open(path, System.IO.FileMode.Append)))
                                    //{
                                    //    sw.WriteLine(ch + " *** " + " ;");
                                    //}
                                    //values = values +" *** " + " ___ ";
                                }
                                if (n % 30 == 0)
                                {
                                    using (StreamWriter sw = new StreamWriter(System.IO.File.Open(path, System.IO.FileMode.Append)))
                                    {
                                        sw.WriteLine(ch + var_label.Replace(';', ' ') + " ; " + values + " ; ");
                                    }
                                    values = "";
                                }
                            }
                            if (values !="")
                                using (StreamWriter sw = new StreamWriter(System.IO.File.Open(path, System.IO.FileMode.Append)))
                                {
                                    sw.WriteLine(ch + var_label.Replace(';', ' ') + " ; " + values + " ; ");
                                }
                        }
                    }
                }
            }
        }

        //this is using the dataset_column_annotation table - strings mostly instead of ids of variables with unit / data type 
        public void prepare_data_mining(string path, string datasetids)
        {
            //string errors = "";
            DatasetManager dm = new DatasetManager();
            DataStructureManager dsm = new DataStructureManager();

            List<Variable> vars = dsm.VariableRepo.Get().ToList<Variable>();

            List<Int64> ds_ids = dm.GetDatasetLatestIds();

            //string path = output_Folder + "prepare_for_mining.csv";
            using (StreamWriter sw = new StreamWriter(System.IO.File.Open(path, System.IO.FileMode.Append)))
            {
                sw.WriteLine("var_id;var_name;data_type_id;data_type_name;data_type_systemtype;data_type_description;unit_id;unit_name;unit_desc;unit_abbrv;datasetID;dsv;title;owner;project;entity;charachteristic;table_variable_value");
            }
            //errors = errors + " -- header written \n  ";
            try
            {
                List<Int64> req_ds = datasetids.Split(';').ToList<string>().Select(Int64.Parse).ToList();
                if (req_ds.Count > 0) ds_ids = req_ds;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            foreach (Int64 datasetID in ds_ids)
            {
                //errors = errors + " -- dataset id"+ datasetID +"  \n  " ;
                DatasetVersion dsv = dm.GetDatasetLatestVersion(datasetID);
                StructuredDataStructure sds = dsm.StructuredDataStructureRepo.Get(dsv.Dataset.DataStructure.Id);

                DataStructure ds = dsm.AllTypesDataStructureRepo.Get(dsv.Dataset.DataStructure.Id);
                string title = xmlDatasetHelper.GetInformationFromVersion(dsv.Id, NameAttributeValues.title);

                if (ds.Self.GetType() == typeof(StructuredDataStructure))
                {
                    //ToDO Javad: 18.07.2017 -> replaced to the new API for fast retrieval of the latest version
                    //
                    //List<AbstractTuple> dataTuples = dm.GetDatasetVersionEffectiveTuples(dsv, 0, 100);
                    //DataTable table = SearchUIHelper.ConvertPrimaryDataToDatatable(dsv, dataTuples);
                    
                   

                    //List<string> var_labels = new List<string>();
                    //List<string> var_ids = new List<string>();
                    //List<string> label_values = new List<string>();

                    //writing the values needed for the mining in a csv file
                    Debug.WriteLine("Structred dataset to be processed  " + datasetID);
                    DatasetManager dsm_ = new DatasetManager();
                    XmlDocument xmlDoc = dsm_.GetDatasetLatestMetadataVersion(datasetID);
                    dsm_.Dispose();

                    XmlNode root = xmlDoc.DocumentElement;
                    string idMetadata = root.Attributes["id"].Value;
                    string owner = "none";
                    string project = "none";

                    //errors = errors + " - Metadata and datastructure " + idMetadata + "  \n  ";

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
                    }

                    try
                    {
                        DataTable table = dm.GetLatestDatasetVersionTuples(datasetID, 0, 0, true);
                        //errors = errors + " - DataTable rows count " + table.Rows.Count + "  \n  ";
                        for (int i = 0; i < table.Columns.Count; i++)
                        {
                            DataColumn column = table.Columns[i];
                            string var_name = column.Caption;
                            string var_id = column.ColumnName.ToString().Replace("var", "");
                            int conv = 0;
                            if (Int32.TryParse(var_id, out conv))
                            {
                                Variable var = (var_id != "") ? vars.FirstOrDefault(x => x.Id.ToString() == var_id) : new Variable();
                                string data_type_id = (var.DataAttribute != null) ? var.DataAttribute.DataType.Id.ToString() : "";
                                string data_type_name = (var.DataAttribute != null) ? var.DataAttribute.DataType.Name : "";
                                string data_type_systemtype = (var.DataAttribute != null) ? var.DataAttribute.DataType.SystemType : "";
                                string data_type_description = (var.DataAttribute != null) ? var.DataAttribute.DataType.Description : "";

                                string unit_desc = (var.DataAttribute != null) ? var.DataAttribute.Unit.Description : "";
                                string unit_name = (var.DataAttribute != null) ? var.DataAttribute.Unit.Name : "";
                                string unit_abbrv = (var.DataAttribute != null) ? var.DataAttribute.Unit.Abbreviation : "";
                                string unit_id = (var.DataAttribute != null) ? var.DataAttribute.Unit.Id.ToString() : "";

                                data_type_id = (data_type_id != null) ? var.DataAttribute.DataType.Id.ToString() : "";
                                data_type_name = (data_type_name != null) ? var.DataAttribute.DataType.Name : "";
                                data_type_systemtype = (data_type_systemtype != null) ? var.DataAttribute.DataType.SystemType : "";
                                data_type_description = (data_type_description  != null) ? var.DataAttribute.DataType.Description : "";

                                unit_desc = (unit_desc != null) ? var.DataAttribute.Unit.Description : "";
                                unit_name = (unit_name != null) ? var.DataAttribute.Unit.Name : "";
                                unit_abbrv = (unit_abbrv != null) ? var.DataAttribute.Unit.Abbreviation : "";
                                unit_id = (unit_id != null) ? var.DataAttribute.Unit.Id.ToString() : "";

                                string ch = "";
                                ch = ch +
                                    var_id.Replace(';', ' ').Replace(Environment.NewLine, String.Empty).Replace(",", String.Empty).Trim() + " ; " +
                                    var_name.Replace(';', ' ').Replace(Environment.NewLine, String.Empty).Replace(",", String.Empty).Trim() + " ; " +
                                    data_type_id.Replace(';', ' ').Replace(Environment.NewLine, String.Empty).Replace(",", String.Empty).Trim() + " ; " +
                                    data_type_name.Replace(';', ' ').Replace(Environment.NewLine, String.Empty).Replace(",", String.Empty).Trim() + " ; " +
                                    data_type_systemtype.Replace(';', ' ').Replace(Environment.NewLine, String.Empty).Replace(",", String.Empty).Trim() + " ; " +
                                    data_type_description.Replace(';', ' ').Replace(Environment.NewLine, String.Empty).Replace(",", String.Empty).Trim() + " ; " +
                                    unit_id.Replace(';', ' ').Replace(Environment.NewLine, String.Empty).Replace(",", String.Empty).Trim() + " ; " +
                                    unit_name.Replace(';', ' ').Replace(Environment.NewLine, String.Empty).Replace(",", String.Empty).Trim() + " ; " +
                                    unit_desc.Replace(';', ' ').Replace(Environment.NewLine, String.Empty).Replace(",", String.Empty).Trim() + " ; " +
                                    unit_abbrv.Replace(';', ' ').Replace(Environment.NewLine, String.Empty).Replace(",", String.Empty).Trim() + " ; " +
                                    datasetID.ToString().Replace(';', ' ').Replace(Environment.NewLine, String.Empty).Replace(",", String.Empty).Trim() + " ; " +
                                    dsv.Id.ToString().Replace(';', ' ').Replace(Environment.NewLine, String.Empty).Replace(",", String.Empty).Trim() + " ; " +
                                    title.Replace(';', ' ').Replace(Environment.NewLine, String.Empty).Replace(",", String.Empty).Trim() + " ; " +
                                    owner.Replace(';', ' ').Replace(Environment.NewLine, String.Empty).Replace(",", String.Empty).Trim() + " ; " +
                                    project.Replace(';', ' ').Replace("\n", String.Empty).Trim() + " ; ";


                                //errors = errors + " - data extracted so far metadata and datas truct \n " + ch + "  \n  ";
                                //var_labels.Add(var_name);
                                //var_ids.Add(var_id.Replace("var",""));


                                //get the entity and the characteristic
                                Aam_Dataset_column_annotationManager aam = new Aam_Dataset_column_annotationManager();
                                List<Aam_Dataset_column_annotation> all_annot = aam.get_all_dataset_column_annotation();
                                Aam_Dataset_column_annotation annot = all_annot.FirstOrDefault(x => x.variable_id.ToString() == var_id);
                                if (annot != null)
                                {
                                    if ((annot.characteristic_id == null) || (annot.entity_id == null))
                                    {
                                        NpgsqlCommand MyCmd = null;
                                        NpgsqlConnection MyCnx = null;
                                        MyCnx = new NpgsqlConnection(Conx);
                                        MyCnx.Open();
                                        string select = "SELECT * FROM \"dataset_column_annotation\" WHERE (datasets_id=" + datasetID + " and variable_id='" + var_id + "' );";
                                        MyCmd = new NpgsqlCommand(select, MyCnx);
                                        NpgsqlDataReader dr = MyCmd.ExecuteReader();
                                        Boolean b = false;
                                        if (dr != null)
                                        {
                                            while (dr.Read())
                                            {
                                                if (dr["datasets_id"] != System.DBNull.Value)
                                                {
                                                    var entity = (String)dr["entity"].ToString().Replace("\n", String.Empty).Trim();
                                                    var characteristic = (String)dr["characteristic"].ToString().Replace("\n", String.Empty).Trim();
                                                    ch = ch + entity + " ; " + characteristic + " ; ";
                                                    b = true;
                                                }
                                            }
                                        }
                                        if (!b)
                                        {
                                            ch = ch + " *** ; *** ;";
                                        }
                                        MyCnx.Close();

                                    }
                                    else
                                    {
                                        ch = ch + annot.entity_id.URI.ToString().Replace(Environment.NewLine, String.Empty).Replace(",", String.Empty).Trim() + " ; " + annot.characteristic_id.URI.ToString().Replace("\n", String.Empty).Trim() + " ; ";
                                    }
                                } 
                                else
                                {
                                    NpgsqlCommand MyCmd = null;
                                    NpgsqlConnection MyCnx = null;
                                    MyCnx = new NpgsqlConnection(Conx);
                                    MyCnx.Open();
                                    string select = "SELECT * FROM \"dataset_column_annotation\" WHERE (datasets_id=" + datasetID + " and variable_id='" + var_id + "' );";
                                    MyCmd = new NpgsqlCommand(select, MyCnx);
                                    NpgsqlDataReader dr = MyCmd.ExecuteReader();
                                    Boolean b = false;
                                    if (dr != null)
                                    {
                                        while (dr.Read())
                                        {
                                            if (dr["datasets_id"] != System.DBNull.Value)
                                            {
                                                var entity = (String)dr["entity"].ToString().Replace("\n", String.Empty).Trim();
                                                var characteristic = (String)dr["characteristic"].ToString().Replace("\n", String.Empty).Trim();
                                                ch = ch + entity + " ; " + characteristic + " ; ";
                                                b = true;
                                            }
                                        }
                                    }
                                    if (!b)
                                    {
                                        ch = ch + " *** ; *** ;";
                                    }
                                    MyCnx.Close();
                                }

                                //ch = annot != null ? ch + annot.entity_id + " ; " + annot.characteristic_id + " ; " : ch + "  ;  ; ";
                                //errors = errors + " - data extracted so far annotations \n " + ch + "  \n  ";
                                try
                                {
                                    for (int k = 0; k < table.Rows.Count; k++)
                                    {
                                        Object obj = table.Rows[k].ItemArray[i];
                                        string copy = "";
                                        copy = ch + obj.ToString().Replace(';', ' ').Replace("\n", String.Empty).Trim() + " ; ";
                                        using (StreamWriter sw = new StreamWriter(System.IO.File.Open(path, System.IO.FileMode.Append)))
                                        {
                                            sw.WriteLine(copy);
                                            Debug.WriteLine("Processed and written " + copy);
                                            //errors = errors + " - data extracted so far variables \n " + ch + "  \n  ";
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine(ex.Message);
                                    ch = ch + " *** " + " ;";
                                    Debug.WriteLine("Not Processed and written " + ch);
                                    using (StreamWriter sw = new StreamWriter(System.IO.File.Open(path, System.IO.FileMode.Append)))
                                    {
                                        sw.WriteLine(ch);
                                    }
                                    //errors = errors + " -error : \n " + ex.Message + "  \n  ";
                                };
                            }

                        }
                        // end of writing the values into a csv
                    }
                    catch (Exception ex)
                    {
                        //errors = errors + " -error : \n " + ex.Message + "  \n  ";
                        Debug.WriteLine(ex.Message);
                    }
                    
                }
                // end of processing the structred data
            }
            dm.Dispose();
            dsm.Dispose();
            // end of looping through the datasets
            //return "";
        }

        //generates a graph from the annotations we used on the portal to create a small knowledge graph from datasets and annotations
        public void get_all_annotations_as_graph_from_entity(string ent)
        {
            DataStructureManager dsm = new DataStructureManager();
            DatasetManager dm = new DatasetManager();
            List<Int64> ds_ids = dm.GetDatasetLatestIds();
            Aam_Dataset_column_annotationManager aam = new Aam_Dataset_column_annotationManager();
            List<Aam_Dataset_column_annotation> all_annot = aam.get_all_dataset_column_annotation();
            foreach (Aam_Dataset_column_annotation annot in all_annot)
            {
                Int64 datasetID = annot.Dataset.Id;
                DatasetVersion dsv = dm.GetDatasetLatestVersion(datasetID);

            }

        }

        //downloads all datasets in csv format 
        public ActionResult prepare_for_neural_net(string path)
        {
            //string errors = "";
            DatasetManager dm = new DatasetManager();
            OutputDataManager odm = new OutputDataManager();


            using ( FileStream fileStream = new FileStream(@"C:\Data\Temp\tmp.zip", FileMode.Create))
            {
                using (ZipArchive archive = new ZipArchive(fileStream, ZipArchiveMode.Create, true))
                {
                    foreach (long id in dm.GetDatasetLatestIds())
                    {
                        try
                        {
                            string fileName = odm.GenerateAsciiFile(id, "text/csv", false);
                            ZipArchiveEntry readmeEntry = archive.CreateEntry(id.ToString()+".csv");
                            using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                            {
                                writer.Write( string.Join( System.Environment.NewLine,System.IO.File.ReadAllLines(fileName)));
                            }
                            //var zipArchiveEntry = archive.CreateEntry(File(fileName, "text/csv", id.ToString()).ToString(), CompressionLevel.Fastest);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                    }
                }
            }
            return File(@"C:\Data\Temp\tmp.zip", "text/csv", "datasets.zip");
        }

         public ActionResult CategoralAnalysis_(long id)
        {
            System.IO.File.AppendAllText(debugFile, "CategoralAnalysis started "+ DateTime.Now + Environment.NewLine);
            ViewData["error"] = "";
            DatasetManager datasetManager = new DatasetManager();
            try
            {
                DatasetVersion datasetVersion = datasetManager.GetDatasetLatestVersion(id);
                AsciiWriter writer = new AsciiWriter(TextSeperator.comma);
                OutputDataManager ioOutputDataManager = new OutputDataManager();
                string title = id.ToString();
                string path = "";

                string message = string.Format("dataset {0} version {1} was downloaded as txt.", id,
                                                datasetVersion.Id);
                path = ioOutputDataManager.GenerateAsciiFile(id, "text/csv",true);

                LoggerFactory.LogCustom(message);

                string absolute_file_path = File(path, "text/csv", title + ".csv").FileName.ToString();
                System.IO.File.AppendAllText(@debugFile, "absolute_file_path -- " + DateTime.Now + " : -- " + absolute_file_path + Environment.NewLine);

                Debug.WriteLine("Dataset id : " + id + "has path : " + absolute_file_path);
                string extension = Path.GetExtension(absolute_file_path);

                if (allowed_extention.Contains(extension))
                {
                    string progToRun = python_script;
                    string outputFolder = output_Folder;

                    //string file = Path.Combine("C:/Users/admin/Desktop/test.xlsx");
                    char[] spliter = { '\r' };

                    System.IO.File.AppendAllText(@debugFile, "progToRun -- " + DateTime.Now + " : -- " + progToRun.ToString() + Environment.NewLine);
                    System.IO.File.AppendAllText(@debugFile, "outputFolder -- " + DateTime.Now + " : -- " + outputFolder.ToString() + Environment.NewLine);

                    System.IO.File.AppendAllText(@debugFile, "process intialized -- " + DateTime.Now + " : -- "  + Environment.NewLine);
                    Process proc = new Process();
                    System.IO.File.AppendAllText(@debugFile, "proc.StartInfo.FileName ="+ python_path +" -- " + DateTime.Now + " : -- "  + Environment.NewLine);
                    proc.StartInfo.FileName = python_path;
                    System.IO.File.AppendAllText(@debugFile, "proc.StartInfo.RedirectStandardOutput = true  -- " + DateTime.Now + " : -- "  + Environment.NewLine);
                    proc.StartInfo.RedirectStandardOutput = true;
                    System.IO.File.AppendAllText(@debugFile, "proc.StartInfo.RedirectStandardError = true  -- " + DateTime.Now + " : -- "  + Environment.NewLine);
                    proc.StartInfo.RedirectStandardError = true;
                    System.IO.File.AppendAllText(@debugFile, "proc.StartInfo.UseShellExecute = false  -- " + DateTime.Now + " : -- "  + Environment.NewLine);
                    proc.StartInfo.UseShellExecute = false;

                    // call hello.py to concatenate passed parameters
                    proc.StartInfo.Arguments = string.Concat(progToRun, " ", absolute_file_path, " ", extension, " ", outputFolder);
                    System.IO.File.AppendAllText(@debugFile, "process going to start-- " + DateTime.Now + " : -- " + outputFolder.ToString() + Environment.NewLine);
                    try
                    {
                        proc.Start();
                    }
                    catch (Exception ex)
                    {
                        System.IO.File.AppendAllText(@debugFile, "process error -- " + DateTime.Now + " : -- " + ex.InnerException.Message + Environment.NewLine);
                        ViewData["error"] = ViewData["error"] + Environment.NewLine + ex.InnerException.Message;
                    }
                    
                    System.IO.File.AppendAllText(@debugFile, "process started now -- " + DateTime.Now + " : -- " + outputFolder.ToString() + Environment.NewLine);
                    //* Read the output (or the error)
                    string output = proc.StandardOutput.ReadToEnd();
                    string err = proc.StandardError.ReadToEnd();

                    System.IO.File.AppendAllText(@debugFile, "output -- " + DateTime.Now + " : -- " + output.ToString() + Environment.NewLine);
                    System.IO.File.AppendAllText(@debugFile, "err -- " + DateTime.Now + " : -- " + err.ToString() + Environment.NewLine);

                    if (err.Length > 0)
                    {
                        ViewData["error"] = ViewData["error"] + Environment.NewLine + err;
                        return PartialView("showDataSetAnalysis");
                    }

                    proc.WaitForExit();

                    lines = output.Split(Environment.NewLine.ToCharArray()).ToList();
                    int index = lines.IndexOf("Numerical");
                    lines = lines.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

                    System.IO.File.AppendAllText(@debugFile, "lines -- " + DateTime.Now + " : -- " + lines.ToString() + Environment.NewLine);

                    List<List<string>> values = new List<List<string>>();
                    List<List<string>> labels = new List<List<string>>();

                    for (int k = 0; k < lines.Count; k++)
                    {
                        string x_label = lines[k];
                        string x_values = lines[k + 1];
                        string y_label = lines[k + 2];
                        string y_values = lines[k + 3];
                        k = k + 3;
                        List<string> bocket = new List<string>();
                        bocket.Add(x_label);
                        bocket.Add(y_label);
                        labels.Add(bocket);
                        bocket = new List<string>();
                        bocket.Add(x_values);
                        bocket.Add(y_values);
                        values.Add(bocket);
                        bocket = new List<string>();
                    }

                    var jsonSerialiser = new JavaScriptSerializer();
                    var json = jsonSerialiser.Serialize(lines);

                    var json_ = JsonConvert.SerializeObject(lines);

                    string filename = Path.GetFileNameWithoutExtension(absolute_file_path);
                    //read the results of the analysis // python script generates an excel table for that
                    List<string> header = new List<string>();
                    List<List<string>> data_lines = new List<List<string>>();
                    using (var reader = new StreamReader(outputFolder + filename + ".csv"))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            List<string> tmp = line.Split(';').ToList<string>();
                            if (tmp.Count > 1)
                                data_lines.Add(tmp);
                        }
                    }

                    header = data_lines[data_lines.Count - 1];
                    data_lines.RemoveAt(data_lines.Count - 1);
                    // end of reading the results

                    System.IO.File.AppendAllText(@debugFile, "header -- " + DateTime.Now + " : -- " + header.ToString() + Environment.NewLine);
                    System.IO.File.AppendAllText(@debugFile, "data_lines -- " + DateTime.Now + " : -- " + data_lines.ToString() + Environment.NewLine);

                    datasetManager.Dispose();
                    FileInfo myfileinf = new FileInfo(absolute_file_path);
                    myfileinf.Delete();

                    ViewData["values"] = values;
                    ViewData["labels"] = labels;
                    ViewData["header"] = header;
                    ViewData["data_lines"] = data_lines;
                    return PartialView("showDataSetAnalysis");
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                System.IO.File.AppendAllText(@debugFile, "Exception -- " + DateTime.Now + " : -- " + ex.InnerException.Message + Environment.NewLine);
            }
            return PartialView("showDataSetAnalysis");

        }

        public JObject getGraphData()
        {
            JObject jObject = new JObject();
            int k = 0;
            for (int i = 0; i < lines.Count; i++)
            {
                JArray Xarray = new JArray();
                Xarray.Add(lines[i]);
                Xarray.Add(lines[i + 1]);

                JArray Yarray = new JArray();
                Yarray.Add(lines[i + 2]);
                Yarray.Add(lines[i + 3]);

                JArray jArray = new JArray();
                jArray.Add(Xarray);
                jArray.Add(Yarray);
                jObject[k.ToString()] = jArray;
                k = k + 1;
                i = i + 3;
            }
            return jObject;
        }

        public ActionResult NumericalAnalysis(long id)
        {
            DatasetManager datasetManager = new DatasetManager();
            try
            {
                DatasetVersion datasetVersion = datasetManager.GetDatasetLatestVersion(id);
                AsciiWriter writer = new AsciiWriter(TextSeperator.comma);
                OutputDataManager ioOutputDataManager = new OutputDataManager();
                string title = id.ToString();
                string path = "";

                string message = string.Format("dataset {0} version {1} was downloaded as txt.", id,
                                                datasetVersion.Id);
                path = ioOutputDataManager.GenerateAsciiFile(id, "text/csv",true);

                LoggerFactory.LogCustom(message);

                string absolute_file_path = File(path, "text/csv", title + ".csv").FileName.ToString();

                Debug.WriteLine("Dataset id : " + id + "has path : " + absolute_file_path);
                string extension = Path.GetExtension(absolute_file_path);

                if (allowed_extention.Contains(Path.GetExtension(absolute_file_path)))
                {
                    string progToRun = python_script;
                    //string file = Path.Combine("C:/Users/admin/Desktop/test.xlsx");
                    char[] spliter = { '\r' };

                    Process proc = new Process();
                    proc.StartInfo.FileName = python_path;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.RedirectStandardError = true;
                    proc.StartInfo.UseShellExecute = false;

                    // call hello.py to concatenate passed parameters
                    proc.StartInfo.Arguments = string.Concat(progToRun, " ", absolute_file_path, " ", extension);
                    proc.Start();

                    //* Read the output (or the error)
                    string output = proc.StandardOutput.ReadToEnd();
                    string err = proc.StandardError.ReadToEnd();

                    proc.WaitForExit();

                    lines = output.Split(Environment.NewLine.ToCharArray()).ToList();
                    int index = lines.IndexOf("Numerical");
                    lines = lines.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

                    List<List<string>> values = new List<List<string>>();
                    List<List<string>> labels = new List<List<string>>();

                    for (int k = 0; k < lines.Count; k++)
                    {
                        string x_label = lines[k];
                        string x_values = lines[k + 1];
                        string y_label = lines[k + 2];
                        string y_values = lines[k + 3];
                        k = k + 3;
                        List<string> bocket = new List<string>();
                        bocket.Add(x_label);
                        bocket.Add(y_label);
                        labels.Add(bocket);
                        bocket = new List<string>();
                        bocket.Add(x_values);
                        bocket.Add(y_values);
                        values.Add(bocket);
                        bocket = new List<string>();
                    }

                    var jsonSerialiser = new JavaScriptSerializer();
                    var json = jsonSerialiser.Serialize(lines);

                    var json_ = JsonConvert.SerializeObject(lines);

                    datasetManager.Dispose();
                    FileInfo myfileinf = new FileInfo(absolute_file_path);
                    myfileinf.Delete();

                    ViewData["values"] = values;
                    ViewData["labels"] = labels;
                    return PartialView("showDataSetAnalysis");
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw ex;
            }
            return PartialView("showDataSetAnalysis");

        }

        public ActionResult DistributionAnalysis(long id)
        {
            DatasetManager datasetManager = new DatasetManager();
            try
            {
                DatasetVersion datasetVersion = datasetManager.GetDatasetLatestVersion(id);
                AsciiWriter writer = new AsciiWriter(TextSeperator.comma);
                OutputDataManager ioOutputDataManager = new OutputDataManager();
                string title = id.ToString();
                string path = "";

                string message = string.Format("dataset {0} version {1} was downloaded as txt.", id,
                                                datasetVersion.Id);
                path = ioOutputDataManager.GenerateAsciiFile(id, "text/csv",true);

                LoggerFactory.LogCustom(message);

                string absolute_file_path = File(path, "text/csv", title + ".csv").FileName.ToString();

                Debug.WriteLine("Dataset id : " + id + "has path : " + absolute_file_path);
                string extension = Path.GetExtension(absolute_file_path);

                if (allowed_extention.Contains(Path.GetExtension(absolute_file_path)))
                {
                    string progToRun = python_script;
                    //string file = Path.Combine("C:/Users/admin/Desktop/test.xlsx");
                    char[] spliter = { '\r' };

                    Process proc = new Process();
                    proc.StartInfo.FileName = python_path;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.RedirectStandardError = true;
                    proc.StartInfo.UseShellExecute = false;

                    // call hello.py to concatenate passed parameters
                    proc.StartInfo.Arguments = string.Concat(progToRun, " ", absolute_file_path, " ", extension);
                    proc.Start();

                    //* Read the output (or the error)
                    string output = proc.StandardOutput.ReadToEnd();
                    string err = proc.StandardError.ReadToEnd();

                    proc.WaitForExit();

                    lines = output.Split(Environment.NewLine.ToCharArray()).ToList();
                    lines = lines.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

                    List<List<string>> values = new List<List<string>>();
                    List<List<string>> labels = new List<List<string>>();

                    for (int k = 0; k < lines.Count; k++)
                    {
                        string x_label = lines[k];
                        string x_values = lines[k + 1];
                        string y_label = lines[k + 2];
                        string y_values = lines[k + 3];
                        k = k + 3;
                        List<string> bocket = new List<string>();
                        bocket.Add(x_label); bocket.Add(y_label);
                        labels.Add(bocket); bocket = new List<string>();
                        bocket.Add(x_values); bocket.Add(y_values);
                        values.Add(bocket); bocket = new List<string>();
                    }

                    var jsonSerialiser = new JavaScriptSerializer();
                    var json = jsonSerialiser.Serialize(lines);

                    var json_ = JsonConvert.SerializeObject(lines);

                    datasetManager.Dispose();
                    FileInfo myfileinf = new FileInfo(absolute_file_path);
                    myfileinf.Delete();

                    ViewData["values"] = values;
                    ViewData["labels"] = labels;
                    return PartialView("showDataSetAnalysis");
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw ex;
            }
            return PartialView("showDataSetAnalysis");
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
        // this class is made for the Deserialization of the JSON object of the JSON file containing the coordinates and well names.
        public class coordinates_GPS
        {
            public string Well_name;
            public string Lat;
            public string Lon;
        }

    }

}