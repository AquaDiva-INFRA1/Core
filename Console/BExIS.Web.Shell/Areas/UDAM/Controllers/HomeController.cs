using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Mvc;
using BExIS.Modules.UDAM.UI.Models;
using BExIS.Utils.Models;
using Telerik.Web.Mvc;
using BExIS.Dlm.Services.Data;
using BExIS.Dlm.Entities.Data;
using BExIS.Dlm.Entities.DataStructure;
using BExIS.Xml.Helpers;
using System.Web.Configuration;
using BExIS.Security.Services.Authentication;
using BExIS.Security.Services.Subjects;
using Newtonsoft.Json;
using BExIS.Dlm.Services.DataStructure;
using BExIS.Security.Services.Authorization;
using BExIS.Security.Entities.Authorization;

namespace BExIS.Modules.UDAM.UI.Controllers
{
    public class HomeController : Controller
    {
        public static Unstructred_data model;
        private static List<HeaderItem> headerItems;
        private static HeaderItem idHeader;

        private static string datasets_root_folder = WebConfigurationManager.AppSettings["DataPath"];

        private static List<String> allowed_extention = new List<string>
        {
            ".fq",".fastq"
        };

        private static String username = "hamdihamed";
        private static String password = "hamdi1992";
        private static string FTPAddress = "ftp://aquadiva-analysis1.inf-bb.uni-jena.de:21";

        static Analysis_Model analysis_scripts = new Analysis_Model();



        // GET: Home
        public ActionResult Index()
        {
            List<HeaderItem> header_items = makeHeader();
            DataTable result_table = CreateDataTable(header_items);
            model = new Unstructred_data(result_table);
            fill_data_table_unstructred(model.result_table);


            ViewData["tools_list"] = analysis_scripts.tools_list;
            ViewData["R_script_paths"] = analysis_scripts.R_script_paths;
            ViewData["Python_script_paths"] = analysis_scripts.Python_script_paths;

            return View(model.result_table);
        }


        [GridAction]
        public ActionResult _CustomBinding(GridCommand command)
        {
            if (model.result_table != null)
            {
                return View(new GridModel(model.result_table));
            }
            return View(model.result_table);
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /*
         * this method fills the unstructred data into the data table
         * */
        private void fill_data_table_unstructred(DataTable m)
        {
            DatasetManager dsm = new DatasetManager();
            List<Int64> all_ds = dsm.GetDatasetLatestIds();
            foreach (Int64 ds_id in all_ds)
            {
                Dataset ds = dsm.GetDataset(ds_id);
                if (!(ds.DataStructure.Self is StructuredDataStructure))
                {
                    Debug.WriteLine("Dataset id : " + ds_id + " is unstructred");

                    DataRow row = m.NewRow();
                    row["ID"] = Int64.Parse(ds_id.ToString());

                    XmlDatasetHelper helper = new XmlDatasetHelper();
                    row["Title"] = helper.GetInformation(ds_id, NameAttributeValues.title);
                    row["Datasetdescription"] = helper.GetInformation(ds_id, NameAttributeValues.description); ;
                    //row["Ownername"] = helper.GetInformation(ds_id, NameAttributeValues.owner);

                    m.Rows.Add(row);

                    List<DatasetVersion> dsv = dsm.GetDatasetVersions(Int64.Parse(ds_id.ToString()));
                    foreach (DatasetVersion dataset_v in dsv)
                    {
                        ICollection<ContentDescriptor> cont_desc = dataset_v.ContentDescriptors;
                        foreach (ContentDescriptor c_d in cont_desc)
                        {
                            Debug.WriteLine("Dataset id : " + Int64.Parse(ds_id.ToString()) + "has path : " + c_d.URI);
                        }
                    }
                }
                else Debug.WriteLine("Dataset id : " + ds_id + " is structred");
            }
        }


        /*
            * Defines the list of HeaderItems that will be used to create the DataTable
            * */
        private List<HeaderItem> makeHeader()
        {
            headerItems = new List<HeaderItem>();

            HeaderItem headerItem = new HeaderItem()
            {
                Name = "ID",
                DisplayName = "ID",
                DataType = "Int64"
            };
            headerItems.Add(headerItem);

            idHeader = headerItem;
            ViewData["ID"] = headerItem;

            headerItem = new HeaderItem()
            {
                Name = "Title",
                DisplayName = "Title",
                DataType = "String"
            };
            headerItems.Add(headerItem);

            headerItem = new HeaderItem()
            {
                Name = "Ownername",
                DisplayName = "Owner",
                DataType = "String"
            };
            headerItems.Add(headerItem);

            headerItem = new HeaderItem()
            {
                Name = "Datasetdescription",
                DisplayName = "Description",
                DataType = "String"
            };
            headerItems.Add(headerItem);

            ViewData["DefaultHeaderList"] = headerItems;

            return headerItems;
        }


        /*
            * Creates a DataTable for given HeaderItems
            * */
        private DataTable CreateDataTable(List<HeaderItem> items)
        {
            DataTable table = new DataTable();

            foreach (HeaderItem item in items)
            {
                table.Columns.Add(new DataColumn()
                {
                    ColumnName = item.Name,
                    Caption = item.DisplayName,
                    DataType = getDataType(item.DataType)
                });
            }
            table.PrimaryKey = new DataColumn[] { table.Columns["ID"] };

            return table;
        }


        /*
            * Needed to create the DataTable
            * Called from within CreateDataTable(List<HeaderItem> items)
            * */
        private Type getDataType(string dataType)
        {
            switch (dataType)
            {
                case "String":
                    {
                        return Type.GetType("System.String");
                    }

                case "Double":
                    {
                        return Type.GetType("System.Double");
                    }

                case "Int16":
                    {
                        return Type.GetType("System.Int16");
                    }

                case "Int32":
                    {
                        return Type.GetType("System.Int32");
                    }

                case "Int64":
                    {
                        return Type.GetType("System.Int64");
                    }

                case "Decimal":
                    {
                        return Type.GetType("System.Decimal");
                    }

                case "DateTime":
                    {
                        return Type.GetType("System.DateTime");
                    }

                default:
                    {
                        return Type.GetType("System.String");
                    }
            }
        }


        /*
            * checks the extension of the file to analyse.
            * the current analysis tool excepts only fq and fastaq extensions
            * */
        public string check_file_extension_to_analyse(string ids, string index, string flag)
        {
            List<string> ids_ = JsonConvert.DeserializeObject<List<string>>(ids);
            foreach (string id_string in ids_)
            {
                long id = Int64.Parse(id_string);
                if (!this.CheckPermission(id))
                {
                    return ("You do not have the right to read this dataset, therefore no analysis are permitted over this dataset.");
                }
                //Int64 id = Int64.Parse(id_);
                //getting the file path of the dataset to send it to the ft´p server for analysis
                DatasetManager dsm = new DatasetManager();
                DatasetVersion dsv = dsm.GetDatasetVersion(dsm.GetDatasetLatestVersionId(id));

                ICollection<ContentDescriptor> cont_desc = dsv.ContentDescriptors;
                foreach (ContentDescriptor c_d in cont_desc)
                {
                    string absolute_file_path = Path.Combine(datasets_root_folder, c_d.URI.ToString());

                    Debug.WriteLine("Dataset id : " + id + "has path : " + absolute_file_path);

                    if (allowed_extention.Contains(Path.GetExtension(c_d.URI)))
                    {
                        try
                        {
                            switch (flag)
                            {
                                case "Rscript":
                                    KeyValuePair<Scripts, string> RScript = analysis_scripts.get_key(Int32.Parse(index), analysis_scripts.R_script_paths);
                                    UploadFiletoAnalysis(absolute_file_path, RScript.Key, RScript.Value);
                                    break;
                                case "Pythonscripts":
                                    KeyValuePair<Scripts, string> PythonScript = analysis_scripts.get_key(Int32.Parse(index), analysis_scripts.Python_script_paths);
                                    UploadFiletoAnalysis(absolute_file_path, PythonScript.Key, PythonScript.Value);
                                    break;
                                case "tool":
                                    KeyValuePair<AnalysisTool, string> AT = analysis_scripts.get_key(Int32.Parse(index), analysis_scripts.tools_list);
                                    UploadFiletoAnalysis(absolute_file_path, AT.Key, AT.Value);
                                    break;
                                default:
                                    return "";
                            }

                            return "Results will be sent to your e-mail, Thank you for your patience. It might take some time !";
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message.ToString());
                            return "Something happened... Please contact the portal's administration";
                        }
                    }
                    else
                    {
                        return "dataset extension is not analyzable ";
                    }
                }
            }

            return "Something happened... Please contact the portal's administration";
        }


        /*
            * Uploads the file to analyse to the server of analysis using ftp connection
            * the analysis script is a python script called using a FLASK server on the VM
            * */
        private void UploadFiletoAnalysis(string filePath, ScriptAndTool id_tool, string value)
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

            string params_ = "file_path=" + filename + "&user_home_directory=" + name + "&email=" + email;

            if (id_tool is AnalysisTool)
            {
                AnalysisTool AT = id_tool as AnalysisTool;
                params_ = params_ + "&exec_tool=" + value.ToLower();
            }

            if (id_tool is Scripts)
            {
                Scripts script = id_tool as Scripts;
                params_ = params_ + "&script=" + script.script + "&language=" + script.script_Type.ToString().ToLower();
            }

            byte[] bytes = Encoding.ASCII.GetBytes(params_);
            request2.ContentLength = bytes.Length;
            try
            {
                using (var reqStream = request2.GetRequestStream())
                {
                    reqStream.Write(bytes, 0, bytes.Length);
                    //var response = (HttpWebResponse)request2.GetResponse();
                    //Debug.WriteLine("response ==> " + response.ToSafeString());
                    reqStream.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message.ToString());
            }

        }

        private bool CheckPermission(long datasetID)
        {
            DatasetManager dm = new DatasetManager();
            EntityPermissionManager entityPermissionManager = new EntityPermissionManager();
            try
            {
                if (dm.IsDatasetCheckedIn(datasetID))
                {
                    // TODO: refactor Download Right not existing, so i set it to read
                    bool downloadAccess = entityPermissionManager.HasEffectiveRight
                        (HttpContext.User.Identity.Name, typeof(Dataset), datasetID, RightType.Read);
                    return downloadAccess;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception Except)
            {
                Debug.WriteLine(Except.ToString());
                return false;
            }
            finally
            {
                dm.Dispose();
                entityPermissionManager.Dispose();
            }

        }
    }
}