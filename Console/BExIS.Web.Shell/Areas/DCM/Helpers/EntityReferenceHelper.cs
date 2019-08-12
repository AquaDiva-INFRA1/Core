﻿using BExIS.Modules.Dcm.UI.Models.EntityReference;
using BExIS.Security.Services.Objects;
using BExIS.Security.Entities.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BExIS.Dlm.Services.MetadataStructure;
using BExIS.Dim.Helpers.Mapping;
using BExIS.Dlm.Services.Data;
using BExIS.Dlm.Entities.Data;
using System.Xml;
using BExIS.Xml.Helpers;
using System.Xml.Linq;
using System.IO;
using Vaiona.Utils.Cfg;

namespace BExIS.Modules.Dcm.UI.Helpers
{
    public class EntityReferenceHelper
    {
        public EntityReferenceHelper()
        {
        }

        public string GetEntityTitle(long id, long typeId, int version = 0)
        {
            EntityManager entityManager = new EntityManager();

            try
            {
                var instanceStore = (IEntityStore)Activator.CreateInstance(entityManager.FindById(typeId).EntityStoreType);

                if (version == 0) return instanceStore.GetTitleById(id);
                else
                {
                    var entityStoreItem = instanceStore.GetVersionsById(id).FirstOrDefault(e => e.Version.Equals(version));
                    if (entityStoreItem != null) return entityStoreItem.Title;

                    return "Not Available";
                }
            }
            finally
            {
                entityManager.Dispose();
            }
        }

        public int CountVersions(long id, long typeId)
        {
            EntityManager entityManager = new EntityManager();

            try
            {
                var instanceStore = (IEntityStore)Activator.CreateInstance(entityManager.FindById(typeId).EntityStoreType);
                return instanceStore.CountVersions(id);
            }
            finally
            {
                entityManager.Dispose();
            }
        }

        public SelectList GetEntityVersions(long id, long typeId)
        {
            EntityManager entityManager = new EntityManager();
            List<SelectListItem> list = new List<SelectListItem>();

            try
            {
                var instanceStore = (IEntityStore)Activator.CreateInstance(entityManager.FindById(typeId).EntityStoreType);

                instanceStore.GetVersionsById(id).ForEach(v => list.Add(new SelectListItem() { Text = v.Version + " :  " + v.CommitComment, Value = v.Version.ToString() }));

                return new SelectList(list, "Value", "Text");
            }
            finally
            {
                entityManager.Dispose();
            }
        }

        public string GetEntityTypeName(long id)
        {
            EntityManager entityManager = new EntityManager();

            try
            {
                return entityManager.Entities.Where(e => e.Id.Equals(id)).Select(e => e.Name).FirstOrDefault();
            }
            finally
            {
                entityManager.Dispose();
            }
        }

        public SelectList GetEntityTypes()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            EntityManager entityManager = new EntityManager();

            try
            {
                entityManager.Entities.ToList().ForEach(e => list.Add(new SelectListItem() { Text = e.Name, Value = e.Id.ToString() }));

                return new SelectList(list, "Value", "Text");
            }
            finally
            {
                entityManager.Dispose();
            }
        }

        public List<EntityStoreItem> GetEntities(long typeId)
        {
            EntityManager entityManager = new EntityManager();

            try
            {
                var instanceStore = (IEntityStore)Activator.CreateInstance(entityManager.FindById(typeId).EntityStoreType);
                return instanceStore.GetEntities();
            }
            finally
            {
                entityManager.Dispose();
            }
        }

        public EntityReference Convert(CreateSimpleReferenceModel model)
        {
            EntityReference tmp = new EntityReference();
            tmp.SourceId = model.SourceId;
            tmp.SourceEntityId = model.SourceTypeId;
            tmp.SourceVersion = model.SourceVersion;
            tmp.TargetId = model.Target;
            tmp.TargetEntityId = model.TargetType;
            tmp.TargetVersion = model.TargetVersion;
            tmp.Context = model.Context;
            tmp.ReferenceType = model.ReferenceType;

            return tmp;
        }

        //public EntityReference Convert(SimpleReferenceModel model, long sourceId, long sourceTypeId)
        //{
        //    EntityReference tmp = new EntityReference();
        //    tmp.SourceId = sourceId;
        //    tmp.SourceEntityId = sourceTypeId;
        //    tmp.TargetId = model.Id;
        //    tmp.TargetEntityId = model.TypeId;
        //    tmp.Context = model.Context;
        //    tmp.ReferenceType = model.ReferenceType;

        //    return tmp;
        //}

        public SimpleReferenceModel GetSimpleReferenceModel(long id, long typeId)
        {
            SimpleReferenceModel tmp = new SimpleReferenceModel();
            tmp.Id = id;
            tmp.TypeId = typeId;
            tmp.Title = GetEntityTitle(id, typeId);
            tmp.Type = GetEntityTypeName(typeId);

            return tmp;
        }

        public SimpleReferenceModel GetTarget(EntityReference entityReference)
        {
            SimpleReferenceModel tmp = new SimpleReferenceModel();
            tmp.Id = entityReference.TargetId;
            tmp.Version = entityReference.TargetVersion;
            tmp.RefId = entityReference.Id;
            tmp.TypeId = entityReference.TargetEntityId;
            tmp.Title = GetEntityTitle(entityReference.TargetId, entityReference.TargetEntityId, entityReference.TargetVersion);
            tmp.Type = GetEntityTypeName(entityReference.TargetEntityId);
            tmp.Context = entityReference.Context;
            tmp.ReferenceType = entityReference.ReferenceType;
            tmp.LatestVersion = entityReference.TargetVersion == CountVersions(entityReference.TargetId, entityReference.TargetEntityId) ? true : false;

            return tmp;
        }

        public SimpleReferenceModel GetSource(EntityReference entityReference)
        {
            SimpleReferenceModel tmp = new SimpleReferenceModel();
            tmp.Id = entityReference.SourceId;
            tmp.Version = entityReference.SourceVersion;
            tmp.RefId = entityReference.Id;
            tmp.TypeId = entityReference.SourceEntityId;
            tmp.Title = GetEntityTitle(entityReference.SourceId, entityReference.SourceEntityId, entityReference.SourceVersion);
            tmp.Type = GetEntityTypeName(entityReference.SourceEntityId);
            tmp.Context = entityReference.Context;
            tmp.ReferenceType = entityReference.ReferenceType;
            tmp.LatestVersion = entityReference.SourceVersion == CountVersions(entityReference.SourceId, entityReference.SourceEntityId) ? true : false;

            return tmp;
        }

        public List<SimpleReferenceModel> GetAllReferences(long id, long typeid)
        {
            List<SimpleReferenceModel> tmp = new List<SimpleReferenceModel>();
            EntityReferenceManager entityReferenceManager = new EntityReferenceManager();
            EntityReferenceHelper helper = new EntityReferenceHelper();

            try
            {
                // get all references where incoming is source
                var list = entityReferenceManager.References.Where(r => r.SourceId.Equals(id) && r.SourceEntityId.Equals(typeid)).ToList();
                list.ForEach(r => tmp.Add(helper.GetTarget(r)));

                //get all refs where incoming is taret
                list = entityReferenceManager.References.Where(r => r.TargetId.Equals(id) && r.TargetEntityId.Equals(typeid)).ToList();

                list.ForEach(r => tmp.Add(helper.GetSource(r)));
            }
            catch (Exception ex)
            {
                throw new Exception("References could not be loaded", ex);
            }
            finally
            {
                entityReferenceManager.Dispose();
            }

            return tmp;
        }

        public List<SimpleReferenceModel> GetSourceReferences(long id, long typeid)
        {
            List<SimpleReferenceModel> tmp = new List<SimpleReferenceModel>();
            EntityReferenceManager entityReferenceManager = new EntityReferenceManager();
            EntityReferenceHelper helper = new EntityReferenceHelper();

            try
            {
                //get all refs where incoming is taret
                var list = entityReferenceManager.References.Where(r => r.TargetId.Equals(id) && r.TargetEntityId.Equals(typeid)).ToList();

                list.ForEach(r => tmp.Add(helper.GetSource(r)));
            }
            catch (Exception ex)
            {
                throw new Exception("References could not be loaded", ex);
            }
            finally
            {
                entityReferenceManager.Dispose();
            }

            return tmp;
        }

        public List<SimpleReferenceModel> GetTargetReferences(long id, long typeid)
        {
            List<SimpleReferenceModel> tmp = new List<SimpleReferenceModel>();
            EntityReferenceManager entityReferenceManager = new EntityReferenceManager();
            EntityReferenceHelper helper = new EntityReferenceHelper();

            try
            {
                // get all references where incoming is source
                var list = entityReferenceManager.References.Where(r => r.SourceId.Equals(id) && r.SourceEntityId.Equals(typeid)).ToList();
                list.ForEach(r => tmp.Add(helper.GetTarget(r)));
            }
            catch (Exception ex)
            {
                throw new Exception("References could not be loaded", ex);
            }
            finally
            {
                entityReferenceManager.Dispose();
            }

            return tmp;
        }

        #region Entity Reference Config

        /// <summary>
        /// this function return a list of all reference types. This types are listed in the entity reference config.xml in the workspace
        /// </summary>
        /// <returns></returns>
        public SelectList GetReferencesTypes()
        {
            string filepath = Path.Combine(AppConfiguration.GetModuleWorkspacePath("DCM"), "EntityReferenceConfig.xml");
            string dir = Path.GetDirectoryName(filepath);

            if (Directory.Exists(dir) && File.Exists(filepath))
            {
                XDocument xdoc = XDocument.Load(filepath);

                var types = xdoc.Root.Descendants("referenceType").Select(e => new SelectListItem() { Text = e.Value, Value = e.Value }).ToList();

                return new SelectList(types, "Text", "Value");
            }
            else
            {
                throw new FileNotFoundException("File EntityReferenceConfig.xml not found in :" + dir, "EntityReferenceConfig.xml");
            }
        }

        #endregion Entity Reference Config
    }
}