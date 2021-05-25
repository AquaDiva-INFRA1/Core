using System;
using BExIS.Modules.OAC.UI.Models;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using BExIS.Modules.Dcm.UI.Controllers;
using BExIS.Modules.Dcm.UI.Models;
using System.Net;
using System.Web.Script.Serialization;
using System.Linq;
using Newtonsoft.Json.Linq;
using BExIS.Dlm.Entities.Data;
using System.Web.Configuration;
using Newtonsoft.Json;
using Vaiona.Utils.Cfg;
using System.Web.Mvc;
using BEXIS.OAC.Entities;
using System.Net.Http;
using System.Text;

namespace BExIS.Modules.OAC.UI.Controllers
{
    public class HomeController : Controller
    {
        static ViewFormModel model;
        public ActionResult Index()
        {
            CreateDatasetController HelperController = new CreateDatasetController();

            model = new ViewFormModel(HelperController.LoadMetadataStructureViewList(), 
                HelperController.LoadDataStructureViewList(),
                GetDataSourceList());

            HelperController.Dispose();

            return View(model);
        }

        public async System.Threading.Tasks.Task<ActionResult> FetchDataFromPortalAsync()
        {
            if (model != null)
                if(model.Accessions.Count > 0 )
                    if (Request.Params["Identifier"] == model.Identifier)
                        return View("Index", model);

            CreateDatasetController HelperController = new CreateDatasetController();

            model = new ViewFormModel()
            {
                MetadataStructureViewList = HelperController.LoadMetadataStructureViewList(),
                DataStructureViewList = HelperController.LoadDataStructureViewList(),
                DataSourceViewList = GetDataSourceList(),
                Accessions = new Dictionary<string, string>()
            };

            HelperController.Dispose();

            try
            {
                string Identifier = Request.Params["Identifier"];
                long MetadataStructureId = long.Parse(Request.Params["SelectedMetadataStructureId"]);
                long DataStructureId = Request.Params["SelectedDataStructureId"] == null ? GetDefaultUnstructuredDataStructureId() : long.Parse(Request.Params["SelectedDataStructureId"]);
                Int32 DataSourceId = Int32.Parse(Request.Params["SelectedDataSourceId"]);

                model.Identifier = Identifier;
                model.SelectedDataSourceId = DataSourceId;
                model.SelectedDataStructureId = DataStructureId;
                model.SelectedMetadataStructureId = MetadataStructureId;

                using (var client = new HttpClient())
                {
                    string url = "http://"+this.ControllerContext.HttpContext.Request.Url.Authority + "/api/SampleAccession/getStudy/";
                    //string param = "studyID=" + Identifier + "&datasource=" + DataSourceId.ToString();
                    string param =  Identifier + "/" + DataSourceId.ToString(); 
                    client.BaseAddress = new Uri(url+ param);
                    //client.DefaultRequestHeaders.Add("studyID", Identifier);
                    //client.DefaultRequestHeaders.Add("datasource", DataSourceId.ToString());
                    var responseTask = client.GetAsync("");
                    responseTask.Wait();
                    //To store result of web api response.   
                    Stream result = await responseTask.Result.Content.ReadAsStreamAsync();
                    string jsonstring = new StreamReader(result).ReadToEnd();
                    model.Accessions = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonstring);
                }
                return View("Index", model);

            }
            catch (WebException e)
            {
                String msg = "";
                HttpWebResponse errorResponse = e.Response as HttpWebResponse;
                if (errorResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    msg = "Sample was not found!";
                }
                else
                {
                    msg = e.Message;
                }

                #region show the error message

                model.Error = "An error occurred: " + msg;

                return View("Index", model);

                #endregion
            }
            catch (Exception e)
            {
                #region show the error message

                model.Error = "An error occurred: " + e.Message;

                return View("Index", model);

                #endregion
            }
        }

        public async System.Threading.Tasks.Task<ActionResult> LoadSamplesViewMetadataAsync(string sample, string project)
        {
            string accessionValue = model.Accessions.FirstOrDefault(xx => xx.Key.Contains(sample)).Value.Replace(" ", "").Replace("\n", "").Replace('"', '\"');
            AccessionMetadata metadata = new AccessionMetadata(JObject.Parse(accessionValue));
            string json_string = JsonConvert.SerializeObject(metadata);
            return PartialView("View", metadata);
        }

        public async System.Threading.Tasks.Task<long> SubmitAsync(string acc)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (string s in acc.Split(',').ToList())
            {
                KeyValuePair<string, string> xx = model.Accessions.FirstOrDefault(x => x.Key.Contains(s));
                dict.Add(xx.Key, xx.Value);
            }

            using (var client = new HttpClient())
            {
                string url = "http://" + this.ControllerContext.HttpContext.Request.Url.Authority + "/api/SampleAccession/add_Dataset";
                client.BaseAddress = new Uri(url );
                StringContent data = new StringContent(JsonConvert.SerializeObject(dict), Encoding.UTF8, "application/json");
                var responseTask = await client.PostAsync(url, data);
                string result = await responseTask.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<dynamic>(result).dataset_id;
            }
            return new Dataset().Id;
        }

        public ActionResult QueryAvailableMappings()
        {

            DataSource source = (DataSource)long.Parse(Request.Params["SelectedDataSourceId"]);
            string sourceName = GetSourceName(source);

            CreateDatasetController HelperController = new CreateDatasetController();
            var msList = HelperController.LoadMetadataStructureViewList();
            var list = new List<object>();
            foreach (var entry in msList)
            {
                var path_mappingFile = Path.Combine(AppConfiguration.GetModuleWorkspacePath("DIM"), "MappingFile_extern_" + sourceName + "_to_intern_" + entry.Title + ".xml");
                list.Add(Json(new { title = entry.Title, id = entry.Id, hasFile = System.IO.File.Exists(path_mappingFile) }));
            }

            JavaScriptSerializer js = new JavaScriptSerializer();
            HelperController.Dispose();
            return Content(js.Serialize(list));

        }

        // internal UI controller related functions
        public long GetDefaultUnstructuredDataStructureId()
        {
            var x = new Dlm.Services.DataStructure.DataStructureManager();
            var y = x.UnStructuredDataStructureRepo.Get();
            return y[0].Id;

        }

        public void ConvertXMLItemKeys(XmlDocument doc, XmlNode parent)
        {
            foreach(XmlNode node in parent.ChildNodes)
            {
                if(node.Name == "item" && node.Attributes["key"] != null)
                {
                    var NewChild = doc.CreateNode(System.Xml.XmlNodeType.Element, "", node.Attributes["key"].Value, "");
                    foreach(XmlNode child in node.ChildNodes)
                    {
                        NewChild.AppendChild(child);
                    }
                    parent.AppendChild(NewChild);
                } else
                {
                    ConvertXMLItemKeys(doc, node);
                }
            }
        }

        public String GetSourceName(DataSource source)
        {
            switch (source)
            {// if a different name is needed, switch it
                case DataSource.NCBI:
                    return "EBI";
                default:
                    return Enum.GetName(typeof(DataSource), source);
            }
        }

        public List<ListViewItem> GetDataSourceList()
        {
            List<ListViewItem> temp = new List<ListViewItem>();

            Type type = typeof(DataSource);

            foreach (DataSource source in Enum.GetValues(type))
            {
                temp.Add(new ListViewItem((long)source, Enum.GetName(type, source)));
            }

            return temp;
        }

    }

}
