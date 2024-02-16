using BExIS.Aam.Entities.Mapping;
using BExIS.Aam.Services;
using BExIS.Dlm.Entities.Data;
using BExIS.Dlm.Entities.DataStructure;
using BExIS.Dlm.Services.Data;
using BExIS.Dlm.Services.DataStructure;
using BExIS.Modules.Aam.UI.Models;
using BExIS.Security.Entities.Authorization;
using BExIS.Security.Services.Authorization;
using BExIS.UI.Helpers;
using BExIS.Utils.Models;
using BExIS.Xml.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Vaiona.Persistence.Api;

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

                using (Aam_Dataset_column_annotationManager dca_m = new Aam_Dataset_column_annotationManager())
                {
                    using (DataStructureManager dsm = new DataStructureManager())
                    {
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

                        dca_m.Dispose();
                        dsm.Dispose();
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View();
            }
        }


        // GET: Aam_Dataset_column_annotation/Create
        public ActionResult Create(string ds_id)
        {
            Aam_Dataset_column_annotationManager dca_ma = new Aam_Dataset_column_annotationManager();
            Aam_Dataset_column_annotation_Model dca_M = new Aam_Dataset_column_annotation_Model();
            
            dca_M.datasets = dca_ma.LoadDataset_Id_Title( this.GetUsernameOrDefault());
            dca_ma.Dispose();
            fill_entites(dca_M);
            fill_variables(dca_M, ds_id);
            ViewData["ds_id"] = ds_id;
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
                        if (k == null) dca_M.entites.Add(uri.Id, uri.label + " - " + uri.URI + " - " + uri.Id);
                    }
                    if (uri.type_uri.ToLower() == "charachteristic")
                    {
                        dca_M.characs.TryGetValue(uri.Id, out k);
                        if (k == null) dca_M.characs.Add(uri.Id, uri.label + " - " + uri.URI + " - " + uri.Id);
                    }
                    if (uri.type_uri.ToLower() == "standard")
                    {
                        dca_M.standards.TryGetValue(uri.Id, out k);
                        if (k == null) dca_M.standards.Add(uri.Id, uri.label + " - " + uri.URI + " - " + uri.Id);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

            }
            dca_M.entites = dca_M.entites.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value); ;
            dca_M.standards = dca_M.standards.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value); 
            dca_M.characs = dca_M.characs.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value); ;
            urim.Dispose();
        }

        private void fill_variables(Aam_Dataset_column_annotation_Model dca_M, string ds_id)
        {
            DatasetManager dsmanager = new DatasetManager();
            DataStructureManager dsm = new DataStructureManager();
            VariableManager vm = new VariableManager();
            List<Variable> variables = new List<Variable>();
            // fill variables
            if (ds_id != null)
            {
                variables = vm.VariableInstanceRepo.Get().ToList().Where(x => x.DataStructure.Id ==dsmanager.GetDataset(Int64.Parse(ds_id)).DataStructure.Id).ToList<Variable>();
            }
            dca_M.DataAttributes.Clear();
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

                if (k == null)
                   dca_M.DataAttributes.Add(ds.Id, ds.Id + " - " + ds.Label);

            }

            dsmanager.Dispose();
            dsm.Dispose();
        }

        // GET: Aam_Dataset_column_annotation/Edit/5
        public ActionResult Edit(int id)
        {
            Aam_Dataset_column_annotationManager dca_ma = new Aam_Dataset_column_annotationManager();
            Aam_Dataset_column_annotation_Model oca_M = new Aam_Dataset_column_annotation_Model();
            oca_M.datasets = dca_ma.LoadDataset_Id_Title(this.GetUsernameOrDefault());
            fill_entites(oca_M);
            fill_variables(oca_M, null);

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
            List<Variable> vars = dsm.VariableRepo.Get().ToList<Variable>();
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

        public void upload_annotation_from_Excel_file(String filePath)
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
                        string entity_uri = clean_entity_URI_for_insert(excellineJson[6].ToString());
                        string entity_label = clean_entity_URI_for_insert(excellineJson[4].ToString());
                        string charac_uri = clean_entity_URI_for_insert(excellineJson[7].ToString());
                        string charac_label = clean_entity_URI_for_insert(excellineJson[5].ToString());

                        VariableInstance variable = new VariableInstance();
                        DataStructureManager dataStructureManager = new DataStructureManager();
                        var structureRepo = dataStructureManager.GetUnitOfWork().GetReadOnlyRepository<StructuredDataStructure>();
                        StructuredDataStructure dataStructure = structureRepo.Get(new DatasetManager().GetDataset(Int64.Parse(ds_id[0])).DataStructure.Id);

                        if (var_id != "")
                        {
                            foreach (VariableInstance var in dataStructure.Variables)
                            {
                                if (var.Id == Int64.Parse(var_id)) variable = var;
                            }
                        }

                        using (Aam_Dataset_column_annotationManager aam = new Aam_Dataset_column_annotationManager())
                        {
                            Aam_Uri uri_M = new Aam_Uri();
                            if ((variable != null)&& (variable.Id != 0))
                            {
                                Aam_Uri uri = uri_M.GetBulkUnitOfWork().GetReadOnlyRepository<Aam_Uri>().Get(x => x.URI == entity_uri).FirstOrDefault<Aam_Uri>();
                                Aam_Uri charac = uri_M.GetBulkUnitOfWork().GetReadOnlyRepository<Aam_Uri>().Get(x => x.URI == charac_uri).FirstOrDefault<Aam_Uri>();
                                Aam_Uri std = uri_M.GetBulkUnitOfWork().GetReadOnlyRepository<Aam_Uri>().Get(x => x.type_uri.ToLower() == "standard").FirstOrDefault<Aam_Uri>();
                                if (uri == null)
                                {
                                    uri = new Aam_Uri(entity_uri, entity_label, "Entity");
                                    uri = new Aam_UriManager().creeate_Aam_Uri(uri);
                                }
                                if (charac == null)
                                {
                                    charac = new Aam_Uri(charac_uri, charac_label, "Charachteristic");
                                    charac = new Aam_UriManager().creeate_Aam_Uri(charac);
                                }
                                if (std == null)
                                {
                                    std = new Aam_Uri("http://ecoinformatics.org/oboe/oboe.1.2/oboe-core.owl#Standard", "Standard", "Standard");
                                    std = new Aam_UriManager().creeate_Aam_Uri(std);
                                }
                                Aam_Dataset_column_annotation an = new Aam_Dataset_column_annotation(
                                    new DatasetManager().GetDataset(Int64.Parse(ds_id[0])), new DatasetManager().GetDatasetLatestVersion(Int64.Parse(ds_id[0])),
                                    variable, uri, charac, std
                                    );
                                if (aam.get_all_dataset_column_annotation().Where<Aam_Dataset_column_annotation>(x => x.variable_id == an.variable_id &&
                               x.Dataset == an.Dataset && x.characteristic_id == an.characteristic_id && x.entity_id == an.entity_id).Count() == 0)
                                    aam.creeate_dataset_column_annotation(an);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            }
        }

        public void upload_observations_from_Excel_file(String filePath)
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

                        string var_id = excellineJson[2].ToString();
                        string Contextualized_entity = clean_entity_URI_for_insert(excellineJson[6].ToString());
                        string Contextualized_entity_label = clean_entity_URI_for_insert(excellineJson[4].ToString());
                        string characContextualizing_entity = clean_entity_URI_for_insert(excellineJson[7].ToString());
                        string characContextualizing_entity_label = clean_entity_URI_for_insert(excellineJson[5].ToString());

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

                        using (Aam_Observation_ContextManager aam = new Aam_Observation_ContextManager())
                        {
                            Aam_Uri uri_M = new Aam_Uri();
                            if ((variable != null) && (variable.Id != 0))
                            {
                                Aam_Uri Contextualized_entity_uri = uri_M.GetBulkUnitOfWork().GetReadOnlyRepository<Aam_Uri>().Get(x => x.URI == Contextualized_entity).FirstOrDefault<Aam_Uri>();
                                Aam_Uri characContextualizing_entity_uri = uri_M.GetBulkUnitOfWork().GetReadOnlyRepository<Aam_Uri>().Get(x => x.URI == characContextualizing_entity).FirstOrDefault<Aam_Uri>();
                                if (Contextualized_entity_uri == null)
                                {
                                    Contextualized_entity_uri = new Aam_Uri(Contextualized_entity, Contextualized_entity_label, "Entity");
                                    Contextualized_entity_uri = new Aam_UriManager().creeate_Aam_Uri(Contextualized_entity_uri);
                                }
                                if (characContextualizing_entity_uri == null)
                                {
                                    characContextualizing_entity_uri = new Aam_Uri(characContextualizing_entity, characContextualizing_entity_label, "Entity");
                                    characContextualizing_entity_uri = new Aam_UriManager().creeate_Aam_Uri(characContextualizing_entity_uri);
                                }
                                Aam_Observation_Context an = new Aam_Observation_Context(
                                    new DatasetManager().GetDataset(Int64.Parse(ds_id[0])), new DatasetManager().GetDatasetLatestVersion(Int64.Parse(ds_id[0])),
                                    Contextualized_entity_uri, characContextualizing_entity_uri
                                    );
                                if (aam.get_all_Aam_Observation_Context().Where<Aam_Observation_Context>(x => x.Contextualized_entity == an.Contextualized_entity &&
                               x.Dataset == an.Dataset && x.Contextualizing_entity == an.Contextualizing_entity).Count() == 0)
                                    aam.creeate_Aam_Observation_Context(an);
                            }
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
    }
    
    public enum DataStructureType
    {
        Structured,
        Unstructured
    }
}
