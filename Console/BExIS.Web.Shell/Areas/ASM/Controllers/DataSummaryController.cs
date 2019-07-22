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

namespace BExIS.Modules.Asm.UI.Controllers
{
    public class DataSummaryController : Controller
    {
        // GET: DataSummary

        static string DatastructAPI = "http://localhost:5412/api/structures/";
        static List<Variable_analytics> VA_list;

        static List<string> project_list_names = new List<string> { "A01", "A02", "A03", "A04", "A05", "A06", "B01", "B02", "B03", "B04", "B05", "C03", "C05", "D01", "D02", "D03", "D04" };

        static string Conx = ConfigurationManager.ConnectionStrings[1].ConnectionString;

        static string python_path = WebConfigurationManager.AppSettings["python_path"].ToString();
        static string python_script = WebConfigurationManager.AppSettings["python_script"].ToString();
        static string output_Folder = WebConfigurationManager.AppSettings["output_Folder"].ToString();

        private static string datasets_root_folder = WebConfigurationManager.AppSettings["DataPath"];
        string[] allowed_extention = new string[] { ".csv", ".xlsx", ".xls" };

        static List<string> lines = new List<string>();
        static String debugFile = Path.Combine(AppConfiguration.GetModuleWorkspacePath("ASM"), "debug.txt");

        public ActionResult CategoralAnalysis(long id)
        {
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
                path = ioOutputDataManager.GenerateAsciiFile(id, title, "text/csv");

                LoggerFactory.LogCustom(message);

                string absolute_file_path = File(path, "text/csv", title + ".csv").FileName.ToString();

                Debug.WriteLine("Dataset id : " + id + "has path : " + absolute_file_path);
                string extension = Path.GetExtension(absolute_file_path);

                if (allowed_extention.Contains(extension))
                {
                    string progToRun = python_script;
                    string outputFolder = output_Folder;

                    //string file = Path.Combine("C:/Users/admin/Desktop/test.xlsx");
                    char[] spliter = { '\r' };

                    Process proc = new Process();
                    proc.StartInfo.FileName = python_path;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.RedirectStandardError = true;
                    proc.StartInfo.UseShellExecute = false;

                    // call hello.py to concatenate passed parameters
                    proc.StartInfo.Arguments = string.Concat(progToRun, " ", absolute_file_path, " ", extension, " ", outputFolder);
                    proc.Start();

                    //* Read the output (or the error)
                    string output = proc.StandardOutput.ReadToEnd();
                    string err = proc.StandardError.ReadToEnd();
                    ViewData["error"] = "";
                    if (err.Length > 0)
                    {
                        ViewData["error"] = err;
                        return PartialView("showDataSetAnalysis");
                    }

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

        /// <summary>
        /// //////////////////////////////// not needed for now
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
                path = ioOutputDataManager.GenerateAsciiFile(id, title, "text/csv");

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


    }
}