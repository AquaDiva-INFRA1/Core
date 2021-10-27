using BExIS.App.Bootstrap.Attributes;
using BExIS.Utils.Route;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using BEXIS.ASM.Services;

//using System.Linq.Dynamic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using Newtonsoft.Json.Linq;

namespace BExIS.Modules.ASM.UI.Controllers
{

    public class SummaryController : ApiController
    {
        private readonly ISummary _summary;
        public SummaryController(ISummary summary)
        {
            this._summary = summary;
        }
        public SummaryController()
        {
            if (this._summary == null)
                _summary = new summaryManager();
        }

        [BExISApiAuthorize]
        [HttpPost]
        [PostRoute("api/Summary/getSummary")]
        [GetRoute("api/Summary/getSummary")]
        public async Task<string> getSummary()
        {
            string res = this.Request.Content.ReadAsStringAsync().Result.ToString();
            Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(res);

            string dataset; 
            dict.TryGetValue("data", out dataset);
            string username; 
            dict.TryGetValue("username", out username);

            string result = await _summary.get_analysisAsync(dataset, username);
            return result;
        }

        [BExISApiAuthorize]
        [HttpPost]
        [PostRoute("api/Summary/getCategrocialAnalysis")]
        [GetRoute("api/Summary/getCategrocialAnalysis")]
        public async Task<string> getCategrocialAnalysis()
        {
            string res = this.Request.Content.ReadAsStringAsync().Result.ToString();
            Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(res);

            string data;
            dict.TryGetValue("data", out data);
            string username;
            dict.TryGetValue("username", out username);

            string result= await _summary.get_summary(data, username);
            return result;
        }

        [BExISApiAuthorize]
        [HttpPost]
        [PostRoute("api/Summary/getSamplingSummary")]
        [GetRoute("api/Summary/getSamplingSummary")]
        public async Task<JObject> getSamplingSummary()
        {
            string res = this.Request.Content.ReadAsStringAsync().Result.ToString();
            Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(res);

            string data;
            dict.TryGetValue("data", out data);
            string username;
            dict.TryGetValue("username", out username);

            JObject jsonObj_ = await _summary.get_sampling_summary(data, username);
            return jsonObj_;
        }
    }

}