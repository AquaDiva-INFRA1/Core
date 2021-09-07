using BExIS.App.Bootstrap.Attributes;
using BExIS.Utils.Route;
using Newtonsoft.Json;
using System;
using System.IO;

//using System.Linq.Dynamic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace BExIS.Modules.ASM.UI.Controllers
{

    public class SummaryController : ApiController
    {
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
                /*
                client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Basic", Convert.ToBase64String(
                        System.Text.ASCIIEncoding.ASCII.GetBytes(
                           $"{yourusername}:{yourpwd}")));
                */
                var responseTask = client.GetAsync("");
                responseTask.Wait();
                //To store result of web api response.   
                Stream result = await responseTask.Result.Content.ReadAsStreamAsync();
                string jsonstring = new StreamReader(result).ReadToEnd();
            }

            return getData(id, -1, token);
        }

        [BExISApiAuthorize]
        [GetRoute("api/Summary/get_data/{id}")]
        [HttpGet]
        public HttpResponseMessage get_data(int id=79)
        {
            return getData(id, -1, "");
        }

        private HttpResponseMessage getData(long id, int variableId, string token)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            string resp = JsonConvert.SerializeObject("");

            response.Content = new StringContent(resp, System.Text.Encoding.UTF8, "application/json");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return response;
        }




    }

}