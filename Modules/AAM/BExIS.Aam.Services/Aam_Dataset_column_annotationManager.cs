using System;
using System.Collections.Generic;
using System.Linq;
using Vaiona.Persistence.Api;
using BExIS.Aam.Entities.Mapping;
using System.Diagnostics.Contracts;
using BExIS.Dlm.Entities.DataStructure;
using VDS.RDF;
using VDS.RDF.Query;
using BExIS.Dlm.Services.DataStructure;
using BExIS.Dlm.Services.Data;
using BExIS.Dlm.Entities.Data;
using BExIS.Xml.Helpers;
using F23.StringSimilarity;
using BExIS.Security.Services.Authorization;
using BExIS.Security.Entities.Authorization;

namespace BExIS.Aam.Services
{

    public class ItemEqualityComparer : IEqualityComparer<Aam_Dataset_column_annotation>
    {
        public bool Equals(Aam_Dataset_column_annotation x, Aam_Dataset_column_annotation y)
        {
            // Two items are equal if their keys are equal.
            return x.entity_id == y.entity_id;
        }

        public int GetHashCode(Aam_Dataset_column_annotation obj)
        {
            return obj.entity_id.GetHashCode();
        }
    }


    public class Aam_Dataset_column_annotationManager : IDisposable
    {

        private XmlDatasetHelper xmlDatasetHelper = new XmlDatasetHelper();


        private IUnitOfWork guow = null;
        private List<Aam_Dataset_column_annotation> AnnotationRepo;

        public Aam_Dataset_column_annotationManager()
        {
            using (IUnitOfWork uow = this.GetUnitOfWork())
            {
                var Aam_Dataset_column_annotationRepo = uow.GetReadOnlyRepository<Aam_Dataset_column_annotation>();

                // the requested version is earlier than the latest regardless of check-in/ out status or its the latest version and the dataset is checked in.
                AnnotationRepo = Aam_Dataset_column_annotationRepo.Get().ToList<Aam_Dataset_column_annotation>();
            }

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
            return (List<Aam_Dataset_column_annotation>)AnnotationRepo.Where(an => (an.variable_id != null ) && (an.variable_id.Label.ToLower().Equals(variableLabel.ToLower()))).ToList();

        }

        public List<Aam_Dataset_column_annotation> get_all_dataset_column_annotationBy_Variable_measures(String variableLabel)
        {
            List<Aam_Dataset_column_annotation> annotation_suggestion = new List<Aam_Dataset_column_annotation>();
            annotation_suggestion.AddRange((List<Aam_Dataset_column_annotation>)this.get_all_dataset_column_annotationByVariableLabel(variableLabel));
            annotation_suggestion.AddRange((List<Aam_Dataset_column_annotation>)this.get_all_dataset_column_annotationByVariable_DataStructure(variableLabel));
            annotation_suggestion.AddRange((List<Aam_Dataset_column_annotation>)this.get_all_dataset_column_annotationByVariable_Unit(variableLabel));
            return annotation_suggestion;
        }
        public List<Aam_Dataset_column_annotation> get_all_dataset_column_annotationByVariable_DataStructure(String variableLabel)
        {
            using (VariableManager vm = new VariableManager())
            {
                List<VariableInstance> variables = (List<VariableInstance>)vm.VariableInstanceRepo.Get().Where(x => x.Label.ToLower() == variableLabel.ToLower());
                List<Aam_Dataset_column_annotation> List = new List<Aam_Dataset_column_annotation>();
                foreach (VariableInstance v in variables)
                {
                    List<Aam_Dataset_column_annotation> l = (List<Aam_Dataset_column_annotation>)AnnotationRepo.Where(an => (an.variable_id != null) && v.DataStructure.Variables.Contains(an.variable_id)).ToList();
                    l.ForEach(x => List.Add(x));
                }
                return List;
            }
        }

        public List<Aam_Dataset_column_annotation> get_all_dataset_column_annotationByVariable_Unit(String variableLabel)
        {
            using (IUnitOfWork uow = this.GetUnitOfWork())
            {
                DataStructureManager dsm = new DataStructureManager();
                //IRepository<Aam_Dataset_column_annotation> repo = uow.GetRepository<Aam_Dataset_column_annotation>();
                List<Variable> variables = (List<Variable>)dsm.VariableRepo.Get().ToList<Variable>().Where(x => x.Label.ToLower() == variableLabel.ToLower()).ToList<Variable>();

                List<Aam_Dataset_column_annotation> List = new List<Aam_Dataset_column_annotation>();
                foreach (Variable v in variables)
                {
                    List<Aam_Dataset_column_annotation> l = (List<Aam_Dataset_column_annotation>)AnnotationRepo.Where(an => (an.variable_id != null) && (an.variable_id.Unit == v.Unit)).ToList();
                    l.ForEach(x => List.Add(x));
                }
                dsm.Dispose();
                return List;
            }
        }

        public Dictionary<Aam_Uri, double> get_all_dataset_column_annotationByVariable_label_matching(String variableLabel)
        {
            using (IUnitOfWork uow = this.GetUnitOfWork())
            {
                IRepository<Aam_Uri> repo = uow.GetRepository<Aam_Uri>();
                Dictionary<Aam_Uri, double> measures = new Dictionary<Aam_Uri, double>();
                foreach(Aam_Uri uri in repo.Get())
                {
                    double d = calculate_similarity(variableLabel.ToLower(), uri.label.ToLower());
                    List<Aam_Uri> l = new List<Aam_Uri>();
                    if ( d > 0.4)
                    {
                        measures.Add(uri, d);
                    }
                }
                return measures;
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

        private double Similarity(string a, string b)
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
