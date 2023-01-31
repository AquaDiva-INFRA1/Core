using BExIS.Dcm.UploadWizard;
using BExIS.Dcm.Wizard;
using BExIS.IO.Transform.Validation.Exceptions;
using BExIS.Modules.Dcm.UI.Models;
using BExIS.UI.Helpers;
using BExIS.Utils.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Routing;
using Vaiona.Logging;

namespace BExIS.Modules.Dcm.UI.Controllers
{
    public class EasyUploadSelectAreasController : Controller
    {
        private EasyUploadTaskManager TaskManager;

        [HttpGet]
        public ActionResult SelectAreas(int index)
        {
            TaskManager = (EasyUploadTaskManager)Session["TaskManager"];

            //set current stepinfo based on index
            if (TaskManager != null)
            {
                TaskManager.SetCurrent(index);

                // remove if existing
                TaskManager.RemoveExecutedStep(TaskManager.Current());
            }

            //Use the given file and the given sheet format to create a json-table
            string filePath = TaskManager.Bus[EasyUploadTaskManager.FILEPATH].ToString();
            FileStream fis = null;
            string jsonTable = "[]";
            List<String[]> Json_table = new List<string[]>();

            SelectAreasModel model = new SelectAreasModel();

            try
            {
                string fileExt = System.IO.Path.GetExtension(filePath);
                if ((fileExt.ToLower().Contains("csv")) || (fileExt.ToLower().Contains("txt")))
                {
                    //fis = new FileStream(Parse_csv_to_xlsx(filePath), FileMode.Open, FileAccess.Read);
                    // generating the JSONTABLEARRAY to select areas

                    String[] all_lines = System.IO.File.ReadAllLines(filePath);

                    foreach (string line in all_lines)
                    {
                        String[] tabs = Regex.Split(line, TaskManager.Bus[EasyUploadTaskManager.CSV_DELIMITER].ToString());
                        Json_table.Add(tabs);
                    }
                    //If the active worksheet was never changed, we default to the first one
                    string activeWorksheet;
                    if (!TaskManager.Bus.ContainsKey(EasyUploadTaskManager.ACTIVE_WOKSHEET_URI))
                    {
                        activeWorksheet = filePath;
                        TaskManager.AddToBus(EasyUploadTaskManager.ACTIVE_WOKSHEET_URI, activeWorksheet);
                    }
                    else
                    {
                        activeWorksheet = TaskManager.Bus[EasyUploadTaskManager.ACTIVE_WOKSHEET_URI].ToString();
                    }

                    //Save the worksheet uris to the model
                    Dictionary<Uri, string> _dict = new Dictionary<Uri, string>();
                    _dict.Add(new Uri(filePath), "Sheet");
                    model.SheetUriDictionary = _dict;

                    TaskManager.AddToBus(EasyUploadTaskManager.SHEET_JSON_DATA, JsonConvert.SerializeObject(Json_table.ToArray()));

                    //Add uri of the active sheet to the model to be able to preselect the correct option in the dropdown
                    model.activeSheetUri = activeWorksheet;
                }
                else
                {
                    //FileStream for the users file
                    fis = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                    //Grab the sheet format from the bus
                    string sheetFormatString = Convert.ToString(TaskManager.Bus[EasyUploadTaskManager.SHEET_FORMAT]);
                    SheetFormat CurrentSheetFormat = 0;
                    Enum.TryParse<SheetFormat>(sheetFormatString, true, out CurrentSheetFormat);

                    //Transforms the content of the file into a 2d-json-array
                    JsonTableGenerator EUEReader = new JsonTableGenerator(fis);
                    //If the active worksheet was never changed, we default to the first one
                    string activeWorksheet;
                    if (!TaskManager.Bus.ContainsKey(EasyUploadTaskManager.ACTIVE_WOKSHEET_URI))
                    {
                        activeWorksheet = EUEReader.GetFirstWorksheetUri().ToString();
                        TaskManager.AddToBus(EasyUploadTaskManager.ACTIVE_WOKSHEET_URI, activeWorksheet);
                    }
                    else
                    {
                        activeWorksheet = TaskManager.Bus[EasyUploadTaskManager.ACTIVE_WOKSHEET_URI].ToString();
                    }
                    //Generate the table for the active worksheet
                    jsonTable = EUEReader.GenerateJsonTable(CurrentSheetFormat, activeWorksheet);

                    //Save the worksheet uris to the model
                    model.SheetUriDictionary = EUEReader.GetWorksheetUris();

                    if (!String.IsNullOrEmpty(jsonTable))
                    {
                        TaskManager.AddToBus(EasyUploadTaskManager.SHEET_JSON_DATA, jsonTable);
                    }

                    //Add uri of the active sheet to the model to be able to preselect the correct option in the dropdown
                    model.activeSheetUri = activeWorksheet;
                }
            }
            catch (Exception ex)
            {
                LoggerFactory.LogCustom(ex.Message);
            }
            finally
            {
                if (fis != null)
                {
                    fis.Close();
                }
            }

            // Check if the areas have already been selected, if yes, use them (Important when jumping back to this step)
            if (TaskManager.Bus.ContainsKey(EasyUploadTaskManager.SHEET_DATA_AREA))
            {
                model.DataArea = (List<string>)TaskManager.Bus[EasyUploadTaskManager.SHEET_DATA_AREA];
            }
            if (TaskManager.Bus.ContainsKey(EasyUploadTaskManager.SHEET_HEADER_AREA))
            {
                model.HeaderArea = TaskManager.Bus[EasyUploadTaskManager.SHEET_HEADER_AREA].ToString();
            }

            model.StepInfo = TaskManager.Current();

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult SelectAreas(object[] data)
        {
            TaskManager = (EasyUploadTaskManager)Session["TaskManager"];
            SelectAreasModel model = new SelectAreasModel();
            model.StepInfo = TaskManager.Current();

            if (TaskManager != null)
            {
                TaskManager.Current().SetValid(false);

                //If Header and data areas have been selected, you can proceed with the next step
                if (TaskManager.Bus.ContainsKey(EasyUploadTaskManager.SHEET_JSON_DATA) &&
                    TaskManager.Bus.ContainsKey(EasyUploadTaskManager.SHEET_DATA_AREA) &&
                    TaskManager.Bus.ContainsKey(EasyUploadTaskManager.SHEET_HEADER_AREA))
                {
                    bool isJsonDataEmpty = String.IsNullOrEmpty(Convert.ToString(TaskManager.Bus[EasyUploadTaskManager.SHEET_JSON_DATA]));
                    bool isDataAreaEmpty = ((List<string>)TaskManager.Bus[EasyUploadTaskManager.SHEET_DATA_AREA]).Count == 0;
                    bool isHeadAreaEmpty = String.IsNullOrEmpty(Convert.ToString(TaskManager.Bus[EasyUploadTaskManager.SHEET_HEADER_AREA]));

                    if (!isJsonDataEmpty && !isDataAreaEmpty && !isHeadAreaEmpty)
                    {
                        TaskManager.Current().SetValid(true);
                    }
                    else
                    {
                        model.ErrorList.Add(new Error(ErrorType.Other, "Some Areas are not selected."));
                    }
                }
                else //Else stay on the same page, display an error
                {
                    model.ErrorList.Add(new Error(ErrorType.Other, "Some Areas are not selected."));
                }

                if (TaskManager.Current().valid == true) //Redirect if the state is valid
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
                    if (TaskManager.Bus.ContainsKey(EasyUploadTaskManager.SHEET_DATA_AREA))
                    {
                        model.DataArea = (List<string>)TaskManager.Bus[EasyUploadTaskManager.SHEET_DATA_AREA];
                    }

                    if (TaskManager.Bus.ContainsKey(EasyUploadTaskManager.SHEET_HEADER_AREA))
                    {
                        model.HeaderArea = TaskManager.Bus[EasyUploadTaskManager.SHEET_HEADER_AREA].ToString();
                    }

                    #region Generate sheet-list and table for active sheet

                    //Grab the sheet format from the bus
                    string sheetFormatString = Convert.ToString(TaskManager.Bus[EasyUploadTaskManager.SHEET_FORMAT]);
                    SheetFormat CurrentSheetFormat = 0;
                    Enum.TryParse<SheetFormat>(sheetFormatString, true, out CurrentSheetFormat);

                    //Open the users file
                    string filePath = TaskManager.Bus[EasyUploadTaskManager.FILEPATH].ToString();
                    FileStream fis = null;
                    string jsonTable = "{}";
                    using (fis = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        //Generate the Sheet-List and grab the active worksheet
                        JsonTableGenerator EUEReader = new JsonTableGenerator(fis);
                        //If the active worksheet was never changed, we default to the first one
                        string activeWorksheet;
                        if (!TaskManager.Bus.ContainsKey(EasyUploadTaskManager.ACTIVE_WOKSHEET_URI))
                        {
                            activeWorksheet = EUEReader.GetFirstWorksheetUri().ToString();
                            TaskManager.AddToBus(EasyUploadTaskManager.ACTIVE_WOKSHEET_URI, activeWorksheet);
                        }
                        else
                        {
                            activeWorksheet = TaskManager.Bus[EasyUploadTaskManager.ACTIVE_WOKSHEET_URI].ToString();
                        }
                        //Generate the table for the active worksheet
                        jsonTable = EUEReader.GenerateJsonTable(CurrentSheetFormat, activeWorksheet);

                        //Save the worksheet uris to the model
                        model.SheetUriDictionary = EUEReader.GetWorksheetUris();

                        if (!String.IsNullOrEmpty(jsonTable))
                        {
                            TaskManager.AddToBus(EasyUploadTaskManager.SHEET_JSON_DATA, jsonTable);
                        }

                        //Add uri of the active sheet to the model to be able to preselect the correct option in the dropdown
                        model.activeSheetUri = activeWorksheet;

                        #endregion Generate sheet-list and table for active sheet

                        model.StepInfo = TaskManager.Current();
                    }
                }
            }

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult ChangeWorksheet(string sheetIdentifier)
        {
            TaskManager = (EasyUploadTaskManager)Session["TaskManager"];

            #region Reset selected units, datatypes and suggestions

            TaskManager.Bus.Remove(EasyUploadTaskManager.VERIFICATION_AVAILABLEUNITS);
            TaskManager.Bus.Remove(EasyUploadTaskManager.VERIFICATION_HEADERFIELDS);
            TaskManager.Bus.Remove(EasyUploadTaskManager.VERIFICATION_MAPPEDHEADERUNITS);
            TaskManager.Bus.Remove(EasyUploadTaskManager.VERIFICATION_ATTRIBUTESUGGESTIONS);
            TaskManager.Bus.Remove(EasyUploadTaskManager.ROWS);

            #endregion Reset selected units, datatypes and suggestions

            #region Generate table for selected sheet

            string filePath = TaskManager.Bus[EasyUploadTaskManager.FILEPATH].ToString();

            string fileExt = System.IO.Path.GetExtension(filePath);
            if ((fileExt.ToLower().Contains("csv")) || (fileExt.ToLower().Contains("txt")))
            {
                return Content((string)TaskManager.Bus[EasyUploadTaskManager.SHEET_JSON_DATA], "application/json");
            }

            FileStream fis = null;
            string jsonTable = "[]";

            try
            {
                //FileStream for the users file
                fis = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                //Grab the sheet format from the bus
                string sheetFormatString = Convert.ToString(TaskManager.Bus[EasyUploadTaskManager.SHEET_FORMAT]);
                SheetFormat CurrentSheetFormat = 0;
                Enum.TryParse<SheetFormat>(sheetFormatString, true, out CurrentSheetFormat);

                //Transforms the content of the file into a 2d-json-array
                JsonTableGenerator EUEReader = new JsonTableGenerator(fis);
                jsonTable = EUEReader.GenerateJsonTable(CurrentSheetFormat, sheetIdentifier);

                if (!String.IsNullOrEmpty(jsonTable))
                {
                    TaskManager.AddToBus(EasyUploadTaskManager.SHEET_JSON_DATA, jsonTable);
                }

                TaskManager.AddToBus(EasyUploadTaskManager.ACTIVE_WOKSHEET_URI, sheetIdentifier);
            }
            catch (Exception ex)
            {
                LoggerFactory.LogCustom(ex.Message);
            }
            finally
            {
                if (fis != null)
                {
                    fis.Close();
                }
            }

            #endregion Generate table for selected sheet

            //Send back the table-data
            return Content(jsonTable, "application/json");
        }

        /*
         * Save the selected area either as data area or as header area
         * */

        [HttpPost]
        public ActionResult SelectedAreaToBus()
        {
            string headerArea = null;
            string dataArea = null;

            foreach (string key in Request.Form.AllKeys)
            {
                if ("dataArea" == key)
                {
                    dataArea = Request.Form[key];
                }
                if ("headerArea" == key)
                {
                    headerArea = Request.Form[key];
                }
            }

            SelectAreasModel model = new SelectAreasModel();

            EasyUploadTaskManager TaskManager = (EasyUploadTaskManager)Session["TaskManager"];

            if (dataArea != null)
            {
                if (TaskManager.Bus.ContainsKey(EasyUploadTaskManager.SHEET_DATA_AREA))
                {
                    model.DataArea = (List<string>)TaskManager.Bus[EasyUploadTaskManager.SHEET_DATA_AREA];
                }

                //dataArea == "" means the resetButton was clicked
                if (model.DataArea == null || dataArea == "")
                {
                    model.DataArea = new List<string>();
                }
                if (dataArea != "")
                {
                    int[] newArea = JsonConvert.DeserializeObject<int[]>(dataArea);
                    Boolean contains = false;
                    foreach (string area in model.DataArea)
                    {
                        int[] oldArea = JsonConvert.DeserializeObject<int[]>(area);
                        //If one of the already selected areas contains the new one, don't add the new one to the selection (prevents duplicate selection)
                        if (oldArea[0] <= newArea[0] && oldArea[2] >= newArea[2] &&
                            oldArea[1] <= newArea[1] && oldArea[3] >= newArea[3])
                        {
                            contains = true;
                        }
                    }
                    if (!contains)
                    {
                        //If the new area contains one (or several) of the already selected areas, remove the old ones
                        for (int i = model.DataArea.Count - 1; i >= 0; i--)
                        {
                            int[] oldArea = JsonConvert.DeserializeObject<int[]>(model.DataArea[i]);

                            if (newArea[0] <= oldArea[0] && newArea[2] >= oldArea[2] &&
                                newArea[1] <= oldArea[1] && newArea[3] >= oldArea[3])
                            {
                                model.DataArea.RemoveAt(i);
                            }
                        }

                        //Insert the new area
                        model.DataArea.Add(dataArea);
                    }
                }

                TaskManager.AddToBus(EasyUploadTaskManager.SHEET_DATA_AREA, model.DataArea);
            }

            if (headerArea != null)
            {
                TaskManager.AddToBus(EasyUploadTaskManager.SHEET_HEADER_AREA, headerArea);
                model.HeaderArea = headerArea;
            }

            string filePath = TaskManager.Bus[EasyUploadTaskManager.FILEPATH].ToString();
            FileStream fis = null;

            try
            {
                string fileExt = System.IO.Path.GetExtension(filePath);
                if ((fileExt.ToLower().Contains("csv")) || (fileExt.ToLower().Contains("txt")))
                {
                    Dictionary<Uri, string> _dict = new Dictionary<Uri, string>();
                    _dict.Add(new Uri(filePath), "Sheet");
                    model.SheetUriDictionary = _dict;
                    model.activeSheetUri = filePath;
                }
                else
                {
                    //FileStream for the users file
                    fis = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                    JsonTableGenerator EUEReader = new JsonTableGenerator(fis);

                    //Get the worksheet uris and save them to the model
                    model.SheetUriDictionary = EUEReader.GetWorksheetUris();
                }

            }
            catch (Exception ex)
            {
                LoggerFactory.LogCustom(ex.Message);
            }
            finally
            {
                if (fis != null)
                {
                    fis.Close();
                }
            }

            Session["TaskManager"] = TaskManager;

            model.StepInfo = TaskManager.Current();

            return PartialView("SelectAreas", model);

        }

        /////////////////////////////////
        private string Parse_csv_to_xlsx(string filePath)
        {
            String output_path = Path.GetDirectoryName(filePath) +
                    Path.DirectorySeparatorChar +
                    Path.GetFileNameWithoutExtension(filePath) + ".xlsx";

            using (SpreadsheetDocument spreedDoc = SpreadsheetDocument.Create(output_path, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart wbPart = spreedDoc.WorkbookPart;
                if (wbPart == null)
                {
                    wbPart = spreedDoc.AddWorkbookPart();
                    wbPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();

                    wbPart.AddNewPart<SharedStringTablePart>();
                    wbPart.SharedStringTablePart.SharedStringTable = new SharedStringTable() { Count = 1, UniqueCount = 1 };

                    wbPart.SharedStringTablePart.SharedStringTable.AppendChild(new SharedStringItem(new Text("test")));
                    wbPart.SharedStringTablePart.SharedStringTable.Save();

                    WorkbookStylesPart stylesheet = wbPart.AddNewPart<WorkbookStylesPart>();
                    Stylesheet workbookstylesheet = new Stylesheet();
                    stylesheet.Stylesheet = workbookstylesheet;
                    stylesheet.Stylesheet.Save();
                }

                string sheetName = "sheet";
                WorksheetPart worksheetPart = null;
                worksheetPart = wbPart.AddNewPart<WorksheetPart>();
                var sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();

                worksheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);

                if (wbPart.Workbook.Sheets == null)
                {
                    wbPart.Workbook.AppendChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>(new DocumentFormat.OpenXml.Spreadsheet.Sheets());
                }

                var sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet()
                {
                    Id = wbPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = sheetName
                };

                var workingSheet = ((WorksheetPart)wbPart.GetPartById(sheet.Id)).Worksheet;

                String[] all_lines = System.IO.File.ReadAllLines(filePath);

                int rowindex = 1;
                foreach (string line in all_lines)
                {
                    DocumentFormat.OpenXml.Spreadsheet.Row row_ = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    row_.RowIndex = (UInt32)rowindex;

                    String[] tabs = line.Split(';');
                    foreach (string tab in tabs)
                    {
                        row_.AppendChild(AddCellWithText(tab));
                    }
                    sheetData.AppendChild(row_);
                    rowindex++;
                }
                wbPart.Workbook.Sheets.AppendChild(sheet);
                wbPart.Workbook.Save();
            }
            return output_path;
        }

        static Cell AddCellWithText(string text)
        {
            Cell c1 = new Cell();
            c1.DataType = CellValues.InlineString;

            InlineString inlineString = new InlineString();
            Text t = new Text();
            t.Text = text;

            inlineString.AppendChild(t);

            c1.AppendChild(inlineString);

            return c1;

        }
    }
}