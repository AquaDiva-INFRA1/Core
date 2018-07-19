﻿using System;
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

namespace BExIS.Aam.Services
{
    public class AnnotationManager : IDisposable
    {
        private IUnitOfWork guow = null;
        private IRepository<Annotation> AnnotationRepo;

        public AnnotationManager()
        {
            guow = this.GetIsolatedUnitOfWork();
            this.AnnotationRepo = guow.GetRepository<Annotation>();
        }

        /// <summary>
        /// Gets all annotations currently stored in the database.
        /// </summary>
        /// <returns>IEnumerable containing all annotation-objects</returns>
        public IEnumerable<Annotation> GetAnnotations()
        {
            IEnumerable<Annotation> output;
            using (IUnitOfWork uow = this.GetUnitOfWork())
            {
                IRepository<Annotation> repo = uow.GetRepository<Annotation>();
                output = repo.Get();
            }
            return output;
        }

        /// <summary>
        /// Get the annotation specified by the given parameters.
        /// This should only return a single annotation.
        /// </summary>
        /// <param name="DatasetId">Id of the Dataset that the desired annotation should refer to</param>
        /// <param name="DatasetVersionId">Id of the DatasetVersion that the desired annotation should refer to</param>
        /// <param name="VariableId">Id of the Variable that the desired annotation should refer to</param>
        /// <returns>A single annotation, specified by the parameters, or null.</returns>
        public Annotation GetAnnotation(long DatasetId, long DatasetVersionId, long VariableId)
        {
            Annotation output;
            using (IUnitOfWork uow = this.GetUnitOfWork())
            {
                IRepository<Annotation> repo = uow.GetRepository<Annotation>();
                var all = repo.Get();
                output = all.FirstOrDefault(an =>
                                an.Dataset.Id == DatasetId &&
                                an.DatasetVersion.Id == DatasetVersionId
                                && an.Variable.Id == VariableId);
            }
            return output;
        }

        /// <summary>
        /// Get the annotation specified by the given Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Annotation GetAnnotation(long id)
        {
            Annotation output;
            using (IUnitOfWork uow = this.GetUnitOfWork())
            {
                IRepository<Annotation> repo = uow.GetRepository<Annotation>();
                output = repo.Get(id);
            }
            return output;
        }

        public List<Annotation> GetAnnotationsByVariableLabel(String variableLabel)
        {
            using (IUnitOfWork uow = this.GetUnitOfWork())
            {
                IRepository<Annotation> repo = uow.GetRepository<Annotation>();
                var all = repo.Get();
                return all.Where(an => an.Variable.Label.ToLower().Equals(variableLabel.ToLower())).ToList();
            }
        }

        /// <summary>
        /// Create an annotation based on the given parameters and store it in the database.
        /// </summary>
        /// <param name="Dataset">Dataset that the annotation is refering to</param>
        /// <param name="DatasetVersion">DatasetVersion that the annotation is refering to</param>
        /// <param name="Variable">Variable that the annotation is refering to</param>
        /// <param name="Entity">URI-String of the entity that the annotation is refering to</param>
        /// <param name="Entity_Label">Label of the Entity</param>
        /// <param name="Characteristic">URI-String of the characteristic that the annotation is refering to</param>
        /// <param name="Characteristic_Label">Label of the Characteristic</param>
        /// <param name="Standard">URI-String of the standard that the annotation is refering to</param>
        /// <param name="Standard_Label">Label of the Standard</param>
        /// <returns>The created annotation</returns>
        public Annotation CreateAnnotation(Dataset Dataset, DatasetVersion DatasetVersion, Variable Variable, String Entity, String Entity_Label, String Characteristic, String Characteristic_Label, String Standard, String Standard_Label)
        {
            //Create the new annotation object
            Annotation newAnnotation = new Annotation(Dataset, DatasetVersion, Variable, Entity, Entity_Label, Characteristic, Characteristic_Label, Standard, Standard_Label);

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

        /// <summary>
        /// Create an annotation based on the given parameters and store it in the database.
        /// </summary>
        /// <param name="Dataset">Dataset that the annotation is refering to</param>
        /// <param name="DatasetVersion">DatasetVersion that the annotation is refering to</param>
        /// <param name="Variable">Variable that the annotation is refering to</param>
        /// <param name="Entity">URI-String of the entity that the annotation is refering to</param>
        /// <param name="Characteristic">URI-String of the characteristic that the annotation is refering to</param>
        /// <param name="Standard">URI-String of the standard that the annotation is refering to</param>
        /// <returns>The created annotation</returns>
        public Annotation CreateAnnotationWithoutLabels(Dataset Dataset, DatasetVersion DatasetVersion, Variable Variable, String Entity, String Characteristic, String Standard)
        {
            IEnumerable<Annotation> allAnnotations = this.GetAnnotations();

            //Create the new annotation object
            Annotation newAnnotation = new Annotation(Dataset, DatasetVersion, Variable, Entity, this.GetLabelForURI(allAnnotations, Entity), 
                Characteristic, this.GetLabelForURI(allAnnotations, Characteristic), Standard, this.GetLabelForURI(allAnnotations, Standard));

            #region Get/Generate the ID's for Entity, Characteristic and Standard
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

        /// <summary>
        /// Create an annotation based on the given parameters and store it in the database.
        /// </summary>
        /// <param name="DatasetId">Id of the Dataset that the annotation is refering to</param>
        /// <param name="DatasetVersionId">Id of the DatasetVersion that the annotation is refering to</param>
        /// <param name="Variable">Variable that the annotation is refering to</param>
        /// <param name="Entity">URI-String of the entity that the annotation is refering to</param>
        /// <param name="Entity_Label">Label of the Entity</param>
        /// <param name="Characteristic">URI-String of the characteristic that the annotation is refering to</param>
        /// <param name="Characteristic_Label">Label of the Characteristic</param>
        /// <param name="Standard">URI-String of the standard that the annotation is refering to</param>
        /// <param name="Standard_Label">Label of the Standard</param>
        /// <returns>The created annotation</returns>
        public Annotation CreateAnnotation(long DatasetId, long DatasetVersionId, Variable Variable, String Entity, String Entity_Label, String Characteristic, String Characteristic_Label, String Standard, String Standard_Label)
        {
            //Grab Dataset and DatasetVersion using the provided IDs
            DatasetManager dsm = new DatasetManager();
            Dataset Dataset = dsm.GetDataset(DatasetId);
            DatasetVersion DatasetVersion = dsm.GetDatasetVersion(DatasetVersionId);

            //Create the new annotation object
            Annotation newAnnotation = this.CreateAnnotation(Dataset, DatasetVersion, Variable, Entity, Entity_Label, Characteristic, Characteristic_Label, Standard, Standard_Label);
            return newAnnotation;
        }

        /// <summary>
        /// Create an annotation based on the given parameters and store it in the database.
        /// </summary>
        /// <param name="DatasetId">Id of the Dataset that the annotation is refering to</param>
        /// <param name="DatasetVersionId">Id of the DatasetVersion that the annotation is refering to</param>
        /// <param name="Variable">Variable that the annotation is refering to</param>
        /// <param name="Entity">URI-String of the entity that the annotation is refering to</param>
        /// <param name="Characteristic">URI-String of the characteristic that the annotation is refering to</param>
        /// <param name="Standard">URI-String of the standard that the annotation is refering to</param>
        /// <returns>The created annotation</returns>
        public Annotation CreateAnnotationWithoutLabels(long DatasetId, long DatasetVersionId, Variable Variable, String Entity, String Characteristic, String Standard)
        {
            //Grab Dataset and DatasetVersion using the provided IDs
            DatasetManager dsm = new DatasetManager();
            Dataset Dataset = dsm.GetDataset(DatasetId);
            DatasetVersion DatasetVersion = dsm.GetDatasetVersion(DatasetVersionId);

            //Grab all annotations for the search for the labels
            IEnumerable<Annotation> allAnnotations = this.GetAnnotations();

            //Create the new annotation object
            Annotation newAnnotation = this.CreateAnnotation(Dataset, DatasetVersion, Variable, Entity, this.GetLabelForURI(allAnnotations, Entity), Characteristic, this.GetLabelForURI(allAnnotations, Characteristic), Standard, this.GetLabelForURI(allAnnotations, Standard));
            return newAnnotation;
        }

        /// <summary>
        /// Create an annotation based on the given parameters with a default standard and store it in the database.
        /// </summary>
        /// <param name="Dataset">Dataset that the annotation is refering to</param>
        /// <param name="DatasetVersion">DatasetVersion that the annotation is refering to</param>
        /// <param name="Variable">Variable that the annotation is refering to</param>
        /// <param name="Entity">URI-String of the entity that the annotation is refering to</param>
        /// <param name="Entity_Label">Label of the Entity</param>
        /// <param name="Characteristic">URI-String of the characteristic that the annotation is refering to</param>
        /// <param name="Characteristic_Label">Label of the Characteristic</param>
        /// <returns></returns>
        public Annotation CreateAnnotation(Dataset Dataset, DatasetVersion DatasetVersion, Variable Variable, String Entity, String Entity_Label, String Characteristic, String Characteristic_Label)
        {
            return this.CreateAnnotation(Dataset, DatasetVersion, Variable, Entity, Entity_Label, Characteristic, Characteristic_Label, "http://ecoinformatics.org/oboe/oboe.1.2/oboe-core.owl#Standard", "Standard");
        }

        /// <summary>
        /// Create an annotation based on the given parameters with a default standard and store it in the database.
        /// </summary>
        /// <param name="Dataset">Dataset that the annotation is refering to</param>
        /// <param name="DatasetVersion">DatasetVersion that the annotation is refering to</param>
        /// <param name="Variable">Variable that the annotation is refering to</param>
        /// <param name="Entity">URI-String of the entity that the annotation is refering to</param>
        /// <param name="Characteristic">URI-String of the characteristic that the annotation is refering to</param>
        /// <returns></returns>
        public Annotation CreateAnnotationWithoutLabels(Dataset Dataset, DatasetVersion DatasetVersion, Variable Variable, String Entity, String Characteristic)
        {
            return this.CreateAnnotationWithoutLabels(Dataset, DatasetVersion, Variable, Entity, Characteristic, "http://ecoinformatics.org/oboe/oboe.1.2/oboe-core.owl#Standard");
        }

        /// <summary>
        /// Create an annotation based on the given parameters with a default standard and store it in the database.
        /// </summary>
        /// <param name="DatasetId">Id of the Dataset that the annotation is refering to</param>
        /// <param name="DatasetVersionId">Id of the DatasetVersion that the annotation is refering to</param>
        /// <param name="Variable">Variable that the annotation is refering to</param>
        /// <param name="Entity">URI-String of the entity that the annotation is refering to</param>
        /// <param name="Entity_Label">Label of the Entity</param>
        /// <param name="Characteristic">URI-String of the characteristic that the annotation is refering to</param>
        /// <param name="Characteristic_Label">Label of the Characteristic</param>
        /// <returns>The created annotation</returns>
        public Annotation CreateAnnotation(long DatasetId, long DatasetVersionId, Variable Variable, String Entity, String Entity_Label, String Characteristic, String Characteristic_Label)
        {
            //Grab Dataset and DatasetVersion using the provided IDs
            DatasetManager dsm = new DatasetManager();
            Dataset Dataset = dsm.GetDataset(DatasetId);
            DatasetVersion DatasetVersion = dsm.GetDatasetVersion(DatasetVersionId);

            //Create the new annotation object
            return this.CreateAnnotation(Dataset, DatasetVersion, Variable, Entity, Entity_Label, Characteristic, Characteristic_Label);
        }

        /// <summary>
        /// Create an annotation based on the given parameters with a default standard and store it in the database.
        /// </summary>
        /// <param name="DatasetId">Id of the Dataset that the annotation is refering to</param>
        /// <param name="DatasetVersionId">Id of the DatasetVersion that the annotation is refering to</param>
        /// <param name="Variable">Variable that the annotation is refering to</param>
        /// <param name="Entity">URI-String of the entity that the annotation is refering to</param>
        /// <param name="Characteristic">URI-String of the characteristic that the annotation is refering to</param>
        /// <returns>The created annotation</returns>
        public Annotation CreateAnnotation(long DatasetId, long DatasetVersionId, Variable Variable, String Entity, String Characteristic)
        {
            //Grab Dataset and DatasetVersion using the provided IDs
            DatasetManager dsm = new DatasetManager();
            Dataset Dataset = dsm.GetDataset(DatasetId);
            DatasetVersion DatasetVersion = dsm.GetDatasetVersion(DatasetVersionId);

            //Grab all annotations for the search for the labels
            IEnumerable<Annotation> allAnnotations = this.GetAnnotations();

            //Create the new annotation object
            return this.CreateAnnotation(Dataset, DatasetVersion, Variable, Entity, this.GetLabelForURI(allAnnotations, Entity), Characteristic, this.GetLabelForURI(allAnnotations, Characteristic));
        }

        /// <summary>
        /// Deletes the annotation specified by the given Id from the database
        /// </summary>
        /// <param name="id">Id of the annotation that is going to be deleted</param>
        /// <returns>True if the process did not cause any problems</returns>
        public bool DeleteAnnotation(long id)
        {
            Contract.Requires(id >= 0);

            using (IUnitOfWork uow = this.GetUnitOfWork())
            {
                IRepository<Annotation> repo = uow.GetRepository<Annotation>();
                repo.Delete(this.GetAnnotation(id));
                uow.Commit();
            }

            // if any problem was detected during the commit, an exception will be thrown!
            return true;
        }

        /// <summary>
        /// Deletes the annotations specified by the given enumerable Ids
        /// </summary>
        /// <param name="ids">Identifiers of the annotations that are going to be deleted</param>
        /// <returns>True if the process did not cause any problems</returns>
        public bool DeleteAnnotation(IEnumerable<long> ids)
        {
            using (IUnitOfWork uow = this.GetUnitOfWork())
            {
                IRepository<Annotation> repo = uow.GetRepository<Annotation>();

                foreach (long id in ids)
                {
                    Contract.Requires(id >= 0);
                    repo.Delete(this.GetAnnotation(id));
                }

                uow.Commit();
            }
            // if any problem was detected during the commit, an exception will be thrown!
            return true;
        }

        /// <summary>
        /// Deletes all annotations refering to the specified Dataset
        /// </summary>
        /// <param name="DatasetId">Identifier of the Dataset</param>
        /// <returns>True if the process did not cause any problems</returns>
        public bool DeleteAnnotationsForDataset(long DatasetId)
        {
            Contract.Requires(DatasetId >= 0);

            using (IUnitOfWork uow = this.GetUnitOfWork())
            {
                IRepository<Annotation> repo = uow.GetRepository<Annotation>();
                IEnumerable<long> hits = repo.Get().Where(an => an.Dataset.Id == DatasetId).Select(an => an.Id);
                this.DeleteAnnotation(hits);
                uow.Commit();
            }

            // if any problem was detected during the commit, an exception will be thrown!
            return true;
        }

        /// <summary>
        /// Deletes all annotations refering to the specified DatasetVersion
        /// </summary>
        /// <param name="DatasetVersionId">Identifier of the DatasetVersion</param>
        /// <returns>True if the process did not cause any problems</returns>
        public bool DeleteAnnotationsForDatasetVersion(long DatasetVersionId)
        {
            Contract.Requires(DatasetVersionId >= 0);

            using (IUnitOfWork uow = this.GetUnitOfWork())
            {
                IRepository<Annotation> repo = uow.GetRepository<Annotation>();
                IEnumerable<long> hits = repo.Get().Where(an => an.DatasetVersion.Id == DatasetVersionId).Select(an => an.Id);
                this.DeleteAnnotation(hits);
                uow.Commit();
            }

            // if any problem was detected during the commit, an exception will be thrown!
            return true;
        }

        /// <summary>
        /// Edits the labels of the given URIs in all Annotations
        /// </summary>
        /// <param name="uris">List of the URIs</param>
        /// <param name="labels">List of the labels (same length as the URI-List)</param>
        /// <returns>True if the editing was successful</returns>
        public bool EditLabels(List<String> uris, List<String> labels)
        {
            Contract.Requires(uris.Count == labels.Count);

            using(IUnitOfWork uow = this.GetUnitOfWork())
            {
                IRepository<Annotation> repo = uow.GetRepository<Annotation>();
                IEnumerable<Annotation> allAnnotations = repo.Get();

                //For each annotation, check for each URI if we have a label for it in our input-lists
                foreach(Annotation an in allAnnotations)
                {
                    int index = uris.IndexOf(an.Entity);
                    if(index != -1)
                    {
                        an.Entity_Label = labels.ElementAt(index);
                    }

                    index = uris.IndexOf(an.Characteristic);
                    if(index != -1)
                    {
                        an.Characteristic_Label = labels.ElementAt(index);
                    }

                    index = uris.IndexOf(an.Standard);
                    if(index != -1)
                    {
                        an.Standard_Label = labels.ElementAt(index);
                    }
                }
                uow.Commit();
            }
            return true;
        }

        #region (Private) Helper functions
        /// <summary>
        /// Gets the id of the given standard from the given enumerable annotations.
        /// Generates a new id if the standard is not used in any annotation yet.
        /// </summary>
        /// <param name="allAnnotations">Enumerable annotations</param>
        /// <param name="standard">Standard that the id should be generated for</param>
        /// <returns>A new id for the given standard</returns>
        private long GetOrGenerateStandardId(IEnumerable<Annotation> allAnnotations, string standard)
        {
            Annotation unicorn = allAnnotations.Where(an => an.Standard!=null && an.Standard.Equals(standard)).FirstOrDefault();
            if (unicorn != null)
            {
                return unicorn.StandardId;
            }
            else
            {
                return GenerateNewStandardID(allAnnotations);
            }
        }

        /// <summary>
        /// Gets the id of the given characteristic from the given enumerable annotations.
        /// Generates a new id if the characteristic is not used in any annotation yet.
        /// </summary>
        /// <param name="allAnnotations">Enumerable annotations</param>
        /// <param name="characteristic">Characteristic that the id should be generated for</param>
        /// <returns>A new id for the given characteristic</returns>
        private long GetOrGenerateCharacteristicId(IEnumerable<Annotation> allAnnotations, string characteristic)
        {
            Annotation unicorn = allAnnotations.Where(an => an.Characteristic!=null && an.Characteristic.Equals(characteristic)).FirstOrDefault();
            if (unicorn != null)
            {
                return unicorn.CharacteristicId;
            }
            else
            {
                return GenerateNewCharacteristicID(allAnnotations);
            }
        }

        /// <summary>
        /// Gets the id of the given entity from the given enumerable annotations.
        /// Generates a new id if the entity is not used in any annotation yet.
        /// </summary>
        /// <param name="allAnnotations">Enumerable annotations</param>
        /// <param name="entity">Entity that the id should be generated for</param>
        /// <returns>A new id for the given entity</returns>
        private long GetOrGenerateEntityID(IEnumerable<Annotation> allAnnotations, string entity)
        {
            Annotation unicorn = allAnnotations.Where(an => an.Entity!=null && an.Entity.Equals(entity)).FirstOrDefault();
            if (unicorn != null)
            {
                return unicorn.EntityId;
            }
            else
            {
                return GenerateNewEntityID(allAnnotations);
            }
        }

        /// <summary>
        /// Gets the id of the given entity from the given enumerable annotations.
        /// Generates a new id if the entity is not used in any annotation yet.
        /// </summary>
        /// <param name="allAnnotations">Enumerable annotations</param>
        /// <param name="entity">Entity that the id should be generated for</param>
        /// <returns>A new id for the given entity</returns>
        private String GetLabelForURI(IEnumerable<Annotation> allAnnotations, string uri)
        {
            Annotation unicorn = allAnnotations.Where(an => an.Entity != null && an.Entity.Equals(uri)).FirstOrDefault();
            if (unicorn != null)
                return unicorn.Entity_Label;

            unicorn = allAnnotations.Where(an => an.Characteristic != null && an.Characteristic.Equals(uri)).FirstOrDefault();
            if (unicorn != null)
                return unicorn.Characteristic_Label;

            unicorn = allAnnotations.Where(an => an.Standard != null && an.Standard.Equals(uri)).FirstOrDefault();
            if (unicorn != null)
                return unicorn.Standard_Label;

            //TODO: Search the ontology to get a label - call SemanticSearchController/FindOntologyLabels for that (can we even do that in here?)

            return "No label found!";
        }

        /// <summary>
        /// Generates a new id for a standard.
        /// Takes the currently largest id and returns this id+1
        /// </summary>
        /// <param name="allAnnotations">Enumerable annotations</param>
        /// <returns>A new id for a standard</returns>
        private long GenerateNewStandardID(IEnumerable<Annotation> allAnnotations)
        {
            return allAnnotations.Select(an => an.StandardId).Max() + 1;
        }

        /// <summary>
        /// Generates a new id for a characteristic.
        /// Takes the currently largest id and returns this id+1
        /// </summary>
        /// <param name="allAnnotations">Enumerable annotations</param>
        /// <returns>A new id for a characteristic</returns>
        private long GenerateNewCharacteristicID(IEnumerable<Annotation> allAnnotations)
        {
            return allAnnotations.Select(an => an.CharacteristicId).Max() + 1;
        }

        /// <summary>
        /// Generates a new id for an entity.
        /// Takes the currently largest id and returns this id+1
        /// </summary>
        /// <param name="allAnnotations">Enumerable annotations</param>
        /// <returns>A new id for an entity</returns>
        private long GenerateNewEntityID(IEnumerable<Annotation> allAnnotations)
        {
            return allAnnotations.Select(an => an.EntityId).Max() + 1;
        }
        #endregion

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
