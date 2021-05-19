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
        public JObject getStudy(string studyID, DataSource datasource)
        {
            return _sampleAccession.fetchStudy(studyID, datasource);
        }

        [System.Web.Http.HttpPost, System.Web.Http.HttpGet]
        //[GetRoute("api/SampleAccession/add_Dataset/{accession_list}")]
        public JObject add_Dataset(string accession_list)
        {
            return _sampleAccession.AddProjectsdataset(JsonConvert.DeserializeObject<Dictionary<string, string>>(accession_list), GetUsernameOrDefault());
        }

        [System.Web.Http.HttpPost, System.Web.Http.HttpGet]
        //[GetRoute("api/SampleAccession/add_Dataset/{accession_list}")]
        //[ResponseType(typeof(ApiDatasetModel))]
        public IHttpActionResult add_Dataset()
        {
            if (this.Request.Headers.Contains("accession_list"))
            {
                string accession_list = this.Request.Headers.GetValues("accession_list").First();
                JObject jsonObj = _sampleAccession.AddProjectsdataset(JsonConvert.DeserializeObject<Dictionary<string, string>>(accession_list), GetUsernameOrDefault());
                return Ok(jsonObj);
            }
            return Content(HttpStatusCode.BadRequest, "");
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