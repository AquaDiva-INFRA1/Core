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
using Vaiona.Persistence.Api;
using Vaiona.Logging;
using System.IO;
using Vaiona.Utils.Cfg;

namespace BEXIS.ASM.Services
{
    public class summaryManager : ISummary
    {
        private string trainig_path = Path.Combine(AppConfiguration.GetModuleWorkspacePath("ASM"), "training.csv");
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
        public async Task<string> get_analysisAsync(string dataset, string username, Boolean semantic_flag)
        {
            string content = prepare_for_classification(dataset);

            //Dictionary<string, string> dict_data = new Dictionary<string, string>();
            //dict_data.Add("username", username);
            //dict_data.Add("data", csv_to_json(content) );

            //var json = JsonConvert.SerializeObject(sb.ToString(), Newtonsoft.Json.Formatting.Indented);
            string content_csv = csv_to_json(content).Replace("[],", "\"\",");
            Dictionary<string, string> params_ = new Dictionary<string, string>();
            params_.Add("content_csv", content_csv);
            params_.Add("semantic_flag", semantic_flag.ToString());
            var stringContent = new StringContent(JsonConvert.SerializeObject(params_, Newtonsoft.Json.Formatting.Indented), Encoding.UTF8, "application/json");

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
            str.AppendLine("datasetID;Datasetversion_id;variable_id;unit;type;entity_id;entityLabel;entity;charachteristic_id;charachteristicLabel;charachteristic;" +
                "standard_id;standard;dataset_title;owner;project;variable_id_from_table;variable_value");
            write_out(str.ToString(),true);
            DataStructureManager dsm = new DataStructureManager();
            DatasetManager dm = new DatasetManager();
            Aam_Dataset_column_annotationManager aam = new Aam_Dataset_column_annotationManager();
            List<Aam_Dataset_column_annotation> all_annot = aam.get_all_dataset_column_annotation();
            var structureRepo = dsm.GetUnitOfWork().GetReadOnlyRepository<StructuredDataStructure>();

            List<string> temp = datasetids.Split(';').ToList<string>();
                List<Int64> req_ds = temp.Select(s => Int64.Parse(s)).ToList();
                foreach (Int64 id in req_ds)
                {
                    Debug.WriteLine("parsing dataset : " + id);
                    try
                    {
                        StructuredDataStructure ds = structureRepo.Get(dm.GetDataset(id).DataStructure.Id);
                        if (ds.Self.GetType() == typeof(StructuredDataStructure))
                        {
                            #region metadat extraction
                            XmlDocument xmlDoc = dm.GetDatasetLatestMetadataVersion(id);
                            XmlNode root = xmlDoc.DocumentElement;
                            string idMetadata = root.Attributes["id"].Value;
                            string owner = "none";
                            string project = "none";
                            string title = xmlDatasetHelper.GetInformationFromVersion(dm.GetDatasetLatestVersion(id).Id, NameAttributeValues.title) != "" ?
                                xmlDatasetHelper.GetInformationFromVersion(dm.GetDatasetLatestVersion(id).Id, NameAttributeValues.title) : "No title";
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
                            #endregion

                            foreach (Variable var in ds.Variables)
                            {
                            #region entity and characteristics extraction
                            //Aam_Dataset_column_annotation variable_annotation = aam.get_dataset_column_annotation_by_variable(var).Where(x => x.Dataset.Id == id).FirstOrDefault();
                            Aam_Dataset_column_annotation variable_annotation = all_annot.Where(x => (x.variable_id.Id == var.Id) && (x.Dataset.Id == id)).FirstOrDefault();
                                #endregion

                                #region create file content
                                string ch = id + ";";
                                ch = ch + dm.GetDatasetLatestVersion(id).Id + ";";
                                ch = ch + var?.Id + ";";
                                ch = ch + var?.Unit?.Name + ";";
                                ch = ch + var?.DataType?.Name + ";";
                                ch = ch + variable_annotation?.entity_id.Id + ";";
                                ch = ch + variable_annotation?.entity_id.label + ";";
                                ch = ch + variable_annotation?.entity_id.URI + ";";
                                ch = ch + variable_annotation?.characteristic_id.Id + ";";
                                ch = ch + variable_annotation?.characteristic_id.label + ";";
                                ch = ch + variable_annotation?.characteristic_id.URI + ";";
                                ch = ch + variable_annotation?.standard_id.Id + ";";
                                ch = ch + variable_annotation?.standard_id.label + ";";
                                ch = ch + title?.Replace(';', ' ') + " ; " + owner?.Replace(';', ' ') + " ; " + project?.Replace(';', ' ') + " ; ";
                                #endregion

                                #region #region extract primary data
                                DataTable table = dm.GetLatestDatasetVersionTuples(id, 0, 0, true);
                                int n = 0;
                                string values = "";
                                for (int i = 0; i < table.Columns.Count; i++)
                                {
                                    if ((table.Rows.Count>0)&&(table.Rows[i]["var" + var.Id].ToString().Replace(';', ' ').Trim().Length != 0))
                                    {
                                        if (!(values.Contains(table.Rows[i]["var" + var.Id].ToString().Replace(';', ' ').Trim())))
                                        {
                                            values = values + table.Rows[i]["var" + var.Id].ToString().Replace(';', ' ').Trim() + "-";
                                            n = n + 1;
                                        }
                                    }
                                    if (n % 30 == 0)
                                    {
                                        str.AppendLine(ch + var?.Label?.Replace(';', ' ') + " ; " + values + " ; ");
                                        write_out(ch + var?.Label?.Replace(';', ' ') + " ; " + values + " ; ", false);
                                        values = "";
                                    }
                                }
                                if (values != "") {
                                    str.AppendLine(ch + var?.Label?.Replace(';', ' ') + " ; " + values + " ; ");
                                    write_out(ch + var?.Label?.Replace(';', ' ') + " ; " + values + " ; " , false);
                                }
                                #endregion
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            return str.ToString();
        }

        private void write_out(string ch, bool init)
        {
            using (StreamWriter writer = new StreamWriter(trainig_path, !init))
            {
                writer.WriteLine(ch);
            }
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

        public async Task<string> export_training_summary()
        {
            using (DatasetManager dm = new DatasetManager())
            {
                return prepare_for_classification(String.Join(";", dm.GetDatasetIds()));
            }
        }
        #endregion

        #region categorical / non categorical analysis
        public async Task<string> get_summary(string dataset, string username)
        {
            string url = WebConfigurationManager.AppSettings["summary_adress"] + "/categoricalAnalysis";
            client.BaseAddress = new Uri(url);
            string res = await getDataset(dataset, username).ConfigureAwait(true);
            var stringContent = new StringContent(await getDataset(dataset, username).ConfigureAwait(true), Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var responseTask = await client.PostAsync(url, stringContent);

            string result = await responseTask.Content.ReadAsStringAsync();
            result = result.Replace("\n", string.Empty).Replace("\r", String.Empty).Replace("\t", String.Empty);
            var json_res = new
            {
                result = result,
                dataset = res
            };
            return JsonConvert.SerializeObject(json_res);
        }

        private async Task<string> getDataset(string id, string username)
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
            return result;
        }

        #endregion

        #region sampling campain
        public async Task<string> get_sampling_summary(string data, string username)
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
                result = result.Replace("\n", string.Empty).Replace("\r", String.Empty).Replace("\t", String.Empty);
            }
            return result;
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
