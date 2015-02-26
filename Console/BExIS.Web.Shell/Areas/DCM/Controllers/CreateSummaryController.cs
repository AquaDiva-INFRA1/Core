﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BExIS.Dcm.CreateDatasetWizard;
using BExIS.Web.Shell.Areas.DCM.Models.Metadata;
using BExIS.Web.Shell.Areas.DCM.Models.Create;
using System.Web.Routing;
using BExIS.Dlm.Entities.Data;
using BExIS.Dlm.Services.Data;
using BExIS.Dlm.Services.DataStructure;
using BExIS.Dlm.Entities.DataStructure;
using BExIS.Dlm.Services.Administration;
using BExIS.Dlm.Entities.Administration;
using BExIS.Dlm.Services.MetadataStructure;
using BExIS.Dlm.Entities.MetadataStructure;
using System.Xml.Linq;
using BExIS.Xml.Helpers;
using BExIS.Dcm.Wizard;
using BExIS.IO.Transform.Validation.Exceptions;
using BExIS.Security.Services.Objects;
using System.Xml;
using BExIS.Security.Services.Authorization;
using BExIS.Security.Entities.Objects;
using BExIS.Security.Services.Subjects;
using BExIS.Xml.Services;
using BExIS.Dlm.Entities.Common;

namespace BExIS.Web.Shell.Areas.DCM.Controllers
{
    public class CreateSummaryController : Controller
    {
        CreateDatasetTaskmanager TaskManager;

        //
        // GET: /DCM/CreateSummary/
        [HttpGet]
        public ActionResult Summary(int index)
        {
            TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];
            CreateSummaryModel model = new CreateSummaryModel();
            //set current stepinfo based on index
            if (TaskManager != null)
            {
                TaskManager.UpdateStepStatus(index);
                TaskManager.SetCurrent(index);
                Session["CreateDatasetTaskmanager"] = TaskManager;


                if (TaskManager.Bus.ContainsKey(CreateDatasetTaskmanager.METADATA_PACKAGE_MODEL_LIST))
                {
                    Dictionary<string, AbstractMetadataStepModel> list = (Dictionary<string, AbstractMetadataStepModel>)TaskManager.Bus[CreateDatasetTaskmanager.METADATA_PACKAGE_MODEL_LIST];
                    model = CreateSummaryModel.Convert(list, TaskManager.Current());
                    model.ErrorList = ValidatePackageModels();
                    model.PageStatus = Models.PageStatus.FirstLoad;
                    return PartialView(model);
                }
                else
                {
                    TaskManager.Bus[CreateDatasetTaskmanager.METADATA_PACKAGE_MODEL_LIST] = new Dictionary<string, MetadataPackageModel>();
                }

                model.ErrorList = ValidatePackageModels();
            }

           model.StepInfo = TaskManager.Current();
           return PartialView(model);
        }

        [HttpPost]
        public ActionResult Summary(int? index, string name = null)
        {
            TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];

            //set current stepinfo based on index
            if (TaskManager != null)
            {

                Session["CreateDatasetTaskmanager"] = TaskManager;
            }

         
            CreateSummaryModel model = new CreateSummaryModel();
            // prepare model
            if (TaskManager.Bus.ContainsKey(CreateDatasetTaskmanager.METADATA_PACKAGE_MODEL_LIST))
            {
                model.StepInfo = TaskManager.Current();

                model.StepInfo.SetValid(false);
                Dictionary<string, AbstractMetadataStepModel> list = (Dictionary<string, AbstractMetadataStepModel>)TaskManager.Bus[CreateDatasetTaskmanager.METADATA_PACKAGE_MODEL_LIST];
                model = CreateSummaryModel.Convert(list, TaskManager.Current());

                //if (ValidatePackageModels().Count==0)
                //{

                    Submit();

                    if (TaskManager.Bus.ContainsKey(CreateDatasetTaskmanager.DATASET_TITLE))
                    {
                        model.DatasetTitle = TaskManager.Bus[CreateDatasetTaskmanager.DATASET_TITLE].ToString();
                        if (String.IsNullOrEmpty(model.DatasetTitle))
                        {
                            model.DatasetTitle = "No title available";
                            TaskManager.Bus[CreateDatasetTaskmanager.DATASET_TITLE] = model.DatasetTitle; 
                        }
                    }
                    else
                    {
                        TaskManager.Bus[CreateDatasetTaskmanager.DATASET_TITLE] = "No title available";
                        model.DatasetTitle = TaskManager.Bus[CreateDatasetTaskmanager.DATASET_TITLE].ToString();
                    }

                    
                    model.DatasetId = Convert.ToInt64(TaskManager.Bus[CreateDatasetTaskmanager.DATASET_ID]);
                    model.StepInfo.SetStatus(StepStatus.exit);
                    model.PageStatus = Models.PageStatus.LastAndSubmitted;
                //}
                //else
                //{
                //    model.PageStatus = Models.PageStatus.Error;
                //    model.ErrorList = ValidatePackageModels();
                //}
                }

            
            //jump to a other step
            if (index.HasValue)
            {
                TaskManager.SetCurrent(index.Value);
                Session["TaskManager"] = TaskManager;
                ActionInfo actionInfo = TaskManager.Current().GetActionInfo;
                return RedirectToAction(actionInfo.ActionName, actionInfo.ControllerName, new RouteValueDictionary { { "area", actionInfo.AreaName }, { "index", TaskManager.GetCurrentStepInfoIndex() } });

            }
            else
                return PartialView(model);

             
        }

        public void Submit()
        {
            #region create dataset

            TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];

            if (TaskManager.Bus.ContainsKey(CreateDatasetTaskmanager.DATASTRUCTURE_ID)
                && TaskManager.Bus.ContainsKey(CreateDatasetTaskmanager.RESEARCHPLAN_ID)
                && TaskManager.Bus.ContainsKey(CreateDatasetTaskmanager.METADATASTRUCTURE_ID))
            {
                Dataset ds;
                DatasetManager dm = new DatasetManager();
                long datasetId = 0;
                // for e new dataset
                if (!TaskManager.Bus.ContainsKey(CreateDatasetTaskmanager.DATASET_ID))
                {
                    long datastructureId = Convert.ToInt64(TaskManager.Bus[CreateDatasetTaskmanager.DATASTRUCTURE_ID]);
                    long researchPlanId = Convert.ToInt64(TaskManager.Bus[CreateDatasetTaskmanager.RESEARCHPLAN_ID]);
                    long metadataStructureId = Convert.ToInt64(TaskManager.Bus[CreateDatasetTaskmanager.METADATASTRUCTURE_ID]);

                    DataStructureManager dsm = new DataStructureManager();

                    DataStructure dataStructure = dsm.StructuredDataStructureRepo.Get(datastructureId);
                    //if datastructure is not a structured one
                    if (dataStructure == null) dataStructure = dsm.UnStructuredDataStructureRepo.Get(datastructureId);

                    ResearchPlanManager rpm = new ResearchPlanManager();
                    ResearchPlan rp = rpm.Repo.Get(researchPlanId);

                    MetadataStructureManager msm = new MetadataStructureManager();
                    MetadataStructure metadataStructure = msm.Repo.Get(metadataStructureId);

                    ds = dm.CreateEmptyDataset(dataStructure, rp, metadataStructure);
                    datasetId = ds.Id;

                    // add security
                    if (GetUserNameOrDefault() != "DEFAULT")
                    {
                        PermissionManager pm = new PermissionManager();
                        SubjectManager sm = new SubjectManager();

                        BExIS.Security.Entities.Subjects.User user = sm.GetUserByName(GetUserNameOrDefault());

                        foreach (RightType rightType in Enum.GetValues(typeof(RightType)).Cast<RightType>())
                        {
                            pm.CreateDataPermission(user.Id, 1, ds.Id, rightType);
                        }
                    }

                }
                else
                {
                    datasetId = Convert.ToInt64(TaskManager.Bus[CreateDatasetTaskmanager.DATASET_ID]);
                }

                TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];

                if (dm.IsDatasetCheckedOutFor(datasetId, GetUserNameOrDefault()) || dm.CheckOutDataset(datasetId, GetUserNameOrDefault()))
                {
                    DatasetVersion workingCopy = dm.GetDatasetWorkingCopy(datasetId);

                    if (TaskManager.Bus.ContainsKey(CreateDatasetTaskmanager.METADATA_XML))
                    {
                        XDocument xMetadata = (XDocument)TaskManager.Bus[CreateDatasetTaskmanager.METADATA_XML];
                        workingCopy.Metadata = XmlMetadataWriter.ToXmlDocument(xMetadata);
                    }

                    TaskManager.AddToBus(CreateDatasetTaskmanager.DATASET_TITLE, XmlDatasetHelper.GetInformation(workingCopy, AttributeNames.title));//workingCopy.Metadata.SelectNodes("Metadata/Description/Description/Title/Title")[0].InnerText);

                    TaskManager.AddToBus(CreateDatasetTaskmanager.DATASET_ID, datasetId);

                    dm.EditDatasetVersion(workingCopy, null, null, null);

                    dm.CheckInDataset(datasetId, "Metadata was submited.", GetUserNameOrDefault());
                }
            }

            #endregion
        }

        #region helper

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
        #endregion

        // Check if existing packageModels have errors or not
        // is valid == ture mean no errors
        private bool IsValid()
        {
            if (TaskManager.Bus.ContainsKey(CreateDatasetTaskmanager.METADATA_PACKAGE_MODEL_LIST))
            {
                Dictionary<string, MetadataPackageModel> list = (Dictionary<string, MetadataPackageModel>)TaskManager.Bus[CreateDatasetTaskmanager.METADATA_PACKAGE_MODEL_LIST];

                foreach (MetadataPackageModel packageModel in list.Values)
                {
                    if (packageModel.ErrorList != null)
                    {
                        if (packageModel.ErrorList.Count > 0) return false;
                    }
                }

                return true;
            }

            return false;
        }

        private List<Error> ValidatePackageModels()
        {
            List<Error> errors = new List<Error>();

            TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];

            // go to every step in the taskmanager
           
            foreach (StepInfo step in TaskManager.Root.Children)
            {
                if (!AllStepsValid(step))
                { 
                    errors.Add(new Error(ErrorType.Other, "Errors in package : "+step.title ));
                }
            }

            return errors;
        }

        private bool AllStepsValid(StepInfo step)
        {
            if (IsStepValid(step))
            {
                if(step.Children.Count>0)
                {
                    foreach(StepInfo childStep in step.Children)
                    {
                        if (!AllStepsValid(childStep))
                        {
                            return false;
                        }
                    }
                }
            }
            else
            {
                return false;
            }


            return true;
        }

        private bool IsStepValid(StepInfo step)
        { 
            if(step.stepStatus.Equals(StepStatus.error))
                return false;

            return true;
        }

        private bool hasRequiredMetadataAttributeUsage(MetadataPackageUsage mpu)
        {
            foreach (MetadataAttributeUsage metadataAttributeUsage in mpu.MetadataPackage.MetadataAttributeUsages)
            {
                if (metadataAttributeUsage.MinCardinality > 0) return true;
            }

            return false;
        }

        private List<MetadataAttributeUsage> GetRequiredMetadataAttributeUsage(MetadataPackageUsage mpu)
        {
            List<MetadataAttributeUsage> list = new List<MetadataAttributeUsage>();

            foreach (MetadataAttributeUsage metadataAttributeUsage in mpu.MetadataPackage.MetadataAttributeUsages)
            {
                if (metadataAttributeUsage.MinCardinality > 0) list.Add(metadataAttributeUsage);
            }

            return list;
        }

        private List<AbstractMetadataStepModel> GetMetadataPackageModelsFromBus(long metadataPackageUsageId)
        {
            Dictionary<string, AbstractMetadataStepModel> packageModelDic = new Dictionary<string, AbstractMetadataStepModel>();
            List<AbstractMetadataStepModel> temp = new List<AbstractMetadataStepModel>();

            if (TaskManager.Bus.ContainsKey(CreateDatasetTaskmanager.METADATA_PACKAGE_MODEL_LIST))
            {
                packageModelDic = (Dictionary<string, AbstractMetadataStepModel>)TaskManager.Bus[CreateDatasetTaskmanager.METADATA_PACKAGE_MODEL_LIST];

                foreach (KeyValuePair<string, AbstractMetadataStepModel> keyValuePair in packageModelDic)
                {
                    if (GetPackageUsageIdFromIdentfifier(keyValuePair.Key).Equals(metadataPackageUsageId))
                        temp.Add(keyValuePair.Value);
                }
            }
            else
            {

                TaskManager.Bus[CreateDatasetTaskmanager.METADATA_PACKAGE_MODEL_LIST] = packageModelDic;
            }

            return temp;
        }

        #region identifier
        private string CreateIdentfifier(long usageId, int number)
        {
            return usageId + "_" + number;
        }

        private long GetPackageUsageIdFromIdentfifier(string key)
        {
            string[] keyArray = key.Split('_');
            long Id = Convert.ToInt64(keyArray[0]);
            return Id;
        }

        private long GetNumberFromIdentfifier(string key)
        {
            string[] keyArray = key.Split('_');
            long Id = Convert.ToInt64(keyArray[1]);
            return Id;
        }
        #endregion
    }
}
