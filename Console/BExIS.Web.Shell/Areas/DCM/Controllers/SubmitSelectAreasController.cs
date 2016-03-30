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

            model.jsonTableData = jsonTable;

            // jump back to this step
            // check if dataset selected
            if (TaskManager.Bus.ContainsKey(TaskManager.SHEET_FORMAT))
            {
                if (String.IsNullOrEmpty(Convert.ToString(TaskManager.Bus[TaskManager.DATASET_ID])))
                {
                    //model.SelectedSheetFormat = TaskManager.Bus[TaskManager.SHEET_FORMAT].ToString();
                }
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
            SelectSheetFormatModel model = new SelectSheetFormatModel();
            model.StepInfo = TaskManager.Current();

            if (TaskManager != null)
            {
                TaskManager.Current().SetValid(false);

                //TODO
                if (TaskManager.Bus.ContainsKey(TaskManager.SHEET_FORMAT))
                {
                    TaskManager.Current().SetValid(true);
                }
                else
                {
                    model.ErrorList.Add(new Error(ErrorType.Other, "Dataset not exist."));
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
        public ActionResult AddSelectedDatasetToBus(string format)
        {

            SelectSheetFormatModel model = new SelectSheetFormatModel();

            TaskManager TaskManager = (TaskManager)Session["TaskManager"];


            if (!String.IsNullOrEmpty(format) && validateSheetFormat(format))
            {
                TaskManager.AddToBus(TaskManager.SHEET_FORMAT, format);
            }
            else
            {
                model.ErrorList.Add(new Error(ErrorType.Other, "Please select a sheet format."));
            }

            Session["TaskManager"] = TaskManager;


            //create Model
            model.StepInfo = TaskManager.Current();


            model.SelectedSheetFormat = format;

            return PartialView("SheetDataStructure", model);
            
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
