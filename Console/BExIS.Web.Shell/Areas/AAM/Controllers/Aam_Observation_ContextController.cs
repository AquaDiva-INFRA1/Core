using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml;
using BExIS.Aam.Entities.Mapping;
using BExIS.Aam.Services;
using BExIS.Dcm.UploadWizard;
using BExIS.Dlm.Entities.Data;
using BExIS.Dlm.Entities.DataStructure;
using BExIS.Dlm.Services.Data;
using BExIS.Dlm.Services.DataStructure;
using BExIS.Modules.Dcm.UI.Models;
using BExIS.Modules.Rpm.UI.Models;
using BExIS.Security.Entities.Authorization;
using BExIS.Security.Services.Authorization;
using BExIS.Xml.Helpers;
using System.Web.Mvc;
using BExIS.Modules.Aam.UI.Models;

namespace BExIS.Modules.Aam.UI.Controllers
{
    public class Aam_Observation_ContextController : Controller
    {

        private XmlDatasetHelper xmlDatasetHelper = new XmlDatasetHelper();

        // GET: Aam_Observation_Context
        public ActionResult Index()
        {
            Aam_Observation_ContextManager Aam_Observation_ContextManager = new Aam_Observation_ContextManager();
            List<Aam_Observation_Context> list = Aam_Observation_ContextManager.get_all_Aam_Observation_Context();
            Aam_Observation_ContextManager.Dispose();
            return View(list);
        }

        // GET: Aam_Observation_Context/Create
        public ActionResult Create()
        {
            Aam_Observation_ContextManager dca_ma = new Aam_Observation_ContextManager();
            Aam_Observation_Context_Model oca_M = new Aam_Observation_Context_Model();
            oca_M.datasets = dca_ma.LoadDataset_Id_Title(this.GetUsernameOrDefault());
            loaduris(oca_M);
            
            dca_ma.Dispose();

            return View(oca_M);
        }

        private void loaduris(Aam_Observation_Context_Model oca_M)
        {
            Aam_UriManager urim = new Aam_UriManager();
            foreach (Aam_Uri uri in urim.get_all_Aam_Uri())
            {
                try
                {
                    string k = null;
                    if (uri.type_uri.ToLower() == "entity")
                    {
                        oca_M.Contextualized_entity.TryGetValue(uri.Id, out k);
                        if (k == null) oca_M.Contextualized_entity.Add(uri.Id, uri.Id+" - " + uri.URI);
                        oca_M.Contextualizing_entity.TryGetValue(uri.Id, out k);
                        if (k == null) oca_M.Contextualizing_entity.Add(uri.Id, uri.Id +" - "+uri.URI);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

            }
            urim.Dispose();
        }

        // POST: Aam_Observation_Context/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                string datasets = Convert.ToString(collection["datasets"]);
                string Contextualizing_entity = Convert.ToString(collection["Contextualizing_entity"]);
                string Contextualized_entity = Convert.ToString(collection["Contextualized_entity"]);

                Aam_Observation_ContextManager oca_ma = new Aam_Observation_ContextManager();
                Aam_Observation_Context oca = new Aam_Observation_Context(
                    new DatasetManager().GetDataset(Int64.Parse(datasets)),
                    new DatasetManager().GetDatasetLatestVersion(Int64.Parse(datasets)),
                    new Aam_UriManager().get_Aam_Uri_by_id(Int64.Parse(Contextualizing_entity)),
                    new Aam_UriManager().get_Aam_Uri_by_id(Int64.Parse(Contextualized_entity))
                    );
                oca_ma.creeate_Aam_Observation_Context(oca);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return View();
            }
        }

        // GET: Aam_Observation_Context/Edit/5
        public ActionResult Edit(int id)
        {
            Aam_Observation_ContextManager dca_ma = new Aam_Observation_ContextManager();
            Aam_Observation_Context_Model oca_M = new Aam_Observation_Context_Model();
            oca_M.datasets = dca_ma.LoadDataset_Id_Title(this.GetUsernameOrDefault());
            loaduris(oca_M);

            Aam_Observation_ContextManager Aam_Observation_ContextManager = new Aam_Observation_ContextManager();
            Aam_Observation_Context Aam_Observation_Context = Aam_Observation_ContextManager.get_Aam_Observation_Context_by_id(id);
            ViewData["dataset"] = Aam_Observation_Context.Dataset.Id;
            ViewData["Contextualized_entity"] = Aam_Observation_Context.Contextualized_entity.Id;
            ViewData["Contextualizing_entity"] = Aam_Observation_Context.Contextualizing_entity.Id;

            return View(oca_M);
        }

        // POST: Aam_Observation_Context/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                string datasets = Convert.ToString(collection["datasets"]);
                string Contextualizing_entity = Convert.ToString(collection["Contextualizing_entity"]);
                string Contextualized_entity = Convert.ToString(collection["Contextualized_entity"]);

                Aam_Observation_ContextManager oca_ma = new Aam_Observation_ContextManager();
                Aam_Observation_Context oca = oca_ma.get_Aam_Observation_Context_by_id(id);
                oca.Dataset = new DatasetManager().GetDataset(Int64.Parse(datasets));
                oca.DatasetVersion = new DatasetManager().GetDatasetLatestVersion(Int64.Parse(datasets));
                oca.Contextualizing_entity = new Aam_UriManager().get_Aam_Uri_by_id(Int64.Parse(Contextualizing_entity));
                oca.Contextualized_entity = new Aam_UriManager().get_Aam_Uri_by_id(Int64.Parse(Contextualized_entity));

                oca_ma.edit_Aam_Observation_Context(oca);
                oca_ma.Dispose();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return View();
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


        public ActionResult Delete(int id)
        {
            try
            {
                Aam_Observation_ContextManager oc_ma = new Aam_Observation_ContextManager();
                Aam_Observation_Context oc = oc_ma.get_Aam_Observation_Context_by_id(id);
                oc_ma.delete_Aam_Observation_Context(oc);
                oc_ma.Dispose();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["error"] = ex.Message;
                return RedirectToAction("Index");
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
}
