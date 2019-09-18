﻿using System;
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

namespace BExIS.Modules.Asm.UI.Controllers
{
    public class DataSummaryController : Controller
    {
        private XmlDatasetHelper xmlDatasetHelper = new XmlDatasetHelper();

        // GET: DataSummary
        private static String username = "hamdihamed";
        private static String password = "hamdi1992";
        private static string FTPAddress = "ftp://aquadiva-analysis1.inf-bb.uni-jena.de:21";
        private static string FlaskServer = "http://aquadiva-analysis1.inf-bb.uni-jena.de:5000";

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


        public ActionResult CategoralAnalysis2(long id)
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
                    String output = UploadFiletoAnalysis(absolute_file_path);

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

                    // remove the array view
                    //string filename = Path.GetFileNameWithoutExtension(absolute_file_path);
                    ////read the results of the analysis // python script generates an excel table for that
                    //List<string> header = new List<string>();
                    //List<List<string>> data_lines = new List<List<string>>();
                    //
                    //List<string> lignes = lines_[1].Substring(2, lines_[1].Length -3 ).Split(',').ToList();
                    //using (StreamWriter file = new StreamWriter(outputFolder + filename + ".csv"))
                    //{
                    //    foreach (string item in lignes)
                    //    {
                    //        file.WriteLine(item.Replace("\\r\\n", "") + ",");
                    //    }
                    //}
                    //
                    //using (var reader = new StreamReader(outputFolder + filename + ".csv"))
                    //{
                    //    string line;
                    //    while ((line = reader.ReadLine()) != null)
                    //    {
                    //        List<string> tmp = line.Split(';').ToList<string>();
                    //        if (tmp.Count > 1)
                    //            data_lines.Add(tmp);
                    //    }
                    //}

                    //header = data_lines[data_lines.Count - 1];
                    //data_lines.RemoveAt(data_lines.Count - 1);
                    // end of reading the results

                    datasetManager.Dispose();
                    FileInfo myfileinf = new FileInfo(absolute_file_path);
                    try
                    {
                        myfileinf.Delete();
                    }
                    catch (Exception exec)
                    {
                        Debug.WriteLine(exec.Message);
                    }
                    

                    ViewData["values"] = values;
                    ViewData["labels"] = labels;
                    //ViewData["header"] = header;
                    //ViewData["data_lines"] = data_lines;
                    return PartialView("showDataSetAnalysis");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            ViewData["values"] = new List<List<string>>();
            ViewData["labels"] = new List<List<string>>();
            ViewData["header"] = new List<string>();
            ViewData["data_lines"] = new List<List<string>>();
            ViewData["error"] = "No categoral information was extracted";
            return PartialView("showDataSetAnalysis");
        }


        private string UploadFiletoAnalysis(string filePath )
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
            WebRequest request = WebRequest.Create(FTPAddress + "/" + name + "/" + filename);
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


            // run the analysis over server tool
            string url = "http://aquadiva-analysis1.inf-bb.uni-jena.de:5000";
            var request2 = (HttpWebRequest)WebRequest.Create(url);
            request2.Method = "POST";
            request2.ContentType = "application/x-www-form-urlencoded";

            string params_ = "file_path=" + filename + "&user_home_directory=" + name;
            byte[] bytes = Encoding.ASCII.GetBytes(params_);
            request2.ContentLength = bytes.Length;
            try
            {
                using (var reqStream = request2.GetRequestStream())
                {
                    reqStream.Write(bytes, 0, bytes.Length);
                    var response = (HttpWebResponse)request2.GetResponse();
                    Debug.WriteLine("response ==> " + response.ToString());
                    reqStream.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message.ToString());
            }



            //Construct a HttpClient for the search-Server
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://aquadiva-analysis1.inf-bb.uni-jena.de:5000?file_path=" + filename + "&user_home_directory=" + name);
            //Set the searchTerm as query-String
            StringBuilder paramBuilder = new StringBuilder();
            paramBuilder.Append(" ");
            String param = HttpUtility.UrlEncode(paramBuilder.ToString().Replace(" ", ""));
            string output = "";
            try
            {
                HttpResponseMessage response = client.GetAsync(param).Result;  // Blocking call!
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


        public void prepare_data_mining()
        {
            DatasetManager dm = new DatasetManager();
            DataStructureManager dsm = new DataStructureManager();

            List<Int64> ds_ids = dm.GetDatasetLatestIds();

            string path = output_Folder + "prepare_for_mining.csv";

            foreach (Int64 datasetID in ds_ids)
            {
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
                    DataTable table = dm.GetLatestDatasetVersionTuples(dsv.Dataset.Id, 0, 100, true);

                    //List<string> var_labels = new List<string>();
                    //List<string> var_ids = new List<string>();
                    //List<string> label_values = new List<string>();

                    //writing the values needed for the mining in a csv file

                    DatasetManager dsm_ = new DatasetManager();
                    XmlDocument xmlDoc = dsm_.GetDatasetLatestMetadataVersion(datasetID);
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

                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        DataColumn column = table.Columns[i];
                        string caption = column.Caption;
                        string var_id = column.ColumnName.ToString().Replace("var","");
                        string ch = "";
                        ch = ch + caption + " ; " + var_id + " ; " + datasetID.ToString() + " ; " + dsv.Id.ToString() + " ; " + title + " ; " + owner + " ; " + project + " ; ";

                        

                        //var_labels.Add(caption);
                        //var_ids.Add(var_id.Replace("var",""));


                        //get the entity and the characteristic
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
                                    var entity = (String)dr["entity"].ToString();
                                    var characteristic = (String)dr["characteristic"].ToString();
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
                        // end of getting the entity and charecteristics

                        try
                        {
                            foreach (Object obj in table.Rows[i].ItemArray)
                            {
                                //label_values.Add(obj.ToString());
                                string copy = "";
                                copy = ch + obj.ToString() + " ; " ;
                                using (StreamWriter sw = new StreamWriter(System.IO.File.Open(path, System.IO.FileMode.Append)))
                                {
                                    sw.WriteLine(copy);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                            ch = ch + " *** " + " ;";
                            using (StreamWriter sw = new StreamWriter(System.IO.File.Open(path, System.IO.FileMode.Append)))
                            {
                                sw.WriteLine(ch);
                            }
                        }
                    }
                    // end of writing the values into a csv
                }
                // end of processing the structred data
            }
            // end of looping through the datasets
        }



        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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