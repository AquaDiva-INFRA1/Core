using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using BEXIS.OAC.Entities;
using Newtonsoft.Json.Linq;
using BExIS.Dlm.Entities.Data;
using BExIS.Dlm.Services.DataStructure;
using BExIS.Dlm.Services.Data;
using System.Xml;
using BExIS.Xml.Helpers;
using BExIS.Dlm.Entities.DataStructure;
using BExIS.Dlm.Services.MetadataStructure;
using BExIS.Dlm.Services.Administration;
using BExIS.Dlm.Entities.Administration;
using BExIS.Dlm.Entities.MetadataStructure;
using System.Xml.Linq;
using Vaiona.Persistence.Api;
using System.IO;
using System.Web.Configuration;
using Vaiona.Logging;
using ChoETL;
using System.Data;
using BExIS.Security.Services.Subjects;
using BExIS.Security.Services.Authorization;
using BExIS.Security.Entities.Subjects;
using BExIS.Security.Entities.Authorization;
using ServiceStack;
using BExIS.IO.Transform.Input;
using BExIS.IO.Transform.Validation.DSValidation;
using BExIS.IO.Transform.Validation.Exceptions;
using BExIS.IO;

namespace BExIS.OAC.Services
{
    public class SampleAccessionManager : ISampleAccession
    {
        static string temp_file_to_save_json_as_csv = Path.GetFullPath(WebConfigurationManager.AppSettings["output_Folder"]);
        WebClient wc;
        public SampleAccessionManager() {
            wc = new WebClient();
        }

        public void Dispose()
        {
            wc.Dispose();
            this.Dispose();
        }

        public JObject fetchStudy(string studyID, DataSource dataSource)
        {
            string url = "";

            #region data source
            switch ((DataSource)dataSource)
            {
                case DataSource.EBI:
                case DataSource.NCBI: // the same in our examples
                                      //Url = "https://www.ebi.ac.uk/biosamples/api/samples/" + Identifier;
                    url = PortalSource.EBI + studyID;
                    break;
                case DataSource.BioGPS:
                    url = PortalSource.BioGPS + studyID + "/values/?format=xml";
                    break;
                default:
                    throw new Exception("Missing implementation for enum " + Enum.GetName(typeof(DataSource), dataSource));
            }
            #endregion data source

            #region fetch accessions from study id into a dictionary <accessionID,accession metadata value>
            Dictionary<string, string> accessions = new Dictionary<string, string>();
            string Accessions_List = wc.DownloadString(url).Trim();
            List<string> All_Accessions = Accessions_List.Split('\n').ToList<string>().GetRange(1, Accessions_List.Split('\n').ToList<string>().Count() - 1);
            int index = 0;
            foreach (string s in All_Accessions)
            {
                string sample_Url = "https://www.ebi.ac.uk/ena/browser/api/xml/" + s.Split('	')[1];

                // string sample_Url = "https://www.ebi.ac.uk/biosamples/api/samples/" + s.Split('	')[1];
                #region download the metadata
                // download the metadata
                string DownloadedData = new WebClient().DownloadString(sample_Url).Trim();

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(DownloadedData);
                string json = JsonConvert.SerializeXmlNode(doc);
                JObject obj = JObject.Parse(json);
                #endregion
                accessions.Add(index.ToString() + " " + s, obj.ToString());
                index++;
            }
            #endregion
            string json_string = JsonConvert.SerializeObject(accessions);

            return JObject.Parse(json_string);
        }

        public JObject AddProjectsdataset(Dictionary<string, string> xx, string username, string metadata)
        {
            try
            {
                DataStructureManager dsm = new DataStructureManager();
                DatasetManager dm = new DatasetManager();
                Dataset ds = new Dataset();
                MetadataStructureManager msm = new MetadataStructureManager();
                ResearchPlanManager rpm = new ResearchPlanManager();
                Security.Services.Objects.EntityManager entityManager = new Security.Services.Objects.EntityManager();
                EntityTemplateManager etm = new EntityTemplateManager();

                #region get data as data table and list of list of strings
                List<string> variables = new List<string>();
                List<List<string>> values = new List<List<string>>();
                foreach (KeyValuePair<string, string> kvp in xx)
                {
                    string accessionValue = kvp.Value.Replace("\r\n", "");
                    if (variables.Count < 1)
                    {
                        foreach (JProperty property in JObject.Parse(accessionValue)["PROJECT_SET"]["PROJECT"])
                        {
                            variables.Add(property.Name.ToLower().Replace('@', ' ').Trim());
                        }
                    }
                    List<string> line_cell_val = new List<string>();
                    foreach (JProperty property in JObject.Parse(accessionValue)["PROJECT_SET"]["PROJECT"])
                    {
                        line_cell_val.Add(property.Value.ToString().ToLower().Replace('@', ' ').Trim());
                    }
                    values.Add(line_cell_val);
                }
                #endregion

                StructuredDataStructure sds = find_data_struct(variables);

                #region create empty dataset to be filled

                ResearchPlan rp = rpm.Repo.Get(1);
                MetadataStructure metadataStructure = msm.Repo.Get().FirstOrDefault(x => x.Id == Int64.Parse(metadata));
                EntityTemplate et = (EntityTemplate)etm.Repo.Get(2);
                ds = dm.CreateEmptyDataset(sds, rp, metadataStructure, et);

                List< DataTuple > datatuples = new List< DataTuple >();
                DataReader reader = new AsciiReader(sds, new AsciiFileReaderInfo() { Seperator = TextSeperator.semicolon }, new IO.IOUtility());
                IEnumerable<string> vairableNames = sds.Variables.Select(v => v.Label);
                List<VariableIdentifier> variableIdentifiers = reader.SetSubmitedVariableIdentifiers(vairableNames.ToList());
                List<Error> errors = reader.ValidateComparisonWithDatatsructure(variableIdentifiers);
                foreach (List<string> row in values)
                {
                    DataTuple dt = reader.ReadRow(row, 1);
                    datatuples.Add(dt);
                }
                

                if (dm.IsDatasetCheckedOutFor(ds.Id, username) || dm.CheckOutDataset(ds.Id, username))
                {
                    DatasetVersion dsv = dm.GetDatasetWorkingCopy(ds.Id);
                    dsv.ModificationInfo = new Vaiona.Entities.Common.EntityAuditInfo()
                    {
                        Performer = username,
                        Comment = "Data",
                        ActionType = Vaiona.Entities.Common.AuditActionType.Create
                    };
                    dm.EditDatasetVersion(dsv, datatuples.ToList(), null, null);
                    dm.CheckInDataset(ds.Id, "Import data from OMIC archives  ", username, ViewCreationBehavior.Create);
                }

                if (dm.IsDatasetCheckedOutFor(ds.Id, username) || dm.CheckOutDataset(ds.Id, username))
                {
                    
                    long METADATASTRUCTURE_ID = metadataStructure.Id;

                    XmlMetadataWriter xmlMetadatWriter = new XmlMetadataWriter(XmlNodeMode.xPath);
                    XDocument metadataX = xmlMetadatWriter.CreateMetadataXml(METADATASTRUCTURE_ID);

                    XmlDocument metadataXml = XmlMetadataWriter.ToXmlDocument(metadataX);

                    DatasetVersion dsv = dm.GetDatasetWorkingCopy(ds.Id);
                    dsv.Title = "OMICs Import";
                    dsv.Description = "Imported OMICs data from user "+username;
                    dsv.Metadata = metadataXml;
                    dm.EditDatasetVersion(dsv, null, null, null);
                    dm.CheckInDataset(ds.Id, "Metadata Imported", username);
                }

                #endregion

                #region security
                EntityPermissionManager entityPermissionManager = new EntityPermissionManager();
                UserPiManager upm = new UserPiManager();
                if (username != "DEFAULT")
                {
                    //Full permissions for the user
                    entityPermissionManager.Create<User>(username, "Dataset", typeof(Dataset), ds.Id, Enum.GetValues(typeof(RightType)).Cast<RightType>().ToList());

                    //Get PIs of the current user
                    List<User> piList = upm.GetPisFromUserByName(username).ToList();
                    foreach (User pi in piList)
                    {
                        //Full permissions for the pis
                        entityPermissionManager.Create<User>(pi.Name, "Dataset", typeof(Dataset), ds.Id, Enum.GetValues(typeof(RightType)).Cast<RightType>().ToList());

                        //Get all users with the same pi
                        List<User> piMembers = upm.GetAllPiMembers(pi.Id).ToList();
                        //Give view and download rights to the members
                        foreach (User piMember in piMembers)
                        {
                            entityPermissionManager.Create<User>(piMember.Name, "Dataset", typeof(Dataset), ds.Id, new List<RightType> {
                                        RightType.Read
                                });
                        }
                    }
                }
                // add security
                if (username != "DEFAULT")
                {
                    foreach (RightType rightType in Enum.GetValues(typeof(RightType)).Cast<RightType>())
                    {
                        //The user gets full permissions
                        // add security
                        if (username != "DEFAULT")
                        {
                            entityPermissionManager.Create<User>(username, "Dataset", typeof(Dataset), ds.Id, Enum.GetValues(typeof(RightType)).Cast<RightType>().ToList());
                        }
                    }
                }

                #endregion security
                dsm.Dispose();
                dm.Dispose();
                msm.Dispose();
                rpm.Dispose();
                entityManager.Dispose();
                etm.Dispose();
                entityPermissionManager.Dispose();

                var result = new
                {
                    dataset_id = ds.Id
                };
                return JObject.Parse(JsonConvert.SerializeObject(result));
            }
            catch (Exception e)
            {
                LoggerFactory.GetFileLogger().LogCustom(e.Message);
                LoggerFactory.GetFileLogger().LogCustom(e.StackTrace);
                return JObject.Parse(JsonConvert.SerializeObject(new { e.Message }));
            }

        }

        private StructuredDataStructure find_data_struct(List<string> variable_names)
        {
            DataStructureManager dsm = new DataStructureManager();
            bool foundReusableDataStructure = false;
            foreach (StructuredDataStructure existingStructure in dsm.StructuredDataStructureRepo.Get().ToList())
            {
                List<VariableInstance> variablesOfExistingStructure = existingStructure.Variables.ToList();
                for (int i = 0; i < variablesOfExistingStructure.Count; i++)
                {
                    if (variablesOfExistingStructure.Count != variable_names.Count)
                        break;
                    Variable exVar = variablesOfExistingStructure.ElementAt(i);
                    if (exVar.Label != variable_names.ElementAt(i))
                    {
                        foundReusableDataStructure = false;
                        break;
                    }
                    foundReusableDataStructure = true;
                }
                
                if (foundReusableDataStructure) 
                    return existingStructure;
            }

            // if no data structure is found, we create a new onw
            StructuredDataStructure sds = dsm.CreateStructuredDataStructure("sequence data" + "_" + System.DateTime.UtcNow.ToString("r"),
                "sequence data" + " " + System.DateTime.UtcNow.ToString("r"), "", "", DataStructureCategory.Generic);

            XmlDocument xmldoc = new XmlDocument();
            XmlElement extraElement = xmldoc.CreateElement("extra");
            XmlElement orderElement = xmldoc.CreateElement("order");
            DataContainerManager dam = new DataContainerManager();
            var dataTypeRepo = this.GetUnitOfWork().GetReadOnlyRepository<DataType>();
            var unitRepo = this.GetUnitOfWork().GetReadOnlyRepository<Unit>();
            List<VariableInstance> vars = new List<VariableInstance>();
            VariableManager vm = new VariableManager();
            foreach (string variable_name in variable_names)
            {
                VariableTemplate vt = vm.CreateVariableTemplate("OMICs variable template",
                    dataTypeRepo.Get().ToList<DataType>().FirstOrDefault(x => x.Name.ToLower() == "string"),
                    unitRepo.Get().ToList<Unit>().FirstOrDefault(x => x.Name.ToLower() == "none"));
                VariableInstance vi = vm.CreateVariable(variable_name, sds.Id, vt.Id);
                vi.OrderNo = variable_names.IndexOf(variable_name) + 1;
                VariableInstance vi_ = vm.CreateVariable(
                   variable_name,
                    dataTypeRepo.Get().ToList<DataType>().FirstOrDefault(x => x.Name.ToLower() == "string"),
                    unitRepo.Get().ToList<Unit>().FirstOrDefault(x => x.Name.ToLower() == "none"),
                    sds.Id,
                    true,
                    false,
                    variable_names.IndexOf(variable_name)+1
                );

                vars.Add(vi);
                XmlElement newVariableXml = xmldoc.CreateElement("variable");
                newVariableXml.InnerText = Convert.ToString(vi.Id);

                orderElement.AppendChild(newVariableXml);
            }
            extraElement.AppendChild(orderElement);
            xmldoc.AppendChild(extraElement);
            sds.Extra = xmldoc;
            sds.Name = "generated import structure " + System.DateTime.UtcNow.ToString("r");
            sds.Description = "sequence data" + "_" + System.DateTime.UtcNow.ToString("r");
            dam.Dispose();

            dsm.Dispose();
            return sds;
        }
    }
}