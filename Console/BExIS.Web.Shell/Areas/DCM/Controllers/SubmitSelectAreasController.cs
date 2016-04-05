using System;
using System.Web.Mvc;
using System.Web.Routing;
using BExIS.IO.Transform.Validation.Exceptions;
using BExIS.Dcm.UploadWizard;
using BExIS.Dcm.Wizard;
using BExIS.Web.Shell.Areas.DCM.Models;
using System.IO;
using OfficeOpenXml;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using BExIS.Dlm.Services.DataStructure;
using System.Collections.Generic;
using System.Linq;

namespace BExIS.Web.Shell.Areas.DCM.Controllers
{
    public class SubmitSelectAreasController : Controller
    {
        private TaskManager TaskManager;

        //
        // GET: /DCM/SubmitSelectAreas/

        [HttpGet]
        public ActionResult SelectAreas(int index)
        {
            TaskManager = (TaskManager)Session["TaskManager"];

            string filePath = TaskManager.Bus[TaskManager.FILEPATH].ToString();
            FileStream fis = null;
            string jsonTable = "{}";

            try {
                fis = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                jsonTable = GenerateJsonTable(fis);

                if (!String.IsNullOrEmpty(jsonTable))
                {
                    TaskManager.AddToBus(TaskManager.SHEET_JSON_DATA, jsonTable);
                }
                UnitManager unitManager = new UnitManager();
                List<BExIS.Dlm.Entities.DataStructure.Unit> tempUnitList = unitManager.Repo.Get().ToList();
                int foo = tempUnitList.Count;

            } catch(Exception ex)
            {
                

            } finally
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

            if (TaskManager.Bus.ContainsKey(TaskManager.SHEET_DATA_AREA))
            {
               
                    model.DataArea = TaskManager.Bus[TaskManager.SHEET_DATA_AREA].ToString();
                
            }

            if (TaskManager.Bus.ContainsKey(TaskManager.SHEET_HEADER_AREA))
            {
                
                    model.HeaderArea = TaskManager.Bus[TaskManager.SHEET_HEADER_AREA].ToString();
                
            }

            model.StepInfo = TaskManager.Current();

            return PartialView(model);

        }

        private string GenerateJsonTable(FileStream fis)
        {
            ExcelPackage ep = new ExcelPackage(fis);

            ExcelWorkbook excelWorkbook = ep.Workbook;
            ExcelWorksheet firstWorksheet = excelWorkbook.Worksheets[1];

            ExcelCellAddress StartCell = firstWorksheet.Dimension.Start;
            ExcelCellAddress EndCell = firstWorksheet.Dimension.End;

            string[][] arr = new string[EndCell.Row][];

            for (int Row = StartCell.Row; Row <= EndCell.Row; Row++)
            {
                TableRow tRow = new TableRow();

                string[] currentRow = new string[EndCell.Column];

                for (int Column = StartCell.Column; Column <= EndCell.Column; Column++)
                {


                    ExcelRange cell = firstWorksheet.Cells[Row, Column];

                    //richTextBox1.Text += "Cell: " + cell.Address + ",";




                    TableCell tCell = new TableCell();
                    //tCell.Text = cell.Address + ": Value [" + cell.Value + "]<br>" + "BGColorRGB [" + colorRgb + "]<br>Formula [" + cell.Formula + "] ";
                    if (cell.Value != null)
                    {
                        tCell.Text = cell.Value.ToString();
                        currentRow[Column - 1] = cell.Value.ToString();
                    }
                    else
                    {
                        tCell.Text = "";
                        currentRow[Column - 1] = "";
                    }
                    tRow.Cells.Add(tCell);

                }

                arr[Row - 1] = currentRow;
            }
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(arr);
        }

        [HttpPost]
        public ActionResult SelectAreas(object[] data)
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
            string headerArea = null;
            string dataArea = null;

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

            if (dataArea != null)
            {
                TaskManager.AddToBus(TaskManager.SHEET_DATA_AREA, dataArea);
                model.DataArea = dataArea;
            }

            if (headerArea != null)
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
