﻿using System.Web.Mvc;
using Vaiona.Web.Mvc.Data;
using Vaiona.Web.Mvc.Modularity;
using BExIS.Dlm.Entities.Data;
using BExIS.Dlm.Entities.DataStructure;
using BExIS.Dlm.Services.Data;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using BExIS.Modules.Rpm.UI.Models;

namespace BExIS.Web.Shell.Controllers
{
    public class HomeController : Controller
    {
        static string Conx = ConfigurationManager.ConnectionStrings[1].ConnectionString;

        [DoesNotNeedDataAccess]
        public ActionResult Index()
        {
            DatasetManager dm = new DatasetManager();
            List<Dataset> datasets = new List<Dataset>();
            List<long> datasetIds = new List<long>();
            datasets = dm.DatasetRepo.Query().OrderBy(p => p.Id).ToList();
            datasetIds = datasets.Select(p => p.Id).ToList();
            long somme = 0;
            foreach (Dataset ds in datasets)
            {
                long noColumns = ds.DataStructure.Self is StructuredDataStructure ? (ds.DataStructure.Self as StructuredDataStructure).Variables.Count() : 0L;
                long noRows = ds.DataStructure.Self is StructuredDataStructure ? dm.GetDatasetLatestVersionEffectiveTupleCount(ds) : 0; // It would save time to calc the row count for all the datasets at once!
                if (ds.Status == DatasetStatus.CheckedIn)
                {
                    somme = somme + (noRows * noColumns);
                }
            }
            ViewData["datasetCount"] = datasets.Count;
            ViewData["Datapoints"] = somme;
            string select = "select count (*) from dataset_column_annotation";
            NpgsqlCommand MyCmd = null;
            NpgsqlConnection MyCnx = null;
            MyCnx = new NpgsqlConnection(Conx);
            MyCnx.Open();
            MyCmd = new NpgsqlCommand(select, MyCnx);

            Int64 count = (Int64)MyCmd.ExecuteScalar();
            MyCnx.Close();


            DataAttributeManagerModel dam = new DataAttributeManagerModel(false);

            ViewData["semantic_Coverage"] = (double)dam.DataAttributeStructs.Count / (double)count;

            return PartialView();

            //var result = this.Render("homepage", "homepage", "Index");
            //return Content(result.ToHtmlString(), "text/html");
        }

        [DoesNotNeedDataAccess]
        public ActionResult SessionTimeout()
        {
            return View();
        }
        
        [DoesNotNeedDataAccess]
        public ActionResult RedirectToWiki()
        {
            return Redirect("https://aquadiva-trac1.inf-bb.uni-jena.de/wiki/doku.php");
        }

        [DoesNotNeedDataAccess]
        public ActionResult RedirectToBugtracker()
        {
            return Redirect("https://aquadiva-trac1.inf-bb.uni-jena.de/mantis/bug_report_page.php");
        }
    }
}
