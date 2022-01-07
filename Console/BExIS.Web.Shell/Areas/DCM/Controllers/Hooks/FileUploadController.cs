﻿using BExIS.Modules.Dcm.UI.Models.Edit;
using BExIS.UI.Hooks;
using BExIS.UI.Hooks.Caches;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Vaiona.Utils.Cfg;

namespace BExIS.Modules.Dcm.UI.Controllers
{
    public class FileUploadController : Controller
    {
        /// <summary>
        /// entry for hook
        /// </summary>
        /// <returns></returns>
        public ActionResult Start(long id, int version)
        {
            //return View();
            return RedirectToAction("load", new { id, version });
        }

        // GET: FileUpload
        public JsonResult Load(long id, int version)
        {
            FileUploadModel model = new FileUploadModel();
            HookManager hookManager = new HookManager();

            // load cache to check existing files
            EditDatasetDetailsCache cache = hookManager.LoadCache<EditDatasetDetailsCache>("dataset", "details", HookMode.edit, id);

            // check if files in list also on server
            string path = Path.Combine(AppConfiguration.DataPath, "datasets", id.ToString(), "Temp");
            for (int i = 0; i < cache.Files.Count; i++)
            {
                var file = cache.Files[i];
                //check if if exist on server or not
                if (file != null && !string.IsNullOrEmpty(file.Name) && System.IO.File.Exists(Path.Combine(path, file.Name)))
                    model.ExistingFiles.Add(file); // if exist  add to model
                else
                    cache.Files.RemoveAt(i); // if not remove from cache
            }

            hookManager.SaveCache(cache, "dataset", "details", HookMode.edit, id);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Upload(long id)
        {
            HookManager hookManager = new HookManager();
            EditDatasetDetailsCache cache = hookManager.LoadCache<EditDatasetDetailsCache>("dataset", "details", HookMode.edit, id);
            List<string> filesNames = new List<string>();

            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";
                        //string filename = Path.GetFileName(Request.Files[i].FileName);

                        HttpPostedFileBase file = files[i];
                        string fname;

                        // Checking for Internet Explorer
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }

                        //data/datasets/1/1/
                        var dataPath = AppConfiguration.DataPath; //Path.Combine(AppConfiguration.WorkspaceRootPath, "Data");
                        var storepath = Path.Combine(dataPath, "Datasets", id.ToString(), "Temp");

                        // if folder not exist
                        if (!Directory.Exists(storepath))
                        {
                            Directory.CreateDirectory(storepath);
                        }

                        var path = Path.Combine(storepath, fname);

                        file.SaveAs(path);

                        cache.Files.Add(new BExIS.UI.Hooks.Caches.FileInfo(fname, file.ContentType, file.ContentLength));
                        filesNames.Add(fname);
                    }
                    // Returns message that successfully uploaded
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
                finally
                {
                    List<string> messages = new List<string> { "Files uploaded" };
                    messages.AddRange(filesNames);

                    cache.Messages.Add(new ResultMessage(DateTime.Now, messages));
                    hookManager.SaveCache(cache, "dataset", "details", HookMode.edit, id);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }
    }
}