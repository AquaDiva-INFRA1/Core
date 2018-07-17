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

namespace BExIS.Modules.Sam.UI.Controllers
{
    public class HomeController : Controller
    {
        static Unstructred_data model;
        static List<HeaderItem> headerItems;
        static HeaderItem idHeader;

        // GET: Home
        public ActionResult Index()
        {
            List<HeaderItem> header_items = makeHeader();
            DataTable result_table = CreateDataTable(header_items);

            return View(result_table);
        }
        

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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

        public void UploadFiletoAnalysis2()
        {
            String username = "PubServerAD";
            String password = "pubserverad";
            string FTPAddress = "ftp://192.168.37.3:21";
            String filePath = @"C:\Users\admin\Desktop\\sequence_data_examples\reads_se_01.fastq";
            String filename = Path.GetFileName(filePath);

            // upload the file to analyse
            WebRequest request = WebRequest.Create(FTPAddress + "/" + filename);
            request.Credentials = new NetworkCredential(username, password);
            request.Method = WebRequestMethods.Ftp.UploadFile;

            try
            {
                using (Stream fileStream = System.IO.File.OpenRead(filePath))
                using (Stream ftpStream = request.GetRequestStream())
                {
                    fileStream.CopyTo(ftpStream);
                }
            }
            catch (WebException e)
            {
                Debug.WriteLine(e.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message.ToString());
            }

            // run the analysis
            try
            {
                string url = "http://192.168.37.3:5000";
                var request2 = (HttpWebRequest)WebRequest.Create(url);
                request2.Method = "POST";
                request2.ContentType = "application/x-www-form-urlencoded";
                byte[] bytes = Encoding.ASCII.GetBytes("file_path=" + filename + "&user_home_directory=" + username);
                request2.ContentLength = bytes.Length;
                using (var reqStream = request2.GetRequestStream())
                {
                    reqStream.Write(bytes, 0, bytes.Length);
                    //var response = (HttpWebResponse)request2.GetResponse();
                    //Debug.WriteLine("response ==> " + response.ToSafeString());
                }
            }
            catch (WebException e)
            {
                Debug.WriteLine(e.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message.ToString());
            }

        }
    }
}