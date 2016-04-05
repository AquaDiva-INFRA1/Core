using System;
using System.Web.Mvc;
using System.Web.Routing;
using BExIS.IO.Transform.Validation.Exceptions;
using BExIS.Dcm.UploadWizard;
using BExIS.Dcm.Wizard;
using BExIS.Web.Shell.Areas.DCM.Models;
using System.IO;
using OfficeOpenXml;

using System.Web.UI.WebControls;
using Vaiona.Logging;
using System.Collections.Generic;
using BExIS.Dlm.Entities.DataStructure;
using System.Web.Script.Serialization;

namespace BExIS.Web.Shell.Areas.DCM.Controllers
{
    public class SubmitVerificationController : Controller
    {
        private TaskManager TaskManager;

        //
        // GET: /DCM/SubmitSelectAreas/

        [HttpGet]
        public ActionResult Verification(int index)
        {
            TaskManager = (TaskManager)Session["TaskManager"];

            string filePath = TaskManager.Bus[TaskManager.FILEPATH].ToString();
            FileStream fis = null;
            string jsonTable = "{}";

            try {
                fis = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                ExcelPackage ep = new ExcelPackage(fis);

                ExcelWorkbook excelWorkbook = ep.Workbook;
                ExcelWorksheet firstWorksheet = excelWorkbook.Worksheets[1];

                ExcelCellAddress StartCell = firstWorksheet.Dimension.Start;
                ExcelCellAddress EndCell = firstWorksheet.Dimension.End;

                //jsonTable = GenerateJsonTable(fis);

                if (!String.IsNullOrEmpty(jsonTable))
                {
                    TaskManager.AddToBus(TaskManager.SHEET_JSON_DATA, jsonTable);
                }

            }
            catch (Exception ex)
            {
                LoggerFactory.LogCustom("Errormessage: " + ex.Message + "; Stacktrace:" + ex.StackTrace);
            }
            finally
            {
                if(fis != null)
                {
                    fis.Close();
                }
            }


            //set current stepinfo based on index
            if (TaskManager != null)
            {
                TaskManager.SetCurrent(index);

                // remove if existing
                TaskManager.RemoveExecutedStep(TaskManager.Current());
            }

            SelectAreasModel model = new SelectAreasModel();

            model.JsonTableData = jsonTable;

            // jump back to this step
            // check if dataset selected
            if (TaskManager.Bus.ContainsKey(TaskManager.SHEET_JSON_DATA))
            {
                if (!String.IsNullOrEmpty(Convert.ToString(TaskManager.Bus[TaskManager.SHEET_JSON_DATA])))
                {
                    model.JsonTableData = TaskManager.Bus[TaskManager.SHEET_JSON_DATA].ToString();
                }
            }

            model.StepInfo = TaskManager.Current();

            return PartialView(model);

        }

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

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            int[] areaValues = serializer.Deserialize<int[]>(selectedAreaJsonArray);

            if(areaValues.Length != 4)
            {
                throw new InvalidOperationException("Given JSON string for selected area got an invalid length of [" + Convert.ToString(areaValues.Length) + "]");
            }

            SheetArea selectedArea = new SheetArea(areaValues[1], areaValues[3], areaValues[0], areaValues[2]);
            

            switch (sheetFormat)
            {
                case SheetFormat.TopDown:
                    headerValues = GetExcelHeaderFieldsLeftRight(excelWorksheet, selectedArea);
                    break;
                case SheetFormat.LeftRight:
                    headerValues = GetExcelHeaderFieldsTopDown(excelWorksheet, selectedArea);
                    break;
                case SheetFormat.Matrix:
                    headerValues.AddRange(GetExcelHeaderFieldsLeftRight(excelWorksheet, selectedArea));
                    headerValues.AddRange(GetExcelHeaderFieldsTopDown(excelWorksheet, selectedArea));
                    break;
                default:
                    break;
            }

            return headerValues;
        }

        /// <summary>
        /// Gets all values from selected header area. This method is for left to right scheme, so the header fields are in one coulumn
        /// </summary>
        /// <param name="excelWorksheet">ExcelWorksheet with the data</param>
        /// <param name="selectedArea">Defined header area with start and end for rows and columns</param>
        /// <returns>Simple list with values of the header fields as string</returns>
        private List<String> GetExcelHeaderFieldsTopDown(ExcelWorksheet excelWorksheet, SheetArea selectedArea)
        {
            List<String> headerValues = new List<string>();

            ExcelCellAddress SheetStartCell = excelWorksheet.Dimension.Start;
            ExcelCellAddress SheetEndCell = excelWorksheet.Dimension.End;

            // constant, because just one row is for header allowed
            int Row = selectedArea.StartRow;

            #region Validation
            bool isStartColumnValid = selectedArea.StartColumn >= SheetStartCell.Column;
            bool isEndColumnValid = selectedArea.EndColumn <= SheetEndCell.Column;
            bool isStartRowValid = selectedArea.StartRow >= SheetStartCell.Row;
            bool isEndRowValid = selectedArea.EndRow <= SheetEndCell.Row;
            

            if ( !isStartColumnValid || !isStartRowValid || !isEndColumnValid || !isEndRowValid)
            {
                throw new InvalidOperationException("Selected area is not located in given excel sheet.");
            }
            #endregion
            
            for (int Column = selectedArea.StartColumn; Column <= selectedArea.StartColumn; Column++)
            {
                ExcelRange cell = excelWorksheet.Cells[Row, Column];

                string headerText = "";

                if (cell.Value != null)
                {
                    headerText = cell.Value.ToString();
                }

                headerValues.Add(headerText);
            }
            

            return headerValues;
        }

        /// <summary>
        /// Gets all values from selected header area. This method is for top to down scheme, so the header fields are in one row
        /// </summary>
        /// <param name="excelWorksheet">ExcelWorksheet with the data</param>
        /// <param name="selectedArea">Defined header area with start and end for rows and columns</param>
        /// <returns>Simple list with values of the header fields as string</returns>
        private List<String> GetExcelHeaderFieldsLeftRight(ExcelWorksheet excelWorksheet, SheetArea selectedArea)
        {
            List<String> headerValues = new List<string>();

            ExcelCellAddress SheetStartCell = excelWorksheet.Dimension.Start;
            ExcelCellAddress SheetEndCell = excelWorksheet.Dimension.End;

            #region Validation
            bool isStartColumnValid = selectedArea.StartColumn >= SheetStartCell.Column;
            bool isEndColumnValid = selectedArea.EndColumn <= SheetEndCell.Column;
            bool isStartRowValid = selectedArea.StartRow >= SheetStartCell.Row;
            bool isEndRowValid = selectedArea.EndRow <= SheetEndCell.Row;


            if (!isStartColumnValid || !isStartRowValid || !isEndColumnValid || !isEndRowValid)
            {
                throw new InvalidOperationException("Selected area is not located in given excel sheet.");
            }
            #endregion

            int Column = selectedArea.StartColumn;

            for (int Row = selectedArea.StartRow; Row <= selectedArea.EndRow; Row++)
            {
                ExcelRange cell = excelWorksheet.Cells[Row, Column];

                string headerText = "";

                if (cell.Value != null)
                {
                    headerText = cell.Value.ToString();
                }

                headerValues.Add(headerText);
            }

            return headerValues;
        }


        [HttpPost]
        public ActionResult Verification(object[] data)
        {
            TaskManager = (TaskManager)Session["TaskManager"];
            SelectAreasModel model = new SelectAreasModel();
            model.StepInfo = TaskManager.Current();

            if (TaskManager != null)
            {
                TaskManager.Current().SetValid(false);

                //TODO
                if (TaskManager.Bus.ContainsKey(TaskManager.SHEET_JSON_DATA) &&
                    TaskManager.Bus.ContainsKey(TaskManager.SHEET_DATA_AREA) &&
                    TaskManager.Bus.ContainsKey(TaskManager.SHEET_HEADER_AREA))
                {
                    bool isJsonDataEmpty = String.IsNullOrEmpty(Convert.ToString(TaskManager.Bus[TaskManager.SHEET_JSON_DATA]));
                    bool isDataAreaEmpty = String.IsNullOrEmpty(Convert.ToString(TaskManager.Bus[TaskManager.SHEET_DATA_AREA]));
                    bool isHeadAreaEmpty = String.IsNullOrEmpty(Convert.ToString(TaskManager.Bus[TaskManager.SHEET_HEADER_AREA]));

                    if (!isJsonDataEmpty && !isDataAreaEmpty && !isHeadAreaEmpty)
                    {
                        TaskManager.Current().SetValid(true);
                    }
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
        public ActionResult SelectedAreaToBus()
        {
            string headerArea = "";
            string dataArea = "";

            foreach (string key in Request.Form.AllKeys)
            {
                if ("dataArea" == key)
                {
                    dataArea = Request.Form[key];
                }
                if("headerArea" == key)
                {
                    headerArea = Request.Form[key];
                }
            }

            SelectAreasModel model = new SelectAreasModel();

            TaskManager TaskManager = (TaskManager)Session["TaskManager"];

            if(!String.IsNullOrEmpty(dataArea))
            {
                TaskManager.AddToBus(TaskManager.SHEET_DATA_AREA, dataArea);
                model.DataArea = dataArea;
            }

            if(!String.IsNullOrEmpty(headerArea))
            {
                TaskManager.AddToBus(TaskManager.SHEET_HEADER_AREA, headerArea);
                model.HeaderArea = headerArea;
            }

            Session["TaskManager"] = TaskManager;


            //create Model
            model.StepInfo = TaskManager.Current();

            return PartialView("SelectAreas", model);

        }


        #region private methods


        #region helper

        // chekc if user exist
        // if true return usernamem otherwise "DEFAULT"
        public string GetUserNameOrDefault()
        {
            string userName = string.Empty;
            try
            {
                userName = HttpContext.User.Identity.Name;
            }
            catch { }

            return !string.IsNullOrWhiteSpace(userName) ? userName : "DEFAULT";
        }

        
        private bool validateSheetFormat(string sheetFormat)
        {
            switch (sheetFormat)
            {
                case "top-down":
                    return true;
                case "left-right":
                    return true;
                case "matrix":
                    return true;
                default:
                    return false;
            }
        }

        #endregion

        #endregion
    }

}
