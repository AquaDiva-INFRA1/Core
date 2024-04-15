using BExIS.App.Bootstrap.Attributes;
using BExIS.ASM.Services;
using BExIS.Utils.Route;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace BExIS.Modules.ASM.UI.Controllers
{

    public class StatisticsController : ApiController
    {
        private readonly IStatisticsExtractor _StatisticsExtractor;
        public StatisticsController(IStatisticsExtractor StatisticsExtractor)
        {
            this._StatisticsExtractor = StatisticsExtractor;
        }
        public StatisticsController()
        {
            _StatisticsExtractor = new StatisticsExtractor();
        }

        #region test MVC endpoint to consume API endpoint using token
        /*
        /// <param name="id">Dataset Id</param>
        [BExISApiAuthorize]
        //[Route("api/Data")]
        [GetRoute("api/Summary/{id}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetAsync(int id)
        {
            string token = this.Request.Headers.Authorization?.Parameter;
            using (var client = new HttpClient())
            {
                string url = this.Request.RequestUri.Authority;  // "http://" + Request.Url.Authority + "/api/DataStatistic/";
                string param = id.ToString();
                client.BaseAddress = new Uri(url + param);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "UeyJRyCegdcfxiKXw99NtgTRYJy2hThzo2GL3TbW55Eiq3ShQKQ8gdiV88a8ZW85");
                
                //client.DefaultRequestHeaders.Authorization =
                //new AuthenticationHeaderValue(
                //    "Basic", Convert.ToBase64String(
                //        System.Text.ASCIIEncoding.ASCII.GetBytes(
                //           $"{yourusername}:{yourpwd}")));
                
                var responseTask = client.GetAsync("");
                responseTask.Wait();
                //To store result of web api response.   
                Stream result = await responseTask.Result.Content.ReadAsStreamAsync();
                string jsonstring = new StreamReader(result).ReadToEnd();
            }
        }
        */
        #endregion
        

        static JObject result;

        [BExISApiAuthorize]
        [GetRoute("api/Statistics/get")]
        public HttpResponseMessage getData()
        {
            JObject metadata_obj = _StatisticsExtractor.allMetadata_extract ();
            if (metadata_obj == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);

            JObject datastructs_obj = _StatisticsExtractor.allDatastructure_extract();
            if (datastructs_obj == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);

            datastructs_obj.Merge(metadata_obj, new JsonMergeSettings
            {
                // union array values together to avoid duplicates
                MergeArrayHandling = MergeArrayHandling.Union
            });

            JObject annotations = _StatisticsExtractor.allAnnotation_extract(datastructs_obj);
            if (annotations == null)
                return Request.CreateResponse(HttpStatusCode.NoContent);

            annotations.Merge(datastructs_obj, new JsonMergeSettings
            {
                // union array values together to avoid duplicates
                MergeArrayHandling = MergeArrayHandling.Union
            });

            Dictionary<string, JObject> results = new Dictionary<string, JObject>();
            results.Add("extra_stats", _StatisticsExtractor.get_extra_stats());
            results.Add("primary_stats", annotations);

            string resp = JsonConvert.SerializeObject(results);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(resp, System.Text.Encoding.UTF8, "application/json");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return response;

        }

        [BExISApiAuthorize]
        [GetRoute("api/Statistics/reset")]
        public HttpResponseMessage reset()
        {
            JObject res = _StatisticsExtractor.reset();
            string resp = JsonConvert.SerializeObject(res);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(resp, System.Text.Encoding.UTF8, "application/json");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return response;
        }

        
    }

}