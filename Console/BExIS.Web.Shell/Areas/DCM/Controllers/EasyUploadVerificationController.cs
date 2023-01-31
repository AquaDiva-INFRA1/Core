using BExIS.Aam.Entities.Mapping;
using BExIS.Aam.Services;
using BExIS.Dcm.UploadWizard;
using BExIS.Dcm.Wizard;
using BExIS.Dlm.Entities.DataStructure;
using BExIS.Dlm.Services.DataStructure;
using BExIS.IO;
using BExIS.IO.Transform.Validation.Exceptions;
using BExIS.IO.Transform.Validation.ValueCheck;
using BExIS.Modules.Dcm.UI.Helpers;
using BExIS.Modules.Dcm.UI.Models;
using BExIS.Utils.Models;
using F23.StringSimilarity;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI.WebControls;
using Vaiona.Persistence.Api;
using Vaiona.Web.Mvc;

namespace BExIS.Modules.Dcm.UI.Controllers
{
    public class EasyUploadVerificationController : BaseController
    {
        private EasyUploadTaskManager TaskManager;

        [HttpGet]
        public ActionResult Verification(int index)
        {

            TaskManager = (EasyUploadTaskManager)Session["TaskManager"];

            List<Dlm.Entities.DataStructure.Unit> tempUnitList = new List<Dlm.Entities.DataStructure.Unit>();
            List<DataType> allDataypes = new List<DataType>();
            List<DataAttribute> allDataAttributes = new List<DataAttribute>();

            using (IUnitOfWork unitOfWork = this.GetUnitOfWork())
            {

                //set current stepinfo based on index
                if (TaskManager != null)
                {
                    TaskManager.SetCurrent(index);

                    // remove if existing
                    TaskManager.RemoveExecutedStep(TaskManager.Current());
                }

                SelectVerificationModel model = new SelectVerificationModel();
                List<DataTypeInfo> dataTypeInfos = new List<DataTypeInfo>();
                List<UnitInfo> unitInfos = new List<UnitInfo>();
                List<DataAttrInfo> dataAttributeInfos = new List<DataAttrInfo>();
                List<EasyUploadSuggestion> suggestions = new List<EasyUploadSuggestion>();
                List<string> headers = new List<string>();


                tempUnitList = unitOfWork.GetReadOnlyRepository<Dlm.Entities.DataStructure.Unit>().Get().ToList();
                allDataypes = unitOfWork.GetReadOnlyRepository<DataType>().Get().ToList();
                allDataAttributes = unitOfWork.GetReadOnlyRepository<DataAttribute>().Get().ToList();


                //Important for jumping back to this step
                if (TaskManager.Bus.ContainsKey(EasyUploadTaskManager.ROWS))
                {
                    model.Rows = (List<RowModel>)TaskManager.Bus[EasyUploadTaskManager.ROWS];
                }

                // get all DataTypes for each Units
                foreach (Dlm.Entities.DataStructure.Unit unit in tempUnitList)
                {
                    UnitInfo unitInfo = new UnitInfo();

                    unitInfo.UnitId = unit.Id;
                    unitInfo.Description = unit.Description;
                    unitInfo.Name = unit.Name;
                    unitInfo.Abbreviation = unit.Abbreviation;
                    unitInfo.DimensionId = unit.Dimension.Id;

                    if (unit.Name.ToLower() == "none")
                    {
                        foreach (DataType fullDataType in allDataypes)
                        {
                            DataTypeInfo dataTypeInfo = new DataTypeInfo();
                            dataTypeInfo.DataTypeId = fullDataType.Id;
                            dataTypeInfo.Description = fullDataType.Description;
                            dataTypeInfo.Name = fullDataType.Name;

                            unitInfo.DataTypeInfos.Add(dataTypeInfo);
                        }

                        unitInfos.Add(unitInfo);
                    }
                    else
                    {
                        Boolean hasDatatype = false; //Make sure only units that have at least one datatype are shown

                        foreach (DataType dummyDataType in unit.AssociatedDataTypes)
                        {
                            if (!hasDatatype)
                                hasDatatype = true;

                            DataTypeInfo dataTypeInfo = new DataTypeInfo();

                            DataType fullDataType = allDataypes.Where(p => p.Id == dummyDataType.Id).FirstOrDefault();
                            dataTypeInfo.DataTypeId = fullDataType.Id;
                            dataTypeInfo.Description = fullDataType.Description;
                            dataTypeInfo.Name = fullDataType.Name;

                            unitInfo.DataTypeInfos.Add(dataTypeInfo);
                        }
                        if (hasDatatype)
                            unitInfos.Add(unitInfo);
                    }
                }

                //Sort the units by name
                unitInfos.Sort(delegate (UnitInfo u1, UnitInfo u2)
                {
                    return String.Compare(u1.Name, u2.Name, StringComparison.InvariantCultureIgnoreCase);
                });


                TaskManager.AddToBus(EasyUploadTaskManager.VERIFICATION_AVAILABLEUNITS, unitInfos);

                // all datatypesinfos 
                dataTypeInfos = unitInfos.SelectMany(u => u.DataTypeInfos).GroupBy(d => d.DataTypeId).Select(g => g.Last()).ToList();
                TaskManager.AddToBus(EasyUploadTaskManager.ALL_DATATYPES, dataTypeInfos);


                //Setall Data AttrInfos to Session -> default
                
                foreach (DataAttribute d in allDataAttributes)
                {
                    try
                    {
                        DataAttrInfo dainfo = new DataAttrInfo(d.Id, d.Unit.Id, d.DataType.Id, d.Description, d.Name, d.Unit.Dimension.Id);
                        dataAttributeInfos.Add(dainfo);
                    }
                    catch (Exception ex)
                    {
                        ViewData["errors"] = ViewData["errors"] + " - " + ex.Message;
                    }
                }
                
                //allDataAttributes.ForEach(d => dataAttributeInfos.Add(new DataAttrInfo(d.Id, d.Unit.Id, d.DataType.Id, d.Description, d.Name, d.Unit.Dimension.Id)));
                Session["DataAttributes"] = dataAttributeInfos;



                string filePath = TaskManager.Bus[EasyUploadTaskManager.FILEPATH].ToString();
                string selectedHeaderAreaJson = TaskManager.Bus[EasyUploadTaskManager.SHEET_HEADER_AREA].ToString();

                FileStream fis = null;
                fis = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                string sheetFormatString = Convert.ToString(TaskManager.Bus[EasyUploadTaskManager.SHEET_FORMAT]);
                SheetFormat sheetFormat = 0;
                Enum.TryParse<SheetFormat>(sheetFormatString, true, out sheetFormat);
                if (TaskManager.Bus[EasyUploadTaskManager.EXTENTION].ToString() != ".csv")
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (ExcelPackage ep = new ExcelPackage(fis))
                    {
                        try
                        {
                            fis.Close();
                            ExcelWorkbook excelWorkbook = ep.Workbook;
                            ExcelWorksheet firstWorksheet = excelWorkbook.Worksheets[1];
                            headers = GetExcelHeaderFields(firstWorksheet, sheetFormat, selectedHeaderAreaJson);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
                else
                {
                    headers = GetExcelHeaderFields(sheetFormat, selectedHeaderAreaJson);
                }

                headers = makeHeaderUnique(headers);

                suggestions = new List<EasyUploadSuggestion>();
                Aam_Dataset_column_annotationManager amm_manager = new Aam_Dataset_column_annotationManager();

                if (!model.Rows.Any())
                {
                    foreach (string varName in headers)
                    {
                        #region suggestions

                            //Add a variable to the suggestions if the names are similar
                            suggestions = getSuggestions(varName, dataAttributeInfos);

                            #endregion

                        //set rowmodel
                        RowModel row = new RowModel(
                            headers.IndexOf(varName),
                            varName,
                            null,
                            null,
                            null,
                            suggestions,
                            unitInfos,
                            dataAttributeInfos,
                            dataTypeInfos,
                            amm_manager.get_all_dataset_column_annotationBy_Variable_measures(varName),
                            amm_manager.get_all_dataset_column_annotationByVariable_label_matching(varName)
                            );

                        model.Rows.Add(row);

                        TaskManager.AddToBus(EasyUploadTaskManager.ROWS, model.Rows);
                        TaskManager.AddToBus(EasyUploadTaskManager.VERIFICATION_MAPPEDHEADERUNITS, RowsToTuples());
                    }
                }

                amm_manager.Dispose();
                TaskManager.AddToBus(EasyUploadTaskManager.VERIFICATION_MAPPEDHEADERUNITS, headers);

                model.StepInfo = TaskManager.Current();

                return PartialView(model);

            
            }
        }

        [HttpPost]
        public ActionResult Verification(object[] data)
        {
            TaskManager = (EasyUploadTaskManager)Session["TaskManager"];
            SelectVerificationModel model = new SelectVerificationModel();
            model.StepInfo = TaskManager.Current();

            //convert all rowmodels to tuples for the next steps
            if (TaskManager != null)
            {
                TaskManager.Current().SetValid(false);

                if (TaskManager.Bus.ContainsKey(EasyUploadTaskManager.ROWS))
                {
                    List<RowModel> rows = (List<RowModel>)TaskManager.Bus[EasyUploadTaskManager.ROWS];
                    model.Rows = rows;
                    bool allSelectionMade = true;

                    foreach (var r in rows)
                    {
                        if (r.SelectedDataAttribute == null ||
                            r.SelectedDataType == null ||
                            r.SelectedUnit == null)
                        {
                            allSelectionMade = false;
                            model.ErrorList.Add(new Error(ErrorType.Other, "Some Areas are not selected."));
                            break;
                        }
                    }

                    TaskManager.Current().SetValid(allSelectionMade);
                }
                else
                {
                    model.ErrorList.Add(new Error(ErrorType.Other, "Some Areas are not selected."));
                }

                if (TaskManager.Current().valid == true)
                {
                    TaskManager.AddExecutedStep(TaskManager.Current());
                    TaskManager.GoToNext();
                    Session["TaskManager"] = TaskManager;
                    ActionInfo actionInfo = TaskManager.Current().GetActionInfo;
                    return RedirectToAction(actionInfo.ActionName, actionInfo.ControllerName, new RouteValueDictionary { { "area", actionInfo.AreaName }, { "index", TaskManager.GetCurrentStepInfoIndex() } });
                }
                else
                {
                    TaskManager.Current().SetStatus(StepStatus.error);

                    //reload model
                    model.StepInfo = TaskManager.Current();
                }
            }


            return PartialView(model);
        }

        [HttpPost]
        public ActionResult SaveSelection(int index, long selectedUnit, long selectedDataType, long selectedAttribute, string varName, string lastSelection, string selectedEntity, string selectedCharachteristic)
        {
            /**
             * if selectedAttribute == -1 == Unknown
                  selectedAttribute == -2 ==  Not found
             * 
             */


            List<DataTypeInfo> dataTypeInfos = new List<DataTypeInfo>();
            List<UnitInfo> unitInfos = new List<UnitInfo>();
            List<DataAttrInfo> dataAttributeInfos = new List<DataAttrInfo>();
            List<EasyUploadSuggestion> suggestions = new List<EasyUploadSuggestion>();
            List<string> headers = new List<string>();

            List<Aam_Dataset_column_annotation> annot_list_by_variable_unit_type = new List<Aam_Dataset_column_annotation>();
            Dictionary<Aam_Uri, double> annot_Dict_by_variable_string_similarities = new Dictionary<Aam_Uri, double>();

            EasyUploadTaskManager TaskManager = (EasyUploadTaskManager)Session["TaskManager"];

            //suggestions

            #region load all lists

            //dataattr
            if (Session["DataAttributes"] != null)
            {
                dataAttributeInfos = (List<DataAttrInfo>)Session["DataAttributes"];
            }

            if (!string.IsNullOrEmpty(varName))
            {
                suggestions = getSuggestions(varName, dataAttributeInfos);
            }

            unitInfos = (List<UnitInfo>)TaskManager.Bus[EasyUploadTaskManager.VERIFICATION_AVAILABLEUNITS];

            dataTypeInfos = (List<DataTypeInfo>)TaskManager.Bus[EasyUploadTaskManager.ALL_DATATYPES];

            #endregion


            #region load current seleted items

            UnitInfo currentUnit = unitInfos.FirstOrDefault(u => u.UnitId == selectedUnit);
            DataTypeInfo currentDataTypeInfo = dataTypeInfos.FirstOrDefault(d => d.DataTypeId.Equals(selectedDataType));

            DataAttrInfo currentDataAttrInfo = null;
            if (selectedAttribute > 0) currentDataAttrInfo = dataAttributeInfos.FirstOrDefault(da => da.Id.Equals(selectedAttribute));
            if (selectedAttribute == -1) currentDataAttrInfo = new DataAttrInfo(-1, 0, 0, "Unknown", "Unknow", 1);
            if (selectedAttribute == -2) currentDataAttrInfo = new DataAttrInfo(-2, 0, 0, "Not found", "Not found", 1); ;

            Aam_UriManager aam_manag = new Aam_UriManager();
            Aam_Uri selected_entity = aam_manag.get_Aam_Uri_by_id(Int64.Parse(selectedEntity));
            Aam_Uri selected_char = aam_manag.get_Aam_Uri_by_id(Int64.Parse(selectedCharachteristic));
            aam_manag.Dispose();

            #endregion

            #region filtering



            if (currentUnit != null)
            {
                unitInfos = unitInfos.Where(u => u.UnitId.Equals(currentUnit.UnitId) || u.DimensionId.Equals(currentUnit.DimensionId)).ToList();

                // filtering data attrs where the attr has the unit or there dimension
                if (selectedAttribute == 0) dataAttributeInfos = dataAttributeInfos.Where(d => d.UnitId.Equals(currentUnit.UnitId) || d.DimensionId.Equals(currentUnit.DimensionId)).ToList();
                else dataAttributeInfos = dataAttributeInfos.Where(d => d.Id.Equals(currentDataAttrInfo.Id)).ToList();

                if (selectedDataType == 0) dataTypeInfos = dataTypeInfos.Where(d => currentUnit.DataTypeInfos.Any(ud => ud.DataTypeId.Equals(d.DataTypeId))).ToList();
                else
                    dataTypeInfos = dataTypeInfos.Where(dt => dt.DataTypeId.Equals(currentDataTypeInfo.DataTypeId)).ToList();
            }

            if (currentDataTypeInfo != null)
            {
                dataTypeInfos = dataTypeInfos.Where(dt => dt.DataTypeId.Equals(currentDataTypeInfo.DataTypeId)).ToList();

                if (selectedAttribute == 0) dataAttributeInfos = dataAttributeInfos.Where(d => d.DataTypeId.Equals(currentDataTypeInfo.DataTypeId)).ToList();
                else dataAttributeInfos = dataAttributeInfos.Where(d => d.Id.Equals(currentDataAttrInfo.Id)).ToList();

                if (selectedUnit == 0) unitInfos = unitInfos.Where(u => u.DataTypeInfos.Any(d => d.DataTypeId.Equals(currentDataTypeInfo.DataTypeId))).ToList();
                else unitInfos.Where(u => u.UnitId.Equals(currentUnit.UnitId) || u.DimensionId.Equals(currentUnit.DimensionId)).ToList();
            }


            if (currentDataAttrInfo != null)
            {
                // is the seletced currentDataAttrInfo a suggestion then overrigth all selected items
                if (currentDataAttrInfo.Id > 0)
                {
                    #region existing selected dataset

                    if (suggestions.Any(s => s.attributeName.Equals(currentDataAttrInfo.Name)))
                    {
                        dataAttributeInfos = dataAttributeInfos.Where(d => d.Id.Equals(currentDataAttrInfo.Id)).ToList();
                        unitInfos = unitInfos.Where(u => u.UnitId.Equals(currentDataAttrInfo.UnitId) || u.DimensionId.Equals(currentDataAttrInfo.DimensionId)).ToList();
                        dataTypeInfos = unitInfos.SelectMany(u => u.DataTypeInfos).GroupBy(d => d.DataTypeId).Select(g => g.Last()).ToList();

                        if (lastSelection == "Suggestions")
                        {
                            currentUnit = unitInfos.FirstOrDefault(u => u.UnitId.Equals(dataAttributeInfos.First().UnitId));
                            currentDataTypeInfo = dataTypeInfos.FirstOrDefault(d => d.DataTypeId.Equals(dataAttributeInfos.First().DataTypeId));
                        }

                    }
                    else
                    {

                        dataAttributeInfos = dataAttributeInfos.Where(d => d.Id.Equals(currentDataAttrInfo.Id)).ToList();

                        //filtering units when data attr is selected, if id or dimension is the same
                        if (selectedUnit == 0)
                        {
                            unitInfos = unitInfos.Where(u => u.UnitId.Equals(currentDataAttrInfo.UnitId) || u.DimensionId.Equals(currentDataAttrInfo.DimensionId)).ToList();
                            currentUnit = unitInfos.FirstOrDefault(u => u.UnitId.Equals(currentDataAttrInfo.UnitId));
                        }
                        else unitInfos = unitInfos.Where(u => u.UnitId.Equals(currentUnit.UnitId) || u.DimensionId.Equals(currentUnit.DimensionId)).ToList();

                        if (selectedDataType == 0)
                        {
                            dataTypeInfos = unitInfos.SelectMany(u => u.DataTypeInfos).GroupBy(d => d.DataTypeId).Select(g => g.Last()).ToList();

                            currentDataTypeInfo = dataTypeInfos.FirstOrDefault(d => d.DataTypeId.Equals(currentDataAttrInfo.DataTypeId));

                        }
                        else dataTypeInfos = dataTypeInfos.Where(dt => dt.DataTypeId.Equals(currentDataTypeInfo.DataTypeId)).ToList();
                    }

                    #endregion existing selected dataset
                }
                else
                if (currentDataAttrInfo.Id == -1) //unknow
                {
                    // do nothing
                }
                else
                if (currentDataAttrInfo.Id == -2) //not found
                {
                    // do nothing
                }
            }


            #region update the annotations with every selection
            string temp_var_name_from_suggestions = (currentDataAttrInfo != null) ? currentDataAttrInfo.Name.ToLower() : varName; // if current Data Attribute has changed selection (currentDataAttrInfo != null)
            Aam_Dataset_column_annotationManager amm_manager = new Aam_Dataset_column_annotationManager();
            annot_list_by_variable_unit_type = amm_manager.get_all_dataset_column_annotationBy_Variable_measures(temp_var_name_from_suggestions);
            annot_Dict_by_variable_string_similarities = amm_manager.get_all_dataset_column_annotationByVariable_label_matching(temp_var_name_from_suggestions);
            amm_manager.Dispose();
            #endregion
            #endregion


            RowModel model = new RowModel(
                    index,
                    varName,
                    currentDataAttrInfo,
                    currentUnit,
                    currentDataTypeInfo,
                    suggestions,
                    unitInfos,
                    dataAttributeInfos,
                    dataTypeInfos,

                    annot_list_by_variable_unit_type,
                    annot_Dict_by_variable_string_similarities
                );
            model.selected_entity = (selected_entity != null) ? selected_entity : new Aam_Uri();
            model.selected_charac = (selected_char != null) ? selected_char : new Aam_Uri();
            TaskManager.AddToBus(EasyUploadTaskManager.ANNOTATION_ENTITY, selected_entity);
            TaskManager.AddToBus(EasyUploadTaskManager.ANNOTATION_CHARACHTERISTIC, selected_entity);
            //update row in the bus of the taskmanager
            UpdateRowInBus(model);

            return PartialView("Row", model);

        }

        private void UpdateRowInBus(RowModel row)
        {
            TaskManager = (EasyUploadTaskManager)Session["TaskManager"];

            if (TaskManager != null && TaskManager.Bus[EasyUploadTaskManager.ROWS] != null)
            {
                List<RowModel> rows = (List<RowModel>)TaskManager.Bus[EasyUploadTaskManager.ROWS];
                if (rows.Any(r => r.Index.Equals(row.Index)))
                {
                    for (int i = 0; i < rows.Count; i++)
                    {
                        RowModel tmp = rows.ElementAt(i);
                        if (tmp.Index.Equals(row.Index))
                        {
                            rows[i] = row;
                            break;
                        }
                    }
                }

                TaskManager.AddToBus(EasyUploadTaskManager.ROWS, rows);
            }

        }

        #region excel stuff

        /// <summary>
        /// 
        /// </summary>
        /// <param name="excelWorksheet"></param>
        /// <param name="sheetFormat"></param>
        /// <param name="selectedAreaJsonArray"></param>
        /// <returns></returns>
        private List<String> GetExcelHeaderFields(ExcelWorksheet excelWorksheet, SheetFormat sheetFormat, string selectedAreaJsonArray)
        {
            List<String> headerValues = new List<string>();

            int[] areaValues = JsonConvert.DeserializeObject<int[]>(selectedAreaJsonArray);

            if (areaValues.Length != 4)
            {
                throw new InvalidOperationException("Given JSON string for selected area got an invalid length of [" + Convert.ToString(areaValues.Length) + "]");
            }

            SheetArea selectedArea = new SheetArea(areaValues[1], areaValues[3], areaValues[0], areaValues[2]);


            switch (sheetFormat)
            {
                case SheetFormat.TopDown:
                    headerValues = GetExcelHeaderFieldsTopDown(excelWorksheet, selectedArea);
                    break;
                case SheetFormat.LeftRight:
                    headerValues = GetExcelHeaderFieldsLeftRight(excelWorksheet, selectedArea);
                    break;
                case SheetFormat.Matrix:
                    headerValues.AddRange(GetExcelHeaderFieldsTopDown(excelWorksheet, selectedArea));
                    headerValues.AddRange(GetExcelHeaderFieldsLeftRight(excelWorksheet, selectedArea));
                    break;
                default:
                    break;
            }

            return headerValues;
        }

        /// <summary>
        /// Gets all values from selected header area. This method is for top to down scheme, so the header fields are in one row
        /// </summary>
        /// <param name="excelWorksheet">ExcelWorksheet with the data</param>
        /// <param name="selectedArea">Defined header area with start and end for rows and columns</param>
        /// <returns>Simple list with values of the header fields as string</returns>
        private List<String> GetExcelHeaderFieldsTopDown(ExcelWorksheet excelWorksheet, SheetArea selectedArea)
        {
            List<String> headerValues = new List<string>();

            String jsonTableString = TaskManager.Bus[EasyUploadTaskManager.SHEET_JSON_DATA].ToString();
            String[][] jsonTable = JsonConvert.DeserializeObject<string[][]>(jsonTableString);

            for (int i = selectedArea.StartColumn; i <= selectedArea.EndColumn; i++)
            {
                headerValues.Add(jsonTable[selectedArea.StartRow][i]);
            }

            return headerValues;
        }

        /// <summary>
        /// Gets all values from selected header area. This method is for left to right scheme, so the header fields are in one coulumn
        /// </summary>
        /// <param name="excelWorksheet">ExcelWorksheet with the data</param>
        /// <param name="selectedArea">Defined header area with start and end for rows and columns</param>
        /// <returns>Simple list with values of the header fields as string</returns>
        private List<String> GetExcelHeaderFieldsLeftRight(ExcelWorksheet excelWorksheet, SheetArea selectedArea)
        {
            List<String> headerValues = new List<string>();

            String jsonTableString = TaskManager.Bus[EasyUploadTaskManager.SHEET_JSON_DATA].ToString();
            String[][] jsonTable = JsonConvert.DeserializeObject<string[][]>(jsonTableString);

            for (int row = selectedArea.StartRow; row <= selectedArea.EndColumn; row++)
            {
                headerValues.Add(jsonTable[row][selectedArea.StartColumn]);
            }

            return headerValues;
        }

        #endregion

        /*
         * Validates each Data row and returns a JSON-Object with the errors (if there are any)
         * */
        [HttpPost]
        public ActionResult ValidateSelection()
        {
            TaskManager = (EasyUploadTaskManager)Session["TaskManager"];

            string JsonArray = TaskManager.Bus[EasyUploadTaskManager.SHEET_JSON_DATA].ToString();

            List<Tuple<int, Error>> ErrorList = ValidateRows(JsonArray);
            List<Tuple<int, ErrorInfo>> ErrorMessageList = new List<Tuple<int, ErrorInfo>>();

            foreach (Tuple<int, Error> error in ErrorList)
            {
                ErrorMessageList.Add(new Tuple<int, ErrorInfo>(error.Item1, new ErrorInfo(error.Item2)));
            }

            return Json(new { errors = ErrorMessageList.ToArray(), errorCount = (ErrorList.Count) });
        }

        #region private methods

        /// <summary>
        /// Determin whether the selected datatypes are suitable
        /// </summary>
        private List<Tuple<int, Error>> ValidateRows(string JsonArray)
        {
            DataTypeManager dtm = new DataTypeManager();

            try
            {
                const int maxErrorsPerColumn = 20;
                TaskManager = (EasyUploadTaskManager)Session["TaskManager"];

                string[][] DeserializedJsonArray = JsonConvert.DeserializeObject<string[][]>(JsonArray);

                List<Tuple<int, Error>> ErrorList = new List<Tuple<int, Error>>();
                List<RowModel> Rows = (List<RowModel>)TaskManager.Bus[EasyUploadTaskManager.ROWS];
                RowModel[] MappedRowsArray = Rows.ToArray();


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

                            RowModel mappedHeader = MappedRowsArray.Where(t => t.Index == SelectedX).FirstOrDefault();

                            DataType datatype = null;
                            if (mappedHeader.SelectedDataType != null && mappedHeader.SelectedDataAttribute != null)
                            {
                                datatype = dtm.Repo.Get(mappedHeader.SelectedDataType.DataTypeId);
                                string datatypeName = datatype.SystemType;

                                #region DataTypeCheck
                                DataTypeCheck dtc;
                                double DummyValue = 0;
                                if (Double.TryParse(vv, out DummyValue))
                                {
                                    if (vv.Contains("."))
                                    {
                                        dtc = new DataTypeCheck(mappedHeader.SelectedDataAttribute.Name, datatypeName, DecimalCharacter.point);
                                    }
                                    else
                                    {
                                        dtc = new DataTypeCheck(mappedHeader.SelectedDataAttribute.Name, datatypeName, DecimalCharacter.comma);
                                    }
                                }
                                else
                                {
                                    dtc = new DataTypeCheck(mappedHeader.SelectedDataAttribute.Name, datatypeName, DecimalCharacter.point);
                                }
                                #endregion

                                var ValidationResult = dtc.Execute(vv, y);
                                if (ValidationResult is Error)
                                {
                                    ErrorList.Add(new Tuple<int, Error>(SelectedX, (Error)ValidationResult));
                                    errorsInColumn++;
                                }

                                if (errorsInColumn >= maxErrorsPerColumn)
                                {
                                    //Break inner (row) loop to jump to the next column
                                    break;
                                }
                            }
                            else
                            {
                                ErrorList.Add(new Tuple<int, Error>(SelectedX, new Error(ErrorType.Other, "No datatype or data attribute selected in row " + SelectedX + 1)));
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
        
        private List<Tuple<int, String, UnitInfo>> RowsToTuples()
        {
            List<Tuple<int, String, UnitInfo>> tmp = new List<Tuple<int, string, UnitInfo>>();
            TaskManager = (EasyUploadTaskManager)Session["TaskManager"];

            if (TaskManager != null && TaskManager.Bus[EasyUploadTaskManager.ROWS] != null)
            {
                List<RowModel> rows = (List<RowModel>)TaskManager.Bus[EasyUploadTaskManager.ROWS];

                foreach (var row in rows)
                {
                    tmp.Add(RowToTuple(row));
                }
            }

            return tmp;
        }

        private Tuple<int, String, UnitInfo> RowToTuple(RowModel row)
        {
            return new Tuple<int, string, UnitInfo>((int)row.Index, row.Name, row.SelectedUnit);
        }

        private List<string> makeHeaderUnique(List<string> header)
        {
            List<string> temp = new List<string>();

            foreach (string s in header)
            {
                if (temp.Contains(s))
                {
                    string tmp;
                    int i = 1;
                    do
                    {
                        tmp = s + " (" + i + ")";
                        i++;
                    }
                    while (temp.Contains(tmp));
                    temp.Add(tmp);
                }
                else
                {
                    temp.Add(s);
                }
            }
            return (temp);
        }

        /// <summary>
        /// Combines multiple String-similarities with equal weight
        /// </summary>
        private double similarity(string a, string b)
        {
            List<double> similarities = new List<double>();
            double output = 0.0;

            var l = new NormalizedLevenshtein();
            similarities.Add(l.Similarity(a, b));
            var jw = new JaroWinkler();
            similarities.Add(jw.Similarity(a, b));
            var jac = new Jaccard();
            similarities.Add(jac.Similarity(a, b));

            foreach (double sim in similarities)
            {
                output += sim;
            }

            return output / similarities.Count;
        }

        private List<EasyUploadSuggestion> getSuggestions(string varName, List<DataAttrInfo> allDataAttributes)
        {
            #region suggestions
            //Add a variable to the suggestions if the names are similar
            List<EasyUploadSuggestion> suggestions = new List<EasyUploadSuggestion>();

            //Calculate similarity metric
            //Accept suggestion if the similarity is greater than some threshold
            double threshold = 0.4;
            IEnumerable<DataAttrInfo> suggestionAttrs = allDataAttributes.Where(att => similarity(att.Name, varName) >= threshold);

            //Order the suggestions according to the similarity
            List<DataAttrInfo> ordered = suggestionAttrs.ToList<DataAttrInfo>();
            ordered.Sort((x, y) => (similarity(y.Name, varName)).CompareTo(similarity(x.Name, varName)));

            //Add the ordered suggestions to the model
            foreach (DataAttrInfo att in ordered)
            {
                suggestions.Add(new EasyUploadSuggestion(att.Id, att.Name, att.UnitId, att.DataTypeId, true));
            }

            //Use the following to order suggestions alphabetically instead of ordering according to the metric
            //model.Suggestions[i] = model.Suggestions[i].Distinct().OrderBy(s => s.attributeName).ToList<EasyUploadSuggestion>();

            //Each Name-Unit-Datatype-Tuple should be unique
            suggestions = suggestions.Distinct().ToList<EasyUploadSuggestion>();
            #endregion

            return suggestions;
        }

        /// <summary>
        /// Store the information about not found concepts in the Bus.
        /// Structure in the bus: List<(headerfieldId, category)>
        /// </summary>
        /// <param name="headerfieldId"></param>
        /// <param name="category"></param>
        /// <param name="checkboxChecked"></param>
        /// <returns></returns>
        [HttpPost]
        public Boolean NoConceptFound(int headerfieldId, string category, Boolean checkboxChecked)
        {
            EasyUploadTaskManager TaskManager = (EasyUploadTaskManager)Session["TaskManager"];
            if (!TaskManager.Bus.ContainsKey(EasyUploadTaskManager.NOCONCEPTSFOUND))
            {
                TaskManager.AddToBus(EasyUploadTaskManager.NOCONCEPTSFOUND, new List<Tuple<int, String>>());
            }

            if (checkboxChecked)
            {
                //The checkbox was checked so add the new information to the bus
                List<Tuple<int, String>> unicorn = (List<Tuple<int, String>>)TaskManager.Bus[EasyUploadTaskManager.NOCONCEPTSFOUND];
                unicorn.Add(new Tuple<int, string>(headerfieldId, category));
                TaskManager.Bus[EasyUploadTaskManager.NOCONCEPTSFOUND] = unicorn;
            }
            else
            {
                //The checkbox was unchecked, remove the respective tuple from our bus
                List<Tuple<int, String>> unicorn = (List<Tuple<int, String>>)TaskManager.Bus[EasyUploadTaskManager.NOCONCEPTSFOUND];
                unicorn.RemoveAll(tuple => tuple.Item1 == headerfieldId && tuple.Item2 == category);
                TaskManager.Bus[EasyUploadTaskManager.NOCONCEPTSFOUND] = unicorn;
            }
            Session["TaskManager"] = TaskManager;
            return true;
        }

        private List<String> GetExcelHeaderFields(SheetFormat sheetFormat, string selectedAreaJsonArray)
        {
            List<String> headerValues = new List<string>();

            int[] areaValues = JsonConvert.DeserializeObject<int[]>(selectedAreaJsonArray);

            if (areaValues.Length != 4)
            {
                throw new InvalidOperationException("Given JSON string for selected area got an invalid length of [" + Convert.ToString(areaValues.Length) + "]");
            }

            SheetArea selectedArea = new SheetArea(areaValues[1], areaValues[3], areaValues[0], areaValues[2]);


            switch (sheetFormat)
            {
                case SheetFormat.TopDown:
                    headerValues = GetExcelHeaderFieldsTopDown(selectedArea);
                    break;
                case SheetFormat.LeftRight:
                    headerValues = GetExcelHeaderFieldsLeftRight(selectedArea);
                    break;
                case SheetFormat.Matrix:
                    headerValues.AddRange(GetExcelHeaderFieldsTopDown(selectedArea));
                    headerValues.AddRange(GetExcelHeaderFieldsLeftRight(selectedArea));
                    break;
                default:
                    break;
            }

            return headerValues;
        }
        private List<String> GetExcelHeaderFieldsTopDown(SheetArea selectedArea)
        {
            List<String> headerValues = new List<string>();

            String jsonTableString = TaskManager.Bus[EasyUploadTaskManager.SHEET_JSON_DATA].ToString();
            String[][] jsonTable = JsonConvert.DeserializeObject<string[][]>(jsonTableString);

            for (int i = selectedArea.StartColumn; i <= selectedArea.EndColumn; i++)
            {
                headerValues.Add(jsonTable[selectedArea.StartRow][i]);
            }

            return headerValues;
        }
        private List<String> GetExcelHeaderFieldsLeftRight(SheetArea selectedArea)
        {
            List<String> headerValues = new List<string>();

            String jsonTableString = TaskManager.Bus[EasyUploadTaskManager.SHEET_JSON_DATA].ToString();
            String[][] jsonTable = JsonConvert.DeserializeObject<string[][]>(jsonTableString);

            for (int row = selectedArea.StartRow; row <= selectedArea.EndColumn; row++)
            {
                headerValues.Add(jsonTable[row][selectedArea.StartColumn]);
            }

            return headerValues;
        }
        #endregion
    }
}