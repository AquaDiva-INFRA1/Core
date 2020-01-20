﻿using System;
using System.Collections.Generic;
using System.Linq;
using Vaiona.Persistence.Api;
using BExIS.Aam.Entities.Mapping;
using System.Diagnostics.Contracts;
using BExIS.Dlm.Entities.DataStructure;
using VDS.RDF;
using VDS.RDF.Query;
using BExIS.Dcm.UploadWizard;
using BExIS.Security.Services.Authorization;
using BExIS.Dlm.Services.DataStructure;
using BExIS.Dlm.Services.Data;
using BExIS.Dlm.Entities.Data;
using BExIS.Security.Entities.Authorization;
using BExIS.Xml.Helpers;
using System.Web.Mvc;
using F23.StringSimilarity;


namespace BExIS.Aam.Services
{
    public class Aam_Dataset_column_annotationManager : IDisposable
    {

        private XmlDatasetHelper xmlDatasetHelper = new XmlDatasetHelper();


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

                    List<Aam_Dataset_column_annotation> q1 = datasetVersionRepo.Query().ToList<Aam_Dataset_column_annotation>();
                    return (q1);
                }
            }
            catch (Exception)
            {
                return new List<Aam_Dataset_column_annotation>();
            }
        }


        public List<Aam_Dataset_column_annotation> get_all_dataset_column_annotationByVariableLabel(String variableLabel)
        {
            using (IUnitOfWork uow = this.GetUnitOfWork())
            {
                IRepository<Aam_Dataset_column_annotation> repo = uow.GetRepository<Aam_Dataset_column_annotation>();
                var all = repo.Get();
                return all.Where(an => an.variable_id.Label.ToLower().Equals(variableLabel.ToLower())).ToList();
            }
        }

        public double calculate_similarity ( string a, string b )
        {
            if ((a != null) && (b != null))
            {
                List<double> similarities = new List<double>();
                double output = 0.0;

                var l = new NormalizedLevenshtein();
                similarities.Add(l.Similarity(a, b));
                var jw = new JaroWinkler();
                similarities.Add(jw.Similarity(a, b));
                var jac = new Jaccard();
                similarities.Add(jac.Similarity(a, b));

                foreach (double sim in similarities)
                {
                    output += sim;
                }

                return output / similarities.Count;
            }
            else return Int64.Parse("0");
        }

        public string get_label(string ADOntologyPath, string uri)
        {
            //Load the ontology as a graph
            IGraph g = new Graph();
            g.LoadFromFile(ADOntologyPath);
            //Create a new queryString
            SparqlParameterizedString queryString = new SparqlParameterizedString();
            //Add some important namespaces
            queryString.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            queryString.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            String commandTextTemplate = "SELECT ?label WHERE {{<{0}> rdfs:label ?label}}";

            //set the entity to search through the graph
            queryString.Namespaces.AddNamespace("entity", new Uri("http://www.co-ode.org/ontologies/pizza/pizza.owl#NamedPizza".Trim()));
            queryString.CommandText = "SELECT ?class WHERE" +
                " { " +
                "?class rdfs:subClassOf entity" +
                "} ";
            // end ofthe settings

            SparqlResultSet results = (SparqlResultSet)g.ExecuteQuery(queryString);
            String res = "";
            if (results.Count != 0)
            {
                res = results[0]["label"].ToString().Split('^')[0];
            }
            if (res.Contains("@"))
                res.Substring(0, res.IndexOf("@"));

            return res;
        }


        public Dictionary<long, string> LoadDataset_Id_Title( string username)
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
                    if (k == null) temp.Add(id, id + " - " + xmlDatasetHelper.GetInformation(id, NameAttributeValues.title));
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
