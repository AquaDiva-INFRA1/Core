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
                string sample_Url = "https://www.ebi.ac.uk/biosamples/api/samples/" + s.Split('	')[1];

                #region download the metadata
                // download the metadata
                string DownloadedData = new WebClient().DownloadString(sample_Url).Trim();

                #endregion
                accessions.Add(index.ToString() + " " + s, DownloadedData);
                index++;
            }
            #endregion
            string json_string = JsonConvert.SerializeObject(accessions);
            
            return JObject.Parse(json_string);
        }

        public JObject AddProjectsdataset(Dictionary<string, string> xx, string username)
        {
            DataStructureManager dsm = new DataStructureManager();
            DatasetManager dm = new DatasetManager();
            XmlDocument MetadataDoc = new XmlDocument();
            Dataset ds = new Dataset();
            XmlDatasetHelper xmlDatasetHelper = new XmlDatasetHelper();
            DataStructure dataStruct = (DataStructure)dsm.AllTypesDataStructureRepo.Get().FirstOrDefault(x => x.Name == "none");
            StructuredDataStructure dataStruct_ = (StructuredDataStructure)dsm.StructuredDataStructureRepo.Get().FirstOrDefault(x => x.Name.ToLower() == "sequence data");
            MetadataStructureManager msm = new MetadataStructureManager();
            ResearchPlanManager rpm = new ResearchPlanManager();
            EntityPermissionManager entityPermissionManager = new EntityPermissionManager();
            List<Error> temp = new List<Error>();
            string temp_file_path = "";
            try
            {
                #region create empty dataset to be filled

                ResearchPlan rp = rpm.Repo.Get().First();
                MetadataStructure metadataStructure = msm.Repo.Get().FirstOrDefault(x => x.Name.ToLower() == "basic abcd");
                ds = dm.CreateEmptyDataset(dataStruct_, rp, metadataStructure);
                if (dm.IsDatasetCheckedOutFor(ds.Id, username) || dm.CheckOutDataset(ds.Id, username ))
                {
                    DatasetVersion dsv = dm.GetDatasetWorkingCopy(ds.Id);
                    long METADATASTRUCTURE_ID = metadataStructure.Id;
                    XmlMetadataWriter xmlMetadatWriter = new XmlMetadataWriter(XmlNodeMode.xPath);
                    XDocument metadataX = xmlMetadatWriter.CreateMetadataXml(METADATASTRUCTURE_ID);
                    XmlDocument metadataXml = XmlMetadataWriter.ToXmlDocument(metadataX);
                    dsv.Metadata = metadataXml;
                    dm.EditDatasetVersion(dsv, null, null, null);
                    dm.CheckInDataset(ds.Id, "Metadata Imported", username );
                }


                #endregion
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            try
            {
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
            }
            catch (Exception ex)
            {
                throw (ex);
            }


            string json = "";
            try
            {
                #region save primary data in csv format and temporary csv file
                var json_array_data = new List<string[]>();
                temp_file_path = temp_file_to_save_json_as_csv + ds.Id + ".csv";

                string data_csv = new AccessionMetadata().Initialise_header(temp_file_path);
                json_array_data.Add(data_csv.Split(','));

                foreach (KeyValuePair<string, string> kvp in xx)
                {
                    AccessionMetadata EBIresponseModel = new AccessionMetadata(JObject.Parse(kvp.Value));
                    data_csv = data_csv + EBIresponseModel.ConvertTocsv(EBIresponseModel, temp_file_path);
                    json_array_data.Add(EBIresponseModel.ConvertTocsv(EBIresponseModel, "").Split(','));
                }
                //json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(json_array_data);
                json = JsonConvert.SerializeObject(json_array_data);
            }
            #endregion
            catch (Exception ex)
            {
                throw (ex);
            }

            #region creating data tuples 

            DataTuple[] rows = null;
            int packageSize = 10000;
            int counter = 0;
            counter++;
            List<string> selectedDataAreaJsonArray = new List<string>() { "[1 , 0 ," + xx.Count.ToString() + "," + (typeof(AccessionMetadata).GetProperties().Count() - 1).ToString() + "]" };
            string selectedHeaderAreaJsonArray = string.Join(" ,", new List<string>() { "[0, 0, 0, " + (typeof(AccessionMetadata).GetProperties().Count() - 1).ToString() + "]" });
            List<int[]> areaDataValuesList = new List<int[]>();
            foreach (string area in selectedDataAreaJsonArray)
            {
                areaDataValuesList.Add(JsonConvert.DeserializeObject<int[]>(area));
            }
            int[] areaHeaderValues = JsonConvert.DeserializeObject<int[]>(selectedHeaderAreaJsonArray);
            Orientation orientation = Orientation.columnwise;
            String worksheetUri = temp_file_path;
            int batchSize = (new Object()).GetUnitOfWork().PersistenceManager.PreferredPushSize;
            int batchnr = 1;

            //try {
            dm.CheckOutDataset(ds.Id, username );
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

                    AsciiReaderEasyUpload reader_ = new AsciiReaderEasyUpload(dataStruct_, afri);
                    FileStream Stream = reader_.Open(temp_file_path);
                    rows = reader_.ReadFile(Stream, System.IO.Path.GetFileName(temp_file_path), Json_table_, afri, dataStruct_, ds.Id, packageSize, fri);
                    Stream.Close();
                    if (rows != null)
                    {
                        dm.EditDatasetVersion(workingCopy, rows.ToList(), null, null);
                    }

                    //Close the Stream so the next ExcelReader can open it again
                    Stream.Close();

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

            //}
            //catch (Exception e)
            //{
            //    throw (e);
            //}
            #endregion

            //System.IO.File.Delete(temp_file_path);
            //dm.CheckInDataset(ds.Id, "Import data from OMIC archives ", GetUsernameOrDefault());

            dsm.Dispose();
            dm.Dispose();
            entityPermissionManager.Dispose();
            var result = new
            {
                dataset_id = ds.Id
            };
            return JObject.Parse(JsonConvert.SerializeObject(result));
        }
    }
}