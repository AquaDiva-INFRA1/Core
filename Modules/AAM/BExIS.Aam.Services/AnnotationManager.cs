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

namespace BExIS.Aam.Services
{
    public class AnnotationManager : IDisposable
    {
        private IUnitOfWork guow = null;
        public IRepository<Annotation> AnnotationRepo;

        public AnnotationManager()
        {
            guow = this.GetIsolatedUnitOfWork();
            this.AnnotationRepo = guow.GetRepository<Annotation>();
        }

        public IEnumerable<Annotation> GetAnnotations()
        {
            return this.AnnotationRepo.Get();
        }

        public Annotation GetAnnotation(Dataset ds, DatasetVersion dsv, Variable variable)
        {
            return this.AnnotationRepo.Get().FirstOrDefault(an => an.Dataset == ds && an.DatasetVersion == dsv && an.Variable == variable);
        }

        public bool DeleteAnnotation(Annotation annotation)
        {
            Contract.Requires(annotation != null);

            using (IUnitOfWork uow = this.GetUnitOfWork())
            {
                IRepository<Annotation> repo = uow.GetRepository<Annotation>();

                repo.Delete(annotation);

                uow.Commit();
            }
            // if any problem was detected during the commit, an exception will be thrown!
            return (true);
        }

        public Annotation CreateAnnotation(
            Dataset Dataset,
            DatasetVersion DatasetVersion,
            Variable Variable,
            String Entity,
            String Characteristic,
            String Standard
            )
        {
            //Create the new annotation object
            Annotation newAnnotation = new Annotation(Dataset, DatasetVersion, Variable, Entity, Characteristic, Standard);

            #region Get/Generate the ID's for Entity, Characteristic and Standard
            IEnumerable<Annotation> allAnnotations = this.GetAnnotations();

            //EntityID
            newAnnotation.EntityId = GetOrGenerateEntityID(allAnnotations, newAnnotation.Entity);

            //CharacteristicID
            newAnnotation.CharacteristicId = GetOrGenerateCharacteristicId(allAnnotations, newAnnotation.Characteristic);

            //StandardID
            newAnnotation.StandardId = GetOrGenerateStandardId(allAnnotations, newAnnotation.Standard);
            #endregion

            //Store the annotation in the DB
            using (IUnitOfWork uow = this.GetUnitOfWork())
            {
                IRepository<Annotation> repo = uow.GetRepository<Annotation>();
                repo.Put(newAnnotation);
                uow.Commit();
            }

            return newAnnotation;
        }

        private long GetOrGenerateStandardId(IEnumerable<Annotation> allAnnotations, string standard)
        {
            Annotation unicorn = allAnnotations.Where(an => an.Standard.Equals(standard)).FirstOrDefault();
            if (unicorn != null)
            {
                return unicorn.StandardId;
            }
            else
            {
                return GenerateNewStandardID(allAnnotations);
            }
        }

        private long GetOrGenerateCharacteristicId(IEnumerable<Annotation> allAnnotations, string characteristic)
        {
            Annotation unicorn = allAnnotations.Where(an => an.Characteristic.Equals(characteristic)).FirstOrDefault();
            if (unicorn != null)
            {
                return unicorn.CharacteristicId;
            }
            else
            {
                return GenerateNewCharacteristicID(allAnnotations);
            }
        }

        private long GetOrGenerateEntityID(IEnumerable<Annotation> allAnnotations, string entity)
        {
            Annotation unicorn = allAnnotations.Where(an => an.Entity.Equals(entity)).FirstOrDefault();
            if (unicorn != null)
            {
                return unicorn.EntityId;
            }
            else
            {
                return GenerateNewEntityID(allAnnotations);
            }
        }

        public Annotation CreateAnnotation(
            Dataset Dataset,
            DatasetVersion DatasetVersion,
            Variable Variable,
            String Entity,
            String Characteristic
            )
        {
            //Create the new annotation object
            Annotation newAnnotation = new Annotation(Dataset, DatasetVersion, Variable, Entity, Characteristic);

            #region Get/Generate the ID's for Entity, Characteristic and Standard
            IEnumerable<Annotation> allAnnotations = this.GetAnnotations();

            //EntityID
            Annotation unicorn = allAnnotations.Where(an => an.Entity.Equals(newAnnotation.Entity)).FirstOrDefault();
            if (unicorn != null)
            {
                newAnnotation.EntityId = unicorn.EntityId;
            }
            else
            {
                newAnnotation.EntityId = GenerateNewEntityID(allAnnotations);
            }

            //CharacteristicID
            unicorn = allAnnotations.Where(an => an.Characteristic.Equals(newAnnotation.Characteristic)).FirstOrDefault();
            if (unicorn != null)
            {
                newAnnotation.CharacteristicId = unicorn.CharacteristicId;
            }
            else
            {
                newAnnotation.CharacteristicId = GenerateNewCharacteristicID(allAnnotations);
            }

            //StandardID
            unicorn = allAnnotations.Where(an => an.Standard.Equals(newAnnotation.Standard)).FirstOrDefault();
            if (unicorn != null)
            {
                newAnnotation.StandardId = unicorn.StandardId;
            }
            else
            {
                newAnnotation.StandardId = GenerateNewStandardID(allAnnotations);
            }
            #endregion

            //Store the annotation in the DB
            using (IUnitOfWork uow = this.GetUnitOfWork())
            {
                IRepository<Annotation> repo = uow.GetRepository<Annotation>();
                repo.Put(newAnnotation);
                uow.Commit();
            }

            return newAnnotation;
        }

        private long GenerateNewStandardID(IEnumerable<Annotation> allAnnotations)
        {
            return allAnnotations.Select(an => an.StandardId).Max() + 1;
        }

        private long GenerateNewCharacteristicID(IEnumerable<Annotation> allAnnotations)
        {
            return allAnnotations.Select(an => an.CharacteristicId).Max() + 1;
        }

        private long GenerateNewEntityID(IEnumerable<Annotation> allAnnotations)
        {
            return allAnnotations.Select(an => an.EntityId).Max() + 1;
        }


        #region IDisposable implementation
        private bool isDisposed = false;
        ~AnnotationManager()
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
