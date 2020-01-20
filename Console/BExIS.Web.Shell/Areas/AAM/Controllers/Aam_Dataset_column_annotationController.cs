using BExIS.Aam.Entities.Mapping;
using BExIS.Aam.Services;
using BExIS.Dlm.Entities.Data;
using BExIS.Dlm.Entities.DataStructure;
using BExIS.Dlm.Services.Data;
using BExIS.Dlm.Services.DataStructure;
using BExIS.Modules.Aam.UI.Models;
using BExIS.Modules.Ddm.UI.Models;
using BExIS.Modules.Rpm.UI.Models;
using BExIS.Security.Entities.Authorization;
using BExIS.Security.Services.Authorization;
using BExIS.Xml.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BExIS.Modules.Aam.UI.Controllers
{
    public class Aam_Dataset_column_annotationController : Controller
    {

        private XmlDatasetHelper xmlDatasetHelper = new XmlDatasetHelper();



        // GET: Aam_Dataset_column_annotation
        public ActionResult Index()
        {
            Aam_Dataset_column_annotationManager Aam_Dataset_column_annotationManager = new Aam_Dataset_column_annotationManager();
            List<Aam_Dataset_column_annotation> list = Aam_Dataset_column_annotationManager.get_all_dataset_column_annotation();
            Aam_Dataset_column_annotationManager.Dispose();
            return View(list);
        }


        // POST: Aam_Dataset_column_annotation/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                string datasets = Convert.ToString(collection["datasets"]);
                string DataAttributes = Convert.ToString(collection["DataAttributes"]);
                string entites = Convert.ToString(collection["entites"]);
                string characs = Convert.ToString(collection["characs"]);
                string standards = Convert.ToString(collection["standards"]);

                Aam_Dataset_column_annotationManager dca_m = new Aam_Dataset_column_annotationManager();

                DataStructureManager dsm = new DataStructureManager();
                Variable variable = dsm.VariableRepo.Get().ToList<Variable>().FirstOrDefault(x => x.Id == int.Parse(DataAttributes));

                Aam_Dataset_column_annotation dca = new Aam_Dataset_column_annotation(
                    new DatasetManager().GetDataset(Int64.Parse(datasets)),
                    new DatasetManager().GetDatasetLatestVersion(Int64.Parse(datasets)),
                    variable,
                    new Aam_UriManager().get_Aam_Uri_by_id(Int64.Parse(entites)),
                    new Aam_UriManager().get_Aam_Uri_by_id(Int64.Parse(characs)),
                    new Aam_UriManager().get_Aam_Uri_by_id(Int64.Parse(standards))
                    );
                dca_m.creeate_dataset_column_annotation(dca);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View();
            }
        }


        // GET: Aam_Dataset_column_annotation/Create
        public ActionResult Create()
        {
            Aam_Dataset_column_annotationManager dca_ma = new Aam_Dataset_column_annotationManager();
            Aam_Dataset_column_annotation_Model dca_M = new Aam_Dataset_column_annotation_Model();

            dca_M.datasets = dca_ma.LoadDataset_Id_Title( this.GetUsernameOrDefault());

            fill_variables(dca_M);
            fill_entites(dca_M);

            return View(dca_M);
        }

        private void fill_entites(Aam_Dataset_column_annotation_Model dca_M)
        {
            Aam_UriManager urim = new Aam_UriManager();
            //fill entites , characs and standards
            foreach (Aam_Uri uri in urim.get_all_Aam_Uri())
            {
                try
                {
                    string k = null;
                    if (uri.type_uri.ToLower() == "entity")
                    {
                        dca_M.entites.TryGetValue(uri.Id, out k);
                        if (k == null) dca_M.entites.Add(uri.Id, uri.Id + " - " + uri.URI);
                    }
                    if (uri.type_uri.ToLower() == "standard")
                    {
                        dca_M.characs.TryGetValue(uri.Id, out k);
                        if (k == null) dca_M.characs.Add(uri.Id, uri.Id + " - " + uri.URI);
                    }
                    if (uri.type_uri.ToLower() == "charachteristic")
                    {
                        dca_M.standards.TryGetValue(uri.Id, out k);
                        if (k == null) dca_M.standards.Add(uri.Id, uri.Id + " - " + uri.URI);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

            }
            urim.Dispose();
        }

        private void fill_variables(Aam_Dataset_column_annotation_Model dca_M)
        {
            DataStructureManager dsm = new DataStructureManager();
            List<Variable> variables =dsm.VariableRepo.Get().ToList<Variable>();
            // fill variables
            foreach (Variable ds in variables)
            {
                string k = null;
                try
                {
                    dca_M.DataAttributes.TryGetValue(ds.Id, out k);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                if (k == null) dca_M.DataAttributes.Add(ds.Id, ds.Id+" - "+ds.Label);
            }
        }

        // GET: Aam_Dataset_column_annotation/Edit/5
        public ActionResult Edit(int id)
        {
            Aam_Dataset_column_annotationManager dca_ma = new Aam_Dataset_column_annotationManager();
            Aam_Dataset_column_annotation_Model oca_M = new Aam_Dataset_column_annotation_Model();
            oca_M.datasets = dca_ma.LoadDataset_Id_Title(this.GetUsernameOrDefault());
            fill_entites(oca_M);
            fill_variables(oca_M);

            Aam_Dataset_column_annotation Aam_Dataset_column_annotation = dca_ma.get_dataset_column_annotation_by_id(id);
            ViewData["dataset"] = Aam_Dataset_column_annotation.Dataset.Id;
            ViewData["entity"] = Aam_Dataset_column_annotation.entity_id.Id;
            ViewData["charac"] = Aam_Dataset_column_annotation.characteristic_id.Id;
            ViewData["std"] = Aam_Dataset_column_annotation.standard_id.Id;
            ViewData["var"] = Aam_Dataset_column_annotation.variable_id.Id;

            return View(oca_M);
        }

        private void fill_vars(Aam_Dataset_column_annotation_Model oca_M)
        {
            DataStructureManager dsm = new DataStructureManager();
            List < Variable > vars = dsm.VariableRepo.Get().ToList<Variable>();
            foreach (Variable var in vars)
            {
                string k = null;
                oca_M.DataAttributes.TryGetValue(var.Id, out k);
                if (k == null) oca_M.DataAttributes.Add(var.Id, var.Id + " - " + var.Label);
            }
        }

        // POST: Aam_Dataset_column_annotation/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                string datasets = Convert.ToString(collection["datasets"]);
                string DataAttributes = Convert.ToString(collection["DataAttributes"]);
                string entites = Convert.ToString(collection["entites"]);
                string characs = Convert.ToString(collection["characs"]);
                string standards = Convert.ToString(collection["standards"]);

                Aam_Dataset_column_annotationManager oca_ma = new Aam_Dataset_column_annotationManager();
                Aam_Dataset_column_annotation oca = oca_ma.get_dataset_column_annotation_by_id(id);
                oca.Dataset = new DatasetManager().GetDataset(Int64.Parse(datasets));
                oca.DatasetVersion = new DatasetManager().GetDatasetLatestVersion(Int64.Parse(datasets));
                oca.entity_id = new Aam_UriManager().get_Aam_Uri_by_id(Int64.Parse(entites));
                oca.characteristic_id = new Aam_UriManager().get_Aam_Uri_by_id(Int64.Parse(characs));
                oca.standard_id = new Aam_UriManager().get_Aam_Uri_by_id(Int64.Parse(standards));

                DataStructureManager dsm = new DataStructureManager();
                oca.variable_id = dsm.VariableRepo.Get().ToList<Variable>().FirstOrDefault(x => x.Id == int.Parse(DataAttributes));

                oca_ma.edit_dataset_column_annotation(oca);
                oca_ma.Dispose();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return View();
            }
        }

        // GET: Aam_Dataset_column_annotation/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                Aam_Dataset_column_annotationManager oc_ma = new Aam_Dataset_column_annotationManager();
                Aam_Dataset_column_annotation oc = oc_ma.get_dataset_column_annotation_by_id(id);
                oc_ma.delete_dataset_column_annotation(oc);
                oc_ma.Dispose();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }


        public Dictionary<long, string> LoadDataset_Id_Title(DataStructureType dataStructureType)
        {
            EntityPermissionManager entityPermissionManager = new EntityPermissionManager();
            DataStructureManager dataStructureManager = new DataStructureManager();
            DatasetManager dm = new DatasetManager();

            try
            {
                List<long> datasetIDs = new List<long>();
                datasetIDs = entityPermissionManager.GetKeys(GetUsernameOrDefault(), "Dataset", typeof(Dataset), RightType.Write).ToList<long>();
                Dictionary<long, string> temp = new Dictionary<long, string>();
                foreach (long id in datasetIDs)
                {
                    string k = null;
                    temp.TryGetValue(id, out k);
                    if (k == null) temp.Add(id, xmlDatasetHelper.GetInformation(id, NameAttributeValues.title));
                }
                return temp;
            }
            finally
            {
                entityPermissionManager.Dispose();
                dataStructureManager.Dispose();
                dm.Dispose();
            }
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

    public enum DataStructureType
    {
        Structured,
        Unstructured
    }
}
