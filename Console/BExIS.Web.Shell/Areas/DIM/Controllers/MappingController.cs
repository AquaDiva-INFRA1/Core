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
using BExIS.Xml.Helpers.Mapping;
using BExIS.Xml.Models.Mapping;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
                    case LinkElementType.MappingConcept:
                        {
                            model.Target = MappingHelper.LoadMappingConcept(targetId, LinkElementPostion.Target, mappingManager);
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


                    // in case of the link element has no xpath because of a older version of bexis 2
                    // every mapping will be check
                    // if a link element has no xpath get it from the generated models and update them for the furture

                    updateXPaths(model.ParentMappings, model.Source, model.Target);

                }
                return View(model);
            }

        }

        private void updateXPaths(List<ComplexMappingModel> mappings, LinkElementRootModel source, LinkElementRootModel target)
        {
            foreach (var mapping in mappings)
            {
                var sourceElement = source.LinkElements.FirstOrDefault(s => s.ElementId == mapping.Source.ElementId && s.Type == mapping.Source.Type);
                if (sourceElement != null)mapping.Source.XPath = sourceElement.XPath;
                var targetElement = target.LinkElements.FirstOrDefault(t => t.ElementId == mapping.Target.ElementId && t.Type == mapping.Target.Type);
                if (targetElement != null) mapping.Target.XPath = targetElement.XPath;

                if (mapping.SimpleMappings.Any())
                {
                    foreach (var simpleMapping in mapping.SimpleMappings)
                    {
                        var sourceSimpleElement = source.LinkElements.FirstOrDefault(s => s.ElementId == simpleMapping.Source.ElementId && s.Complexity == LinkElementComplexity.Simple);
                        if (sourceSimpleElement != null) simpleMapping.Source.XPath = sourceSimpleElement.XPath;
                        var targetSimpleElement = target.LinkElements.FirstOrDefault(t => t.ElementId == simpleMapping.Target.ElementId && t.Complexity == LinkElementComplexity.Simple);
                        if (targetSimpleElement != null) simpleMapping.Target.XPath = targetSimpleElement.XPath;
                    }
                }

                foreach (var sourceChildren in mapping.Source.Children)
                { 
                    var sc = source.LinkElements.FirstOrDefault(s => s.ElementId == sourceChildren.ElementId && s.Complexity == LinkElementComplexity.Simple);
                    if(sc!=null) sourceChildren.XPath = sc.XPath;
                }

                foreach (var targetChildren in mapping.Target.Children)
                {
                    var tc = target.LinkElements.FirstOrDefault(s => s.ElementId == targetChildren.ElementId && s.Complexity == LinkElementComplexity.Simple);
                    if (tc != null) targetChildren.XPath = tc.XPath;
                }
            }
        }

        public ActionResult Mapping(long sourceId = 1, long targetId = 0,
            LinkElementType sourceType = LinkElementType.System, LinkElementType targetType = LinkElementType.System,
            LinkElementPostion position = LinkElementPostion.Target)
        {
            var model = generateModel(sourceId, targetId, sourceType, targetType, position);

            return View("Index", model);
        }

        //public ActionResult Switch(long sourceId = 1, long targetId = 0,
        //    LinkElementType sourceType = LinkElementType.System, LinkElementType targetType = LinkElementType.System,
        //    LinkElementPostion position = LinkElementPostion.Target)
        //{
        //    var model = generateModel(targetId,sourceId, targetType, sourceType, position);

        //    return PartialView("Index", model);
        //}

        private MappingMainModel generateModel(long sourceId = 1, long targetId = 0,
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
                    case LinkElementType.MappingConcept:
                        {
                            model.Source = MappingHelper.LoadMappingConcept(sourceId, LinkElementPostion.Source, mappingManager);
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
                    case LinkElementType.MappingConcept:
                        {
                            model.Target = MappingHelper.LoadMappingConcept(targetId, LinkElementPostion.Target, mappingManager);
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

                return model;
               
            }
            finally
            {
                mappingManager.Dispose();
            }
        }

        public ActionResult ReloadTarget(long sourceId = 1, long targetId = 0, LinkElementType sourceType = LinkElementType.System, LinkElementType targetType = LinkElementType.System, LinkElementPostion position = LinkElementPostion.Target)
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
                    case LinkElementType.MappingConcept:
                        {
                            model = MappingHelper.LoadMappingConcept(id, position, mappingManager);
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
                            source = MappingHelper.LoadFromMetadataStructure(sourceId, LinkElementPostion.Source, mappingManager);
                            break;
                        }
                    case LinkElementType.MappingConcept:
                        {
                            source = MappingHelper.LoadMappingConcept(sourceId, LinkElementPostion.Source, mappingManager);
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
                    case LinkElementType.MappingConcept:
                        {
                            target = MappingHelper.LoadMappingConcept(targetId, position, mappingManager);
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

                updateXPaths(model, source, target);

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

        public ActionResult SaveMapping(ComplexMappingModel model, bool both=false)
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
                // also create the mapping in the other direction
                Mapping rootMappingReverse = null;
                if (both) rootMappingReverse = MappingHelper.CreateIfNotExistMapping(targetParent, sourceParent, 0, null, null, mappingManager);

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
                // also create the mapping in the other direction
                Mapping mappingReverse = null;
                if(both)mappingReverse = MappingHelper.CreateIfNotExistMapping(target, source, 1, null, rootMappingReverse, mappingManager);

                model.Id = mapping.Id;
                model.ParentId = mapping.Parent.Id;
                #endregion

                #region create or update simple mapping

                MappingHelper.UpdateSimpleMappings(source.Id, target.Id, model.SimpleMappings, mapping, mappingReverse, mappingManager, both);

                #endregion

                #region generate or update mapping file

                updateMappingFile(rootMapping);

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

        private void updateMappingFile(Mapping root)
        {
            //// get all complexMappings with simple mappings
            //var complexMappings = MappingHelper.LoadMappings(root);

            //List<XmlMappingRoute> routes = new List<XmlMappingRoute>();

            //foreach (var complexMapping in complexMappings)
            //{
            //    if(complexMapping.SimpleMappings.Any())
            //    {
            //        foreach (var simpleMapping in complexMapping.SimpleMappings)
            //        {
            //            // create a XmlMappingroute for each simplemapping
            //            XmlMappingRoute r = new XmlMappingRoute();
            //            r.Source = new Source(simpleMapping.Source.XPath);
            //            r.Destination = new Destination(simpleMapping.Target.XPath, complexMapping.Target.Name);

            //            routes.Add(r);
            //        }
            //    }
            //}

            //if (routes.Any())
            //{
            //    XmlSchemaManager manager = new XmlSchemaManager();
            //    manager.GenerateMappingFile(root.Source.ElementId, root.Source.Name, root.Target.Name, routes);
            //}
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
            Dictionary<long, string> datasets_return = new Dictionary<long, string>();
            using (MetadataStructureManager msm = new MetadataStructureManager())
            {
                MetadataStructure sourcemetadata = msm.Repo.Get().FirstOrDefault(m => m.Id == sourceId);
                MetadataStructure ms = msm.Repo.Get().FirstOrDefault(x => x.Id == targetId);
                using (DatasetManager dm = new DatasetManager())
                {
                    if (dataset_ids == null)
                    {
                        List<long> datasets = dm.GetDatasetLatestIds().Where(x => (dm.GetDataset(x).MetadataStructure.Id == sourceId) &&
                            (dm.GetDataset(x).Status == DatasetStatus.CheckedIn)).ToList<long>();
                        datasets_return = datasets.ToDictionary(keySelector: m => m, elementSelector: m => dm.GetDatasetLatestVersion(m).Title);
                        return JsonConvert.SerializeObject(datasets_return);
                    }
                    using (MappingManager mappingManager = new MappingManager())
                    {
                        List<Mapping> mappings__ = mappingManager.MappingRepo.Get()
                            .Where(x =>
                                (x.Source.Type == LinkElementType.MetadataStructure) &&
                                (x.Source.Name == sourcemetadata.Name) &&
                                (x.Target.Name == ms.Name))
                            .ToList<Mapping>();

                        foreach (string dataset_id in dataset_ids.Split(','))
                        {
                            long datasetId = long.Parse(dataset_id);

                            datasetId = dm.GetDatasetVersion(datasetId).Dataset.Id;

                            dm.CheckOutDataset(datasetId, GetUsernameOrDefault());
                            XmlDocument metadataDoc = dm.GetDatasetLatestMetadataVersion(datasetId);

                            XmlMetadataWriter xmlMetadatWriter = new XmlMetadataWriter(XmlNodeMode.xPath);
                            XDocument metadataX = xmlMetadatWriter.CreateMetadataXml(targetId);
                            XmlDocument metadataXml_target = XmlMetadataWriter.ToXmlDocument(metadataX);
                            XmlDocument metadataXml_target_cp = metadataXml_target;
                            for (int i = 0; i < mappings__.Count; i++)
                            {
                                Mapping mapping_ = mappings__[i];
                                try
                                {
                                    List<Mapping> children_mappings = mappingManager.GetChildMapping(mapping_.Id).ToList<Mapping>();
                                    if (children_mappings.Count == 0)
                                    {
                                        // built Xpaths
                                        string source_path = "";
                                        string target_path = "";
                                        do
                                        {
                                            source_path = mapping_.Source.XPath + source_path;
                                            target_path = mapping_.Target.XPath + target_path;
                                            mapping_ = mapping_.Parent;
                                        }
                                        while (mapping_.Parent != null);

                                        Mapping parent_node_mapping = mappings__[i].Parent;

                                        string parent_node_mapping_xpath_source = parent_node_mapping.Source.XPath;
                                        string child_node_mapping_sub_xpath_source = mappings__[i].Source.XPath[0].Equals('/') ? mappings__[i].Source.XPath.Remove(0, 1) : mappings__[i].Source.XPath;

                                        string parent_node_mapping_xpath_target = parent_node_mapping.Target.XPath;
                                        string child_node_mapping_sub_xpath_target = mappings__[i].Target.XPath[0].Equals('/') ? mappings__[i].Target.XPath.Remove(0, 1) : mappings__[i].Target.XPath;

                                        int parent_node_occurences_done_source = mappings__.FindAll(x => (x.Parent.Source.XPath == parent_node_mapping.Source.XPath) && (mappings__.IndexOf(x) < i)).Count;
                                        int parent_node_occurences_done_Target = mappings__.FindAll(x => (x.Parent.Target.XPath == parent_node_mapping.Target.XPath) && (mappings__.IndexOf(x) < i)).Count;

                                        int numberof_parentnodes_source = metadataDoc.SelectNodes(parent_node_mapping_xpath_source).Count;
                                        int numberof_parentnodes_target = metadataXml_target_cp.SelectNodes(parent_node_mapping_xpath_target).Count; // exisitng nodes of parent mapping 

                                        if ((parent_node_occurences_done_source == 0) || (parent_node_occurences_done_Target == 0))
                                        {
                                            int counter = numberof_parentnodes_target;
                                            if ((parent_node_occurences_done_Target != 0) && (parent_node_occurences_done_source == 0)) counter = 0;
                                            while (counter < numberof_parentnodes_source)
                                            {
                                                XmlNode new_node = metadataXml_target_cp.SelectSingleNode(parent_node_mapping_xpath_target).Clone();
                                                var child_nodes = new List<XmlNode>(new_node.ChildNodes.Cast<XmlNode>());
                                                for (int index = 0; index < child_nodes.Count(); index++)
                                                {
                                                    if (child_nodes[index].ChildNodes.Count > 0)
                                                        child_nodes.AddRange(child_nodes[index].Cast<XmlNode>());
                                                }
                                                child_nodes.Where(x => x.ChildNodes.Count == 0).ToList<XmlNode>().ForEach(x => x.InnerText = "");
                                                metadataXml_target_cp.SelectSingleNode(parent_node_mapping_xpath_target).ParentNode.AppendChild(new_node);
                                                counter++;
                                            }
                                        }

                                        int current_index_target = Math.Abs(metadataXml_target_cp.SelectNodes(parent_node_mapping_xpath_target).Count - metadataDoc.SelectNodes(parent_node_mapping_xpath_source).Count);


                                        foreach (XmlNode parent_node_mapping_xml_source in metadataDoc.SelectNodes(parent_node_mapping_xpath_source))
                                        {
                                            XmlNode parent_node_mapping_xml_target = metadataXml_target_cp.SelectNodes(parent_node_mapping_xpath_target)[current_index_target];

                                            for (int k = 0; k < parent_node_mapping_xml_source.SelectNodes(child_node_mapping_sub_xpath_source).Count; k++)
                                            {
                                                XmlNode child_node_child_source = parent_node_mapping_xml_source.SelectNodes(child_node_mapping_sub_xpath_source)[k];
                                                XmlNode child_node_child_target = parent_node_mapping_xml_target.SelectNodes(child_node_mapping_sub_xpath_target)[k];

                                                if (child_node_child_target is null)
                                                {
                                                    XmlNode new_node = parent_node_mapping_xml_target.SelectSingleNode(child_node_mapping_sub_xpath_target).Clone();
                                                    var child_nodes = new List<XmlNode>(new_node.ChildNodes.Cast<XmlNode>());
                                                    for (int index = 0; index < child_nodes.Count(); index++)
                                                    {
                                                        if (child_nodes[index].ChildNodes.Count > 0)
                                                            child_nodes.AddRange(child_nodes[index].Cast<XmlNode>());
                                                    }
                                                    child_nodes.Where(x => x.ChildNodes.Count == 0).ToList<XmlNode>().ForEach(x => x.InnerText = "");
                                                    parent_node_mapping_xml_target.SelectNodes(child_node_mapping_sub_xpath_target)[k-1].ParentNode.AppendChild(new_node);
                                                    parent_node_mapping_xml_target.SelectSingleNode(child_node_mapping_sub_xpath_target).ParentNode.AppendChild(new_node);
                                                    child_node_child_target = parent_node_mapping_xml_target.SelectNodes(child_node_mapping_sub_xpath_target)[k];
                                                }

                                                string source_value = child_node_child_source.InnerText;

                                                if (!string.IsNullOrEmpty(mappings__[i].TransformationRule.RegEx))
                                                {
                                                    Regex rg = new Regex(mappings__[i].TransformationRule.RegEx);
                                                    string index = mappings__[i].TransformationRule.Mask.Trim().Split('[', ']').Where(x => string.IsNullOrEmpty(x) == false).ToList<string>()[1];
                                                    source_value = rg.Matches(child_node_child_source.InnerText)[Int32.Parse(index)].Value;
                                                }
                                                
                                                if (!string.IsNullOrEmpty(child_node_child_target?.InnerText))
                                                    child_node_child_target.InnerText += " " + source_value;
                                                else 
                                                    child_node_child_target.InnerText = source_value;

                                            }
                                            current_index_target++;
                                        }

                                    }
                                    else
                                    {
                                        mappings__.AddRange(children_mappings);
                                        mappings__.RemoveAt(i);
                                        i--;
                                    }
                                }
                                catch (NullReferenceException ex)
                                {
                                    //Reference of the title node is missing
                                    throw new NullReferenceException(ex.Message);

                                }
                            }
                            DatasetVersion dsv = dm.GetDatasetWorkingCopy(datasetId);
                            dsv.Metadata = metadataXml_target_cp;
                            dm.EditDatasetVersion(dsv, null, null, null);
                            dm.CheckInDataset(datasetId, "Metadata Imported", GetUsernameOrDefault(), ViewCreationBehavior.None);

                            using (IUnitOfWork unitOfWork = this.GetUnitOfWork())
                            {
                                Dataset ds = unitOfWork.GetReadOnlyRepository<Dataset>().Get().FirstOrDefault(x => x.Id == datasetId);
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