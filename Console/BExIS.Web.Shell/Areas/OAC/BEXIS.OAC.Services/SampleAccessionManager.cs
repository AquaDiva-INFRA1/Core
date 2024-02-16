using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using BEXIS.OAC.Entities;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using BExIS.Dlm.Entities.Data;
using BExIS.Dlm.Services.DataStructure;
using BExIS.Dlm.Services.Data;
using System.Xml;
using BExIS.Xml.Helpers;
using BExIS.Dlm.Entities.DataStructure;
using BExIS.Dlm.Services.MetadataStructure;
using BExIS.Dlm.Services.Administration;
using BExIS.Security.Services.Authorization;
using BExIS.IO.Transform.Validation.Exceptions;
using BExIS.Dlm.Entities.Administration;
using BExIS.Dlm.Entities.MetadataStructure;
using System.Xml.Linq;
using BExIS.Security.Entities.Authorization;
using BExIS.Security.Entities.Subjects;
using BExIS.IO;
using Vaiona.Persistence.Api;
using BExIS.IO.Transform.Input;
using Vaiona.Entities.Common;
using System.IO;
using System.Web.Configuration;
using System.Data;
using BExIS.IO.Transform.Validation.DSValidation;
using BExIS.Utils.NH.Querying;
using Vaiona.Logging;
using System.Security.Cryptography;

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
                accessions.Add(index.ToString() + " " + s, obj["SAMPLE_SET"]["SAMPLE"].ToString());
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
                string temp_file_path = "";

                #region create empty dataset to be filled

                StructuredDataStructure sds = find_data_struct();

                ResearchPlan rp = rpm.Repo.Get().First();
                MetadataStructure metadataStructure = msm.Repo.Get().FirstOrDefault(x => x.Id == Int64.Parse(metadata));
                EntityTemplateManager etm = new EntityTemplateManager();
                EntityTemplate et = new EntityTemplate("OMICs Template", "OMICs Template", null, metadataStructure);
                EntityTemplate existing = etm.Repo.Get().FirstOrDefault(x => x == et);
                if (existing != et) 
                    et = etm.Create(et);
                ds = dm.CreateEmptyDataset(sds, rp, metadataStructure, et);
                if (dm.IsDatasetCheckedOutFor(ds.Id, username) || dm.CheckOutDataset(ds.Id, username))
                {
                    DatasetVersion dsv = dm.GetDatasetWorkingCopy(ds.Id);
                    long METADATASTRUCTURE_ID = metadataStructure.Id;
                    XmlMetadataWriter xmlMetadatWriter = new XmlMetadataWriter(XmlNodeMode.xPath);
                    XDocument metadataX = xmlMetadatWriter.CreateMetadataXml(METADATASTRUCTURE_ID);
                    XmlDocument metadataXml = XmlMetadataWriter.ToXmlDocument(metadataX);
                    dsv.Metadata = metadataXml;
                    dm.EditDatasetVersion(dsv, null, null, null);
                    dm.CheckInDataset(ds.Id, "Metadata Imported", username);
                }
                #endregion

                # region create json object from json_array_data where every line represents the csv file content line including the header as an array of strings 
                string json = "";

                var json_array_data = new List<string[]>();
                temp_file_path = temp_file_to_save_json_as_csv + ds.Id + ".csv";

                //string data_csv = new AccessionMetadataV2().Initialise_header(temp_file_path);
                string data_csv_ = new AccessionMetadataV2().Initialise_header(temp_file_path, false);

                //json_array_data.Add(data_csv.Split(','));
                json_array_data.Add(data_csv_.Split(','));

                foreach (KeyValuePair<string, string> kvp in xx)
                {
                    string accessionValue = kvp.Value.Replace("\r\n", "");
                    string json_string_ = JsonConvert.SerializeObject(accessionValue);
                    AccessionMetadataV2 EBIresponseModel = JsonConvert.DeserializeObject<AccessionMetadataV2>(accessionValue);

                    //data_csv = data_csv + EBIresponseModel.convertToCSV(EBIresponseModel, temp_file_path);
                    data_csv_ = data_csv_ + EBIresponseModel.convertToCSV(EBIresponseModel, temp_file_path, false);

                    json_array_data.Add(EBIresponseModel.convertToCSV(EBIresponseModel, "").Split(','));
                }
                //json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(json_array_data);
                json = JsonConvert.SerializeObject(json_array_data);

                #endregion

                #region bulk upload and validate data against data structure and using json_array_data for AsciiFileReaderInfo dimensions
                DataTuple[] rows = null;
                int packageSize = 10000;
                int counter = 0;
                counter++;
                List<string> selectedDataAreaJsonArray = new List<string>() { "[1 , 0 ," + xx.Count.ToString() + "," + (json_array_data[0].Length -1) + "]" };
                string selectedHeaderAreaJsonArray = string.Join(" ,", new List<string>() { "[0, 0, 0, " + (json_array_data[0].Length-1) + "]" });
                List<int[]> areaDataValuesList = new List<int[]>();
                foreach (string area in selectedDataAreaJsonArray)
                {
                    areaDataValuesList.Add(JsonConvert.DeserializeObject<int[]>(area));
                }
                int[] areaHeaderValues = JsonConvert.DeserializeObject<int[]>(selectedHeaderAreaJsonArray);
                Orientation orientation = Orientation.columnwise;
                //String worksheetUri = temp_file_path;
                int batchSize = (new Object()).GetUnitOfWork().PersistenceManager.PreferredPushSize;
                int batchnr = 1;

                dm.CheckOutDataset(ds.Id, username);
                DatasetVersion workingCopy = dm.GetDatasetWorkingCopy(ds.Id);

                foreach (int[] areaDataValues in areaDataValuesList)
                {
                    //First batch starts at the start of the current data area
                    int currentBatchStartRow = areaDataValues[0] + 1;
                    while (currentBatchStartRow <= areaDataValues[2] + 1) //While the end of the current data area has not yet been reached
                    {
                        //End row is start row plus batch size
                        int currentBatchEndRow = currentBatchStartRow + batchSize;

                        //Set the indices for the reader
                        EasyUploadFileReaderInfo fri = new EasyUploadFileReaderInfo
                        {
                            DataStartRow = currentBatchStartRow,
                            //End row is either at the end of the batch or the end of the marked area
                            //DataEndRow = (currentBatchEndRow > areaDataValues[2] + 1) ? areaDataValues[2] + 1 : currentBatchEndRow,
                            DataEndRow = Math.Min(currentBatchEndRow, areaDataValues[2] + 1),
                            //Column indices as marked in a previous step
                            DataStartColumn = areaDataValues[1] + 1,
                            DataEndColumn = areaDataValues[3] + 1,

                            //Header area as marked in a previous step
                            VariablesStartRow = areaHeaderValues[0] + 1,
                            VariablesStartColumn = areaHeaderValues[1] + 1,
                            VariablesEndRow = areaHeaderValues[2] + 1,
                            VariablesEndColumn = areaHeaderValues[3] + 1,

                            Offset = areaDataValues[1],
                            Orientation = orientation
                        };


                        #region csv / txt parsing to get data tuples and variables
                        AsciiFileReaderInfo afri = new AsciiFileReaderInfo();
                        afri.Seperator = TextSeperator.comma;

                        List<String[]> Json_table_ = JsonConvert.DeserializeObject<List<String[]>>
                            (json);


                        AsciiReaderEasyUpload reader_ = new AsciiReaderEasyUpload(sds, afri);
                        //FileStream Stream = reader_.Open(temp_file_path);
                        rows = reader_.ReadFile(null , System.IO.Path.GetFileName(temp_file_path), Json_table_, afri, sds, ds.Id, packageSize, fri);
                        //Stream.Close();
                        if (rows != null)
                        {
                            dm.EditDatasetVersion(workingCopy, rows.ToList(), null, null);
                        }

                        //Close the Stream so the next ExcelReader can open it again
                        //Stream.Close();

                        //Debug information
                        int lines = (areaDataValues[2] + 1) - (areaDataValues[0] + 1);
                        int batches = lines / batchSize;
                        batchnr++;

                        //Next batch starts after the current one
                        currentBatchStartRow = currentBatchEndRow + 1;

                        #endregion csv parsing to get data tuples and variables 
                    }
                }
                workingCopy.ModificationInfo = new EntityAuditInfo()
                {
                    Performer = username,
                    Comment = "Data",
                    ActionType = AuditActionType.Create
                };

                dm.CheckInDataset(ds.Id, "Import data from OMIC archives  ", username);
                #endregion

                dsm.Dispose();
                dm.Dispose();
                msm.Dispose();
                rpm.Dispose();
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

        private StructuredDataStructure find_data_struct()
        {
            DataStructureManager dsm = new DataStructureManager();
            StructuredDataStructure sds = dsm.CreateStructuredDataStructure("sequence data" + "_" + System.DateTime.UtcNow.ToString("r"), "sequence data" + " " + System.DateTime.UtcNow.ToString("r"), "", "", DataStructureCategory.Generic);
            bool foundReusableDataStructure = false;
            List<StructuredDataStructure> allDatastructures = dsm.StructuredDataStructureRepo.Get().ToList();
            foreach (StructuredDataStructure existingStructure in allDatastructures)
            {
                if (!foundReusableDataStructure)
                {
                    //For now a datastructure is considered an exact match if it contains variables with
                    //the same names (labels), datatypes and units in the correct order
                    List<VariableInstance> variablesOfExistingStructure = existingStructure.Variables.ToList();
                    foundReusableDataStructure = true;
                    if (variablesOfExistingStructure.Count != ((System.Reflection.TypeInfo)typeof(AccessionMetadataV2)).DeclaredFields.ToList().Count)
                        foundReusableDataStructure = false;
                    else
                    {
                        for (int i = 0; i < variablesOfExistingStructure.Count; i++)
                        {
                            Variable exVar = variablesOfExistingStructure.ElementAt(i);
                            if (exVar.Label != ((System.Reflection.TypeInfo)typeof(AccessionMetadataV2)).DeclaredFields.ToList().ElementAt(i).Name)
                            {
                                foundReusableDataStructure = false;
                            }
                        }
                    }
                    if (foundReusableDataStructure)
                    {
                        sds = existingStructure;
                    }

                }
            }

            // if no data structure is found, we create a new onw
            XmlDocument xmldoc = new XmlDocument();
            XmlElement extraElement = xmldoc.CreateElement("extra");
            XmlElement orderElement = xmldoc.CreateElement("order");
            DataContainerManager dam = new DataContainerManager();
            var dataTypeRepo = this.GetUnitOfWork().GetReadOnlyRepository<Dlm.Entities.DataStructure.DataType>();
            var unitRepo = this.GetUnitOfWork().GetReadOnlyRepository<Unit>();
            List<VariableInstance> vars = new List<VariableInstance>();
            string header = new AccessionMetadataV2().Initialise_header("");
            if (sds.Variables.Count == 0)
            {
                foreach (string variable_name in header.Split(','))
                {
                    VariableManager vm = new VariableManager();
                    VariableTemplate vt = vm.CreateVariableTemplate("OMICs variable template", 
                        dataTypeRepo.Get().ToList<Dlm.Entities.DataStructure.DataType>().FirstOrDefault(x => x.Name == "String") ,
                        unitRepo.Get().ToList<Unit>().FirstOrDefault(x => x.Name == "None"));
                    VariableInstance vi = vm.CreateVariable(variable_name, sds.Id,vt.Id);
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
            }
            dam.Dispose();

            dsm.Dispose();
            return sds;
        }

    }
}