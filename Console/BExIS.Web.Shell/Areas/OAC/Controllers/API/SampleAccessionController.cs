using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using BExIS.OAC.Services;
using BExIS.Utils.Route;
using BExIS.App.Bootstrap.Attributes;
using System.Web.Routing;
using BEXIS.OAC.Entities;
using Newtonsoft.Json.Linq;
using BExIS.Dlm.Entities.Data;
using Newtonsoft.Json;
using System.Net;

namespace BExIS.Modules.OAC.UI.Controllers
{
    public class SampleAccessionController : ApiController
    {
        private readonly ISampleAccession _sampleAccession;
        public SampleAccessionController(ISampleAccession sampleAccession)
        {
            this._sampleAccession = sampleAccession;
        }
        public SampleAccessionController()
        {
            _sampleAccession = new SampleAccessionManager();
        }

        // GET: Sample
        [BExISApiAuthorize]
        [GetRoute("api/SampleAccession/getStudy/{studyID}/{datasource}")]
        public JObject getStudy(string studyID, DataSource datasource)
        {
            try
            {
                return _sampleAccession.fetchStudy(studyID, datasource);
            }
            catch (Exception e)
            {
                return new JObject () ;
            }
            
        }

        [BExISApiAuthorize]
        [System.Web.Http.HttpPost, System.Web.Http.HttpGet]
        //[GetRoute("api/SampleAccession/add_Dataset/{accession_list}")]
        //[ResponseType(typeof(ApiDatasetModel))]
        public IHttpActionResult add_Dataset()
        {
            try
            {
                System.IO.StreamReader reader = new System.IO.StreamReader(HttpContext.Current.Request.InputStream);
                reader.BaseStream.Position = 0;
                string requestFromPost = reader.ReadToEnd();
                JObject jsonObj_ = _sampleAccession.AddProjectsdataset(JsonConvert.DeserializeObject<Dictionary<string, string>>(requestFromPost), GetUsernameOrDefault());
                return Ok(jsonObj_);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, e.Message);
            }
        }

        private string GetUsernameOrDefault()
        {
            string username = string.Empty;
            try
            {
                username = User.Identity.Name;
            }
            catch { }

            return !string.IsNullOrWhiteSpace(username) ? username : "DEFAULT";
        }
    }
}