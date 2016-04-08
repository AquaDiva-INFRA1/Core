using System;
using System.Web.Mvc;
using System.Web.Routing;
using BExIS.IO.Transform.Validation.Exceptions;
using BExIS.Dcm.UploadWizard;
using BExIS.Dcm.Wizard;
using BExIS.Web.Shell.Areas.DCM.Models;

namespace BExIS.Web.Shell.Areas.DCM.Controllers
{
    public class SubmitSheetDataStructureController : Controller
    {
        private TaskManager TaskManager;

        //
        // GET: /DCM/SubmitSheetDataStructure/

        [HttpGet]
        public ActionResult SheetDataStructure(int index)
        {
            TaskManager = (TaskManager)Session["TaskManager"];
            //set current stepinfo based on index
            if (TaskManager != null)
            {
                TaskManager.SetCurrent(index);

                // remove if existing
                TaskManager.RemoveExecutedStep(TaskManager.Current());
            }

            SelectSheetFormatModel model = new SelectSheetFormatModel();

            // jump back to this step
            // check if dataset selected
            if (TaskManager.Bus.ContainsKey(TaskManager.SHEET_FORMAT))
            {
                if (!String.IsNullOrEmpty(Convert.ToString(TaskManager.Bus[TaskManager.SHEET_FORMAT])))
                {
                    model.SelectedSheetFormat = TaskManager.Bus[TaskManager.SHEET_FORMAT].ToString();
                }
            }

            model.StepInfo = TaskManager.Current();

            return PartialView(model);

        }

        [HttpPost]
        public ActionResult SheetDataStructure(object[] data)
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
                    model.ErrorList.Add(new Error(ErrorType.Other, "Please select a sheet format."));
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
                case "TopDown":
                    return true;
                case "LeftRight":
                    return true;
                case "Matrix":
                    return true;
                default:
                    return false;
            }
        }

        #endregion

        #endregion
    }
}
