using System;
using System.Web.Mvc;
using System.Web.Routing;
using BExIS.IO.Transform.Validation.Exceptions;
using BExIS.Dcm.UploadWizard;
using BExIS.Dcm.Wizard;
using BExIS.Web.Shell.Areas.DCM.Models;
using System.IO;
using Vaiona.Logging;
using BExIS.IO.Transform.Input;
using BExIS.Dlm.Entities.DataStructure;

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

                string sheetFormatString = Convert.ToString(TaskManager.Bus[TaskManager.SHEET_FORMAT]);

                SheetFormat CurrentSheetFormat = 0;
                Enum.TryParse<SheetFormat>(sheetFormatString, true, out CurrentSheetFormat);

                NewStructuredExcelReader NSEReader = new NewStructuredExcelReader();
                NSEReader.Open(fis);
                jsonTable = NSEReader.GenerateJsonTable(CurrentSheetFormat);

                if (!String.IsNullOrEmpty(jsonTable))
                {
                    TaskManager.AddToBus(TaskManager.SHEET_JSON_DATA, jsonTable);
                }

            }
            catch (Exception ex)
            {
                LoggerFactory.LogCustom(ex.Message);
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
