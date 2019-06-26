using System;
using System.Collections.Generic;
using System.Web.Mvc;
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
using System.Linq;
using BExIS.Dlm.Entities.Data;
using System.Web.Configuration;
using System.IO;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using BExIS.Modules.Ddm.UI.Controllers;
using BExIS.IO.Transform.Output;
using BExIS.IO;
using Vaiona.Logging;
using System.Text;
using System.Data;
using BExIS.UI.Helpers;
using BExIS.Utils.Models;

namespace BExIS.Modules.Asm.UI.Controllers
{
    public class AnalyticsController : Controller
    {
        static string DatastructAPI = "http://localhost:5412/api/structures/";
        static List<Variable_analytics> VA_list;
        
        static string Conx = ConfigurationManager.ConnectionStrings[1].ConnectionString;

        private static string datasets_root_folder = WebConfigurationManager.AppSettings["DataPath"];
        string[] allowed_extention = new string[] { ".csv", ".xlsx" ,".xls" };

        static List<string> lines = new List<string>();

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

                List<string> variable_id = new List<string>();
                List<string> variable_label = new List<string>();
                List<string> dataType = new List<string>();
                List<string> unit = new List<string>();
                List<string> variable_concept_entity = new List<string>();
                List<string> variable_concept_caracteristic = new List<string>();
                

                //Construct a HttpClient for the search-Server
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(DatastructAPI);
                //Set the searchTerm as query-String
                String param = HttpUtility.UrlEncode(id.ToString());
                 
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
                        Variable_analytics VA = new Variable_analytics(id, variable_id, variable_label, variable_concept_entity, variable_concept_caracteristic, dataType, unit);
                        VA_list.Add(VA);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
            
            Data_container_analytics datacontaineranalytics = new Data_container_analytics();

            ViewData["datacontaineranalytics"] = datacontaineranalytics;

            ViewData["VA_list"] = VA_list;
            return View(VA_list);
        }
        
        public ActionResult Download_Report()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("dataset_id ,Semantic Coverage, variable_id,variable_label,variable_concept_entity,variable_concept_caracteristic,dataType,unit");
            foreach (var va in VA_list)
            {
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
                    sb.AppendLine(
                        va.dataset_id.ToString() +","+
                        Concepts_count.ToString()+"/"+va.variable_id.Count.ToString() + "," +
                        va.variable_id[0].ToString() + "," +
                        va.variable_label[0]+","+
                        va.variable_concept_entity[0] + "," +
                        va.variable_concept_caracteristic[0] + "," +
                        va.dataType[0] + "," +
                        va.unit[0]);

                    for (int kk = 1; kk < va.variable_id.Count; kk++)
                    {
                        sb.AppendLine( 
                            ", ,"+
                            va.variable_id[kk].ToString() + "," +
                            va.variable_label[kk].ToString() + "," +
                            va.variable_concept_entity[kk].ToString() + "," +
                            va.variable_concept_caracteristic[kk].ToString() + "," +
                            va.unit[kk].ToString() + "," +
                            va.dataType[kk].ToString());
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


        public JObject getGraphData()
        {
            JObject jObject = new JObject();
            int k = 0;
            for (int i = 0; i< lines.Count; i++)
            {
                JArray Xarray = new JArray();
                Xarray.Add(lines[i]);
                Xarray.Add(lines[i+1]);

                JArray Yarray = new JArray();
                Yarray.Add(lines[i+2]);
                Yarray.Add(lines[i+3]);

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
                path = ioOutputDataManager.GenerateAsciiFile(id, title, "text/csv");

                LoggerFactory.LogCustom(message);

                string absolute_file_path = File(path, "text/csv", title + ".csv").FileName.ToString();

                Debug.WriteLine("Dataset id : " + id + "has path : " + absolute_file_path);
                string extension = Path.GetExtension(absolute_file_path);

                if (allowed_extention.Contains(Path.GetExtension(absolute_file_path)))
                {
                    string progToRun = @"D:/Hamdi/python_data_summary_scripts/numericalstatistics.py";
                    //string file = Path.Combine("C:/Users/admin/Desktop/test.xlsx");
                    char[] spliter = { '\r' };

                    Process proc = new Process();
                    proc.StartInfo.FileName = @"C:\Users\Markus\AppData\Local\Programs\Python\Python37\python.exe";
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

        public ActionResult CategoralAnalysis(long id)
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
                path = ioOutputDataManager.GenerateAsciiFile(id, title, "text/csv");

                LoggerFactory.LogCustom(message);

                string absolute_file_path = File(path, "text/csv", title + ".csv").FileName.ToString();

                Debug.WriteLine("Dataset id : " + id + "has path : " + absolute_file_path);
                string extension = Path.GetExtension(absolute_file_path);

                if (allowed_extention.Contains(Path.GetExtension(absolute_file_path)))
                {
                    string progToRun = @"D:/Hamdi/python_data_summary_scripts/categoralanalysis.py";
                    string outputFolder = @"D:/Hamdi/Tmp/";

                    //string file = Path.Combine("C:/Users/admin/Desktop/test.xlsx");
                    char[] spliter = { '\r' };

                    Process proc = new Process();
                    proc.StartInfo.FileName = @"C:\Users\Markus\AppData\Local\Programs\Python\Python37\python.exe";
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.RedirectStandardError = true;
                    proc.StartInfo.UseShellExecute = false;

                    // call hello.py to concatenate passed parameters
                    proc.StartInfo.Arguments = string.Concat(progToRun, " ", absolute_file_path, " ", extension, " ", outputFolder);
                    proc.Start();

                    //* Read the output (or the error)
                    string output = proc.StandardOutput.ReadToEnd();
                    string err = proc.StandardError.ReadToEnd();
                    if ( err != null )
                        return PartialView("showDataSetAnalysis");


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

                    string filename = Path.GetFileNameWithoutExtension(absolute_file_path);
                    //read the results of the analysis // python script generates an excel table for that
                    List<string> header = new List<string>();
                    List<List<string>> data_lines = new List<List<string>>();
                    using (var reader = new StreamReader(outputFolder + filename + ".csv"))
                    {
                        string line ;
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
                path = ioOutputDataManager.GenerateAsciiFile(id, title, "text/csv");

                LoggerFactory.LogCustom(message);

                string absolute_file_path = File(path, "text/csv", title + ".csv").FileName.ToString();

                Debug.WriteLine("Dataset id : " + id + "has path : " + absolute_file_path);
                string extension = Path.GetExtension(absolute_file_path);

                if (allowed_extention.Contains(Path.GetExtension(absolute_file_path)))
                {
                    string progToRun = @"D:/Hamdi/python_data_summary_scripts/CatAlgorithm.py";
                    //string file = Path.Combine("C:/Users/admin/Desktop/test.xlsx");
                    char[] spliter = { '\r' };

                    Process proc = new Process();
                    proc.StartInfo.FileName = @"C:\Users\Markus\AppData\Local\Programs\Python\Python37\python.exe";
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


        public String getDatasetsByProjects(string Project_id, String dataset_ids)
        {
            if ((Project_id == null) || (dataset_ids.Length < 1))
                return "Parameters are Wrong";

            List<string> headers = new List<string>();
            List<string> contents = new List<string>();

            foreach (string id in dataset_ids.Split(',').ToList<string>())
            {
                string header = "";
                string content = "";

                try
                {
                    // call fo the API to get the variables from the data structures using the internal API
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(DatastructAPI);
                    //Set the searchTerm as query-String
                    String param = HttpUtility.UrlEncode(id.ToString());
                    HttpResponseMessage response = client.GetAsync(param).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        JObject json_ds_struct = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                        JArray json_variables = JArray.Parse(json_ds_struct["Variables"].ToString());

                        //get the header in one string
                        foreach (JObject json_variable in json_variables)
                        {
                            string var_id = json_variable["Id"].ToString();
                            string var_label = json_variable["Label"].ToString();
                            header = header + "," + var_label;
                        }
                        //end of get the header in one string

                        // get the content in one string
                        DatasetManager dsm = new DatasetManager();
                        if (dsm.GetDataset(Int64.Parse(id)) != null)
                        {
                            DataTable dt = dsm.ConvertToDataTable(dsm.GetDatasetLatestVersion(dsm.GetDataset(Int64.Parse(id))));
                            DataRowCollection rows = dt.Rows;
                            for (int i = 0; i<rows.Count; i++)
                            {
                                var row = rows[i];
                                for (int j = 0; j< json_variables.Count; j++)
                                {
                                    string ele = row[j].ToString();
                                    ele = ele.Replace(System.Environment.NewLine, " ");
                                    ele = ele.Replace(",", " ");
                                    ele = ele.Replace(";", " ");
                                    content = content + "," + ele;
                                }
                                content = content + Environment.NewLine;
                            }
                        }

                        // check if the header is the same or is just another data wiuth the same data structure
                        // if it exists, the content will be added to the existing content that matches the data et header (data struct)
                        // else it will be added as another partial header
                        if (header.Length>0)
                        {
                            int index = headers.IndexOf(header);
                            if (index == -1)
                            {
                                headers.Add(header);
                                contents.Add(content);
                            }
                            else
                            {
                                contents[index] = contents[index] + Environment.NewLine + content;
                            }
                        }
                        
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message.ToString());
                    return "";
                }
            }

            return "";
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