using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BExIS.Dlm.Entities.Data;
using BExIS.Dlm.Entities.DataStructure;
using BExIS.Dlm.Services.Data;
using System.Configuration;
using System.Linq;
using Vaiona.Web.Mvc.Data;
using BExIS.Aam.Entities.Mapping;
using BExIS.Aam.Services;
using System.IO;
using Vaiona.Utils.Cfg;
using Newtonsoft.Json.Linq;
using BExIS.Dlm.Services.DataStructure;

namespace BExIS.Modules.Ddm.UI.Controllers
{
    public class LandingPageController : Controller
    {
        static string Conx = ConfigurationManager.ConnectionStrings[1].ConnectionString;

        [DoesNotNeedDataAccess]
        [HttpGet]
        public ActionResult Index()
        {
            String temp_file = Path.Combine(AppConfiguration.GetModuleWorkspacePath("ASM"), "Analytics_temp.txt");
            JObject stats_obj = JObject.Parse(System.IO.File.ReadAllText(temp_file));
            ViewData["datasetCount"] = stats_obj["dataset_count"].ToString();
            ViewData["Datapoints"] = stats_obj["datapoints"].ToString();

            //using (VariableManager vm = new VariableManager())
            //using (Aam_Dataset_column_annotationManager aam_manager = new Aam_Dataset_column_annotationManager())
            //{
            //    int var_count = vm.VariableInstanceRepo.Get().Count;
            //    int covered_var = aam_manager.get_all_dataset_column_annotation().Count;
            //    ViewData["semantic_Coverage"] = (covered_var / var_count * 100).ToString();
            //}

            return View("Index");

            //var result = this.Render("homepage", "homepage", "Index");
            //return Content(result.ToHtmlString(), "text/html");
        }
    }
} 