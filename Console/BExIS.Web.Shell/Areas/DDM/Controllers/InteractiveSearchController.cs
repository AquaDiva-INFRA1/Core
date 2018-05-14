using BExIS.Dlm.Services.Data;
using BExIS.Modules.Ddm.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Vaiona.Utils.Cfg;
using VDS.RDF;
using VDS.RDF.Query;

namespace BExIS.Modules.Ddm.UI.Controllers
{
    public class InteractiveSearchController : Controller
    {
        static Dictionary<String, List<OntologyMapping>> mappingDic;

        public void Index ()
        {
            DatasetManager DsM = new DatasetManager();
            Dictionary<Int64, XmlDocument> All_DataSets_Metadata = DsM.GetDatasetLatestMetadataVersions();
            Get_URI_from_term("");
            /*
            foreach(KeyValuePair<Int64,XmlDocument> dataset_metadata in All_DataSets_Metadata)
            {
                Debug.WriteLine("Dataset id : " + dataset_metadata.Key.ToString());
                Debug.WriteLine("Metadata file : " + dataset_metadata.Value.ToString());
            }
            */
        }


        public void Get_URI_from_term(String term)
        {
            string entity = "";

            SparqlParameterizedString queryString = new SparqlParameterizedString();

            queryString.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            queryString.Namespaces.AddNamespace("owl", new Uri("http://www.w3.org/2002/07/owl#"));
            queryString.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            //queryString.Namespaces.AddNamespace("entity", new Uri(entity));

            queryString.CommandText = "SELECT ?s WHERE" +
                " { " +
                "?a rdf:about ?s " +
                "} ";

            IGraph g = new Graph();
            g.LoadFromFile(Path.Combine(AppConfiguration.GetModuleWorkspacePath("DCM"), "Semantic Search", "Ontologies", "ad-ontology-merged.owl"));
            SparqlResultSet results = (SparqlResultSet)g.ExecuteQuery(queryString);
            String res = "";
            if (results.Count != 0)
            {
                res = results[0]["label"].ToString().Split('^')[0];
            }
            Debug.WriteLine("results : ==> "+res);
        }
    }
}
