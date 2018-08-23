﻿using System;
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
using BExIS.Modules.Ddm.UI.Helpers;
using Vaiona.Utils.Cfg;
using Vaiona.Persistence.Api;
using Npgsql;
using System.Xml;
using System.Configuration;
using BExIS.Dlm.Services.DataStructure;
using BExIS.Dlm.Entities.DataStructure;
using System.Net.Http;
using System.Net.Sockets;
using System.Web;
using Newtonsoft.Json.Linq;
using BExIS.IO.Transform.Output;
using System.IO;
using Vaiona.Logging;

namespace BExIS.Modules.Ddm.UI.Controllers
{
    public class InteractiveSearchController : Controller
    {
        static string DatastructAPI = "http://localhost:5412/api/structures/";
        static List<Variable_analytics> VA_list;



        static string Conx = ConfigurationManager.ConnectionStrings[1].ConnectionString;
        static DataTable m;

        static List<HeaderItem> headerItems;
        static HeaderItem idHeader;
        
        static String mappingDictionaryFilePath = Path.Combine(AppConfiguration.GetModuleWorkspacePath("DDM"), "Semantic Search", "mappings.txt");
        static String autocompletionFilePath = Path.Combine(AppConfiguration.GetModuleWorkspacePath("DDM"), "Semantic Search", "autocompletion.txt");
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
            return View(m);
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

            String path = Path.Combine(AppConfiguration.GetModuleWorkspacePath("DDM"), "Semantic Search", "Ontologies", "ad-ontology-merged.owl");
            ontologies.Add(new OntologyNamePair(path, "ADOntology"));

            String results_ = "";
            //Just for testing purposes
            StringBuilder sb = new StringBuilder();
            foreach (OntologyNamePair ontology in ontologies)
            {
                String ontologyPath = ontology.getPath();
                results_ = results_ + ontologyPath + "\n";
                //Load the ontology as a graph
                IGraph g = new Graph();
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
                    MyCmd = new NpgsqlCommand(select, MyCnx);

                    NpgsqlDataReader dr = MyCmd.ExecuteReader();
                    if (dr != null)
                    {
                        while (dr.Read())
                        {
                            if (dr["datasets_id"] != System.DBNull.Value)
                            {
                                var datasets_id = dr["datasets_id"].ToSafeString();
                                var variable_id = dr["variable_id"].ToSafeString();
                                results_ = results_ + datasets_id + " -->" + variable_id + "\n";
                                Debug.WriteLine(datasets_id + " --->");

                                DatasetManager dsm = new DatasetManager();
                                try { 
                                    DatasetVersion dsv = dsm.GetDatasetLatestVersion(Int64.Parse(datasets_id));
                                    List<AbstractTuple> ds_tuples = dsm.GetDatasetVersionEffectiveTuples(dsv);
                                    foreach (AbstractTuple tuple in ds_tuples)
                                    {
                                        XmlDocument xml = tuple.XmlVariableValues;

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
                                }
                                catch (Exception ex)
                                {
                                    //throw ex;
                                }
                            }
                        }
                    }
                    MyCnx.Close();
                }
            }
            Debug.WriteLine("results after search : " + results_);

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
                            if ((item.Lat.ToSafeString().IndexOf(lon.Substring(0, lon.Length - 1)) > -1) && (item.Lon.ToSafeString().IndexOf(lat.Substring(0, lat.Length - 1)) > -1))
                            {
                                return item.Well_name;
                            }
                        }
                        catch (NullReferenceException e)
                        {
                            Debug.WriteLine(e.ToSafeString());
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
                Name = "Ownername",
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

        
        #region statistics
        /* this action reveals a semantic coverage for our data portal and needs to be accessed via URL ... no button for it ...
        */
        public ActionResult analytics()
    {
            DatasetManager DM = new DatasetManager();
            List <long> ds_ids = DM.GetDatasetLatestIds();
            DataStructureManager DStructM = new DataStructureManager();
            List <DataStructure> dataStructs = (List<DataStructure>) DStructM.AllTypesDataStructureRepo.Get();

            VA_list = new List<Variable_analytics>();

            foreach (long id in ds_ids)
            {

                List<string> variable_id = new List<string>();
                List<string> variable_label = new List<string>();
                List<string> dataType = new List<string>();
                List<string> unit = new List<string>();
                List<string> variable_concept_entity = new List<string>();
                List<string> variable_concept_caracteristic = new List<string>();
                

                //Construct a HttpClient for the search-Server
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(DatastructAPI);
                //Set the searchTerm as query-String
                String param = HttpUtility.UrlEncode(id.ToSafeString());
                 
                try
                {
                    HttpResponseMessage response = client.GetAsync(param).Result;  
                    if (response.IsSuccessStatusCode)
                    {
                        JObject json_ds_struct = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                        JArray json_variables = JArray.Parse(json_ds_struct["Variables"].ToString());

                        foreach (JObject json_variable in json_variables)
                        {
                            variable_id.Add(json_variable["Id"].ToSafeString());
                            variable_label.Add(json_variable["Label"].ToSafeString());
                            dataType.Add(json_variable["DataType"].ToSafeString());
                            unit.Add(json_variable["Unit"].ToSafeString());
                            
                            string select = "SELECT datasets_id, variable_id, entity , characteristic FROM dataset_column_annotation WHERE datasets_id= \'" +
                                id + "\' AND variable_id = \'" + json_variable["Id"].ToSafeString() + "\'";
                            NpgsqlCommand MyCmd = null;
                            NpgsqlConnection MyCnx = null;
                            MyCnx = new NpgsqlConnection(Conx);
                            MyCnx.Open();
                            MyCmd = new NpgsqlCommand(select, MyCnx);

                            NpgsqlDataReader dr = MyCmd.ExecuteReader();
                            if (dr != null)
                            {
                                while (dr.Read())
                                {
                                    if ((dr["datasets_id"] != System.DBNull.Value) && ((dr["variable_id"] != System.DBNull.Value)))
                                    {
                                        if (dr["entity"].ToSafeString() == "")
                                        {
                                            variable_concept_entity.Add("No Annotation");
                                        }
                                        else
                                        {
                                            variable_concept_entity.Add(dr["entity"].ToSafeString());
                                        }

                                        if (dr["characteristic"].ToSafeString() == "")
                                        {
                                            variable_concept_caracteristic.Add("No Annotation");
                                        }
                                        else
                                        {
                                            variable_concept_caracteristic.Add(dr["characteristic"].ToSafeString());
                                        }
                                    }
                                }
                                if (dr.HasRows == false)
                                {
                                    variable_concept_entity.Add("No Annotation");
                                    variable_concept_caracteristic.Add("No Annotation");
                                }
                            }
                            MyCnx.Close();
                        }
                        // create a new instance of variable analytics
                        Variable_analytics VA = new Variable_analytics(id, variable_id, variable_label, variable_concept_entity, variable_concept_caracteristic, dataType, unit);
                        VA_list.Add(VA);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToSafeString());
                }
            }
            ViewData["VA_list"] = VA_list;
            return View(VA_list);
        }
        
        public ActionResult Download_Report()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("dataset_id ,Semantic Coverage, variable_id,variable_label,variable_concept_entity,variable_concept_caracteristic,dataType,unit");
            foreach (var va in VA_list)
            {
                if (va.variable_id.Count > 0)
                {
                    int Concepts_count = 0;
                    int Caracteristics_count = 0;


                    foreach (String s in va.variable_concept_entity)
                    {
                        if (s != "No Annotation")
                        {
                            Concepts_count = Concepts_count + 1;
                        }
                    }
                    foreach (String s in va.variable_concept_caracteristic)
                    {
                        if (s != "No Annotation")
                        {
                            Caracteristics_count = Caracteristics_count + 1;
                        }
                    }
                    sb.AppendLine(
                        va.dataset_id.ToSafeString() +","+
                        Concepts_count.ToSafeString()+"/"+va.variable_id.Count.ToSafeString() + "," +
                        va.variable_id[0].ToSafeString() + "," +
                        va.variable_label[0]+","+
                        va.variable_concept_entity[0] + "," +
                        va.variable_concept_caracteristic[0] + "," +
                        va.dataType[0] + "," +
                        va.unit[0]);

                    for (int kk = 1; kk < va.variable_id.Count; kk++)
                    {
                        sb.AppendLine( 
                            ", ,"+
                            va.variable_id[kk].ToSafeString() + "," +
                            va.variable_label[kk].ToSafeString() + "," +
                            va.variable_concept_entity[kk].ToSafeString() + "," +
                            va.variable_concept_caracteristic[kk].ToSafeString() + "," +
                            va.unit[kk].ToSafeString() + "," +
                            va.dataType[kk].ToSafeString());
                    }
                }
            }
            // save the string builder sb and ask for download 

            Console.WriteLine(sb.ToString());
            System.IO.File.WriteAllText(
                System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "REPORT.csv"),
                sb.ToString());
            
            return File(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "REPORT.csv"), "text/csv", "REPORT.csv");
        }
    }

    public class Variable_analytics
    {
        public long dataset_id;
        public List<string> variable_id;
        public List<string> variable_label;
        public List<string> variable_concept_entity;
        public List<string> variable_concept_caracteristic;
        public List<string> dataType;
        public List<string> unit;

        public Variable_analytics(long dataset_id, List<string> variable_id, List<string> variable_label, List<string> variable_concept_entity, List<string> variable_concept_caracteristic, List<string> dataType, List<string> unit)
        {
            this.dataset_id = dataset_id;
            this.variable_id = variable_id;
            this.variable_label = variable_label;
            this.variable_concept_entity = variable_concept_entity;
            this.variable_concept_caracteristic = variable_concept_caracteristic;
            this.dataType = dataType;
            this.unit = unit;
        }
    }
    
    #endregion statistics
}