using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.IO;
using System.Diagnostics;
using BExIS.Dlm.Services.Data;
using BExIS.Dlm.Entities.Data;
using BExIS.Utils.Models;
using System.Text;
using Newtonsoft.Json;
using Telerik.Web.Mvc;
using System.Data;
using BExIS.Xml.Helpers;
using VDS.RDF;
using VDS.RDF.Query;
using Vaiona.Utils.Cfg;
using Vaiona.Persistence.Api;
using System.Xml;
using System.Configuration;
using BExIS.Modules.Ddm.UI.Helpers;
using BExIS.Aam.Services;
using BExIS.Aam.Entities.Mapping;
using System.Web.Configuration;
using Npgsql;

namespace BExIS.Modules.Ddm.UI.Controllers
{
    public class InteractiveSearchController : Controller
    {
        private XmlDatasetHelper xmlDatasetHelper = new XmlDatasetHelper();

        static string DatastructAPI = "http://localhost:5412/api/structures/";
        
        static string Conx = ConfigurationManager.ConnectionStrings[1].ConnectionString;
        static DataTable m = new DataTable();

        static List<HeaderItem> headerItems;
        static HeaderItem idHeader;

        static String ad_ontology_merged_owl = WebConfigurationManager.AppSettings["ad-ontology-merged.owl"];
        String ADOntologyPath = Path.Combine(AppConfiguration.GetModuleWorkspacePath("DDM"), "Semantic Search", "Ontologies", ad_ontology_merged_owl);

        static String Gps_coordinates_for_wells = Path.Combine(AppConfiguration.GetModuleWorkspacePath("DDM"), "Interactive Search", "D03_well coordinates_20180525.json");

        // start page - wellcome page for the interactive search 
        // checks if the request is coming from a refresh page or from another feature controller to reset the data table or keep the old search
        public ActionResult Index()
        {
            string referrer_URI = Request.UrlReferrer.AbsoluteUri;
            headerItems = makeHeader();
            if  ((m == null) || (referrer_URI.IndexOf("interactiveSearch") < 0))
            {
                m = CreateDataTable(headerItems);
            }
            return View("Index_",m);
        }

        // this action fills the data table after clicking on location name
        // the data table is static due to the custom binding for the data table view in the table_result.cshtml
        // after calling this method, the javascript fucntion will refresh the page so the binding of the data table will take effect
        //[HttpPost] ActionResult
        public Boolean fill_data_table_for_binding_from_Image(String well_name)
        {
            well_name = well_name.ToLower();
            List<String> dataset_Ids_results_for_data_table = new List<string>();

            List<OntologyNamePair> ontologies = new List<OntologyNamePair>();

            ontologies.Add(new OntologyNamePair(ADOntologyPath, "ADOntology"));

            String results_ = "";
            //Just for testing purposes
            StringBuilder sb = new StringBuilder();
            foreach (OntologyNamePair ontology in ontologies)
            {
                String ontologyPath = ontology.getPath();
                results_ = results_ + ontologyPath + "\n";
                //Load the ontology as a graph
                using (IGraph g = new Graph())
                {
                    g.LoadFromFile(ontologyPath);
                    SparqlParameterizedString queryString = new SparqlParameterizedString();
                    queryString.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));

                    //get all the classes annotated under the site entity which refers to location sites
                    queryString.CommandText = "SELECT ?subject ?object where { ?subject rdfs:subClassOf <http://purl.obolibrary.org/obo/BFO_0000029> } ";
                    // "SELECT ?subject ?object where { ?subject rdfs:subClassOf ?object } " ==> this returns all the classes and their subclasses
                    SparqlResultSet results = (SparqlResultSet)g.ExecuteQuery(queryString);
                    List<String> URI_classes = new List<string>();
                    URI_classes.Add("http://purl.obolibrary.org/obo/BFO_0000029");

                    foreach (SparqlResult res in results.Results)
                    {
                        URI_classes.Add(res["subject"].ToString());
                    }

                    foreach (String uri in URI_classes)
                    {
                        results_ = results_ + uri + "\n";
                        String uri_ = uri.Replace("/", " \\/ ");

                        using (Aam_Dataset_column_annotationManager aam_ = new Aam_Dataset_column_annotationManager())
                        {
                            List<Aam_Dataset_column_annotation> annotations = aam_.get_all_dataset_column_annotation().FindAll(x => x.entity_id.URI == uri);
                            foreach (Aam_Dataset_column_annotation aam in annotations)
                            {
                                results_ = results_ + aam.Dataset.Id + " -->" + aam.variable_id.Id + "\n";
                            }
                        }
                    }
                }
            }

            Debug.WriteLine("results after search : " + results_);

            //fill the results including the title of the datasets which can cover more results
            using (DatasetManager dsm_ = new DatasetManager())
            {
                Dictionary<Int64, XmlDocument> datasets_ids = dsm_.GetDatasetLatestMetadataVersions();
                foreach (KeyValuePair<Int64, XmlDocument> kvp in datasets_ids)
                {
                    try
                    {
                        string title = xmlDatasetHelper.GetInformationFromVersion(kvp.Key, NameAttributeValues.title);
                        string description = xmlDatasetHelper.GetInformationFromVersion(kvp.Key, NameAttributeValues.description);
                        if (title.Contains(well_name) || (description.Contains(well_name))
                            || (title.IndexOf(well_name, StringComparison.OrdinalIgnoreCase) != -1)
                            || (description.IndexOf(well_name, StringComparison.OrdinalIgnoreCase) != -1))
                        {
                            if (dataset_Ids_results_for_data_table.Find(x => x == kvp.Key.ToString()) == null)
                            {
                                dataset_Ids_results_for_data_table.Add(kvp.Key.ToString());
                            }
                        }
                    }
                    catch (Exception ex_)
                    {
                        Debug.WriteLine(ex_.ToString());
                    }
                }
            }

            //DataTable m;
            m = CreateDataTable(headerItems);
            if (dataset_Ids_results_for_data_table != null)
            {
                foreach (String dataset_id in dataset_Ids_results_for_data_table)
                {
                    DataRow row = m.NewRow();
                    row["ID"] = Int64.Parse(dataset_id);
                    //row["VersionID"] = Int64.Parse(r.versionno);

                    //Grab the Metadata of the current ID
                    long datasetID = long.Parse(dataset_id);
                    string description = "";
                    string title = "";
                    string owner = "";

                    Dataset dataset = null;
                    using (IUnitOfWork uow = this.GetUnitOfWork())
                    {
                        var datasetRepo = uow.GetReadOnlyRepository<Dataset>();
                        dataset = datasetRepo.Get(datasetID);
                    }

                    if (dataset != null)
                    {
                        //Grab the Metadata
                        XmlDatasetHelper helper = new XmlDatasetHelper();
                        description = helper.GetInformation(datasetID, NameAttributeValues.description);
                        title = helper.GetInformation(datasetID, NameAttributeValues.title);
                        owner = helper.GetInformation(datasetID, NameAttributeValues.owner);

                        row["Title"] = title;
                        row["Datasetdescription"] = description;
                        row["Owner"] = owner;

                        m.Rows.Add(row);
                    }
                }
            }
            ViewData["DefaultHeaderList"] = headerItems;
            ViewData["ID"] = idHeader;
            if (m.Rows.Count > 0)
            {
                return true;
            }
            else return false;
            //return View(m);
        }

        // this action is related to display the data table "m" in the table_result.cshtml
        [GridAction]
        public ActionResult _CustomBinding(GridCommand command)
        {
            if (m != null)
            {
                return View(new GridModel(m));
            }
            return View();
        }
        private DataTable CreateDataTable(List<HeaderItem> items)
        {
            DataTable table = new DataTable();
            foreach (HeaderItem item in items)
            {
                table.Columns.Add(new DataColumn()
                {
                    ColumnName = item.Name,
                    Caption = item.DisplayName,
                    DataType = getDataType(item.DataType)
                });
            }
            table.PrimaryKey = new DataColumn[] { table.Columns["ID"] };

            return table;
        }

        // this method parses the JSON file containing the well names and their coordinates to get the well name from the coordinates.
        // It is made due to the fact that the leaflet.js map view return only the coordinates and a reason to fetch the well name from coordinates is needed
        public String parse_Json_location(String location_coordinates)
        {
            //"LatLng(51.080258, 10.42626)"
            using (StreamReader r = new StreamReader(Gps_coordinates_for_wells))
            {
                string json = r.ReadToEnd();
                List<coordinates_GPS> items = JsonConvert.DeserializeObject<List<coordinates_GPS>>(json);
                if (location_coordinates.Length > 0)
                {
                    string lon = location_coordinates.Substring(location_coordinates.IndexOf('(') + 1, location_coordinates.IndexOf(',') - location_coordinates.IndexOf('(') - 1);
                    string lat = location_coordinates.Substring(location_coordinates.IndexOf(", ") + 2, location_coordinates.IndexOf(')') - location_coordinates.IndexOf(',') - 2);

                    foreach (coordinates_GPS item in items)
                    {
                        try
                        {
                            if ((item.Lat.ToString().IndexOf(lon.Substring(0, lon.Length - 1)) > -1) && (item.Lon.ToString().IndexOf(lat.Substring(0, lat.Length - 1)) > -1))
                            {
                                return item.Well_name;
                            }
                        }
                        catch (NullReferenceException e)
                        {
                            Debug.WriteLine(e.ToString());
                        }

                    }
                }
                else
                {
                    return json;
                }
            }
            return "";
        }

        // this class is made for the Deserialization of the JSON object of the JSON file containing the coordinates and well names.
        public class coordinates_GPS
        {
            public string Well_name;
            public string Lat;
            public string Lon;
        }

        // creating the header of the data table 
        private List<HeaderItem> makeHeader()
        {
            headerItems = new List<HeaderItem>();

            HeaderItem headerItem = new HeaderItem()
            {
                Name = "ID",
                DisplayName = "ID",
                DataType = "Int64"
            };
            headerItems.Add(headerItem);

            idHeader = headerItem;
            ViewData["ID"] = headerItem;

            headerItem = new HeaderItem()
            {
                Name = "Title",
                DisplayName = "Title",
                DataType = "String"
            };
            headerItems.Add(headerItem);

            headerItem = new HeaderItem()
            {
                Name = "Owner",
                DisplayName = "Owner",
                DataType = "String"
            };
            headerItems.Add(headerItem);

            headerItem = new HeaderItem()
            {
                Name = "Datasetdescription",
                DisplayName = "Description",
                DataType = "String"
            };
            headerItems.Add(headerItem);

            ViewData["DefaultHeaderList"] = headerItems;

            return headerItems;
        }

        // filling the data type of the data table columns
        private Type getDataType(string dataType)
        {
            switch (dataType)
            {
                case "String":
                    {
                        return Type.GetType("System.String");
                    }

                case "Double":
                    {
                        return Type.GetType("System.Double");
                    }

                case "Int16":
                    {
                        return Type.GetType("System.Int16");
                    }

                case "Int32":
                    {
                        return Type.GetType("System.Int32");
                    }

                case "Int64":
                    {
                        return Type.GetType("System.Int64");
                    }

                case "Decimal":
                    {
                        return Type.GetType("System.Decimal");
                    }

                case "DateTime":
                    {
                        return Type.GetType("System.DateTime");
                    }

                default:
                    {
                        return Type.GetType("System.String");
                    }
            }
        }

        // this action fills the data table after clicking on location name
        // the data table is static due to the custom binding for the data table view in the table_result.cshtml
        // after calling this method, the javascript fucntion will refresh the page so the binding of the data table will take effect
        //[HttpPost] ActionResult
        public Boolean fill_data_table_for_binding(String location_coordinates)
        {
            string well_name = parse_Json_location(location_coordinates);
            List<String> dataset_Ids_results_for_data_table = new List<string>();

            List<OntologyNamePair> ontologies = new List<OntologyNamePair>();

            ontologies.Add(new OntologyNamePair(ADOntologyPath, "ADOntology"));

            String results_ = "";
            //Just for testing purposes
            StringBuilder sb = new StringBuilder();
            foreach (OntologyNamePair ontology in ontologies)
            {
                String ontologyPath = ontology.getPath();
                results_ = results_ + ontologyPath + "\n";
                //Load the ontology as a graph
                using (IGraph g = new Graph())
                {
                    g.LoadFromFile(ontologyPath);
                    SparqlParameterizedString queryString = new SparqlParameterizedString();
                    queryString.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));

                    //get all the classes annotated under the site entity which refers to location sites
                    queryString.CommandText = "SELECT ?subject ?object where { ?subject rdfs:subClassOf <http://purl.obolibrary.org/obo/BFO_0000029> } ";
                    // "SELECT ?subject ?object where { ?subject rdfs:subClassOf ?object } " ==> this returns all the classes and their subclasses
                    SparqlResultSet results = (SparqlResultSet)g.ExecuteQuery(queryString);
                    List<String> URI_classes = new List<string>();
                    URI_classes.Add("http://purl.obolibrary.org/obo/BFO_0000029");

                    foreach (SparqlResult res in results.Results)
                    {
                        URI_classes.Add(res["subject"].ToString());
                    }

                    NpgsqlCommand MyCmd = null;
                    NpgsqlConnection MyCnx = null;


                    foreach (String uri in URI_classes)
                    {
                        results_ = results_ + uri + "\n";
                        MyCnx = new NpgsqlConnection(Conx);
                        MyCnx.Open();
                        String uri_ = uri.Replace("/", " \\/ ");
                        string select = "SELECT datasets_id, variable_id, version_id FROM dataset_column_annotation WHERE entity= \'" + @uri + "\'";
                        using (MyCmd = new NpgsqlCommand(select, MyCnx))
                        {
                            NpgsqlDataReader dr = MyCmd.ExecuteReader();
                            if (dr != null)
                            {
                                while (dr.Read())
                                {
                                    if (dr["datasets_id"] != System.DBNull.Value)
                                    {
                                        var datasets_id = dr["datasets_id"].ToString();
                                        var variable_id = dr["variable_id"].ToString();
                                        results_ = results_ + datasets_id + " -->" + variable_id + "\n";
                                        Debug.WriteLine(datasets_id + " --->");

                                        using (DatasetManager dsm = new DatasetManager())
                                        {
                                            try
                                            {
                                                /*
                                                DatasetVersion dsv = dsm.GetDatasetLatestVersion(Int64.Parse(datasets_id));
                                                List<AbstractTuple> ds_tuples = dsm.GetDatasetVersionEffectiveTuples(dsv);
                                                foreach (AbstractTuple tuple in ds_tuples)
                                                {
                                                    XmlDocument xml = tuple.JsonVariableValues;

                                                    XmlNodeList item_List = xml.GetElementsByTagName("Item");//containing the tag <Item> to be parsed one by one
                                                    foreach (XmlNode item in item_List)
                                                    {
                                                        XmlNodeList childnodes = item.ChildNodes;//containing the tag <Property> to be parsed one by one
                                                        foreach (XmlNode childnode in childnodes)
                                                        {
                                                            if (childnode.Attributes[0].Value == "VariableId")
                                                            {
                                                                if (childnode.Attributes[2].Value == variable_id.ToSafeString())
                                                                {
                                                                    String Data_Value = childnodes[2].Attributes[2].Value;
                                                                    if (well_name.ToLower().IndexOf(Data_Value.ToLower()) > -1)
                                                                    {
                                                                        //Debug.WriteLine(childnode.Attributes[2].Value);
                                                                        //Debug.WriteLine(Data_Value);
                                                                        results_ = results_ + Data_Value + "\n";
                                                                        if (dataset_Ids_results_for_data_table.Find(x => x == datasets_id) == null)
                                                                        {
                                                                            dataset_Ids_results_for_data_table.Add(datasets_id);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                */
                                            }
                                            catch (Exception ex)
                                            {
                                                //throw ex;
                                            }
                                        }

                                    }
                                }
                            }
                        }
                        MyCnx.Close();
                    }
                }
            }
            Debug.WriteLine("results after search : " + results_);

            //fill the results including the title of the datasets which can cover more results
            using (DatasetManager dsm_ = new DatasetManager())
            {
                Dictionary<Int64, XmlDocument> datasets_ids = dsm_.GetDatasetLatestMetadataVersions();
                foreach (KeyValuePair<Int64, XmlDocument> kvp in datasets_ids)
                {
                    try
                    {
                        string title = xmlDatasetHelper.GetInformationFromVersion(kvp.Key, NameAttributeValues.title);
                        string description = xmlDatasetHelper.GetInformationFromVersion(kvp.Key, NameAttributeValues.description);
                        if (title.Contains(well_name))
                        {
                            if (dataset_Ids_results_for_data_table.Find(x => x == kvp.Key.ToString()) == null)
                            {
                                dataset_Ids_results_for_data_table.Add(kvp.Key.ToString());
                            }
                        }
                    }
                    catch (Exception ex_)
                    {
                        Debug.WriteLine(ex_.ToString());
                    }
                }
            }

            //DataTable m;
            m = CreateDataTable(headerItems);
            if (dataset_Ids_results_for_data_table != null)
            {
                foreach (String dataset_id in dataset_Ids_results_for_data_table)
                {
                    DataRow row = m.NewRow();
                    row["ID"] = Int64.Parse(dataset_id);
                    //row["VersionID"] = Int64.Parse(r.versionno);

                    //Grab the Metadata of the current ID
                    long datasetID = long.Parse(dataset_id);
                    string description = "";
                    string title = "";
                    string owner = "";

                    Dataset dataset = null;
                    using (IUnitOfWork uow = this.GetUnitOfWork())
                    {
                        var datasetRepo = uow.GetReadOnlyRepository<Dataset>();
                        dataset = datasetRepo.Get(datasetID);
                    }

                    if (dataset != null)
                    {
                        //Grab the Metadata
                        XmlDatasetHelper helper = new XmlDatasetHelper();
                        description = helper.GetInformation(datasetID, NameAttributeValues.description);
                        title = helper.GetInformation(datasetID, NameAttributeValues.title);
                        //owner = helper.GetInformation(datasetID, NameAttributeValues.owner);

                        row["Title"] = title;
                        row["Datasetdescription"] = description;
                        row["Ownername"] = owner;

                        m.Rows.Add(row);
                    }
                }
            }
            ViewData["DefaultHeaderList"] = headerItems;
            ViewData["ID"] = idHeader;
            if (m.Rows.Count > 0)
            {
                return true;
            }
            else return false;
            //return View(m);
        }
    }
}