using BExIS.App.Bootstrap.Attributes;
using BExIS.OAC.Services;
using BExIS.Utils.Route;
using BEXIS.OAC.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;
using System.Web.Http;

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
            if (this._sampleAccession == null)
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
            catch (Exception)
            {
                return new JObject();
            }

        }

        [BExISApiAuthorize]
        [PostRoute("api/SampleAccession/add_study")]
        [GetRoute("api/SampleAccession/add_study")]
        public JObject add_study()
        {
            try
            {

                //Tuple<string, string> data = JsonConvert.DeserializeObject<Tuple<string, string>>(data_);
                string res = this.Request.Content.ReadAsStringAsync().Result.ToString();
                Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(res);

                string accession; dict.TryGetValue("data", out accession);
                string username; dict.TryGetValue("username", out username);
                string metadata; dict.TryGetValue("metadata", out metadata);


                JObject jsonObj_ = _sampleAccession.AddProjectsdataset(JsonConvert.DeserializeObject<Dictionary<string, string>>(accession), username, metadata);
                return jsonObj_;
                //return Ok(jsonObj_);
            }
            catch (Exception e)
            {
                return JObject.Parse(JsonConvert.SerializeObject(new { e.Message }));
                //return Content(HttpStatusCode.BadRequest, e.Message);
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