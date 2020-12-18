using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BExIS.Dlm.Entities.Data;
using BExIS.Dlm.Entities.DataStructure;
using BExIS.Dlm.Services.Data;
using Npgsql;
using System.Configuration;
using System.Linq;
using BExIS.Modules.Rpm.UI.Models;
using Vaiona.Web.Mvc.Data;
using BExIS.Aam.Entities.Mapping;
using BExIS.Aam.Services;

namespace BExIS.Modules.Ddm.UI.Controllers
{
    public class LandingPageController : Controller
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
                long noColumns = ds.DataStructure.Self is StructuredDataStructure ? (ds.DataStructure.Self as StructuredDataStructure).Variables.Count : 0L;
                long noRows = ds.DataStructure.Self is StructuredDataStructure ? dm.GetDatasetLatestVersionEffectiveTupleCount(ds) : 0; // It would save time to calc the row count for all the datasets at once!
                if (ds.Status == DatasetStatus.CheckedIn)
                {
                    somme = somme + (noRows * noColumns);
                }
            }
            dm.Dispose();
            ViewData["datasetCount"] = datasets.Count;
            ViewData["Datapoints"] = somme;

            Aam_Dataset_column_annotationManager aam_manager = new Aam_Dataset_column_annotationManager();
            Int64 count =  aam_manager.get_all_dataset_column_annotation().Count;


            DataAttributeManagerModel dam = new DataAttributeManagerModel(false);

            ViewData["semantic_Coverage"] = (double)dam.DataAttributeStructs.Count / (double)count;

            return View("Index");

            //var result = this.Render("homepage", "homepage", "Index");
            //return Content(result.ToHtmlString(), "text/html");
        }
    }
}