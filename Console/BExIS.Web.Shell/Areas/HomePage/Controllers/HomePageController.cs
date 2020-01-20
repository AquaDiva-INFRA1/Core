using BExIS.Dlm.Entities.Data;
using BExIS.Dlm.Entities.DataStructure;
using BExIS.Dlm.Services.Data;
using BExIS.Dlm.Services.DataStructure;
using BExIS.Modules.Rpm.UI.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace BExIS.Modules.HomePage.UI.Controllers
{
    public class HomePageController : Controller
    {

        static string Conx = ConfigurationManager.ConnectionStrings[1].ConnectionString;

        public ActionResult Index ()
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
            
            ViewData["semantic_Coverage"] =  (double) dam.DataAttributeStructs.Count / (double)count  ;
            return PartialView("HomePage");
        }

    }
}
