using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using BExIS.Dlm.Services.Data;
using System.Xml;
using System.Net.Http;
using BExIS.Aam.Entities.Mapping;
using BExIS.Aam.Services;
using System.Dynamic;
using System.IO;
using Vaiona.Utils.Cfg;
using BExIS.Dlm.Services.DataStructure;
using BExIS.Dlm.Entities.DataStructure;
using Vaiona.Persistence.Api;
using System.Diagnostics;
using System.Threading.Tasks;
using BExIS.Dlm.Entities.Data;
using Vaiona.Logging;
using System.Web.Configuration;

namespace BExIS.ASM.Services
{
    public class StatisticsExtractor : IStatisticsExtractor
    {
        
        static Aam_Dataset_column_annotationManager aam_manager = new Aam_Dataset_column_annotationManager();
        static DatasetManager dm = new DatasetManager();
        static DataStructureManager dataStructureManager = new DataStructureManager();

        static List<Aam_Dataset_column_annotation> aam_list = aam_manager.get_all_dataset_column_annotation();
        static List<StructuredDataStructure> structureRepo = dataStructureManager.GetUnitOfWork().GetReadOnlyRepository<StructuredDataStructure>().Get().ToList<StructuredDataStructure>();
        static Dictionary<string, string> stats_extra = new Dictionary<string, string>();

        public List<VariableTemplate> DataAttributeStruct_list_in_use = new List<VariableTemplate>();
        public List<VariableTemplate> DataAttributeStruct_list_non_use = new List<VariableTemplate>();

        public List<Unit> EditUnitModel_list_in_use = new List<Unit>();
        public List<Unit> EditUnitModel_list_non_use = new List<Unit>();

        public List<DataType> DataType_in_use = new List<DataType>();
        public List<DataType> DataType_non_use = new List<DataType>();

        public List<StructuredDataStructure> DataStruct_in_use = new List<StructuredDataStructure>();
        public List<StructuredDataStructure> DataStruc_non_use = new List<StructuredDataStructure>();
        private readonly string temp_file = Path.Combine(AppConfiguration.GetModuleWorkspacePath("ASM"), "Analytics_temp.txt");

        static string DatastructAPI = WebConfigurationManager.AppSettings["BaseAdress"] + "/api/structures/";


        public StatisticsExtractor()
        {
            JObject stats_obj = JObject.Parse(System.IO.File.ReadAllText(temp_file));
            string datasetCount = stats_obj["dataset_count"].ToString();
            string Datapoints = stats_obj["datapoints"].ToString();

            int counting = 0;
            foreach (StructuredDataStructure x in structureRepo)
            {
                counting = counting + x.Variables.Count;
            }
            stats_extra.Clear();
            stats_extra.Add("dataset_count", datasetCount);
            stats_extra.Add("datapoints", Datapoints);
            stats_extra.Add("counting", counting.ToString());
            this.fill_lists();
        }

        public JObject get_extra_stats()
        {
            string json_string = JsonConvert.SerializeObject(stats_extra);
            return JObject.Parse(json_string);
        }

        public JObject reset()
        {
            Task.Run(() => this.refresh_stats_async());

            aam_list = aam_manager.get_all_dataset_column_annotation();

            String temp_file = Path.Combine(AppConfiguration.GetModuleWorkspacePath("ASM"), "Analytics_temp.txt");
            JObject stats_obj = JObject.Parse(System.IO.File.ReadAllText(temp_file));
            string datasetCount = stats_obj["dataset_count"].ToString();
            string Datapoints = stats_obj["datapoints"].ToString();

            List<StructuredDataStructure> structureRepo = dataStructureManager.GetUnitOfWork().GetReadOnlyRepository<StructuredDataStructure>().Get().ToList<StructuredDataStructure>();
            int counting = 0;
            foreach (StructuredDataStructure x in structureRepo)
            {
                counting = counting + x.Variables.Count;
            }

            stats_extra = new Dictionary<string, string>();
            stats_extra.Add("dataset_count", datasetCount);
            stats_extra.Add("datapoints", Datapoints);
            stats_extra.Add("counting", counting.ToString());

            this.fill_lists();

            Dictionary<string, string> res = new Dictionary<string, string>();
            res.Add("status", "true");
            string json_string = JsonConvert.SerializeObject(res);
            return JObject.Parse(json_string);
        }
        private async Task<Boolean> refresh_stats_async()
        {
            List<Dataset> datasets = new List<Dataset>();
            List<long> datasetIds = new List<long>();
            datasets = dm.DatasetRepo.Query().OrderBy(p => p.Id).ToList();
            datasetIds = datasets.Select(p => p.Id).ToList();
            long somme = 0;
            foreach (Dataset ds in datasets)
            {
                long noColumns = ds.DataStructure.Self is StructuredDataStructure ? (ds.DataStructure.Self as StructuredDataStructure).Variables.Count : 0L;
                long noRows = ds.DataStructure.Self is StructuredDataStructure ? dm.GetDatasetLatestVersionEffectiveTupleCount(ds) : 0; // It would save time to calc the row count for all the datasets at once!
                if (ds.Status == DatasetStatus.CheckedIn)
                {
                    somme = somme + (noRows * noColumns);
                }
            }
            dm.Dispose();
            string json = "{\"dataset_count\" : " + datasets.Count + " , \"datapoints\" : " + somme + " }";
            using (StreamWriter sw = System.IO.File.CreateText(temp_file))
            {
                sw.WriteLine(json);
            }
            return true;
            //The ASP.NET Session doesn't allow you to perform parallel requests from the same session.
            //Having a RESTful API relying on session is a very bad design and IMHO should be rearchitectured so that it doesn't rely on state.
        }

        public JObject allAnnotation_extract(JObject obj)
        {
            JObject annotations = new JObject();
            Dictionary<long, JArray> result = new Dictionary<long, JArray>();
            foreach (KeyValuePair<string,JToken> dict in obj)
            {
                result.Add(Int64.Parse(dict.Key) , annotation_extract(dict.Value["Variables"], dict.Key));
            }

            string json_string = JsonConvert.SerializeObject(result);
            return JObject.Parse(json_string);


            return annotations;
        }
        public JArray annotation_extract (JToken json_variables_, string id)
        {
            List<string> variable_id = new List<string>();
            List<string> variable_label = new List<string>();
            List<string> dataType = new List<string>();
            List<string> unit = new List<string>();
            List<string> variable_concept_entity = new List<string>();
            List<string> variable_concept_caracteristic = new List<string>();

            JArray res = new JArray();
            int i = 0;
            if (json_variables_ == null) return res;
            foreach (JObject json_variable in json_variables_)
            {
                variable_id.Add(json_variable["Id"].ToString());
                variable_label.Add(json_variable["Label"].ToString());
                dataType.Add(json_variable["DataType"].ToString());
                unit.Add(json_variable["Unit"].ToString());
                Aam_Dataset_column_annotation annot = null;
                try
                {
                    annot = aam_list.Where(x=> x.variable_id != null).ToList().Find(x => (x.Dataset.Id == long.Parse(id)) && (x.variable_id.Id == long.Parse(json_variable["Id"].ToString())));
                }
                catch (Exception e)
                {
                    LoggerFactory.GetFileLogger().LogCustom(e.Message);
                    LoggerFactory.GetFileLogger().LogCustom(e.StackTrace);
                }
                if (annot != null)
                {
                    if (annot.entity_id.URI.ToString() == "")
                    {
                        variable_concept_entity.Add(annot.entity_id.label.ToString() + " - No Annotation");
                        json_variable["entity_id"] = annot.entity_id.label.ToString() + " - No Annotation";
                    }
                    else
                    {
                        variable_concept_entity.Add(annot.entity_id.label.ToString() + " - " + annot.entity_id.URI.ToString());
                        json_variable["entity_id"] = annot.entity_id.label.ToString() + " - " + annot.entity_id.URI.ToString();
                    }

                    if (annot.characteristic_id.URI.ToString() == "")
                    {
                        variable_concept_caracteristic.Add(annot.characteristic_id.label.ToString() + " - No Annotation");
                        json_variable["characteristic_id"] = annot.characteristic_id.label.ToString() + " - No Annotation";
                    }
                    else
                    {
                        variable_concept_caracteristic.Add(annot.characteristic_id.label.ToString() + " - " + annot.characteristic_id.URI.ToString());
                        json_variable["characteristic_id"] = annot.characteristic_id.label.ToString() + " - " + annot.characteristic_id.URI.ToString();
                    }
                    res.Add(json_variable);
                    //res[i] = json_variable;
                    i++;
                    //res.Add(id, json_variable);
                }

            }
            return res;
        }

        public JObject datastructure_extract (string datastructure_id)
        {
            JObject json_ds_struct = new JObject();
            if (datastructure_id == null) return json_ds_struct;

            //Set the searchTerm as query-String
            String param = HttpUtility.UrlEncode(datastructure_id);

            try
            {
                using(var client = new HttpClient() )
                { 
                    client.BaseAddress = new Uri(DatastructAPI);
                    HttpResponseMessage response = client.GetAsync(param).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        json_ds_struct = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                        JArray json_variables = JArray.Parse(json_ds_struct["Variables"].ToString());
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return json_ds_struct;
        }
        public JObject allDatastructure_extract()
        {
            Dictionary<long, JObject> result = new Dictionary<long, JObject>();
            foreach (long id in dm.GetDatasetLatestIds())
            {
                result.Add(id, datastructure_extract(dm.GetDataset(id)?.DataStructure?.Id.ToString()));
            }

            string json_string = JsonConvert.SerializeObject(result);
            return JObject.Parse(json_string);
        }

        public JObject metadata_extract (string id)
        {
            //get the owner of the dataset
            XmlDocument xmlDoc = dm.GetDatasetLatestMetadataVersion(Int32.Parse(id));
            XmlNode root = xmlDoc.DocumentElement;
            string idMetadata = root.Attributes["id"].Value;

            string owner = "";
            string project = "";

            if (idMetadata == "1")
            {
                XmlNodeList nodeList_givenName = xmlDoc.SelectNodes("/Metadata/Creator/PersonEML/Givenname/Name");
                XmlNodeList nodeList_Surname = xmlDoc.SelectNodes("/Metadata/Creator/PersonEML/Surname/Name");
                owner = nodeList_givenName[0].InnerText + " " + nodeList_Surname[0].InnerText;
                XmlNodeList nodeList_Title = xmlDoc.SelectNodes("/Metadata/Project/ProjectEML/Title/Title");
                XmlNodeList nodeList_Personnelgivenname = xmlDoc.SelectNodes("/Metadata/Project/ProjectEML/Personnelgivenname/Name");
                XmlNodeList nodeList_Personnelsurname = xmlDoc.SelectNodes("/Metadata/Project/ProjectEML/Personnelsurname/Name");
                project = nodeList_Title[0].InnerText + "/" + nodeList_Personnelgivenname[0].InnerText + " " + nodeList_Personnelsurname[0].InnerText;
            }
            else if (idMetadata == "2")
            {
                XmlNodeList nodeList_givenName = xmlDoc.SelectNodes("/Metadata/Owner/Owner/FullName/Name");
                owner = nodeList_givenName[0].InnerText;
                XmlNodeList nodeList_Title = xmlDoc.SelectNodes("/Metadata/Owner/Owner/Role/Role");
                XmlNodeList nodeList_SourceInstitutionID = xmlDoc.SelectNodes("/Metadata/Unit/Unit/SourceInstitutionID/Id");
                XmlNodeList nodeList_SourceID = xmlDoc.SelectNodes("/Metadata/Unit/Unit/SourceID/Id");
                XmlNodeList nodeList_UnitID = xmlDoc.SelectNodes("/Metadata/Unit/Unit/UnitID/Id");
                project = nodeList_Title[0].InnerText + "/" + nodeList_SourceInstitutionID[0].InnerText + " - " + nodeList_SourceID[0].InnerText + " - " + nodeList_UnitID[0].InnerText;
            }
            else if (idMetadata == "3")
            {
                XmlNodeList nodeList_givenName = xmlDoc.SelectNodes("/Metadata/Metadata/MetadataType/Owners/OwnersType/Owner/Contact/Person/PersonName/FullName/FullNameType");
                foreach (XmlElement node in nodeList_givenName)
                {
                    owner = node.InnerText + " - " + owner;
                }
                //owner = nodeList_givenName[0].InnerText;
                XmlNodeList nodeList_Title = xmlDoc.SelectNodes("/Metadata/Metadata/MetadataType/Owners/OwnersType/Owner/Contact/Organisation/Organisation/Name/Label/Representation/RepresentationType/Text/TextType");
                XmlNodeList nodeList_SourceInstitutionID = xmlDoc.SelectNodes("/Metadata/Metadata/MetadataType/Owners/OwnersType/Owner/Contact/Organisation/Organisation/OrgUnits/OrgUnitsType/OrgUnit/OrgUnitType");
                project = nodeList_Title[0].InnerText + "/" + nodeList_SourceInstitutionID[0].InnerText;
            }

            foreach (string proj in new List<string> { "A01", "A02", "A03", "A04", "A05", "A06", "B01", "B02", "B03", "B04", "B05", "C03", "C05", "D01", "D02", "D03", "D04" })
            {
                if (project.Contains(proj))
                    project = proj;
            }

            dynamic result = new ExpandoObject();
            result.id = id;
            result.owner = owner;
            result.project = project;
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            return JObject.Parse(json);

        }
        public JObject allMetadata_extract()
        {
            Dictionary<long, JObject> result = new Dictionary<long, JObject>();
            foreach (long id in dm.GetDatasetLatestIds())
            {
                result.Add(id, metadata_extract(id.ToString()));
            }

            string json_string = JsonConvert.SerializeObject(result);
            return JObject.Parse(json_string);
        }

        public void Dispose()
        {
            dm.Dispose();
            aam_list.Clear();
            aam_manager.Dispose();
            dataStructureManager.Dispose();
        }

        private void fill_lists()
        {
            VariableManager dam = new VariableManager();
            foreach (VariableTemplate var_temp in dam.VariableTemplateRepo.Get())
            {
                if (var_temp.Approved) DataAttributeStruct_list_in_use.Add(var_temp);
                else DataAttributeStruct_list_non_use.Add(var_temp);
            }

            UnitManager umm = new UnitManager();
            foreach (Unit unit in umm.Repo.Get())
            {
                if (unit.AssociatedDataTypes.Count()>0) EditUnitModel_list_in_use.Add(unit);
                else EditUnitModel_list_non_use.Add(unit);
            }


            DataTypeManager dataTypeManager = null;
            try
            {
                dataTypeManager = new DataTypeManager();
                List<DataType> datatypeList = dataTypeManager.Repo.Get().Where(d => d.DataContainers.Count != null).ToList();
                foreach (DataType datatype in datatypeList)
                {
                    if (datatype.DataContainers.Count > 0) DataType_in_use.Add(datatype);
                    else DataType_non_use.Add(datatype);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message.ToString());
            }
            finally
            {
                dataTypeManager.Dispose();
            }

            foreach (StructuredDataStructure ds in structureRepo)
            {
                if (ds.Datasets.Count()>0) this.DataStruct_in_use.Add(ds);
                else this.DataStruc_non_use.Add(ds);
            }

            stats_extra.Add("DataAttributeStruct_list_in_use", DataAttributeStruct_list_in_use.Count().ToString());
            stats_extra.Add("DataAttributeStruct_list_non_use", DataAttributeStruct_list_non_use.Count().ToString());
            stats_extra.Add("EditUnitModel_list_in_use", EditUnitModel_list_in_use.Count().ToString());
            stats_extra.Add("EditUnitModel_list_non_use", EditUnitModel_list_non_use.Count().ToString());
            stats_extra.Add("DataType_in_use", DataType_in_use.Count().ToString());
            stats_extra.Add("DataType_non_use", DataType_non_use.Count().ToString());
            stats_extra.Add("DataStruct_in_use", DataStruct_in_use.Count().ToString());
            stats_extra.Add("DataStruc_non_use", DataStruc_non_use.Count().ToString());

        }

    }
}