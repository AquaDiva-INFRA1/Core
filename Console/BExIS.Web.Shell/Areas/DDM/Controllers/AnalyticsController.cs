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
    public class AnalyticsController : Controller
    {
        static string DatastructAPI = "http://localhost:5412/api/structures/";
        static List<Variable_analytics> VA_list;
        
        static string Conx = ConfigurationManager.ConnectionStrings[1].ConnectionString;
        
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
}