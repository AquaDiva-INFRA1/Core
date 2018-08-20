using BExIS.Dcm.UploadWizard;
using BExIS.Dlm.Entities.Administration;
using BExIS.Dlm.Entities.Data;
using BExIS.Dlm.Entities.DataStructure;
using BExIS.Dlm.Entities.MetadataStructure;
using BExIS.Dlm.Services.Data;
using BExIS.Dlm.Services.DataStructure;
using BExIS.Dlm.Services.MetadataStructure;
using BExIS.IO;
using BExIS.IO.Transform.Input;
using BExIS.IO.Transform.Validation.DSValidation;
using BExIS.IO.Transform.Validation.Exceptions;
using BExIS.IO.Transform.Validation.ValueCheck;
using BExIS.Modules.Dcm.UI.Helpers;
using BExIS.Modules.Dcm.UI.Models;
using BExIS.Security.Entities.Authorization;
using BExIS.Security.Entities.Subjects;
using BExIS.Security.Services.Authorization;
using BExIS.Security.Services.Subjects;
using BExIS.Xml.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml;
using System.Xml.Linq;
using Vaiona.Persistence.Api;
using Vaiona.Utils.Cfg;
using Vaiona.Web.Mvc;
using Vaiona.Web.Mvc.Modularity;

namespace BExIS.Modules.Dcm.UI.Controllers
{
    public class EasyUploadSummaryController : BaseController
    {
        private EasyUploadTaskManager TaskManager;
        private FileStream Stream;
        XmlDatasetHelper xmlDatasetHelper = new XmlDatasetHelper();

        private static IDictionary<Guid, int> tasks = new Dictionary<Guid, int>();
        private static String MissingConceptsLoggingPath = Path.Combine(AppConfiguration.GetModuleWorkspacePath("DCM"), "Logging", "MissingConcepts" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day  + ".txt");

        [HttpGet]
        public ActionResult Summary(int index)
        {
            MetadataStructureManager msm = new MetadataStructureManager();

            try
            {
                TaskManager = (EasyUploadTaskManager)Session["TaskManager"];

                //set current stepinfo based on index
                if (TaskManager != null)
                {
                    TaskManager.SetCurrent(index);
                    // remove if existing
                    TaskManager.RemoveExecutedStep(TaskManager.Current());
                }

                EasyUploadSummaryModel model = new EasyUploadSummaryModel();
                model.StepInfo = TaskManager.Current();

                if (TaskManager.Bus.ContainsKey(EasyUploadTaskManager.FILENAME))
                {
                    model.DatasetTitle = Convert.ToString(TaskManager.Bus[EasyUploadTaskManager.FILENAME]);

                    if (TaskManager.Bus.ContainsKey(EasyUploadTaskManager.DESCRIPTIONTITLE))
                    {
                        string tmp = Convert.ToString(TaskManager.Bus[EasyUploadTaskManager.DESCRIPTIONTITLE]);
                        if (!String.IsNullOrWhiteSpace(tmp))
                        {
                            model.DatasetTitle = Convert.ToString(TaskManager.Bus[EasyUploadTaskManager.DESCRIPTIONTITLE]);
                        }
                    }
                }

                if (TaskManager.Bus.ContainsKey(EasyUploadTaskManager.SCHEMA))
                {

                    long id = Convert.ToInt64(TaskManager.Bus[EasyUploadTaskManager.SCHEMA]);
                    model.MetadataSchemaTitle = msm.Repo.Get(m => m.Id == id).FirstOrDefault().Name;
                    msm.Dispose();
                }

                if (TaskManager.Bus.ContainsKey(EasyUploadTaskManager.SHEET_FORMAT))
                {
                    model.FileFormat = TaskManager.Bus[EasyUploadTaskManager.SHEET_FORMAT].ToString();
                }

                if (TaskManager.Bus.ContainsKey(EasyUploadTaskManager.SHEET_HEADER_AREA))
                {
                    string selectedHeaderAreaJsonArray = TaskManager.Bus[EasyUploadTaskManager.SHEET_HEADER_AREA].ToString();
                    int[] areaHeaderValues = JsonConvert.DeserializeObject<int[]>(selectedHeaderAreaJsonArray);

                    if (model.FileFormat.ToLower() == "topdown")
                    {
                        model.NumberOfHeaders = (areaHeaderValues[3]) - (areaHeaderValues[1]) + 1;
                    }

                    if (model.FileFormat.ToLower() == "leftright")
                    {
                        model.NumberOfHeaders = (areaHeaderValues[2]) - (areaHeaderValues[0]) + 1;
                    }
                }

                if (TaskManager.Bus.ContainsKey(EasyUploadTaskManager.SHEET_DATA_AREA))
                {
                    List<string> selectedDataAreaJsonArray = (List<String>)TaskManager.Bus[EasyUploadTaskManager.SHEET_DATA_AREA];
                    List<int[]> areaDataValuesList = new List<int[]>();
                    model.NumberOfData = 0;
                    foreach (string jsonArray in selectedDataAreaJsonArray)
                    {
                        areaDataValuesList.Add(JsonConvert.DeserializeObject<int[]>(jsonArray));
                    }
                    foreach (int[] areaDataValues in areaDataValuesList)
                    {
                        if (model.FileFormat.ToLower() == "leftright")
                        {
                            model.NumberOfData += (areaDataValues[3]) - (areaDataValues[1]) + 1;
                        }

                        if (model.FileFormat.ToLower() == "topdown")
                        {
                            model.NumberOfData += (areaDataValues[2]) - (areaDataValues[0]) + 1;
                        }
                    }

                }

                return PartialView("EasyUploadSummary", model);
            }
            finally
            {
                msm.Dispose();
            }
        }

        [HttpPost]
        public ActionResult Summary(object[] data)
        {
            MetadataStructureManager msm = new MetadataStructureManager();
            try
            {

                TaskManager = (EasyUploadTaskManager)Session["TaskManager"];
                EasyUploadSummaryModel model = new EasyUploadSummaryModel();

                model.StepInfo = TaskManager.Current();
                model.ErrorList = FinishUpload(TaskManager);


                if (model.ErrorList.Count > 0)
                {
                    #region Populate model with data from the TaskManager
                    if (TaskManager.Bus.ContainsKey(EasyUploadTaskManager.FILENAME))
                    {
                        model.DatasetTitle = Convert.ToString(TaskManager.Bus[EasyUploadTaskManager.FILENAME]);
                    }

                    if (TaskManager.Bus.ContainsKey(EasyUploadTaskManager.SCHEMA))
                    {

                        long id = Convert.ToInt64(TaskManager.Bus[EasyUploadTaskManager.SCHEMA]);
                        model.MetadataSchemaTitle = msm.Repo.Get(m => m.Id == id).FirstOrDefault().Name;
                    }

                    if (TaskManager.Bus.ContainsKey(EasyUploadTaskManager.SHEET_FORMAT))
                    {
                        model.FileFormat = TaskManager.Bus[EasyUploadTaskManager.SHEET_FORMAT].ToString();
                    }

                    if (TaskManager.Bus.ContainsKey(EasyUploadTaskManager.SHEET_HEADER_AREA))
                    {
                        string selectedHeaderAreaJsonArray = TaskManager.Bus[EasyUploadTaskManager.SHEET_HEADER_AREA].ToString();
                        int[] areaHeaderValues = JsonConvert.DeserializeObject<int[]>(selectedHeaderAreaJsonArray);

                        if (model.FileFormat.ToLower() == "topdown")
                        {
                            model.NumberOfHeaders = (areaHeaderValues[3]) - (areaHeaderValues[1]) + 1;
                        }

                        if (model.FileFormat.ToLower() == "leftright")
                        {
                            model.NumberOfHeaders = (areaHeaderValues[2]) - (areaHeaderValues[0]) + 1;
                        }
                    }

                    if (TaskManager.Bus.ContainsKey(EasyUploadTaskManager.SHEET_DATA_AREA))
                    {
                        List<string> selectedDataAreaJsonArray = (List<string>)TaskManager.Bus[EasyUploadTaskManager.SHEET_DATA_AREA];
                        List<int[]> areaDataValuesList = new List<int[]>();
                        foreach (string area in selectedDataAreaJsonArray)
                        {
                            areaDataValuesList.Add(JsonConvert.DeserializeObject<int[]>(area));
                        }

                        foreach (int[] areaDataValues in areaDataValuesList)
                        {
                            if (model.FileFormat.ToLower() == "leftright")
                            {
                                model.NumberOfData = (areaDataValues[3]) - (areaDataValues[1]) + 1;
                            }

                            if (model.FileFormat.ToLower() == "topdown")
                            {
                                model.NumberOfData = (areaDataValues[2]) - (areaDataValues[0]) + 1;
                            }
                        }

                    }

                    #endregion
                    return PartialView("EasyUploadSummary", model);

                }
                else
                {
                    return null;
                }
            }
            finally
            {
                msm.Dispose();
            }
        }

        //[MeasurePerformance]
        //temporary solution: norman
        //For original solution, look into Aquadiva Code
        public List<Error> FinishUpload(EasyUploadTaskManager taskManager)
        {
            DataStructureManager dsm = new DataStructureManager();
            DatasetManager dm = new DatasetManager();
            DataContainerManager dam = new DataContainerManager();
            EntityPermissionManager entityPermissionManager = new EntityPermissionManager();

            List<Error> temp = new List<Error>();
            try
            {
                using (IUnitOfWork unitOfWork = this.GetUnitOfWork())
                {

                    // initialize all necessary manager

                    DataTuple[] rows = null;

                    //First, try to validate - if there are errors, return immediately
                    string JsonArray = TaskManager.Bus[EasyUploadTaskManager.SHEET_JSON_DATA].ToString();
                    List<Error> ValidationErrors = ValidateRows(JsonArray);
                    if (ValidationErrors.Count != 0)
                    {
                        temp.AddRange(ValidationErrors);
                        return temp;
                    }

                    string timestamp = DateTime.UtcNow.ToString("r");
                    string title = Convert.ToString(TaskManager.Bus[EasyUploadTaskManager.FILENAME]);

                    if (TaskManager.Bus.ContainsKey(EasyUploadTaskManager.DESCRIPTIONTITLE))
                    {
                        string tmp = Convert.ToString(TaskManager.Bus[EasyUploadTaskManager.DESCRIPTIONTITLE]);
                        if (!String.IsNullOrWhiteSpace(tmp))
                        {
                            title = Convert.ToString(TaskManager.Bus[EasyUploadTaskManager.DESCRIPTIONTITLE]);
                        }
                    }

                    StructuredDataStructure sds = dsm.CreateStructuredDataStructure(title, title + " " + timestamp, "", "", DataStructureCategory.Generic);

                    TaskManager.AddToBus(EasyUploadTaskManager.DATASTRUCTURE_ID, sds.Id);
                    TaskManager.AddToBus(EasyUploadTaskManager.DATASTRUCTURE_TITLE, title + " " + timestamp);

                    if (!TaskManager.Bus.ContainsKey(EasyUploadTaskManager.DATASET_TITLE))
                    {
                        TaskManager.AddToBus(EasyUploadTaskManager.DATASET_TITLE, title);
                        TaskManager.AddToBus(EasyUploadTaskManager.TITLE, title);
                    }

                    MetadataStructure metadataStructure = null;
                    if (TaskManager.Bus.ContainsKey(EasyUploadTaskManager.SCHEMA))
                    {
                        long metadataStructureId = Convert.ToInt64(TaskManager.Bus[EasyUploadTaskManager.SCHEMA]);
                        metadataStructure = unitOfWork.GetReadOnlyRepository<MetadataStructure>()
                            .Get(m => m.Id == metadataStructureId).FirstOrDefault();
                    }
                    else
                    {
                        //Default option but shouldn't happen because previous steps can't be finished without selecting the metadata-structure
                        metadataStructure = unitOfWork.GetReadOnlyRepository<MetadataStructure>()
                            .Get(m => m.Name.ToLower().Contains("eml")).FirstOrDefault();
                    }
                    ResearchPlan rp = unitOfWork.GetReadOnlyRepository<ResearchPlan>().Get().FirstOrDefault();
                    TaskManager.AddToBus(EasyUploadTaskManager.RESEARCHPLAN_ID, rp.Id);
                    TaskManager.AddToBus(EasyUploadTaskManager.RESEARCHPLAN_TITLE, rp.Title);

                    #region Progress Information

                    if (TaskManager.Bus.ContainsKey(EasyUploadTaskManager.CURRENTPACKAGESIZE))
                    {
                        TaskManager.Bus[EasyUploadTaskManager.CURRENTPACKAGESIZE] = 0;
                    }
                    else
                    {
                        TaskManager.Bus.Add(EasyUploadTaskManager.CURRENTPACKAGESIZE, 0);
                    }

                    if (TaskManager.Bus.ContainsKey(EasyUploadTaskManager.CURRENTPACKAGE))
                    {
                        TaskManager.Bus[EasyUploadTaskManager.CURRENTPACKAGE] = 0;
                    }
                    else
                    {
                        TaskManager.Bus.Add(EasyUploadTaskManager.CURRENTPACKAGE, 0);
                    }

                    #endregion

                    #region DataStructure
                    XmlDocument xmldoc = new XmlDocument();
                    XmlElement extraElement = xmldoc.CreateElement("extra");
                    XmlElement orderElement = xmldoc.CreateElement("order");


                    List<EasyUploadVariableInformation> MappedHeaders = (List<EasyUploadVariableInformation>)TaskManager.Bus[EasyUploadTaskManager.VERIFICATION_MAPPEDHEADERUNITS];
                    //Sorting necessary to prevent problems when inserting the tuples
                    MappedHeaders.Sort((head1, head2) => head1.headerId.CompareTo(head2.headerId));
                    List<VariableIdentifier> identifiers = new List<VariableIdentifier>();

                    var dataTypeRepo = unitOfWork.GetReadOnlyRepository<DataType>();
                    var unitRepo = unitOfWork.GetReadOnlyRepository<Unit>();
                    var dataAttributeRepo = unitOfWork.GetReadOnlyRepository<DataAttribute>();

                    List<DataAttribute> allDataAttributes = dataAttributeRepo.Get().ToList();

                    //CreatedVariables: <List<Tuple<headerId, Variable>>
                    List<Tuple<int, Variable>> createdVariables = new List<Tuple<int, Variable>>();

                    foreach (EasyUploadVariableInformation Entry in MappedHeaders)
                    {
                        int i = MappedHeaders.IndexOf(Entry);

                        DataType dataType = dataTypeRepo.Get(Entry.unitInfo.SelectedDataTypeId);
                        Unit CurrentSelectedUnit = unitRepo.Get(Entry.unitInfo.UnitId);

                        DataAttribute CurrentDataAttribute = new DataAttribute();
                        //If possible, map the chosen variable name, unit and datatype to an existing DataAttribute (Exact match)
                        DataAttribute existingDataAttribute = allDataAttributes.Where(da => da.Name.ToLower().Equals(TrimAndLimitString(Entry.variableName).ToLower()) &&
                                                                                            da.DataType.Id == dataType.Id &&
                                                                                            da.Unit.Id == CurrentSelectedUnit.Id).FirstOrDefault();
                        if (existingDataAttribute != null)
                        {
                            CurrentDataAttribute = existingDataAttribute;
                        }
                        else
                        {
                            //No matching DataAttribute => Create a new one
                            CurrentDataAttribute = dam.CreateDataAttribute(TrimAndLimitString(Entry.variableName), Entry.variableName, "", false, false, "", MeasurementScale.Categorial, DataContainerType.ReferenceType, "", dataType, CurrentSelectedUnit, null, null, null, null, null, null);
                        }

                        Variable newVariable = dsm.AddVariableUsage(sds, CurrentDataAttribute, true, Entry.variableName, "", "", "");
                        createdVariables.Add(Tuple.Create(Entry.headerId, newVariable));
                        VariableIdentifier vi = new VariableIdentifier
                        {
                            name = newVariable.Label,
                            id = newVariable.Id
                        };
                        identifiers.Add(vi);

                        XmlElement newVariableXml = xmldoc.CreateElement("variable");
                        newVariableXml.InnerText = Convert.ToString(newVariable.Id);

                        orderElement.AppendChild(newVariableXml);
                    }
                    extraElement.AppendChild(orderElement);
                    xmldoc.AppendChild(extraElement);

                    sds.Extra = xmldoc;
                    sds.Name = "generated import structure " + timestamp;
                    sds.Description = "automatically generated structured data structure by user " + GetUsernameOrDefault() + " for file " + title + " on " + timestamp;

                    #endregion

                    Dataset ds = null;
                    ds = dm.CreateEmptyDataset(sds, rp, metadataStructure);

                    long datasetId = ds.Id;
                    long sdsId = sds.Id;


                    if (dm.IsDatasetCheckedOutFor(datasetId, GetUsernameOrDefault()) || dm.CheckOutDataset(datasetId, GetUsernameOrDefault()))
                    {
                        DatasetVersion dsv = dm.GetDatasetWorkingCopy(datasetId);
                        long METADATASTRUCTURE_ID = metadataStructure.Id;
                        XmlMetadataWriter xmlMetadatWriter = new XmlMetadataWriter(XmlNodeMode.xPath);
                        XDocument metadataX = xmlMetadatWriter.CreateMetadataXml(METADATASTRUCTURE_ID);
                        XmlDocument metadataXml = XmlMetadataWriter.ToXmlDocument(metadataX);
                        dsv.Metadata = metadataXml;
                        try
                        {
                            dsv.Metadata = xmlDatasetHelper.SetInformation(dsv, metadataXml, NameAttributeValues.title, title);
                        }
                        catch (NullReferenceException ex)
                        {
                            //Reference of the title node is missing
                            throw new NullReferenceException("The extra-field of this metadata-structure is missing the title-node-reference!");
                        }
                        dm.EditDatasetVersion(dsv, null, null, null);
                    }


                    #region security/permissions
                    if (GetUsernameOrDefault() != "DEFAULT")
                    {
                        UserPiManager upm = new UserPiManager();

                        //Full permissions for the user
                        entityPermissionManager.Create<User>(GetUsernameOrDefault(), "Dataset", typeof(Dataset), ds.Id, Enum.GetValues(typeof(RightType)).Cast<RightType>().ToList());

                        //Get PIs of the current user
                        List<User> piList = upm.GetPisFromUserByName(GetUsernameOrDefault()).ToList();
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
                                        RightType.Read,
                                        RightType.Download
                                });
                            }
                        }
                    }
                    #endregion security


                    #region excel reader

                    int packageSize = 10000;
                    //HACK ?
                    TaskManager.Bus[EasyUploadTaskManager.CURRENTPACKAGESIZE] = packageSize;

                    int counter = 0;

                    dm.CheckOutDatasetIfNot(ds.Id, GetUsernameOrDefault()); // there are cases, the dataset does not get checked out!!
                    if (!dm.IsDatasetCheckedOutFor(ds.Id, GetUsernameOrDefault()))
                    {
                        throw new Exception(string.Format("Not able to checkout dataset '{0}' for  user '{1}'!", ds.Id, GetUsernameOrDefault()));
                    }

                    DatasetVersion workingCopy = dm.GetDatasetWorkingCopy(ds.Id);

                    counter++;
                    TaskManager.Bus[EasyUploadTaskManager.CURRENTPACKAGE] = counter;

                    //rows = reader.ReadFile(Stream, TaskManager.Bus[TaskManager.FILENAME].ToString(), oldSds, (int)id, packageSize).ToArray();

                    List<string> selectedDataAreaJsonArray = (List<string>)TaskManager.Bus[EasyUploadTaskManager.SHEET_DATA_AREA];
                    string selectedHeaderAreaJsonArray = TaskManager.Bus[EasyUploadTaskManager.SHEET_HEADER_AREA].ToString();
                    List<int[]> areaDataValuesList = new List<int[]>();
                    foreach (string area in selectedDataAreaJsonArray)
                    {
                        areaDataValuesList.Add(JsonConvert.DeserializeObject<int[]>(area));
                    }
                    int[] areaHeaderValues = JsonConvert.DeserializeObject<int[]>(selectedHeaderAreaJsonArray);

                    Orientation orientation = 0;

                    switch (TaskManager.Bus[EasyUploadTaskManager.SHEET_FORMAT].ToString())
                    {
                        case "LeftRight":
                            orientation = Orientation.rowwise;
                            break;
                        case "Matrix":
                            //orientation = Orientation.matrix;
                            break;
                        default:
                            orientation = Orientation.columnwise;
                            break;
                    }

                    String worksheetUri = null;
                    //Get the Uri to identify the correct worksheet
                    if (TaskManager.Bus.ContainsKey(EasyUploadTaskManager.ACTIVE_WOKSHEET_URI))
                    {
                        worksheetUri = TaskManager.Bus[EasyUploadTaskManager.ACTIVE_WOKSHEET_URI].ToString();
                    }

                    int batchSize = (new Object()).GetUnitOfWork().PersistenceManager.PreferredPushSize;
                    int batchnr = 1;
                    foreach (int[] areaDataValues in areaDataValuesList)
                    {
                        //First batch starts at the start of the current data area
                        int currentBatchStartRow = areaDataValues[0] + 1;
                        while (currentBatchStartRow <= areaDataValues[2] + 1) //While the end of the current data area has not yet been reached
                        {
                            //Create a new reader each time because the reader saves ALL tuples it read and therefore the batch processing wouldn't work
                            EasyUploadExcelReader reader = new EasyUploadExcelReader();
                            // open file
                            Stream = reader.Open(TaskManager.Bus[EasyUploadTaskManager.FILEPATH].ToString());

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

                            //Set variable identifiers because they might differ from the variable names in the file
                            reader.setSubmittedVariableIdentifiers(identifiers);

                            //Read the rows and convert them to DataTuples
                            rows = reader.ReadFile(Stream, TaskManager.Bus[EasyUploadTaskManager.FILENAME].ToString(), fri, sds, (int)datasetId, worksheetUri);

                            //After reading the rows, add them to the dataset
                            if (rows != null)
                                dm.EditDatasetVersion(workingCopy, rows.ToList(), null, null);

                            //Close the Stream so the next ExcelReader can open it again
                            Stream.Close();

                            //Debug information
                            int lines = (areaDataValues[2] + 1) - (areaDataValues[0] + 1);
                            int batches = lines / batchSize;
                            batchnr++;

                            //Next batch starts after the current one
                            currentBatchStartRow = currentBatchEndRow + 1;
                        }

                    }

                    #endregion


                    dm.CheckInDataset(ds.Id, "upload data from upload wizard", GetUsernameOrDefault());

                    #region Persist annotations

                    if (this.IsAccessibale("AAM", "Annotation", "CreateAnnotation"))
                    {
                        /* Annotations stored on the bus in form of a dictionary
                        * Key: Tuple<headerID, category> Value: conceptURI
                        * Category is currently only "Entity" or "Characteristic"
                        * */
                        Dictionary<Tuple<int, string>, Tuple<string, Boolean>> annotations = null;
                        if (TaskManager.Bus.ContainsKey(EasyUploadTaskManager.ANNOTATIONMAPPING))
                        {
                            //Get the selected annotations from the bus
                            annotations = (Dictionary<Tuple<int, string>, Tuple<string, Boolean>>)TaskManager.Bus[EasyUploadTaskManager.ANNOTATIONMAPPING];
                        }

                        #region Handle case "No matching concept found"
                        /*If the user stated that he didn't find an entity or characteristic, we should just set it to NULL
                        This will create partial or empty annotations that we can easily edit when we add the terms to the ontology
                        Also we log those occurences into a file so we'll know that terms are missing in the ontology
                        */
                        if (TaskManager.Bus.ContainsKey(EasyUploadTaskManager.NOCONCEPTSFOUND))
                        {
                            List<Tuple<int, String>> noConceptsFoundList = (List<Tuple<int, String>>)TaskManager.Bus[EasyUploadTaskManager.NOCONCEPTSFOUND];
                            foreach(Tuple<int, String> noConceptsFound in noConceptsFoundList)
                            {
                                annotations[noConceptsFound] = null; //Sets the Entity or Characteristic to null

                                //Grab all information that we want to use for logging
                                StringBuilder sb = new StringBuilder();
                                DataTypeManager dtm = new DataTypeManager();
                                sb.Append("User " + this.GetUsernameOrDefault() + " could not find " + noConceptsFound.Item2 + " for the following variable:\n");
                                EasyUploadVariableInformation currentHeader = MappedHeaders.Where(mh => mh.headerId == noConceptsFound.Item1).FirstOrDefault();
                                if(currentHeader != null)
                                {
                                    sb.Append("\tVariable Name: ");
                                    sb.Append(currentHeader.variableName);
                                    sb.Append("\n");
                                    sb.Append("\tUnit: ");
                                    sb.Append(currentHeader.unitInfo.Name);
                                    sb.Append("\n");
                                    sb.Append("\tDataType: ");
                                    sb.Append(dtm.Repo.Get(currentHeader.unitInfo.SelectedDataTypeId).Name);
                                    sb.Append("\n");
                                    sb.Append("\n");
                                }

                                using (StreamWriter writer = new StreamWriter(MissingConceptsLoggingPath, true))
                                {
                                    writer.WriteLine(sb.ToString());
                                }
                            }
                        }
                        #endregion

                        //First I have to build a structure that contains the Entity and the Characteristic for each headerId
                        //So the new structure will be Dictionary<headerId, EntityCharacteristicPair>
                        Dictionary<int, EntityCharacteristicPair> annotationsPerHeaderId = new Dictionary<int, EntityCharacteristicPair>();
                        foreach (KeyValuePair<Tuple<int, string>, Tuple<string, Boolean>> kvp in annotations)
                        {
                            if(kvp.Value != null) //kvp.Value is null if the user selected "no concept found" without ever selecting an option in the first place
                            {
                                //If we didn't find annotations for this headerId yet, create a dummy that will be filled in the next step
                                if (!annotationsPerHeaderId.ContainsKey(kvp.Key.Item1))
                                {
                                    annotationsPerHeaderId.Add(kvp.Key.Item1, new EntityCharacteristicPair());
                                }
                                //Now we know there's at least a dummy and we can add entity or characteristic, depending on what we currently have in our iteration
                                if (kvp.Key.Item2 == "Entity")
                                {
                                    annotationsPerHeaderId[kvp.Key.Item1].mappedEntityURI = kvp.Value.Item1;
                                }
                                else if (kvp.Key.Item2 == "Characteristic")
                                {
                                    annotationsPerHeaderId[kvp.Key.Item1].mappedCharacteristicURI = kvp.Value.Item1;
                                }
                            }
                        }

                        //Now we can create and persist the annotation for each headerId (=Variable)
                        foreach (KeyValuePair<int, EntityCharacteristicPair> kvp in annotationsPerHeaderId)
                        {
                            String entityLabel = null;
                            String characteristicLabel = null;

                            if (this.IsAccessibale("DDM", "SemanticSearch", "FindOntologyLabels"))
                            {
                                List<String> uris = new List<string>() { kvp.Value.mappedEntityURI, kvp.Value.mappedCharacteristicURI };
                                ContentResult labelResult = (ContentResult)this.Run("DDM", "SemanticSearch", "FindOntologyLabels", new RouteValueDictionary()
                                {
                                    {"serializedURIList", JsonConvert.SerializeObject(uris) }
                                });
                                List<String> labels = JsonConvert.DeserializeObject<List<String>>(labelResult.Content);

                                //We should get exactly two labels, one for the characteristic and one for the entity
                                if(labels.Count == 2)
                                {
                                    entityLabel = labels.ElementAt(0);
                                    characteristicLabel = labels.ElementAt(1);
                                }
                                else {
                                    throw new Exception("Incorrect number of labels!");
                                }
                            }

                            var unicorn = this.Run("AAM", "Annotation", "CreateAnnotationWithoutStandard", new RouteValueDictionary()
                            {
                                {"DatasetId", ds.Id },
                                {"DatasetVersionId", dm.GetDatasetLatestVersionId(ds.Id) },
                                //CreatedVariables: <List<Tuple<headerId, Variable>>
                                {"Variable", createdVariables.Where(v => v.Item1 == kvp.Key).FirstOrDefault().Item2 },
                                {"Entity", kvp.Value.mappedEntityURI },
                                {"EntityLabel", entityLabel },
                                {"CharacteristicLabel", characteristicLabel },
                                {"Characteristic", kvp.Value.mappedCharacteristicURI }
                            });
                            Debug.WriteLine(unicorn);
                        }
                    }
                    #endregion

                    //Reindex search
                    if (this.IsAccessibale("DDM", "SearchIndex", "ReIndexSingle"))
                    {

                        this.Run("DDM", "SearchIndex", "ReIndexSingle", new RouteValueDictionary() { { "id", datasetId } });
                    }

                    TaskManager.AddToBus(EasyUploadTaskManager.DATASET_ID, ds.Id);

                    return temp;
                }
            }
            catch (Exception ex)
            {
                temp.Add(new Error(ErrorType.Other, "An error occured during the upload. " +
                    "Please try again later. If this problem keeps occuring, please contact your administrator."));
                return temp;
            }
            finally
            {
                dsm.Dispose();
                dm.Dispose();
                dam.Dispose();
                //sm.Dispose();
                entityPermissionManager.Dispose();
            }
        }

        #region private methods

        public string GetUsernameOrDefault()
        {
            string username = string.Empty;
            try
            {
                username = HttpContext.User.Identity.Name;
            }
            catch { }

            return !string.IsNullOrWhiteSpace(username) ? username : "DEFAULT";
        }

        /*
         * Removes whitespaces at the beginning and end of the string
         * and limits the string to 255 characters
         * */
        private string TrimAndLimitString(string str, int limit = 255)
        {
            if (str != "" && str != null)
            {
                str = str.Trim();
                if (str.Length > limit)
                    str = str.Substring(0, limit);
            }
            return (str);
        }

        /// <summary>
        /// Determin whether the selected datatypes are suitable
        /// </summary>
        private List<Error> ValidateRows(string JsonArray)
        {
            DataTypeManager dtm = new DataTypeManager();


            try
            {
                const int maxErrorsPerColumn = 20;

                TaskManager = (EasyUploadTaskManager)Session["TaskManager"];

                string[][] DeserializedJsonArray = JsonConvert.DeserializeObject<string[][]>(JsonArray);

                List<Error> ErrorList = new List<Error>();
                List<EasyUploadVariableInformation> MappedHeaders = (List<EasyUploadVariableInformation>)TaskManager.Bus[EasyUploadTaskManager.VERIFICATION_MAPPEDHEADERUNITS];
                EasyUploadVariableInformation[] MappedHeadersArray = MappedHeaders.ToArray();


                List<string> DataArea = (List<string>)TaskManager.Bus[EasyUploadTaskManager.SHEET_DATA_AREA];
                List<int[]> IntDataAreaList = new List<int[]>();
                foreach (string area in DataArea)
                {
                    IntDataAreaList.Add(JsonConvert.DeserializeObject<int[]>(area));
                }

                foreach (int[] IntDataArea in IntDataAreaList)
                {
                    string[,] SelectedDataArea = new string[(IntDataArea[2] - IntDataArea[0]), (IntDataArea[3] - IntDataArea[1])];

                    for (int x = IntDataArea[1]; x <= IntDataArea[3]; x++)
                    {
                        int errorsInColumn = 0;
                        for (int y = IntDataArea[0]; y <= IntDataArea[2]; y++)
                        {
                            int SelectedY = y - (IntDataArea[0]);
                            int SelectedX = x - (IntDataArea[1]);
                            string vv = DeserializedJsonArray[y][x];

                            EasyUploadVariableInformation mappedHeader = MappedHeaders.Where(t => t.headerId == SelectedX).FirstOrDefault();

                            DataType datatype = null;

                            if (mappedHeader.unitInfo.SelectedDataTypeId == -1)
                            {
                                datatype = dtm.Repo.Get(mappedHeader.unitInfo.DataTypeInfos.FirstOrDefault().DataTypeId);
                            }
                            else
                            {
                                datatype = dtm.Repo.Get(mappedHeader.unitInfo.SelectedDataTypeId);
                            }

                            string datatypeName = datatype.SystemType;


                            DataTypeCheck dtc;
                            double DummyValue = 0;
                            if (Double.TryParse(vv, out DummyValue))
                            {
                                if (vv.Contains("."))
                                {
                                    dtc = new DataTypeCheck(mappedHeader.variableName, datatypeName, DecimalCharacter.point);
                                }
                                else
                                {
                                    dtc = new DataTypeCheck(mappedHeader.variableName, datatypeName, DecimalCharacter.comma);
                                }
                            }
                            else
                            {
                                dtc = new DataTypeCheck(mappedHeader.variableName, datatypeName, DecimalCharacter.point);
                            }

                            var ValidationResult = dtc.Execute(vv, y);
                            if (ValidationResult is Error)
                            {
                                ErrorList.Add((Error)ValidationResult);
                                errorsInColumn++;
                            }

                            if (errorsInColumn >= maxErrorsPerColumn)
                            {
                                //Break inner (row) loop to jump to the next column
                                break;
                            }
                        }
                    }
                }

                return ErrorList;
            }
            finally
            {
                dtm.Dispose();
            }
        }


        #endregion
    }
}