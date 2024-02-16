using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using BExIS.Aam.Entities.Mapping;
using BExIS.Aam.Services;
using Vaiona.Utils.Cfg;

namespace BExIS.Modules.Aam.UI.Controllers
{
    public class Aam_UriController : Controller
    {
        public static string path_temp = "";
        // GET: Aam_Uri
        public ActionResult Index()
        {
            Aam_UriManager uri_manager = new Aam_UriManager();
            List <Aam_Uri> list =  uri_manager.get_all_Aam_Uri();
            uri_manager.Dispose();
            return View(list);
        }

        // GET: Aam_Uri/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Aam_Uri/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                string URI = Convert.ToString(collection["URI"]);
                string label = Convert.ToString(collection["label"]);
                string type_uri = Convert.ToString(collection["type_uri"]);
                Aam_UriManager uri_manager = new Aam_UriManager();
                Aam_Uri Aam_Uri = new Aam_Uri(URI, label, type_uri);
                uri_manager.creeate_Aam_Uri(Aam_Uri);
                uri_manager.Dispose();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                string URI = Convert.ToString(collection["URI"]);
                string label = Convert.ToString(collection["label"]);
                string type_uri = Convert.ToString(collection["type_uri"]);
                Aam_Uri Aam_Uri = new Aam_Uri(URI, label, type_uri);
                return View(Aam_Uri);
            }
        }

        // GET: Aam_Uri/Edit/5
        public ActionResult Edit(int id)
        {
            Aam_UriManager uri_manager = new Aam_UriManager();
            Aam_Uri Aam_Uri = uri_manager.get_Aam_Uri_by_id(id);
            return View(Aam_Uri);
        }

        // POST: Aam_Uri/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                string URI = Convert.ToString(collection["URI"]);
                string label = Convert.ToString(collection["label"]);
                string type_uri = Convert.ToString(collection["type_uri"]);
                Aam_UriManager uri_manager = new Aam_UriManager();
                Aam_Uri Aam_Uri = uri_manager.get_Aam_Uri_by_id(id);
                Aam_Uri.label = label;
                Aam_Uri.type_uri = type_uri;
                Aam_Uri.URI = URI;
                uri_manager.edit_Aam_Uri(Aam_Uri);
                uri_manager.Dispose();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                string URI = Convert.ToString(collection["URI"]);
                string label = Convert.ToString(collection["label"]);
                string type_uri = Convert.ToString(collection["type_uri"]);
                Aam_Uri Aam_Uri = new Aam_Uri(URI, label, type_uri);
                return View(Aam_Uri);
            }
        }


        public ActionResult Delete(int id)
        {
            try
            {
                Aam_UriManager uri_manager = new Aam_UriManager();
                Aam_Uri Aam_Uri = uri_manager.get_Aam_Uri_by_id(id);
                uri_manager.delete_Aam_Uri(Aam_Uri);
                uri_manager.Dispose();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }


        public void SelectFileProcess(HttpPostedFileBase SelectFileUploader)
        {
            if (SelectFileUploader != null)
            {
                //data/datasets/1/1/
                string dataPath = AppConfiguration.DataPath; //Path.Combine(AppConfiguration.WorkspaceRootPath, "Data");
                string storepath = Path.Combine(dataPath, "Temp", GetUsernameOrDefault());

                // if folder not exist
                if (!Directory.Exists(storepath))
                {
                    Directory.CreateDirectory(storepath);
                }

                path_temp = Path.Combine(storepath, SelectFileUploader.FileName);

                SelectFileUploader.SaveAs(path_temp);
            }

        }

        public JsonResult update_semantics()
        {
            AnnotationManager AM = new AnnotationManager();
            return Json(AM.Update_semantic_API_data(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult import()
        {
            Aam_UriManager uri_ma = new Aam_UriManager();
            uri_ma.fill_onto_from_file(path_temp);
            return RedirectToAction("Index");
        }

        public string GetUsernameOrDefault()
        {
            string username = string.Empty;
            try
            {
                username = HttpContext.User.Identity.Name;
            }
            catch { }

            return !string.IsNullOrWhiteSpace(username) ? username : "DEFAULT";
        }
    }
}
