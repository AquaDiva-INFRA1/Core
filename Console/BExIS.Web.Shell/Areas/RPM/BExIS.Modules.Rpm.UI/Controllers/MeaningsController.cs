﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using BExIS.App.Bootstrap.Attributes;
using BExIS.Dlm.Entities.DataStructure;
using BExIS.Dlm.Entities.Meanings;
using BExIS.Dlm.Services.DataStructure;
using BExIS.Utils.Route;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BExIS.Modules.Rpm.UI.Controllers
{
    public class LinksMeaningsController : System.Web.Mvc.Controller
    {
        public System.Web.Mvc.ActionResult Index()
        {
            return View();
        }
    }
    public class MeaningsController : ApiController
    {

        private readonly BExIS.Dlm.Entities.Meanings.ImeaningManagr _meaningManager;
        public MeaningsController(ImeaningManagr _meaningManager)
        {
            this._meaningManager = _meaningManager;
        }
        public MeaningsController()
        {
            if (this._meaningManager == null)
               this._meaningManager = new meaningManager();
        }


        [BExISApiAuthorize]
        [HttpPost,HttpGet]
        [PostRoute("api/Meanings/Index")]
        [GetRoute("api/Meanings/Index")]
        public JObject Index()
        {
            return _meaningManager.getMeanings();
        }

        [BExISApiAuthorize]
        [HttpPost,HttpGet]
        [PostRoute("api/Meanings/Details")]
        [GetRoute("api/Meanings/Details")]
        public JObject Details()
        {
            string id = this.Request.Content.ReadAsStringAsync().Result.ToString();
            Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(id);

            return  _meaningManager.getMeaning(long.Parse(dict["id"]));
        }

        [BExISApiAuthorize]
        [HttpPost,HttpGet]
        [PostRoute("api/Meanings/Create")]
        [GetRoute("api/Meanings/Create")]
        public JObject Create(HttpRequestMessage request)
        {
            try
            {
                String Name = Convert.ToString(HttpContext.Current.Request.Form["Name"]);

                String ShortName = Convert.ToString(HttpContext.Current.Request.Form["ShortName"]);
                
                String Description = Convert.ToString(HttpContext.Current.Request.Form["Description"]);

                Selectable selectable = (Selectable)Enum.Parse(typeof(Selectable), Convert.ToString(HttpContext.Current.Request.Form["selectable"]));

                Approved approved = (Approved)Enum.Parse(typeof(Approved), Convert.ToString(HttpContext.Current.Request.Form["approved"]));

                List<string> externalLink = HttpContext.Current.Request.Form["externalLink[]"] != null ? Convert.ToString(HttpContext.Current.Request.Form["externalLink[]"]).Split(',').ToList<string>() : new List<string>();

                List<string> related_meaning = HttpContext.Current.Request.Form["related_meaning"]!= null ? Convert.ToString(HttpContext.Current.Request.Form["related_meaning"]).Split(',').ToList<string>() : new List<string>();
                List<string> variable = HttpContext.Current.Request.Form["Variable"] != null ? Convert.ToString(HttpContext.Current.Request.Form["Variable"]).Split(',').ToList<string>() : new List<string>();

                JObject res = _meaningManager.addMeaning(Name, ShortName, Description, selectable, approved, variable, externalLink, related_meaning);
                return res;
            }
            catch
            {
                return null;
            }
        }


        [BExISApiAuthorize]
        [HttpPost, HttpGet]
        [PostRoute("api/Meanings/EditMeaning")]
        [GetRoute("api/Meanings/EditMeaning")]
        public JObject EditMeaning(HttpRequestMessage request)
        {
            Meaning m = null;
            try
            {
                String Name = Convert.ToString(HttpContext.Current.Request.Form["Name"]);

                String ShortName = Convert.ToString(HttpContext.Current.Request.Form["ShortName"]);

                String Description = Convert.ToString(HttpContext.Current.Request.Form["Description"]);

                Selectable selectable = (Selectable)Enum.Parse(typeof(Selectable), Convert.ToString(HttpContext.Current.Request.Form["selectable"]));

                Approved approved = (Approved)Enum.Parse(typeof(Approved), Convert.ToString(HttpContext.Current.Request.Form["approved"]));

                List<string> externalLink = HttpContext.Current.Request.Form["externalLink[]"] != null ? Convert.ToString(HttpContext.Current.Request.Form["externalLink[]"]).Split(',').ToList<string>() : new List<string>();

                List<string> related_meaning = HttpContext.Current.Request.Form["related_meaning"] != null ? Convert.ToString(HttpContext.Current.Request.Form["related_meaning"]).Split(',').ToList<string>() : new List<string>();
                List<string> variable = HttpContext.Current.Request.Form["Variable[]"] != null ? Convert.ToString(HttpContext.Current.Request.Form["Variable[]"]).Split(',').ToList<string>() : new List<string>();

                string id = Convert.ToString(HttpContext.Current.Request.Form["id"]);
                //if (related_meaning.Count== 0 )
                //    related_meaning = _meaningManager.getMeaning(long.Parse(id)).ToObject<Meaning>();
                JObject res = _meaningManager.editMeaning(id, Name, ShortName, Description, selectable, approved, variable, externalLink, related_meaning);
                return res;

                JObject obj = _meaningManager.getMeaning(Int64.Parse(id));
                m = JsonConvert.DeserializeObject<Meaning>(obj["Value"].ToString());
            }
            catch
            {
                return null;
            }
        }

        [BExISApiAuthorize]
        [HttpPost,HttpGet]
        [PostRoute("api/Meanings/Delete")]
        [GetRoute("api/Meanings/Delete")]
        public JObject Delete(HttpRequestMessage request)
        {
            try
            {
                string id = this.Request.Content.ReadAsStringAsync().Result.ToString();
                return _meaningManager.deleteMeaning(Int64.Parse(id));
            }
            catch
            {
                return null;
            }
        }

        [BExISApiAuthorize]
        [HttpPost, HttpGet]
        [PostRoute("api/Meanings/getVariables")]
        [GetRoute("api/Meanings/getVariables")]
        public JObject getVariables()
        {
            using (DataStructureManager dsm = new DataStructureManager())
            {
                List<Variable> variables = (List<Variable>)dsm.VariableRepo.Get();
                Dictionary<long, string> fooDict = variables.ToDictionary(f => f.Id, f => f.Label);
                string json_string = JsonConvert.SerializeObject(fooDict);
                return JObject.Parse(json_string);
            }
        }

        [BExISApiAuthorize]
        [HttpPost, HttpGet]
        [PostRoute("api/Meanings/updateRelatedManings")]
        [GetRoute("api/Meanings/updateRelatedManings")]
        public JObject updateRelatedManings (HttpRequestMessage request)
        {
            try
            {
                String parentID = Convert.ToString(HttpContext.Current.Request.Form["parentID"]);
                String childID = Convert.ToString(HttpContext.Current.Request.Form["childID"]);
                return _meaningManager.updateRelatedManings(parentID, childID);
            }
            catch(Exception exc)
            {
                return null;
            }
        }


        // external links endpoints

        [BExISApiAuthorize]
        [HttpPost, HttpGet]
        [PostRoute("api/Meanings/getExternalLinks")]
        [GetRoute("api/Meanings/getExternalLinks")]
        public JObject getExternalLinks()
        {
            return _meaningManager.getExternalLinks();
        }

        [BExISApiAuthorize]
        [HttpPost, HttpGet]
        [PostRoute("api/Meanings/createExternalLink")]
        [GetRoute("api/Meanings/createExternalLink")]
        public JObject createExternalLink(HttpRequestMessage request)
        {
            try
            {
                String uri = Convert.ToString(HttpContext.Current.Request.Form["uri"]);

                String name = Convert.ToString(HttpContext.Current.Request.Form["name"]);

                String type = Convert.ToString(HttpContext.Current.Request.Form["type"]);

                JObject res = _meaningManager.addExternalLink(uri, name, type);
                return res;
            }
            catch
            {
                return null;
            }
        }

        [BExISApiAuthorize]
        [HttpPost, HttpGet]
        [PostRoute("api/Meanings/editExternalLinks")]
        [GetRoute("api/Meanings/editExternalLinks")]
        public JObject editExternalLinks(HttpRequestMessage request)
        {
            ExternalLink m = null;
            try
            {
                String URI = Convert.ToString(HttpContext.Current.Request.Form["URI"]);

                String Name = Convert.ToString(HttpContext.Current.Request.Form["Name"]);

                String Type = Convert.ToString(HttpContext.Current.Request.Form["Type"]);

                string id = Convert.ToString(HttpContext.Current.Request.Form["id"]);
                JObject res = _meaningManager.editExternalLink(id, URI, Name, Type);
                return res;

                JObject obj = _meaningManager.getExternalLink(Int64.Parse(id));
                m = JsonConvert.DeserializeObject<ExternalLink>(obj["Value"].ToString());
            }
            catch
            {
                return null;
            }
        }

        [BExISApiAuthorize]
        [HttpPost, HttpGet]
        [PostRoute("api/Meanings/deleteExternalLinks")]
        [GetRoute("api/Meanings/deleteExternalLinks")]
        public JObject deleteExternalLinks(HttpRequestMessage request)
        {
            try
            {
                string id = this.Request.Content.ReadAsStringAsync().Result.ToString();
                return _meaningManager.deleteExternalLink(Int64.Parse(id));
            }
            catch
            {
                return null;
            }
        }

        [BExISApiAuthorize]
        [HttpPost, HttpGet]
        [PostRoute("api/Meanings/DetailExternalLinks")]
        [GetRoute("api/Meanings/DetailExternalLinks")]
        public JObject DetailExternalLinks()
        {
            string id = this.Request.Content.ReadAsStringAsync().Result.ToString();
            Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(id);

            return _meaningManager.getExternalLink(long.Parse(dict["id"]));
        }
    }
}
