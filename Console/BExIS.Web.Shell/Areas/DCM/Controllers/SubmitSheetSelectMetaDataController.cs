using BExIS.Dcm.UploadWizard;
using BExIS.Dcm.Wizard;
using BExIS.Dlm.Entities.MetadataStructure;
using BExIS.Dlm.Services.MetadataStructure;
using BExIS.IO.Transform.Validation.Exceptions;
using BExIS.Web.Shell.Areas.DCM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BExIS.Web.Shell.Areas.DCM.Controllers
{
    public class SubmitSheetSelectMetaDataController : Controller
    {
        private TaskManager TaskManager;

        [HttpGet]
        public ActionResult SheetSelectMetaData(int index)
        {
            TaskManager = (TaskManager)Session["TaskManager"];

            SelectMetaDataModel model = new SelectMetaDataModel();

            MetadataStructureManager msm = new MetadataStructureManager();
            foreach(MetadataStructure metadataStructure in msm.Repo.Get())
            {
                model.AvailableMetadata.Add(new Tuple<long, string>(metadataStructure.Id, metadataStructure.Name));
            }

            if (TaskManager.Bus.ContainsKey(TaskManager.SCHEMA))
            {
                model.SelectedMetaDataId = Convert.ToInt64(TaskManager.Bus[TaskManager.SCHEMA]);
            }


            return PartialView(model);
        }

        [HttpPost]
        public ActionResult SheetSelectMetaData(object[] data)
        {
            TaskManager = (TaskManager)Session["TaskManager"];
            SelectMetaDataModel model = new SelectMetaDataModel();
            model.StepInfo = TaskManager.Current();

            if (TaskManager != null)
            {
                TaskManager.Current().SetValid(false);

                if (TaskManager.Bus.ContainsKey(TaskManager.SCHEMA))
                {
                    if (Convert.ToInt64(TaskManager.Bus[TaskManager.SCHEMA]) >= 0)
                    {
                        TaskManager.Current().SetValid(true);
                    }
                }
                else
                {
                    model.ErrorList.Add(new Error(ErrorType.Other, "No Metadata schema is selected."));
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
                    MetadataStructureManager msm = new MetadataStructureManager();
                    foreach (MetadataStructure metadataStructure in msm.Repo.Get())
                    {
                        model.AvailableMetadata.Add(new Tuple<long, string>(metadataStructure.Id, metadataStructure.Name));
                    }

                    //reload model
                    model.StepInfo = TaskManager.Current();
                }
            }

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult SaveMetaDataSelection(object[] data)
        {
            TaskManager = (TaskManager)Session["TaskManager"];
            SelectMetaDataModel model = new SelectMetaDataModel();

            long metadataId = -1;

            if (TaskManager != null)
            {
                MetadataStructureManager msm = new MetadataStructureManager();
                foreach (MetadataStructure metadataStructure in msm.Repo.Get())
                {
                    model.AvailableMetadata.Add(new Tuple<long, string>(metadataStructure.Id, metadataStructure.Name));
                }

                TaskManager.Current().SetValid(false);

                foreach (string key in Request.Form.AllKeys)
                {
                    if ("metadataId" == key)
                    {
                        metadataId = Convert.ToInt64(Request.Form[key]);
                    }
                }

                if(metadataId >= 0)
                {
                    model.SelectedMetaDataId = metadataId;

                    TaskManager.AddToBus(TaskManager.SCHEMA, model.SelectedMetaDataId);
                    TaskManager.Current().SetValid(true);
                }
            }

            return PartialView("SheetSelectMetaData", model);
        }
    }
}