﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml;
using BExIS.IO.Transform.Validation.Exceptions;
using BExIS.Dcm.UploadWizard;
using BExIS.Dcm.Wizard;
using BExIS.Dlm.Entities.Administration;
using BExIS.Dlm.Entities.Data;
using BExIS.Dlm.Entities.DataStructure;
using BExIS.Dlm.Entities.MetadataStructure;
using BExIS.Dlm.Services.Administration;
using BExIS.Dlm.Services.Data;
using BExIS.Dlm.Services.DataStructure;
using BExIS.Dlm.Services.MetadataStructure;
using BExIS.Web.Shell.Areas.DCM.Models;
using Vaiona.Utils.Cfg;
using BExIS.Web.Shell.Areas.DCM.Models.Metadata;
using System.Xml.Linq;
using BExIS.Xml.Helpers;
using BExIS.Xml.Services;

namespace BExIS.Web.Shell.Areas.DCM.Controllers
{
    public class SubmitSpecifyDatasetController : Controller
    {
        private TaskManager TaskManager;

        //
        // GET: /DCM/SpecifyDataset/

        [HttpGet]
        public ActionResult SpecifyDataset(int index)
        {
            TaskManager = (TaskManager)Session["TaskManager"];
            //set current stepinfo based on index
            if (TaskManager != null)
            {
                TaskManager.SetCurrent(index);

                // remove if existing
                TaskManager.RemoveExecutedStep(TaskManager.Current());
            }

            ChooseDatasetViewModel model = new ChooseDatasetViewModel();

            // jump back to this step
            // check if dataset selected
            if (TaskManager.Bus.ContainsKey(TaskManager.DATASET_ID))
                if (Convert.ToInt32(TaskManager.Bus[TaskManager.DATASET_ID]) > 0)
                {
                    model.DatasetTitle = TaskManager.Bus[TaskManager.DATASET_TITLE].ToString();
                    model.SelectedDatasetId = Convert.ToInt32(TaskManager.Bus[TaskManager.DATASET_ID]);
                }

            model.StepInfo = TaskManager.Current();
            if ((List<ListViewItem>)Session["DatasetVersionViewList"] != null) model.DatasetsViewList = (List<ListViewItem>)Session["DatasetVersionViewList"];

            return PartialView(model);

        }

        [HttpPost]
        public ActionResult SpecifyDataset(object[] data)
        {
            TaskManager = (TaskManager)Session["TaskManager"];
            ChooseDatasetViewModel model = new ChooseDatasetViewModel();
            model.StepInfo = TaskManager.Current();

            if (TaskManager != null)
            {
                TaskManager.Current().SetValid(false);

                if (TaskManager.Bus.ContainsKey(TaskManager.DATASET_ID))
                {
                    DatasetManager dm = new DatasetManager();
                    Dataset ds = new Dataset();
                    try
                    {
                        dm = new DatasetManager();
                        ds = dm.GetDataset((long)Convert.ToInt32(TaskManager.Bus[TaskManager.DATASET_ID]));

                        TaskManager.AddToBus(TaskManager.DATASTRUCTURE_ID, ((DataStructure)(ds.DataStructure.Self)).Id);
                        TaskManager.AddToBus(TaskManager.DATASTRUCTURE_TITLE, ((DataStructure)(ds.DataStructure.Self)).Name);

                        TaskManager.Current().SetValid(true);

                    }
                    catch
                    {
                        model.ErrorList.Add(new Error(ErrorType.Other, "Dataset not exist."));
                    }
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
                    if ((List<ListViewItem>)Session["DatasetVersionViewList"] != null) model.DatasetsViewList = (List<ListViewItem>)Session["DatasetVersionViewList"];
              }
            }

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult AddSelectedDatasetToBus(string id)
        {

            ChooseDatasetViewModel model = new ChooseDatasetViewModel();

            TaskManager TaskManager = (TaskManager)Session["TaskManager"];

            DatasetManager datasetManager = new DatasetManager();
            long datasetId;

            if (TaskManager.Bus.ContainsKey(TaskManager.DATASET_STATUS))
            {
                if (TaskManager.Bus[TaskManager.DATASET_STATUS].Equals("new"))
                {
                    //long newid = Convert.ToInt64(TaskManager.Bus[TaskManager.DATASET_ID]);
                    //datasetManager.PurgeDataset(newid);

                }

                TaskManager.Bus[TaskManager.DATASET_STATUS] = "edit";
            }
            else
            {
                datasetId = Convert.ToInt64(id);
                if (datasetManager.GetDatasetVersionEffectiveTupleCount(datasetManager.GetDatasetLatestVersion(datasetId)) > 0)
                {
                    TaskManager.AddToBus("DatasetStatus", "edit");
                }
                else
                    TaskManager.AddToBus("DatasetStatus", "new");
            }


            datasetId = Convert.ToInt64(id);

            Dataset dataset = datasetManager.GetDataset(datasetId);

            DatasetVersion datasetVersion;

            if(datasetManager.IsDatasetCheckedIn(datasetId))
            {
              datasetVersion = datasetManager.GetDatasetLatestVersion(datasetId);

              TaskManager.AddToBus(TaskManager.DATASET_ID, datasetId);

              //Add Metadata to Bus
              //TITLE
              TaskManager.AddToBus(TaskManager.DATASET_TITLE, XmlDatasetHelper.GetInformation(datasetVersion, AttributeNames.title));

              ResearchPlanManager rpm = new ResearchPlanManager();
              ResearchPlan rp = rpm.Repo.Get(datasetVersion.Dataset.ResearchPlan.Id);
              TaskManager.AddToBus(TaskManager.RESEARCHPLAN_ID, rp.Id);
              TaskManager.AddToBus(TaskManager.RESEARCHPLAN_TITLE, rp.Title);

            }
            else
            {
                model.ErrorList.Add(new Error(ErrorType.Dataset, "Dataset is not checked in."));
            }

            Session["TaskManager"] = TaskManager;


            //create Model
            model.StepInfo = TaskManager.Current();
            if ((List<ListViewItem>)Session["DatasetVersionViewList"] != null) model.DatasetsViewList = (List<ListViewItem>)Session["DatasetVersionViewList"];

            if (TaskManager.Bus.ContainsKey(TaskManager.DATASET_TITLE))
                model.DatasetTitle = TaskManager.Bus[TaskManager.DATASET_TITLE].ToString();
                model.SelectedDatasetId = Convert.ToInt32(id);
            return PartialView("SpecifyDataset", model);
        }
 
        #region private methods


        #region helper

        // chekc if user exist
        // if true return usernamem otherwise "DEFAULT"
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

        #endregion

        #endregion
    }
}
