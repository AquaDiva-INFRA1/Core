using BExIS.Dim.Entities.Mapping;
using BExIS.Dim.Helpers.Mapping;
using BExIS.Dim.Services;
using BExIS.Dlm.Entities.Administration;
using BExIS.Dlm.Entities.Data;
using BExIS.Dlm.Entities.MetadataStructure;
using BExIS.Dlm.Services.Data;
using BExIS.Dlm.Services.MetadataStructure;
using BExIS.Modules.Dim.UI.Helper;
using BExIS.Modules.Dim.UI.Models.Mapping;
using BExIS.Xml.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;
using Vaiona.Persistence.Api;

namespace BExIS.Modules.Dim.UI.Controllers
{
    public class MappingController : Controller
    {
        // GET: DIM/Mapping
        public ActionResult Index(long sourceId = 1, long targetId = 0, LinkElementType type = LinkElementType.System)
        {
            using (MappingManager mappingManager = new MappingManager())
            {

                MappingMainModel model = new MappingMainModel();
                // load from mds example
                model.Source = MappingHelper.LoadFromMetadataStructure(sourceId, LinkElementPostion.Source, mappingManager);


                switch (type)
                {
                    case LinkElementType.System:
                        {
                            model.Target = MappingHelper.LoadfromSystem(LinkElementPostion.Target, mappingManager);
                            model.SelectionList = MappingHelper.LoadSelectionList();
                            break;
                        }
                    case LinkElementType.MetadataStructure:
                        {
                            model.Target = MappingHelper.LoadFromMetadataStructure(targetId, LinkElementPostion.Target, mappingManager);
                            model.SelectionList = MappingHelper.LoadSelectionList();
                            break;
                        }
                }

                if (model.Source != null && model.Target != null)
                {
                    //get linkelements
                    LinkElement source = mappingManager.GetLinkElement(sourceId, LinkElementType.MetadataStructure);
                    LinkElement target = mappingManager.GetLinkElement(targetId, type);

                    if (source != null && target != null)
                    {

                        //get root mapping
                        Mapping rootMapping = mappingManager.GetMapping(source, target);

                        if (rootMapping != null)
                        {
                            //get complex mappings
                            model.ParentMappings = MappingHelper.LoadMappings(rootMapping);
                        }

                    }
                }
                return View(model);
            }
 
        }

        public ActionResult Mapping(long sourceId = 1, long targetId = 0,
            LinkElementType sourceType = LinkElementType.System, LinkElementType targetType = LinkElementType.System,
            LinkElementPostion position = LinkElementPostion.Target)
        {
            MappingManager mappingManager = new MappingManager();

            try
            {
                MappingMainModel model = new MappingMainModel();
                // load from mds example
                //model.Source = MappingHelper.LoadFromMetadataStructure(sourceId, LinkElementPostion.Source);
                /*
                 * Here the source and target will switch the sides
                 */
                #region load Source from Target

                switch (sourceType)
                {
                    case LinkElementType.System:
                        {
                            model.Source = MappingHelper.LoadfromSystem(LinkElementPostion.Source, mappingManager);
                            if (!model.SelectionList.Any()) model.SelectionList = MappingHelper.LoadSelectionList();
                            break;
                        }
                    case LinkElementType.MetadataStructure:
                        {
                            model.Source = MappingHelper.LoadFromMetadataStructure(sourceId, LinkElementPostion.Source, mappingManager);
                            if (!model.SelectionList.Any()) model.SelectionList = MappingHelper.LoadSelectionList();
                            break;
                        }
                }

                #endregion

                #region load Target
                switch (targetType)
                {
                    case LinkElementType.System:
                        {
                            model.Target = MappingHelper.LoadfromSystem(LinkElementPostion.Target, mappingManager);
                            if (!model.SelectionList.Any()) model.SelectionList = MappingHelper.LoadSelectionList();
                            break;
                        }
                    case LinkElementType.MetadataStructure:
                        {
                            model.Target = MappingHelper.LoadFromMetadataStructure(targetId, LinkElementPostion.Target, mappingManager);
                            if (!model.SelectionList.Any()) model.SelectionList = MappingHelper.LoadSelectionList();
                            break;
                        }
                }

                #endregion
                if (model.Source != null && model.Target != null)
                {
                    //get linkelements
                    LinkElement source = mappingManager.GetLinkElement(sourceId, sourceType);
                    LinkElement target = mappingManager.GetLinkElement(targetId, targetType);

                    if (source != null && target != null)
                    {

                        //get root mapping
                        Mapping rootMapping = mappingManager.GetMapping(source, target);

                        if (rootMapping != null)
                        {
                            //get complex mappings
                            model.ParentMappings = MappingHelper.LoadMappings(rootMapping);
                        }
                    }
                }

                return View("Index", model);
            }
            finally
            {
                mappingManager.Dispose();
            }
        }

        public ActionResult Switch(long sourceId = 1, long targetId = 0,
            LinkElementType sourceType = LinkElementType.System, LinkElementType targetType = LinkElementType.System,
            LinkElementPostion position = LinkElementPostion.Target)
        {

            return RedirectToAction("Mapping", new
            {
                sourceId = targetId,
                targetId = sourceId,
                sourceType = targetType,
                targetType = sourceType,
                position = position
            });
        }

        public ActionResult ReloadTarget(long sourceId , long targetId , LinkElementType sourceType , LinkElementType targetType , LinkElementPostion position = LinkElementPostion.Target)
        {
            MappingManager mappingManager = new MappingManager();

            try
            {
                LinkElementRootModel model = null;

                long id = position.Equals(LinkElementPostion.Source) ? sourceId : targetId;
                LinkElementType type = position.Equals(LinkElementPostion.Source) ? sourceType : targetType;


                switch (type)
                {
                    case LinkElementType.System:
                        {
                            model = MappingHelper.LoadfromSystem(position, mappingManager);

                            break;
                        }
                    case LinkElementType.MetadataStructure:
                        {
                            model = MappingHelper.LoadFromMetadataStructure(id, position, mappingManager);
                            break;
                        }
                }

                return PartialView("LinkElemenRoot", model);
            }
            finally
            {
                mappingManager.Dispose();
            }
        }

        //public ActionResult ReloadMapping(long sourceId = 1, long targetId = 0, LinkElementType sourceType = LinkElementType.System, LinkElementType targetType = LinkElementType.System, LinkElementPostion position = LinkElementPostion.Target)

        public ActionResult ReloadMapping(long sourceId , long targetId , LinkElementType sourceType , LinkElementType targetType , LinkElementPostion position = LinkElementPostion.Target)
        {

            MappingManager mappingManager = new MappingManager();
            try
            {

                List<ComplexMappingModel> model = new List<ComplexMappingModel>();

                // load from mds example
                LinkElementRootModel source = null;

                switch (sourceType)
                {
                    case LinkElementType.System:
                        {
                            source = MappingHelper.LoadfromSystem(LinkElementPostion.Source, mappingManager);

                            break;
                        }
                    case LinkElementType.MetadataStructure:
                        {
                            source = MappingHelper.LoadFromMetadataStructure(targetId, LinkElementPostion.Source, mappingManager);
                            break;
                        }
                }

                LinkElementRootModel target = null;
                switch (targetType)
                {
                    case LinkElementType.System:
                        {
                            target = MappingHelper.LoadfromSystem(LinkElementPostion.Target, mappingManager);

                            break;
                        }
                    case LinkElementType.MetadataStructure:
                        {
                            target = MappingHelper.LoadFromMetadataStructure(targetId, LinkElementPostion.Target, mappingManager);
                            break;
                        }
                }

                if (target != null)
                {

                    //get linkelements
                    LinkElement sourceLE = mappingManager.GetLinkElement(sourceId, sourceType);
                    LinkElement targetLE = mappingManager.GetLinkElement(targetId, targetType);

                    if (sourceLE != null && targetLE != null)
                    {

                        //get root mapping
                        Mapping rootMapping = mappingManager.GetMapping(sourceLE, targetLE);

                        if (rootMapping != null)
                        {
                            //get complex mappings
                            model = MappingHelper.LoadMappings(rootMapping);
                        }
                    }
                }
                
                return PartialView("Mappings", model);
            }
            finally
            {
                mappingManager.Dispose();
            }
        }

        public ActionResult AddMappingElement(LinkElementModel linkElementModel)
        {
            linkElementModel = MappingHelper.LoadChildren(linkElementModel);

            return PartialView("MappingLinkElement", linkElementModel);
        }

        public ActionResult SaveMapping(ComplexMappingModel model)
        {

            MappingManager mappingManager = new MappingManager();
            //save link element if not exits
            //source 
            try
            {

                #region save or update RootMapping

                //create source Parents if not exist
                LinkElement sourceParent = MappingHelper.CreateIfNotExistLinkElement(model.Source.Parent, mappingManager);

                //create source Parents if not exist
                LinkElement targetParent = MappingHelper.CreateIfNotExistLinkElement(model.Target.Parent, mappingManager);

                //create root mapping if not exist
                Mapping rootMapping = MappingHelper.CreateIfNotExistMapping(sourceParent, targetParent, 0, null, null, mappingManager);

                #endregion

                #region save or update complex mapping
                LinkElement source;
                LinkElement target;

                //create source
                source = MappingHelper.CreateIfNotExistLinkElement(model.Source, sourceParent.Id, mappingManager);

                model.Source.Id = source.Id;
                model.Source = MappingHelper.LoadChildren(model.Source);

                //create target
                target = MappingHelper.CreateIfNotExistLinkElement(model.Target, targetParent.Id, mappingManager);

                model.Target.Id = target.Id;
                model.Target = MappingHelper.LoadChildren(model.Target);

                //save mapping
                Mapping mapping = MappingHelper.CreateIfNotExistMapping(source, target, 1, null, rootMapping, mappingManager);
                model.Id = mapping.Id;
                model.ParentId = mapping.Parent.Id;
                #endregion

                #region create or update simple mapping

                MappingHelper.UpdateSimpleMappings(source.Id, target.Id, model.SimpleMappings, mapping, mappingManager);

                #endregion

                //load all mappings
                return PartialView("Mapping", model);
            }
            finally
            {
                mappingManager.Dispose();
                MappingUtils.Clear();
            }
        }

        public ActionResult LoadEmptyMapping()
        {
            return PartialView("Mapping", new ComplexMappingModel());
        }

        public JsonResult DeleteMapping(long id)
        {
            try
            {
                using (MappingManager mappingManager = new MappingManager())
                {
                    MappingHelper.DeleteMapping(id, mappingManager);

                    //ToDo delete also all simple mappings that are belonging to the complex mapping
                    return Json(true);
                }
            }
            catch (Exception ex)
            {

                return Json(ex.Message);
            }
            finally
            {
                MappingUtils.Clear();
            }
        }

        [HttpPost]
        public string Convertdatasetmetadata(string dataset_ids, long sourceId, long targetId)
        {
            Dictionary <long,string> datasets_return = new Dictionary<long,string>();
            using (MetadataStructureManager msm = new MetadataStructureManager())
            {
                MetadataStructure ms = msm.Repo.Get().FirstOrDefault(x => x.Id == targetId);
                using (DatasetManager dm = new DatasetManager())
                {
                    if (dataset_ids == null)
                    {
                        List<long> datasets = dm.GetDatasetLatestIds().Where(x => (dm.GetDataset(x).MetadataStructure.Id == sourceId) && 
                            (dm.GetDataset(x).Status== DatasetStatus.CheckedIn)).ToList<long> ();
                        datasets_return = datasets.ToDictionary(keySelector: m => m, elementSelector: m => dm.GetDatasetLatestVersion(m).Title);
                        return JsonConvert.SerializeObject(datasets_return);
                    }
                    using (MappingManager mappingManager = new MappingManager())
                    {
                        List<Mapping> mappings = mappingManager.GetChildMapping(
                            mappingManager.GetMappings().FirstOrDefault(x => x.Source.ElementId == sourceId && x.Target.ElementId == targetId).Source,
                            mappingManager.GetMappings().FirstOrDefault(x => x.Source.ElementId == sourceId && x.Target.ElementId == targetId).Target
                            ).ToList<Mapping>();

                        foreach (string dataset_id in dataset_ids.Split(','))
                        {
                            long datasetId = long.Parse(dataset_id);

                            if (dm.GetDataset(datasetId) == null)
                                datasetId = dm.GetDatasetVersion(datasetId).Dataset.Id;

                            dm.CheckOutDataset(datasetId, GetUsernameOrDefault());
                            XmlDocument metadataDoc = dm.GetDatasetLatestMetadataVersion(datasetId);

                            XmlMetadataWriter xmlMetadatWriter = new XmlMetadataWriter(XmlNodeMode.xPath);
                            XDocument metadataX = xmlMetadatWriter.CreateMetadataXml(targetId);
                            XmlDocument metadataXml_target = XmlMetadataWriter.ToXmlDocument(metadataX);
                            
                            foreach (Mapping mapping in mappings)
                            {
                                try
                                {
                                    if (mapping.Source.XPath.Trim() != "")
                                    {
                                        XmlNodeList elemList_target = metadataXml_target.SelectNodes(mapping.Target.XPath); int index = 0;
                                        foreach (XmlNode node in metadataDoc.SelectNodes(mapping.Source.XPath))
                                        {
                                            string source_value = node.InnerText.Trim();
                                            elemList_target[index].InnerText = source_value;
                                            index++;
                                        }
                                        
                                    }
                                }
                                catch (NullReferenceException ex)
                                {
                                    //Reference of the title node is missing
                                    throw new NullReferenceException("The extra-field of this metadata-structure is missing the title-node-reference!");
                                }
                            }

                            DatasetVersion dsv = dm.GetDatasetWorkingCopy(datasetId);
                            dsv.Metadata = metadataXml_target;
                            dm.EditDatasetVersion(dsv, null, null, null);
                            dm.CheckInDataset(datasetId, "Metadata Imported", GetUsernameOrDefault());

                            using (IUnitOfWork unitOfWork = this.GetUnitOfWork())
                            {
                                Dataset ds = unitOfWork.GetReadOnlyRepository<Dataset>().Get().FirstOrDefault(x=>x.Id==datasetId);
                                ds.MetadataStructure = ms;
                                unitOfWork.Commit();
                                datasets_return.Add(ds.Id, dm.GetDatasetLatestVersion(ds.Id).Title);
                            }

                        }
                    }
                }
            }
            return JsonConvert.SerializeObject(datasets_return);
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