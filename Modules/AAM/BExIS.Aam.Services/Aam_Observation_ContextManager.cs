using System;
using System.Collections.Generic;
using System.Linq;
using Vaiona.Persistence.Api;
using BExIS.Aam.Entities.Mapping;
using System.Diagnostics.Contracts;
using BExIS.Dlm.Entities.DataStructure;
using BExIS.Security.Services.Authorization;
using BExIS.Dlm.Services.DataStructure;
using BExIS.Dlm.Services.Data;
using BExIS.Xml.Helpers;
using BExIS.Dlm.Entities.Data;
using BExIS.Security.Entities.Authorization;

namespace BExIS.Aam.Services
{
    public class Aam_Observation_ContextManager : IDisposable
    {

        private XmlDatasetHelper xmlDatasetHelper = new XmlDatasetHelper();

        private IUnitOfWork guow = null;
        private IRepository<Aam_Observation_Context> AnnotationRepo;

        public Aam_Observation_ContextManager()
        {

        }

        public Aam_Observation_Context creeate_Aam_Observation_Context(Aam_Observation_Context an)
        {
            using (IUnitOfWork uow = this.GetUnitOfWork())
            {
                IRepository<Aam_Observation_Context> repo = uow.GetRepository<Aam_Observation_Context>();

                repo.Put(an);
                uow.Commit();
                // This creates the MV when there is no data tuples attached to the dataset, hence faster.
                // However, it asks for REFRESH to tell the underlying database to mark the MV as queryable.
                //updateMaterializedView(dataset.Id, ViewCreationBehavior.Create | ViewCreationBehavior.Refresh);
                return (an);
            }
        }

        public Boolean delete_Aam_Observation_Context(Aam_Observation_Context an)
        {
            try
            {
                using (IUnitOfWork uow = this.GetUnitOfWork())
                {
                    IRepository<Aam_Observation_Context> repo = uow.GetRepository<Aam_Observation_Context>();

                    repo.Delete(an);
                    uow.Commit();
                    // This creates the MV when there is no data tuples attached to the dataset, hence faster.
                    // However, it asks for REFRESH to tell the underlying database to mark the MV as queryable.
                    //updateMaterializedView(dataset.Id, ViewCreationBehavior.Create | ViewCreationBehavior.Refresh);
                    return (true);
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Aam_Observation_Context edit_Aam_Observation_Context(Aam_Observation_Context an)
        {
            try
            {
                Contract.Requires(an != null);
                Contract.Requires(an.Id >= 0);

                Contract.Ensures(Contract.Result<Aam_Observation_Context>() != null && Contract.Result<Aam_Observation_Context>().Id >= 0);

                using (IUnitOfWork uow = this.GetUnitOfWork())
                {
                    IRepository<Aam_Observation_Context> repo = uow.GetRepository<Aam_Observation_Context>();
                    repo.Merge(an);
                    var merged = repo.Get(an.Id);
                    repo.Put(merged);
                    uow.Commit();
                    return (merged);
                }
            }
            catch (Exception)
            {
                return an;
            }
        }

        public Aam_Observation_Context get_Aam_Observation_Context_by_id(long id)
        {
            try
            {
                using (IUnitOfWork uow = this.GetUnitOfWork())
                {
                    var Aam_Observation_ContextRepo = uow.GetReadOnlyRepository<Aam_Observation_Context>();

                    // the requested version is earlier than the latest regardless of check-in/ out status or its the latest version and the dataset is checked in.
                    Aam_Observation_Context an = Aam_Observation_ContextRepo.Query(p => p.Id == id).FirstOrDefault();
                    return (an);
                }
            }
            catch (Exception)
            {
                return new Aam_Observation_Context();
            }
        }


        public List<Aam_Observation_Context> get_all_Aam_Observation_Context()
        {
            try
            {
                using (IUnitOfWork uow = this.GetUnitOfWork())
                {
                    var datasetVersionRepo = uow.GetReadOnlyRepository<Aam_Observation_Context>();

                    List <Aam_Observation_Context> q1 = datasetVersionRepo.Query().ToList<Aam_Observation_Context>();
                    return (q1);
                }
            }
            catch (Exception)
            {
                return new List<Aam_Observation_Context>();
            }
        }



        public Dictionary<long, string> LoadDataset_Id_Title(string username)
        {
            EntityPermissionManager entityPermissionManager = new EntityPermissionManager();
            DataStructureManager dataStructureManager = new DataStructureManager();
            DatasetManager dm = new DatasetManager();

            try
            {
                List<long> datasetIDs = new List<long>();
                datasetIDs = entityPermissionManager.GetKeys(username, "Dataset", typeof(Dataset), RightType.Write).ToList<long>();
                Dictionary<long, string> temp = new Dictionary<long, string>();
                foreach (long id in datasetIDs)
                {
                    string k = null;
                    temp.TryGetValue(id, out k);
                    if (k == null)
                        if (dm.GetDatasetLatestMetadataVersion(id) != null)
                            temp.Add(id, id + " - " + xmlDatasetHelper.GetInformation(id, dm.GetDatasetLatestMetadataVersion(id), NameAttributeValues.title));
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










        #region IDisposable implementation
        private bool isDisposed = false;
        ~Aam_Observation_ContextManager()
        {
            Dispose(true);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    if (guow != null)
                        guow.Dispose();
                    isDisposed = true;
                }
            }
        }
        #endregion
    }
}
