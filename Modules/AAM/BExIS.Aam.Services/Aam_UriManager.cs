using System;
using System.Collections.Generic;
using System.Linq;
using Vaiona.Persistence.Api;
using BExIS.Aam.Entities.Mapping;
using System.Diagnostics.Contracts;
using BExIS.Dlm.Entities.DataStructure;
using VDS.RDF;
using VDS.RDF.Query;
using System.Diagnostics;

namespace BExIS.Aam.Services
{
    public class Aam_UriManager : IDisposable
    {
        private IUnitOfWork guow = null;
        private IRepository<Aam_Uri> AnnotationRepo;

        public Aam_UriManager()
        {

        }

        public Aam_Uri creeate_Aam_Uri(Aam_Uri an)
        {
            using (IUnitOfWork uow = this.GetUnitOfWork())
            {
                IRepository<Aam_Uri> repo = uow.GetRepository<Aam_Uri>();

                repo.Put(an);
                uow.Commit();
                // This creates the MV when there is no data tuples attached to the dataset, hence faster.
                // However, it asks for REFRESH to tell the underlying database to mark the MV as queryable.
                //updateMaterializedView(dataset.Id, ViewCreationBehavior.Create | ViewCreationBehavior.Refresh);
                return (an);
            }
        }

        public Boolean delete_Aam_Uri(Aam_Uri an)
        {
            try
            {
                using (IUnitOfWork uow = this.GetUnitOfWork())
                {
                    IRepository<Aam_Uri> repo = uow.GetRepository<Aam_Uri>();

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

        public Aam_Uri edit_Aam_Uri(Aam_Uri an)
        {
            try
            {
                Contract.Requires(an != null);
                Contract.Requires(an.Id >= 0);

                Contract.Ensures(Contract.Result<Aam_Uri>() != null && Contract.Result<Aam_Uri>().Id >= 0);

                using (IUnitOfWork uow = this.GetUnitOfWork())
                {
                    IRepository<Aam_Uri> repo = uow.GetRepository<Aam_Uri>();
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

        public Aam_Uri get_Aam_Uri_by_id(long id)
        {
            try
            {
                using (IUnitOfWork uow = this.GetUnitOfWork())
                {
                    var Aam_UriRepo = uow.GetReadOnlyRepository<Aam_Uri>();

                    // the requested version is earlier than the latest regardless of check-in/ out status or its the latest version and the dataset is checked in.
                    Aam_Uri an = Aam_UriRepo.Query(p => p.Id == id).FirstOrDefault();
                    return (an);
                }
            }
            catch (Exception)
            {
                return new Aam_Uri();
            }
        }

        public Aam_Uri get_Aam_Uri_by_uri(string uri)
        {
            try
            {
                using (IUnitOfWork uow = this.GetUnitOfWork())
                {
                    var Aam_UriRepo = uow.GetReadOnlyRepository<Aam_Uri>();

                    // the requested version is earlier than the latest regardless of check-in/ out status or its the latest version and the dataset is checked in.
                    Aam_Uri an = Aam_UriRepo.Query(p => p.URI == uri).FirstOrDefault();
                    return (an);
                }
            }
            catch (Exception)
            {
                return new Aam_Uri();
            }
        }


        public List<Aam_Uri> get_all_Aam_Uri()
        {
            try
            {
                using (IUnitOfWork uow = this.GetUnitOfWork())
                {
                    var datasetVersionRepo = uow.GetReadOnlyRepository<Aam_Uri>();

                    List<Aam_Uri> q1 = datasetVersionRepo.Query().ToList<Aam_Uri>();
                    return (q1);
                }
            }
            catch (Exception)
            {
                return new List<Aam_Uri>();
            }
        }

        public List<Aam_Uri> get_all_Aam_Uri_by_type(string type)
        {
            try
            {
                using (IUnitOfWork uow = this.GetUnitOfWork())
                {
                    var datasetVersionRepo = uow.GetReadOnlyRepository<Aam_Uri>();

                    List<Aam_Uri> q1 = datasetVersionRepo.Query().ToList<Aam_Uri>().Where(x=> x.type_uri == type).ToList<Aam_Uri>();
                    return (q1);
                }
            }
            catch (Exception)
            {
                return new List<Aam_Uri>();
            }
        }


        public Boolean fill_onto_from_file(string ADOntologyPath)
        {
            try { 
                Aam_UriManager uri_ma = new Aam_UriManager();

                //Load the ontology as a graph
                IGraph g = new Graph();
                g.LoadFromFile(ADOntologyPath);
                //Create a new queryString
                SparqlParameterizedString queryString = new SparqlParameterizedString();
                //Add some important namespaces
                //Add some important namespaces
                queryString.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
                queryString.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
                queryString.Namespaces.AddNamespace("owl", new Uri("http://www.w3.org/2002/07/owl#"));
                queryString.Namespaces.AddNamespace("oboe.1.0", new Uri("http://ecoinformatics.org/oboe/oboe.1.2/oboe-core.owl#"));
                queryString.Namespaces.AddNamespace("oboe.1.2", new Uri("http://ecoinformatics.org/oboe/oboe.1.2/oboe-core.owl#"));

                #region Characteristics
                //Grab all subclasses (transitively) of oboe-core:Characteristic
                queryString.CommandText =
                    "SELECT DISTINCT ?s ?label WHERE " +
                    "{?s rdfs:subClassOf* oboe.1.2:Characteristic ." +
                    "?s rdfs:label ?label . }";

                //Debugging output
                //Console.WriteLine(queryString.ToString());

                //Execute the query & Insert results in Dictionary with ConceptGroup "Characteristic"
                SparqlResultSet results = (SparqlResultSet)g.ExecuteQuery(queryString);

                foreach (SparqlResult res in results.Results)
                {
                    string uri = new Uri(res["s"].ToString()).ToString();
                    String label = res["label"].ToString();
                    if (uri.Contains("^^"))
                    {
                        label = label.Split(new String[] { "^^" }, StringSplitOptions.None)[0];
                    }
                    if (label.Contains("@"))
                    {
                        label = label.Split('@')[0];
                    }
                    if (uri_ma.AnnotationRepo.Get(x => x.URI == uri).Count == 0)
                        uri_ma.creeate_Aam_Uri(new Aam_Uri(uri, label, "Charachteristic"));
                }
                #endregion

                #region entities
                //Grab all subclasses (transitively) of oboe-core:Characteristic
                queryString.CommandText =
                        "SELECT DISTINCT ?s ?label WHERE " +
                        "{?s rdfs:subClassOf* oboe.1.2:Entity ." +
                        "?s rdfs:label ?label . }";

                //Debugging output
                //Console.WriteLine(queryString.ToString());

                //Execute the query & Insert results in Dictionary with ConceptGroup "Characteristic"
                results = (SparqlResultSet)g.ExecuteQuery(queryString);

                foreach (SparqlResult res in results.Results)
                {
                    string uri = new Uri(res["s"].ToString()).ToString();
                    String label = res["label"].ToString();
                    if (uri.Contains("^^"))
                    {
                        label = label.Split(new String[] { "^^" }, StringSplitOptions.None)[0];
                    }
                    if (label.Contains("@"))
                    {
                        label = label.Split('@')[0];
                    }
                    if (uri_ma.AnnotationRepo.Get(x => x.URI == uri).Count == 0)
                        uri_ma.creeate_Aam_Uri(new Aam_Uri(uri, label, "Entity"));
                }
                #endregion

                #region standards
                //Grab all subclasses (transitively) of oboe-core:Characteristic
                queryString.CommandText =
                        "SELECT DISTINCT ?s ?label WHERE " +
                        "{?s rdfs:subClassOf* oboe.1.2:Standard ." +
                        "?s rdfs:label ?label . }";

                //Debugging output
                //Console.WriteLine(queryString.ToString());

                //Execute the query & Insert results in Dictionary with ConceptGroup "Characteristic"
                results = (SparqlResultSet)g.ExecuteQuery(queryString);

                foreach (SparqlResult res in results.Results)
                {
                    string uri = new Uri(res["s"].ToString()).ToString();
                    String label = res["label"].ToString();
                    if (uri.Contains("^^"))
                    {
                        label = label.Split(new String[] { "^^" }, StringSplitOptions.None)[0];
                    }
                    if (label.Contains("@"))
                    {
                        label = label.Split('@')[0];
                    }
                    if (uri_ma.AnnotationRepo.Get(x => x.URI == uri).Count == 0)
                        uri_ma.creeate_Aam_Uri(new Aam_Uri(uri, label, "Standard"));
                }
                #endregion
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }









        #region IDisposable implementation
        private bool isDisposed = false;
        ~Aam_UriManager()
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
