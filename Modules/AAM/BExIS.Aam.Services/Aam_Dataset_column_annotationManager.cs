using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vaiona.Persistence.Api;
using BExIS.Aam.Entities.Mapping;
using System.Diagnostics.Contracts;
using BExIS.Dlm.Entities.Data;
using BExIS.Dlm.Entities.DataStructure;
using BExIS.Dlm.Services.Data;
using VDS;
using VDS.RDF;
using VDS.RDF.Query;
using System.IO;
using Vaiona.Utils.Cfg;

namespace BExIS.Aam.Services
{
    public class Aam_Dataset_column_annotationManager : IDisposable
    {
        private IUnitOfWork guow = null;
        private IRepository<Aam_Dataset_column_annotation> AnnotationRepo;

        public Aam_Dataset_column_annotationManager()
        {

        }

        public Aam_Dataset_column_annotation creeate_dataset_column_annotation(Aam_Dataset_column_annotation an)
        {
            using (IUnitOfWork uow = this.GetUnitOfWork())
            {
                IRepository<Aam_Dataset_column_annotation> repo = uow.GetRepository<Aam_Dataset_column_annotation>();

                repo.Put(an);
                uow.Commit();
                // This creates the MV when there is no data tuples attached to the dataset, hence faster.
                // However, it asks for REFRESH to tell the underlying database to mark the MV as queryable.
                //updateMaterializedView(dataset.Id, ViewCreationBehavior.Create | ViewCreationBehavior.Refresh);
                return (an);
            }
        }

        public Boolean delete_dataset_column_annotation(Aam_Dataset_column_annotation an)
        {
            try
            {
                using (IUnitOfWork uow = this.GetUnitOfWork())
                {
                    IRepository<Aam_Dataset_column_annotation> repo = uow.GetRepository<Aam_Dataset_column_annotation>();

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

        public Aam_Dataset_column_annotation edit_dataset_column_annotation(Aam_Dataset_column_annotation an)
        {
            try
            {
                Contract.Requires(an != null);
                Contract.Requires(an.Id >= 0);

                Contract.Ensures(Contract.Result<Aam_Dataset_column_annotation>() != null && Contract.Result<Aam_Dataset_column_annotation>().Id >= 0);

                using (IUnitOfWork uow = this.GetUnitOfWork())
                {
                    IRepository<Aam_Dataset_column_annotation> repo = uow.GetRepository<Aam_Dataset_column_annotation>();
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

        public Aam_Dataset_column_annotation get_dataset_column_annotation_by_id(long id)
        {
            try
            {
                using (IUnitOfWork uow = this.GetUnitOfWork())
                {
                    var Aam_Dataset_column_annotationRepo = uow.GetReadOnlyRepository<Aam_Dataset_column_annotation>();

                    // the requested version is earlier than the latest regardless of check-in/ out status or its the latest version and the dataset is checked in.
                    Aam_Dataset_column_annotation an = Aam_Dataset_column_annotationRepo.Query(p => p.Id == id).FirstOrDefault();
                    return (an);
                }
            }
            catch (Exception)
            {
                return new Aam_Dataset_column_annotation();
            }
        }

        public Aam_Dataset_column_annotation get_dataset_column_annotation_by_variable(Variable id)
        {
            try
            {
                using (IUnitOfWork uow = this.GetUnitOfWork())
                {
                    var Aam_Dataset_column_annotationRepo = uow.GetReadOnlyRepository<Aam_Dataset_column_annotation>();

                    // the requested version is earlier than the latest regardless of check-in/ out status or its the latest version and the dataset is checked in.
                    Aam_Dataset_column_annotation an = Aam_Dataset_column_annotationRepo.Query(p => p.variable_id == id).FirstOrDefault();
                    return (an);
                }
            }
            catch (Exception)
            {
                return new Aam_Dataset_column_annotation();
            }
        }


        public List<Aam_Dataset_column_annotation> get_all_dataset_column_annotation()
        {
            try
            {
                using (IUnitOfWork uow = this.GetUnitOfWork())
                {
                    var datasetVersionRepo = uow.GetReadOnlyRepository<Aam_Dataset_column_annotation>();

                    List <Aam_Dataset_column_annotation> q1 = datasetVersionRepo.Query().ToList<Aam_Dataset_column_annotation>();
                    return (q1);
                }
            }
            catch (Exception)
            {
                return new List<Aam_Dataset_column_annotation>();
            }
        }












        #region IDisposable implementation
        private bool isDisposed = false;
        ~Aam_Dataset_column_annotationManager()
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
