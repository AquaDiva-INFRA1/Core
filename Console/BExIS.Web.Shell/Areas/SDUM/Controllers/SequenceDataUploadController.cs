using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Diagnostics;
using BExIS.Dlm.Services.Data;
using Npgsql;
using System.Configuration;
using BExIS.Dlm.Services.DataStructure;
using BExIS.Dlm.Entities.DataStructure;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json.Linq;
using BExIS.Modules.Rpm.UI.Models;
using System.Linq;
using System.Xml;
using BExIS.Aam.Services;
using BExIS.Aam.Entities.Mapping;
using BExIS.Modules.Rpm.UI.Controllers;
using BExIS.IO.Transform.Output;
using BExIS.Utils.Models;
using System.Data;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using BExIS.Modules.SDUM.UI.Models;

namespace BExIS.Modules.Sdum.UI.Controllers
{
    public class SequenceDataUploadController : Controller
    {
        static EBIresponseModel EBIresponseModel = new EBIresponseModel();
        static string ebiSampleAPIurl = "https://www.ebi.ac.uk/biosamples/api/samples/";
        public ActionResult Index(string sampleID)
        {
            if (sampleID != null)
            {
                sampleID = sampleID.Replace("\"", "");
                var request = (HttpWebRequest)WebRequest.Create(ebiSampleAPIurl + sampleID);

                var response = (HttpWebResponse)request.GetResponse();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                JObject json = JObject.Parse(responseString);

                EBIresponseModel = new EBIresponseModel(json);
                return View("Index", EBIresponseModel);
            }

            return View("Index");
        }

        public ActionResult getMetadataFromSampleID(string sampleID)
        {
            sampleID = sampleID.Replace("\"", "");
            var request = (HttpWebRequest)WebRequest.Create(ebiSampleAPIurl+sampleID);

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            JObject json = JObject.Parse(responseString);

            EBIresponseModel = new EBIresponseModel(json);

            var oMycustomclassname = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseString);

            return View(EBIresponseModel);
        }


    }
    
}