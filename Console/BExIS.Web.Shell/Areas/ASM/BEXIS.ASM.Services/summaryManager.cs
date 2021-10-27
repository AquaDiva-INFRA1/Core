using BExIS.Aam.Entities.Mapping;
using BExIS.Aam.Services;
using BExIS.Dlm.Entities.DataStructure;
using BExIS.Dlm.Services.Data;
using BExIS.Dlm.Services.DataStructure;
using BExIS.Security.Services.Subjects;
using BExIS.Xml.Helpers;
using ChoETL;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Xml;

namespace BEXIS.ASM.Services
{
    public class summaryManager : ISummary
    {
        private XmlDatasetHelper xmlDatasetHelper ;
        HttpClient client;
        public summaryManager()
        {
            client = new HttpClient();
            xmlDatasetHelper = new XmlDatasetHelper();
        }

        public void Dispose()
        {
            client.Dispose();
            this.Dispose();
        }

        #region classification
        public async Task<string> get_analysisAsync(string dataset, string username)
        {
            string content = prepare_for_classification(dataset);
            
            Dictionary<string, string> dict_data = new Dictionary<string, string>();
            dict_data.Add("username", username);
            dict_data.Add("data", csv_to_json(content) );

            //var json = JsonConvert.SerializeObject(sb.ToString(), Newtonsoft.Json.Formatting.Indented);
            var stringContent = new StringContent(csv_to_json(content).Replace("[],", "\"\","), Encoding.UTF8, "application/json");

            string url = WebConfigurationManager.AppSettings["summary_adress"]+"/predict";
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var responseTask = await client.PostAsync(url, stringContent);
            string result = await responseTask.Content.ReadAsStringAsync();
            return result;
        }
        private string prepare_for_classification(string datasetids)
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("datasetID;Datasetversion_id;variable_id;unit;type;entity_id;entity;charachteristic_id;charachteristic;" +
                "standard_id;standard;dataset_title;owner;project;variable_id_from_table;variable_value");

            DataStructureManager dsm = new DataStructureManager();
            DatasetManager dm = new DatasetManager();
            Aam_Dataset_column_annotationManager aam = new Aam_Dataset_column_annotationManager();
            List<Aam_Dataset_column_annotation> all_annot = aam.get_all_dataset_column_annotation();
            try
            {
                List<string> temp = datasetids.Split(';').ToList<string>();
                List<Int64> req_ds = temp.Select(s => Int64.Parse(s)).ToList();
                if (req_ds.Count > 0) all_annot = (List<Aam_Dataset_column_annotation>)all_annot.FindAll(x => req_ds.Contains(x.Dataset.Id));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            foreach (Aam_Dataset_column_annotation annot in all_annot)
            {
                string ch = "";
                string title = xmlDatasetHelper.GetInformationFromVersion(annot.DatasetVersion.Id, NameAttributeValues.title) != "" ? xmlDatasetHelper.GetInformationFromVersion(annot.DatasetVersion.Id, NameAttributeValues.title) : "No title";
                ch = ch +
                    annot.Dataset.Id + ";" +
                    annot.DatasetVersion.Id + ";" +
                    annot.variable_id.Id + ";" +
                    annot.variable_id.Unit.Name + ";" +
                    annot.variable_id.DataAttribute.DataType.Name + ";" +
                    annot.entity_id.Id + ";" +
                    annot.entity_id.URI + ";" +
                    annot.characteristic_id.Id + ";" +
                    annot.characteristic_id.URI + ";" +
                    annot.standard_id.Id + ";" +
                    annot.standard_id.URI + ";" +
                    title.Replace(';', ' ') + ";";

                StructuredDataStructure sds = dsm.StructuredDataStructureRepo.Get(annot.Dataset.DataStructure.Id);
                DataStructure ds = dsm.AllTypesDataStructureRepo.Get(annot.Dataset.DataStructure.Id);



                if (ds.Self.GetType() == typeof(StructuredDataStructure))
                {
                    DataTable table = dm.GetLatestDatasetVersionTuples(annot.DatasetVersion.Dataset.Id, 0, 0, true);

                    DatasetManager dsm_ = new DatasetManager();
                    XmlDocument xmlDoc = dsm_.GetDatasetLatestMetadataVersion(annot.Dataset.Id);
                    XmlNode root = xmlDoc.DocumentElement;
                    string idMetadata = root.Attributes["id"].Value;
                    string owner = "none";
                    string project = "none";

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
                        XmlNodeList nodeList_Title = xmlDoc.SelectNodes("/Metadata/Metadata/MetadataType/Owners/OwnersType/Owner/Contact/Organisation/Organisation/Name/Label/Representation/RepresentationType/Text/TextType");
                        XmlNodeList nodeList_SourceInstitutionID = xmlDoc.SelectNodes("/Metadata/Metadata/MetadataType/Owners/OwnersType/Owner/Contact/Organisation/Organisation/OrgUnits/OrgUnitsType/OrgUnit/OrgUnitType");
                    }
                    ch = ch + owner.Replace(';', ' ') + " ; " + project.Replace(';', ' ') + " ; ";

                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        DataColumn column = table.Columns[i];
                        string col_name = column.ColumnName.ToString().Replace("var", "");
                        string var_label = column.Caption != "" ? column.Caption : "NO Label";
                        if (col_name == annot.variable_id.Id.ToString())
                        {
                            string values = "";
                            int n = 0;
                            foreach (DataRow obj in table.Rows)
                            {
                                try
                                {
                                    string elem = obj.ItemArray[i].ToString();
                                    if (elem.ToString().Replace(';', ' ').Trim().Length != 0)
                                    {
                                        if (!(values.Contains(elem.ToString().Replace(';', ' ').Trim())))
                                        {
                                            values = values + elem.ToString().Replace(';', ' ').Trim() + "-";
                                            n = n + 1;
                                        }
                                    }
                                }
                                catch (Exception exc)
                                {
                                    Debug.WriteLine(exc.Message);
                                }
                                if (n % 30 == 0)
                                {
                                    str.AppendLine(ch + var_label.Replace(';', ' ') + " ; " + values + " ; ");
                                    values = "";
                                }
                            }
                            if (values != "")
                                str.AppendLine(ch + var_label.Replace(';', ' ') + " ; " + values + " ; ");
                        }
                    }
                }
            }
            return str.ToString();
        }

        private string csv_to_json(string content)
        {
            StringBuilder sb = new StringBuilder();
            using (var p = ChoCSVReader.LoadText(content).WithFirstLineHeader().WithDelimiter(";").IgnoreEmptyLine())
            {
                using (var w = new ChoJSONWriter(sb))
                    w.Write(p);
            }
            return sb.ToString();
        }
        #endregion

        #region categorical / non categorical analysis
        public async Task<JObject> get_summary(string dataset, string username)
        {
            string url = WebConfigurationManager.AppSettings["summary_adress"] + "/categoricalAnalysis";
            client.BaseAddress = new Uri(url);
            var stringContent = new StringContent((string)await getDataset(dataset, username).ConfigureAwait(true), Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var responseTask = await client.PostAsync(url, stringContent);

            string result = await responseTask.Content.ReadAsStringAsync();
            return JObject.Parse(result);
        }

        private async Task<JObject> getDataset(string id, string username)
        {
            UserManager userManager = new UserManager();
            var token = userManager.Users.Where(u => u.Name.Equals(username)).FirstOrDefault().Token;
            string result = "";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                string url = WebConfigurationManager.AppSettings["BaseAdress"] + "/api/DataStatistic/" + id.ToString();
                client.BaseAddress = new Uri(url);
                var responseTask = await client.GetAsync(url);
                result = await responseTask.Content.ReadAsStringAsync();
            }
            userManager.Dispose();
            return JObject.Parse(result);
        }

        #endregion

        #region sampling campain
        public async Task<JObject> get_sampling_summary(string data, string username)
        {
            string ds361 = getDatasetOut(WebConfigurationManager.AppSettings["dataset_Sampling_data_id"], username).Result;
            string ds362 = getDatasetOut(WebConfigurationManager.AppSettings["dataset_Sampling_dates_id"], username).Result;

            Dictionary<string, string> dict_data = new Dictionary<string, string>();
            dict_data.Add("username", username);
            dict_data.Add("dataset_Sampling_data", ds361);
            dict_data.Add("dataset_Sampling_dates", ds362);

            string result = "";
            using (var client = new HttpClient())
            {
                string url = WebConfigurationManager.AppSettings["summary_adress"] + "/getvalue";
                var stringContent = new StringContent(JsonConvert.SerializeObject(dict_data), Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.BaseAddress = new Uri(url);
                var responseTask = await client.PostAsync(url+ data, stringContent);
                result = await responseTask.Content.ReadAsStringAsync();
            }
            return JObject.Parse(result);
        }

        private async Task<string> getDatasetOut(string id, string username)
        {
            UserManager userManager = new UserManager();
            var token = userManager.Users.Where(u => u.Name.Equals(username)).FirstOrDefault().Token;
            string result = "";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                string url = WebConfigurationManager.AppSettings["BaseAdress"] + "/api/data/" + id.ToString();
                client.BaseAddress = new Uri(url);
                result = client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
                //var responseTask = await client.GetAsync(url);
                //result = await responseTask.Content.ReadAsStringAsync();
            }
            userManager.Dispose();
            return result;
        }

        #endregion



    }
}
