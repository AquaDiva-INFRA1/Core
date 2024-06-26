using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using BExIS.Aam.Entities.Mapping;
using BExIS.Aam.Services;
using BExIS.Dlm.Entities.Data;
using BExIS.Dlm.Services.Data;
using BExIS.Dlm.Services.DataStructure;
using BExIS.Security.Entities.Authorization;
using BExIS.Security.Services.Authorization;
using BExIS.Xml.Helpers;
using BExIS.Modules.Aam.UI.Models;
using System.IO;
using BExIS.Utils.Models;
using BExIS.UI.Helpers;
using Newtonsoft.Json.Linq;
using BExIS.Dlm.Entities.DataStructure;
using Vaiona.Persistence.Api;

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
                        if (k == null) oca_M.Contextualized_entity.Add(uri.Id, uri.label+" - "+ uri.URI + " - " + uri.Id);
                        oca_M.Contextualizing_entity.TryGetValue(uri.Id, out k);
                        if (k == null) oca_M.Contextualizing_entity.Add(uri.Id, uri.label + " - " + uri.URI + " - " + uri.Id);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

            }
            oca_M.Contextualized_entity = oca_M.Contextualized_entity.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value); ;
            oca_M.Contextualizing_entity = oca_M.Contextualizing_entity.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
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
                return View(new Aam_Observation_Context_Model());
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

        public void upload_Observation_from_Excel_file(String filePath)
        {
            //FileStream for the users file
            FileStream fis = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            //Grab the sheet format from the bus
            string sheetFormatString = Convert.ToString("TopDown");
            SheetFormat CurrentSheetFormat = 0;
            Enum.TryParse<SheetFormat>(sheetFormatString, true, out CurrentSheetFormat);

            //Transforms the content of the file into a 2d-json-array
            JsonTableGenerator EUEReader = new JsonTableGenerator(fis);
            //If the active worksheet was never changed, we default to the first one
            string activeWorksheet = EUEReader.GetFirstWorksheetUri().ToString();
            //Generate the table for the active worksheet
            string jsonTable = EUEReader.GenerateJsonTable(CurrentSheetFormat, activeWorksheet);
            JArray textArray = JArray.Parse(jsonTable);
            int index = 0;

            string[] ds_id = new string[2];
            for (int k = 1; k < textArray.Count; k++)
            {
                if (textArray[k].ToString().Length > 1)
                {
                    try
                    {
                        var excelline = textArray[k];
                        JArray excellineJson = JArray.Parse(excelline.ToString());

                        if (excellineJson[0].ToString().Length > 1)
                        {
                            ds_id = excellineJson[0].ToString().Split('.');
                        }

                        string attribute_name = excellineJson[3].ToString();
                        string var_id = excellineJson[2].ToString();
                        string contextualised_uri = clean_entity_URI_for_insert(excellineJson[6].ToString());
                        string contextualised_label = clean_entity_URI_for_insert(excellineJson[4].ToString());
                        string contextualizing_uri = clean_entity_URI_for_insert(excellineJson[7].ToString());
                        string contextualizing_label = clean_entity_URI_for_insert(excellineJson[5].ToString());

                        VariableInstance variable = new VariableInstance();
                        DataStructureManager dataStructureManager = new DataStructureManager();
                        var structureRepo = dataStructureManager.GetUnitOfWork().GetReadOnlyRepository<StructuredDataStructure>();
                        StructuredDataStructure dataStructure = structureRepo.Get(new DatasetManager().GetDataset(Int64.Parse(ds_id[0])).DataStructure.Id);

                        if (var_id != "")
                        {
                            foreach (Variable var in dataStructure.Variables)
                            {
                                if (var.Id == Int64.Parse(var_id)) variable = (VariableInstance)var;
                            }
                        }

                        Aam_Observation_ContextManager aam = new Aam_Observation_ContextManager();
                        Aam_Uri uri_M = new Aam_Uri();
                        if (variable != null)
                        {
                            Aam_Uri uri = uri_M.GetBulkUnitOfWork().GetReadOnlyRepository<Aam_Uri>().Get(x => x.URI == contextualised_uri).FirstOrDefault<Aam_Uri>();
                            Aam_Uri charac = uri_M.GetBulkUnitOfWork().GetReadOnlyRepository<Aam_Uri>().Get(x => x.URI == contextualizing_uri).FirstOrDefault<Aam_Uri>();
                            Aam_Uri std = uri_M.GetBulkUnitOfWork().GetReadOnlyRepository<Aam_Uri>().Get(x => x.type_uri.ToLower() == "standard").FirstOrDefault<Aam_Uri>();
                            if (uri == null)
                            {
                                uri = new Aam_Uri(contextualised_uri, contextualised_label, "Entity");
                                uri = new Aam_UriManager().creeate_Aam_Uri(uri);
                            }
                            if (charac == null)
                            {
                                charac = new Aam_Uri(contextualizing_uri, contextualizing_label, "Charachteristic");
                                charac = new Aam_UriManager().creeate_Aam_Uri(charac);
                            }
                            if (std == null)
                            {
                                std = new Aam_Uri("http://ecoinformatics.org/oboe/oboe.1.2/oboe-core.owl#Standard", "Standard", "Standard");
                                std = new Aam_UriManager().creeate_Aam_Uri(std);
                            }
                            Aam_Observation_Context an = new Aam_Observation_Context(
                                new DatasetManager().GetDataset(Int64.Parse(ds_id[0])), new DatasetManager().GetDatasetLatestVersion(Int64.Parse(ds_id[0])),
                                uri, charac
                                );
                            if (aam.get_all_Aam_Observation_Context().Where<Aam_Observation_Context>(x => x.Dataset == an.Dataset && 
                            x.Contextualized_entity == an.Contextualized_entity && x.Contextualizing_entity == an.Contextualizing_entity).Count() == 0)
                                aam.creeate_Aam_Observation_Context(an);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            }
        }

        public string clean_entity_URI_for_insert(string uri)
        {
            return uri.Replace("'", "''").Replace(System.Environment.NewLine, "").Replace("\n", "");
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
