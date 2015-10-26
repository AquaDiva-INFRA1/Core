﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using BExIS.Dcm.CreateDatasetWizard;
using BExIS.Dcm.UploadWizard;
using BExIS.Dcm.Wizard;
using BExIS.Dlm.Entities.Administration;
using BExIS.Dlm.Entities.Common;
using BExIS.Dlm.Entities.Data;
using BExIS.Dlm.Entities.DataStructure;
using BExIS.Dlm.Entities.MetadataStructure;
using BExIS.Dlm.Services.Administration;
using BExIS.Dlm.Services.Data;
using BExIS.Dlm.Services.DataStructure;
using BExIS.Dlm.Services.MetadataStructure;
using BExIS.Dlm.Services.TypeSystem;
using BExIS.IO.Transform.Validation.Exceptions;
using BExIS.Security.Entities.Objects;
using BExIS.Security.Services.Authorization;
using BExIS.Security.Services.Subjects;
using BExIS.Web.Shell.Areas.DCM.Models;
using BExIS.Web.Shell.Areas.DCM.Models.Create;
using BExIS.Web.Shell.Areas.DCM.Models.CreateDataset;
using BExIS.Web.Shell.Areas.DCM.Models.Metadata;
using BExIS.Xml.Helpers;
using BExIS.Xml.Services;
using Vaiona.Utils.Cfg;

namespace BExIS.Web.Shell.Areas.DCM.Controllers
{
    public class CreateDatasetController : Controller
    {
        private CreateDatasetTaskmanager TaskManager;

        //
        // GET: /DCM/CreateDataset/

        public ActionResult Index()
        {
            Session["CreateDatasetTaskmanager"] = null;
            if (TaskManager == null) TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];

            if (TaskManager == null)
            {

                TaskManager = new CreateDatasetTaskmanager();

                Session["CreateDatasetTaskmanager"] = TaskManager;
                Session["MetadataStructureViewList"] = LoadMetadataStructureViewList();
                Session["DataStructureViewList"] = LoadDataStructureViewList();
                

                SetupModel Model = GetDefaultModel();

                return View(Model);
            }

            return View();
        }

        public ActionResult StoreSelectedDatasetSetup(SetupModel model)
        {
            CreateDatasetTaskmanager TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];

            if (model == null)
            {
                model = GetDefaultModel();
                return PartialView("Index", model);
            }

            model = LoadLists(model);

            if (ModelState.IsValid)
            {
                TaskManager.AddToBus(CreateDatasetTaskmanager.METADATASTRUCTURE_ID, model.SelectedMetadatStructureId);
                TaskManager.AddToBus(CreateDatasetTaskmanager.DATASTRUCTURE_ID, model.SelectedDataStructureId);

                ResearchPlanManager rpm = new ResearchPlanManager();

                TaskManager.AddToBus(CreateDatasetTaskmanager.RESEARCHPLAN_ID, rpm.Repo.Get().First().Id);
                // creat a new dataset
                //CreateANewDataset(model.SelectedDatastructureId, model.SelectedResearchPlanId, model.SelectedMetadatStructureId);

                CreateXml();

                AdvanceTaskManager(model.SelectedMetadatStructureId);

                return RedirectToAction("StartMetadataEditor","CreateDataset");

            }

            return View("Index", model);
        }

        public ActionResult StartMetadataEditor()
        {
            TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];
            List<StepModelHelper> stepInfoModelHelpers = new List<StepModelHelper>();

            foreach (var stepInfo in TaskManager.StepInfos)
            {
                
                StepModelHelper stepModelHelper = GetStepModelhelper(stepInfo.Id);

                if (stepModelHelper.Model == null)
                {
                    if (stepModelHelper.Usage is MetadataPackageUsage)
                        stepModelHelper.Model = CreatePackageModel(stepInfo.Id, false);
   
                     if (stepModelHelper.Usage is MetadataNestedAttributeUsage)
                        stepModelHelper.Model = CreateCompoundModel(stepInfo.Id, false);

                    getChildModelsHelper(stepModelHelper);
                }

                stepInfoModelHelpers.Add(stepModelHelper);

            }

            MetadataEditorModel Model = new MetadataEditorModel();
            Model.StepModelHelpers = stepInfoModelHelpers;

            return View("MetadataEditor", Model);
        }

        public ActionResult ReloadMetadataEditor()
        {
            TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];

            if(TaskManager.Bus.ContainsKey(CreateDatasetTaskmanager.METADATA_STEP_MODEL_HELPER))
            {
                List<StepModelHelper> stepInfoModelHelpers = (List<StepModelHelper>)TaskManager.Bus[CreateDatasetTaskmanager.METADATA_STEP_MODEL_HELPER];

                MetadataEditorModel Model = new MetadataEditorModel();
                Model.StepModelHelpers = stepInfoModelHelpers;

                return View("MetadataEditor", Model);
            }

            return RedirectToAction("StartMetadataEditor","CreateDataset");
        }

        private StepModelHelper getChildModelsHelper(StepModelHelper stepModelHelper)
        {
            if (stepModelHelper.Model.StepInfo.Children.Count > 0)
            {
                foreach (var childStep in stepModelHelper.Model.StepInfo.Children)
                {
                    StepModelHelper childStepModelHelper = GetStepModelhelper(childStep.Id);

                    if (childStepModelHelper.Model == null)
                    {
                        if (childStepModelHelper.Usage is MetadataPackageUsage)
                            childStepModelHelper.Model = CreatePackageModel(childStep.Id, false);

                        if (childStepModelHelper.Usage is MetadataNestedAttributeUsage)
                            childStepModelHelper.Model = CreateCompoundModel(childStep.Id, false);

                        if (childStepModelHelper.Usage is MetadataAttributeUsage)
                            childStepModelHelper.Model = CreateCompoundModel(childStep.Id, false);
                    }

                    childStepModelHelper = getChildModelsHelper(childStepModelHelper);

                    stepModelHelper.Childrens.Add(childStepModelHelper);
                }
            }

            return stepModelHelper;
        }

        [HttpPost]
        public ActionResult StoreSelectedOption(long id, string type)
        {
            TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];
            string key = "";

            switch (type)
            {
                case "ms": key = CreateDatasetTaskmanager.METADATASTRUCTURE_ID; break;
            }

            if (key != "")
            {
                if (TaskManager.Bus.ContainsKey(key))
                    TaskManager.Bus[key] = id;
                else
                    TaskManager.Bus.Add(key, id);
            }

            return Content("");
        }
        
        private SetupModel GetDefaultModel()
        {
            SetupModel model = new SetupModel();

            model = LoadLists(model);

            if (TaskManager.Bus.ContainsKey(CreateDatasetTaskmanager.METADATASTRUCTURE_ID))
                model.SelectedMetadatStructureId = Convert.ToInt64(TaskManager.Bus[CreateDatasetTaskmanager.METADATASTRUCTURE_ID]);

            return model;
        }

        private SetupModel LoadLists(SetupModel model)
        {
            if ((List<ListViewItem>)Session["MetadataStructureViewList"] != null) model.MetadataStructureViewList = (List<ListViewItem>)Session["MetadataStructureViewList"];
            if ((List<ListViewItem>)Session["DataStructureViewList"] != null) model.DataStructureViewList = (List<ListViewItem>)Session["DataStructureViewList"];

            return model;
        }

        private void AdvanceTaskManager(long MetadataStructureId)
        {
            TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];

            MetadataStructureManager metadataStructureManager = new MetadataStructureManager();

            List<MetadataPackageUsage> metadataPackageList = metadataStructureManager.GetEffectivePackages(MetadataStructureId).ToList();

            List<StepModelHelper> stepModelHelperList = new List<StepModelHelper>();
            if (TaskManager.Bus.ContainsKey(CreateDatasetTaskmanager.METADATA_STEP_MODEL_HELPER))
            {
                TaskManager.Bus[CreateDatasetTaskmanager.METADATA_STEP_MODEL_HELPER] = stepModelHelperList;
            }
            else
            {
                TaskManager.Bus.Add(CreateDatasetTaskmanager.METADATA_STEP_MODEL_HELPER, stepModelHelperList);
            }

            TaskManager.StepInfos = new List<StepInfo>();

            foreach (MetadataPackageUsage mpu in metadataPackageList)
            {


                //only add none optional usages
                StepInfo si = new StepInfo(mpu.Label)
                {
                    Id = TaskManager.GenerateStepId(),
                    parentTitle = mpu.MetadataPackage.Name,
                    Parent = TaskManager.Root,
                    IsInstanze = false,
                    //GetActionInfo = new ActionInfo
                    //{
                    //    ActionName = "SetMetadataPackage",
                    //    ControllerName = "CreateSetMetadataPackage",
                    //    AreaName = "DCM"
                    //},

                    //PostActionInfo = new ActionInfo
                    //{
                    //    ActionName = "SetMetadataPackage",
                    //    ControllerName = "CreateSetMetadataPackage",
                    //    AreaName = "DCM"
                    //}
                };

                TaskManager.StepInfos.Add(si);
                StepModelHelper stepModelHelper = new StepModelHelper(si.Id, 1, mpu, "Metadata//" + mpu.Label.Replace(" ", string.Empty) + "[1]");

                stepModelHelperList.Add(stepModelHelper);

                if (mpu.MinCardinality > 0)
                {

                    si = AddStepsBasedOnUsage(mpu, si, "Metadata//" + mpu.Label.Replace(" ", string.Empty) + "[1]");
                    TaskManager.Root.Children.Add(si);

                    TaskManager.Bus[CreateDatasetTaskmanager.METADATA_STEP_MODEL_HELPER] = stepModelHelperList;
                }
            }

            TaskManager.Bus[CreateDatasetTaskmanager.METADATA_STEP_MODEL_HELPER] = stepModelHelperList;
            Session["CreateDatasetTaskmanager"] = TaskManager;
        }

        private List<BaseUsage> GetCompoundAttributeUsages(BaseUsage usage)
        {
            List<BaseUsage> list = new List<BaseUsage>();

            if (usage is MetadataPackageUsage)
            {
                MetadataPackageUsage mpu = (MetadataPackageUsage)usage;

                foreach (MetadataAttributeUsage mau in mpu.MetadataPackage.MetadataAttributeUsages)
                {
                    list.AddRange(GetCompoundAttributeUsages(mau));
                }
            }

            if (usage is MetadataAttributeUsage)
            {
                MetadataAttributeUsage mau = (MetadataAttributeUsage)usage;

                if (mau.MetadataAttribute.Self is MetadataCompoundAttribute)
                {
                    list.Add(mau);

                    MetadataCompoundAttribute mca = (MetadataCompoundAttribute)mau.MetadataAttribute.Self;

                    foreach (MetadataNestedAttributeUsage mnau in mca.MetadataNestedAttributeUsages)
                    {
                        list.AddRange(GetCompoundAttributeUsages(mnau));
                    }
                }

            }

            if (usage is MetadataNestedAttributeUsage)
            {
                MetadataNestedAttributeUsage mnau = (MetadataNestedAttributeUsage)usage;


                if (mnau.Member.Self is MetadataCompoundAttribute)
                {
                    list.Add(mnau);

                    MetadataCompoundAttribute mca = (MetadataCompoundAttribute)mnau.Member.Self;

                    foreach (MetadataNestedAttributeUsage m in mca.MetadataNestedAttributeUsages)
                    {
                        list.AddRange(GetCompoundAttributeUsages(m));
                    }
                }
            }

            return list;
        }

        private void CreateXml()
        {
            TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];

            // load metadatastructure with all packages and attributes

            if (TaskManager.Bus.ContainsKey(CreateDatasetTaskmanager.METADATASTRUCTURE_ID))
            {
                XmlMetadataWriter xmlMetadatWriter = new XmlMetadataWriter(XmlNodeMode.xPath);


                XDocument metadataXml = xmlMetadatWriter.CreateMetadataXml(Convert.ToInt64(TaskManager.Bus[CreateDatasetTaskmanager.METADATASTRUCTURE_ID]));

                // locat path
                //string path = Path.Combine(AppConfiguration.GetModuleWorkspacePath("DCM"), "metadataTemp.Xml");

                TaskManager.AddToBus(CreateDatasetTaskmanager.METADATA_XML, metadataXml);

                //setup loaded
                if (TaskManager.Bus.ContainsKey(CreateDatasetTaskmanager.SETUP_LOADED))
                    TaskManager.Bus[CreateDatasetTaskmanager.SETUP_LOADED] = true;
                else
                    TaskManager.Bus.Add(CreateDatasetTaskmanager.SETUP_LOADED, true);


                //save
                //metadataXml.Save(path);
            }

        }

        #region Models

        private MetadataPackageModel CreatePackageModel(int stepId, bool validateIt)
        {
            StepInfo stepInfo = TaskManager.Get(stepId);
            StepModelHelper stepModelHelper = GetStepModelhelper(stepId);

            long metadataPackageId = stepModelHelper.Usage.Id;
            long metadataStructureId = Convert.ToInt64(TaskManager.Bus[CreateDatasetTaskmanager.METADATASTRUCTURE_ID]);

            MetadataStructureManager mdsManager = new MetadataStructureManager();
            MetadataPackageManager mdpManager = new MetadataPackageManager();
            MetadataPackageUsage mpu = (MetadataPackageUsage)LoadUsage(stepModelHelper.Usage);
            MetadataPackageModel model = new MetadataPackageModel();

            model = MetadataPackageModel.Convert(mpu, stepModelHelper.Number);
            model.ConvertMetadataAttributeModels(mpu, metadataStructureId, stepId);

            if (stepInfo.IsInstanze == false)
            {
                //get Instance
                if (TaskManager.Bus.ContainsKey(CreateDatasetTaskmanager.METADATA_XML))
                {
                    XDocument xMetadata = (XDocument)TaskManager.Bus[CreateDatasetTaskmanager.METADATA_XML];
                    model.ConvertInstance(xMetadata, stepModelHelper.XPath);
                }
            }
            else
            {
                if (stepModelHelper.Model != null)
                {
                    model = (MetadataPackageModel)stepModelHelper.Model;
                }
                else
                {
                    stepModelHelper.Model = model;
                }
            }

            //if (validateIt)
            //{
            //    //validate packages
            //    List<Error> errors = validateStep(stepModelHelper.Model);
            //    if (errors != null)
            //        model.ErrorList = errors;
            //    else
            //        model.ErrorList = new List<Error>();

            //}

            model.StepInfo = stepInfo;

            return model;
        }

        private MetadataCompoundAttributeModel CreateCompoundModel(int stepId, bool validateIt)
        {
            StepInfo stepInfo = TaskManager.Get(stepId);
            StepModelHelper stepModelHelper = GetStepModelhelper(stepId);

            long metadataStructureId = Convert.ToInt64(TaskManager.Bus[CreateDatasetTaskmanager.METADATASTRUCTURE_ID]);
            long Id = stepModelHelper.Usage.Id;

            MetadataCompoundAttributeModel model = MetadataCompoundAttributeModel.ConvertToModel(stepModelHelper.Usage, stepModelHelper.Number);

            // get children
            model.ConvertMetadataAttributeModels(LoadUsage(stepModelHelper.Usage), metadataStructureId, stepInfo.Id);
            model.StepInfo = TaskManager.Get(stepId);

            if (stepInfo.IsInstanze == false)
            {
                //get Instance
                if (TaskManager.Bus.ContainsKey(CreateDatasetTaskmanager.METADATA_XML))
                {
                    XDocument xMetadata = (XDocument)TaskManager.Bus[CreateDatasetTaskmanager.METADATA_XML];
                    model.ConvertInstance(xMetadata, stepModelHelper.XPath);
                }
            }
            else
            {
                if (stepModelHelper.Model != null)
                {
                    model = (MetadataCompoundAttributeModel)stepModelHelper.Model;
                }
                else
                {
                    stepModelHelper.Model = model;
                }
            }

            //if (validateIt)
            //{
            //    //validate packages
            //    List<Error> errors = validateStep(stepModelHelper.Model);
            //    if (errors != null)
            //        model.ErrorList = errors;
            //    else
            //        model.ErrorList = new List<Error>();

            //}


            model.StepInfo = stepInfo;

            return model;
        }

        #endregion

        #region Add and Remove

        public ActionResult AddMetadataAttributeUsage(int id, int parentid, int number, int parentModelNumber, int parentStepId)
        {
            TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];

            long metadataStructureId = Convert.ToInt64(TaskManager.Bus[CreateDatasetTaskmanager.METADATASTRUCTURE_ID]);

            List<StepModelHelper> list = (List<StepModelHelper>)TaskManager.Bus[CreateDatasetTaskmanager.METADATA_STEP_MODEL_HELPER];

            StepModelHelper stepModelHelperParent = list.Where(s => s.StepId.Equals(parentStepId)).FirstOrDefault();


            BaseUsage parentUsage = LoadUsage(stepModelHelperParent.Usage);
            int pNumber = stepModelHelperParent.Number;

            BaseUsage metadataAttributeUsage = UsageHelper.GetChildren(parentUsage).Where(u => u.Id.Equals(id)).FirstOrDefault();

            //BaseUsage metadataAttributeUsage = UsageHelper.GetSimpleUsageById(parentUsage, id);

            MetadataAttributeModel modelAttribute = MetadataAttributeModel.Convert(metadataAttributeUsage, parentUsage, metadataStructureId, parentModelNumber, stepModelHelperParent.StepId);
            modelAttribute.Number = ++number;

            AbstractMetadataStepModel model = stepModelHelperParent.Model;

            model.MetadataAttributeModels.Insert(number - 1, modelAttribute);
            UpdateChildrens(stepModelHelperParent);


            //addtoxml
            AddAttributeToXml(parentUsage, parentModelNumber, metadataAttributeUsage, number);

            model.ConvertInstance((XDocument)TaskManager.Bus[CreateDatasetTaskmanager.METADATA_XML], stepModelHelperParent.XPath + "//" + metadataAttributeUsage.Label.Replace(" ", string.Empty));

            if (model != null)
            {

                if (model is MetadataPackageModel)
                {
                    return PartialView("_metadataPackageUsageView", stepModelHelperParent);
                }

                if (model is MetadataCompoundAttributeModel)
                {
                    return PartialView("_metadataCompoundAttributeView", stepModelHelperParent);
                }
            }

            return PartialView("_metadataCompoundAttributeView", stepModelHelperParent);

        }

        public ActionResult RemoveMetadataAttributeUsage(object value, int id, int parentid, int number, int parentModelNumber, int parentStepId)
        {
            TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];
            List<StepModelHelper> list = (List<StepModelHelper>)TaskManager.Bus[CreateDatasetTaskmanager.METADATA_STEP_MODEL_HELPER];

            StepModelHelper stepModelHelperParent = list.Where(s => s.StepId.Equals(parentStepId)).FirstOrDefault();
            stepModelHelperParent.Model.MetadataAttributeModels.RemoveAt(number - 1);
            UpdateChildrens(stepModelHelperParent);

            AbstractMetadataStepModel model = stepModelHelperParent.Model;

            BaseUsage parentUsage = stepModelHelperParent.Usage;
            BaseUsage attrUsage = UsageHelper.GetSimpleUsageById(parentUsage, id);

            //remove from xml
            RemoveAttributeToXml(stepModelHelperParent.Usage, stepModelHelperParent.Number, attrUsage, number, UsageHelper.GetNameOfType(attrUsage));

            if (model != null)
            {

                if (model is MetadataPackageModel)
                {
                    return PartialView("_metadataPackageUsageView", stepModelHelperParent);
                }

                if (model is MetadataCompoundAttributeModel)
                {
                    return PartialView("_metadataCompoundAttributeView", stepModelHelperParent);
                }
            }

            return null;
        }

        public ActionResult UpMetadataAttributeUsage(object value, int id, int parentid, int number, int parentModelNumber, int parentStepId)
        {
            TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];
            List<StepModelHelper> list = (List<StepModelHelper>)TaskManager.Bus[CreateDatasetTaskmanager.METADATA_STEP_MODEL_HELPER];

            StepModelHelper stepModelHelperParent = list.Where(s => s.StepId.Equals(parentStepId)).FirstOrDefault();

            Up(stepModelHelperParent, id, number);

            UpdateChildrens(stepModelHelperParent);


            AbstractMetadataStepModel model = stepModelHelperParent.Model;

            if (model != null)
            {
                if (model is MetadataPackageModel)
                {
                    return PartialView("_metadataPackageUsageView", stepModelHelperParent);
                }

                if (model is MetadataCompoundAttributeModel)
                {
                    return PartialView("_metadataCompoundAttributeView", stepModelHelperParent);
                }
            }

            return null;
        }

        public ActionResult DownMetadataAttributeUsage(object value, int id, int parentid, int number, int parentModelNumber, int parentStepId)
        {
            TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];
            List<StepModelHelper> list = (List<StepModelHelper>)TaskManager.Bus[CreateDatasetTaskmanager.METADATA_STEP_MODEL_HELPER];

            StepModelHelper stepModelHelperParent = list.Where(s => s.StepId.Equals(parentStepId)).FirstOrDefault();

            Down(stepModelHelperParent, id, number);

            UpdateChildrens(stepModelHelperParent);

            AbstractMetadataStepModel model = stepModelHelperParent.Model;

            if (model != null)
            {
                if (model is MetadataPackageModel)
                {
                    return PartialView("_metadataPackageUsageView", stepModelHelperParent);
                }

                if (model is MetadataCompoundAttributeModel)
                {
                    return PartialView("_metadataCompoundAttributeView", stepModelHelperParent);
                }
            }

            return null;
        }

        public ActionResult AddComplexUsage(int parentStepId, int number)
        {
            TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];

            //TaskManager.SetCurrent(TaskManager.Get(parentStepId));

            long metadataStructureId = Convert.ToInt64(TaskManager.Bus[CreateDatasetTaskmanager.METADATASTRUCTURE_ID]);
            int position = number + 1;

            StepModelHelper parentStepModelHelper = GetStepModelhelper(parentStepId);
            BaseUsage u = LoadUsage(parentStepModelHelper.Usage);

            //Create new step
            StepInfo newStep = new StepInfo(u.Label + "Type")
            {
                Id = TaskManager.GenerateStepId(),
                parentTitle = parentStepModelHelper.Model.StepInfo.title,
                Parent = parentStepModelHelper.Model.StepInfo,
                IsInstanze = true,
            };

            string xPath = parentStepModelHelper.XPath + "//" + newStep.title.Replace(" ", string.Empty) + "[" + position + "]";

            newStep.Children = GetChildrenSteps(u, newStep, xPath);

            // add to parent stepId
            parentStepModelHelper.Model.StepInfo.Children.Add(newStep);
            TaskManager.StepInfos.Add(newStep);

            // create Model
            AbstractMetadataStepModel model = null;

            if (u is MetadataAttributeUsage || u is MetadataNestedAttributeUsage)
            {
                model = MetadataCompoundAttributeModel.ConvertToModel(parentStepModelHelper.Usage, number);
                model.Number = position;
                ((MetadataCompoundAttributeModel)model).ConvertMetadataAttributeModels(LoadUsage(parentStepModelHelper.Usage), metadataStructureId, newStep.Id);
            }

            if (u is MetadataPackageUsage)
            {
                model = MetadataPackageModel.Convert(parentStepModelHelper.Usage, number);
                model.Number = position;
                ((MetadataPackageModel)model).ConvertMetadataAttributeModels(LoadUsage(parentStepModelHelper.Usage), metadataStructureId, newStep.Id);
            }

            // create StepModel for new step
            StepModelHelper newStepModelhelper = new StepModelHelper
            {
                StepId = newStep.Id,
                Usage = parentStepModelHelper.Usage,
                Number = position,
                Model = model,
                XPath = xPath,
            };

            newStepModelhelper.Model.StepInfo = newStep;
            newStepModelhelper = getChildModelsHelper(newStepModelhelper);


            // add stepmodel to dictionary
            AddStepModelhelper(newStepModelhelper);

            //Update metadata xml
            //add step to metadataxml
            AddCompoundAttributeToXml(model.Source, model.Number, parentStepModelHelper.XPath);


            // add step to parent and update title of steps
            parentStepModelHelper.Model.StepInfo.Children.Insert(newStepModelhelper.Number - 1, newStep);
            for (int i = 0; i < parentStepModelHelper.Model.StepInfo.Children.Count; i++)
            {
                StepInfo si = parentStepModelHelper.Model.StepInfo.Children.ElementAt(i);
                si.title = (i + 1).ToString();
            }

            ////add stepModel to parentStepModel
            parentStepModelHelper.Childrens.Insert(newStepModelhelper.Number - 1, newStepModelhelper);

            //update childrens of the parent step based on number
            for (int i = 0; i < parentStepModelHelper.Childrens.Count; i++)
            {
                StepModelHelper smh = parentStepModelHelper.Childrens.ElementAt(i);
                smh.Number = i + 1;
            }

            //TaskManager.SetCurrent(newStep);
            ////add childsteps
            ////newStep.Children = GetChildrenSteps(u, parentStepModelHelper.Model.StepInfo, parentStepModelHelper.XPath);
            ////TaskManager.StepInfos.Add(newStep);

            //// add step to parent and update title of steps
            //parentStepModelHelper.Model.StepInfo.Children.Insert(newStepModelhelper.Number-1,newStep);
            //for (int i = 0; i < parentStepModelHelper.Model.StepInfo.Children.Count; i++)
            //{
            //    StepInfo si = parentStepModelHelper.Model.StepInfo.Children.ElementAt(i);
            //    si.title = ""+i+1;
            //}


            //newStepModelhelper = GenerateModelsForChildrens(newStep, metadataStructureId);

            //////add stepModel to parentStepModel
            //parentStepModelHelper.Childrens.Insert(newStepModelhelper.Number - 1, newStepModelhelper);

            ////update childrens of the parent step based on number
            //for (int i = 0; i < parentStepModelHelper.Childrens.Count; i++)
            //{
            //    StepModelHelper smh = parentStepModelHelper.Childrens.ElementAt(i);
            //    smh.UpdatePosition(i + 1);
            //}

            //// load InstanzB for parentmodel
            parentStepModelHelper.Model.ConvertInstance((XDocument)(TaskManager.Bus[CreateDatasetTaskmanager.METADATA_XML]), parentStepModelHelper.XPath);

            ////Add to Steps
            ////model.StepInfo = newStep;



            if (u is MetadataAttributeUsage || u is MetadataNestedAttributeUsage)
            {
                return PartialView("_metadataCompoundAttributeView", parentStepModelHelper);
            }

            if (u is MetadataPackageUsage)
            {
                return PartialView("_metadataPackageView", parentStepModelHelper);
            }

            return null;
        }

        public ActionResult RemoveComplexUsage(int parentStepId, int number)
        {
            TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];

            TaskManager.SetCurrent(TaskManager.Get(parentStepId));

            StepModelHelper stepModelHelper = GetStepModelhelper(parentStepId);
            RemoveFromXml(stepModelHelper.XPath + "//" + UsageHelper.GetNameOfType(stepModelHelper.Usage).Replace(" ", string.Empty) + "[" + number + "]");

            BaseUsage u = LoadUsage(stepModelHelper.Usage);

            if (u is MetadataAttributeUsage || u is MetadataNestedAttributeUsage)
            {
                CreateCompoundModel(TaskManager.Current().Id, true);
            }

            if(u is MetadataPackageUsage)
            {
                stepModelHelper.Model = CreatePackageModel(TaskManager.Current().Id, true);
            }

            stepModelHelper.Childrens.RemoveAt(number-1);

            //add stepModel to parentStepModel
            for (int i = 0; i < stepModelHelper.Childrens.Count; i++)
            {
                stepModelHelper.Childrens.ElementAt(i).Number = i + 1;
            }
            
            TaskManager.Remove(TaskManager.Current(), number - 1);

            return PartialView("_metadataPackageView", stepModelHelper);
        }

        //public ActionResult AddMetadataCompoundAttributeUsage(int parentStepId, int number)
        //{

        //    TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];

        //    TaskManager.SetCurrent(TaskManager.Get(parentStepId));

        //    long metadataStructureId = Convert.ToInt64(TaskManager.Bus[CreateDatasetTaskmanager.METADATASTRUCTURE_ID]);


        //    StepModelHelper parentStepModelHelper = GetStepModelhelper(parentStepId);
        //    BaseUsage u = LoadUsage(parentStepModelHelper.Usage);

        //    string label = "";
        //    if (u is MetadataAttributeUsage)
        //        label = ((MetadataAttributeUsage)u).MetadataAttribute.Name;

        //    if (u is MetadataNestedAttributeUsage)
        //        label = ((MetadataNestedAttributeUsage)u).Member.Name;

        //    if (u is MetadataPackageUsage)
        //        label = ((MetadataPackageUsage)u).MetadataPackage.Name;


        //    //Create new step
        //    StepInfo newStep = new StepInfo(label)
        //    {
        //        Id = TaskManager.GenerateStepId(),
        //        parentTitle = parentStepModelHelper.Model.StepInfo.title,
        //        Parent = parentStepModelHelper.Model.StepInfo,
        //        IsInstanze = true,
        //    };

        //    TaskManager.StepInfos.Add(newStep);
        //    // create Model for stepRemove
        //    MetadataCompoundAttributeModel model = MetadataCompoundAttributeModel.ConvertToModel(parentStepModelHelper.Usage, number);
        //    model.Number += 1;
        //    model.ConvertMetadataAttributeModels(LoadUsage(parentStepModelHelper.Usage), metadataStructureId, newStep.Id);

        //    // create StepModel for new step
        //    StepModelHelper newStepModelhelper = new StepModelHelper
        //    {
        //        StepId = newStep.Id,
        //        Usage = parentStepModelHelper.Usage,
        //        Number = model.Number,
        //        Model = model,
        //        XPath = parentStepModelHelper.XPath + "//" + label.Replace(" ", string.Empty) + "[" + model.Number + "]"
        //    };

        //    // add stepmodel to dictionary
        //    AddStepModelhelper(newStepModelhelper);

        //    //add stepModel to parentStepModel
        //    parentStepModelHelper.Childrens.Insert(newStepModelhelper.Number - 1, newStepModelhelper);

        //    //update childrens of the parent step based on number
        //    for (int i = 0; i < parentStepModelHelper.Childrens.Count; i++)
        //    {
        //        parentStepModelHelper.Childrens.ElementAt(i).Number = i + 1;
        //    }

        //    //add to bus
        //    //AddModelIntoBus(model);

        //    UpdateChildrens(parentStepModelHelper);

        //    //Update metadata xml
        //    //add step
        //    AddCompoundAttributeToXml(model.Source, model.Number, parentStepModelHelper.XPath);


        //    // load InstanzB for parentmodel
        //    parentStepModelHelper.Model.ConvertInstance((XDocument)(TaskManager.Bus[CreateDatasetTaskmanager.METADATA_XML]), parentStepModelHelper.XPath);



        //    TaskManager.Current().Children.AddRange(UpdateStepsBasedOnUsage(newStepModelhelper.Usage, TaskManager.Current(), parentStepModelHelper.XPath));

        //    //Add to Steps
        //    model.StepInfo = newStep;

        //    return PartialView("_metadataCompountAttributeView", parentStepModelHelper);
        //}

        //public ActionResult RemoveMetadataCompoundAttributeUsage(int parentStepId, int number)
        //{
        //    TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];

        //    TaskManager.SetCurrent(TaskManager.Get(parentStepId));

        //    StepModelHelper stepModelHelper = GetStepModelhelper(parentStepId);
        //    RemoveFromXml(stepModelHelper.XPath + "//" + UsageHelper.GetNameOfType(stepModelHelper.Usage).Replace(" ", string.Empty) + "[" + number + "]");


        //    stepModelHelper.Model = CreateCompoundModel(TaskManager.Current().Id, true);

        //    stepModelHelper.Childrens.RemoveAt(number - 1);

        //    //add stepModel to parentStepModel
        //    for (int i = 0; i < stepModelHelper.Childrens.Count; i++)
        //    {
        //        stepModelHelper.Childrens.ElementAt(i).Number = i + 1;
        //    }

        //    TaskManager.Remove(TaskManager.Current(), number - 1);

        //    return PartialView("_metadataCompoundAttributeView", stepModelHelper);
        //}

        #endregion

        #region Submit And Create

        public ActionResult Submit()
        {

            SubmitDataset();

            return RedirectToAction("ReloadMetadataEditor", "CreateDataset");
        }
            
        public void SubmitDataset()
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

        private List<Tuple<StepInfo, List<Error>>> ValidatePackageModels()
        {
            List<Tuple<StepInfo, List<Error>>> errors = new List<Tuple<StepInfo, List<Error>>>(); ;

            TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];

            List<StepModelHelper> list = (List<StepModelHelper>)TaskManager.Bus[CreateDatasetTaskmanager.METADATA_STEP_MODEL_HELPER];

            foreach (StepModelHelper stepModeHelper in list)
            {
                // if model exist then validate attributes
                if (stepModeHelper.Model != null)
                {
                    if (stepModeHelper.Model.StepInfo.stepStatus == StepStatus.error)
                        errors.Add(new Tuple<StepInfo, List<Error>>(stepModeHelper.Model.StepInfo, stepModeHelper.Model.ErrorList));
                }
                // else check for required elements 
                else
                {
                    stepModeHelper.Usage = LoadUsage(stepModeHelper.Usage);
                    if (UsageHelper.HasUsagesWithSimpleType(stepModeHelper.Usage))
                    {
                        if (UsageHelper.HasRequiredSimpleTypes(stepModeHelper.Usage))
                        {
                            StepInfo step = TaskManager.Get(stepModeHelper.StepId);
                            if (step != null && step.IsInstanze)
                            {
                                Error error = new Error(ErrorType.Other, String.Format("{0} : {1} {2}", "Step: ", stepModeHelper.Usage.Label, "is not valid. There are fields that are required and not yet completed are."));
                                List<Error> tempErrors = new List<Error>();
                                tempErrors.Add(error);

                                errors.Add(new Tuple<StepInfo, List<Error>>(step, tempErrors));

                                step.stepStatus = StepStatus.error;
                            }
                        }
                    }
                }


            }

            return errors;
        }

        #endregion

        #region Validation

        //XX number of index des values nötig
        [HttpPost]
        public JsonResult ValidateMetadataAttributeUsage(object value, int id, int parentid, string parentname, int number, int parentModelNumber, int parentStepId)
        {
            TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];
            StepModelHelper stepModelHelper = GetStepModelhelper(parentStepId);

            long ParentUsageId = stepModelHelper.Usage.Id;
            BaseUsage parentUsage = LoadUsage(stepModelHelper.Usage);
            int pNumber = stepModelHelper.Number;

            BaseUsage metadataAttributeUsage = UsageHelper.GetChildren(parentUsage).Where(u => u.Id.Equals(id)).FirstOrDefault();

            //UpdateXml
            long metadataStructureId = Convert.ToInt64(TaskManager.Bus[CreateDatasetTaskmanager.METADATASTRUCTURE_ID]);
            MetadataAttributeModel model = MetadataAttributeModel.Convert(metadataAttributeUsage, parentUsage, metadataStructureId, parentModelNumber, stepModelHelper.StepId);
            model.Value = value;
            model.Number = number;

            UpdateAttribute(parentUsage, parentModelNumber, metadataAttributeUsage, number, value);

            if (stepModelHelper.Model.MetadataAttributeModels.Where(a => a.Id.Equals(id) && a.Number.Equals(number)).Count() > 0)
            {
                // select the attributeModel and change the value
                stepModelHelper.Model.MetadataAttributeModels.Where(a => a.Id.Equals(id) && a.Number.Equals(number)).FirstOrDefault().Value = model.Value;

            }
            else
            {
                stepModelHelper.Model.MetadataAttributeModels.Add(model);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult ValidateStep()
        //{
        //    TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];
        //    int currentStepIndex = TaskManager.GetCurrentStepInfoIndex();

        //    StepModelHelper stepModelHelper = GetStepModelhelper(parentStepId);

        //    long usageId = stepModelHelper.Usage.Id;
        //    BaseUsage usage = LoadUsage(stepModelHelper.Usage);
        //    int number = stepModelHelper.Number;


        //    List<Error> errors = validateStep(stepModelHelper.Model);

        //    TaskManager.Current().notExecuted = false;

        //    if (errors == null || errors.Count == 0)
        //        TaskManager.Current().SetStatus(StepStatus.success);
        //    else
        //        TaskManager.Current().SetStatus(StepStatus.error);


        //    if (TaskManager.IsRoot(TaskManager.Current().Parent.Parent))
        //        return PartialView("_metadataPackageUsageView", CreatePackageModel(currentStepIndex, true));
        //    else
        //        return PartialView("_metadataCompoundAttributeView", CreateCompoundModel(currentStepIndex, true));

        //}

        //private void Validate(StepModelHelper stepModelHelper)
        //{

        //    TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];

        //    if (stepModelHelper != null)
        //    {
        //        List<Error> errors = validateStep(stepModelHelper.Model);

        //        if (errors == null || errors.Count == 0)
        //        {
        //            TaskManager.Current().SetValid(true);
        //            TaskManager.Current().SetStatus(StepStatus.success);
        //            //UpdateXml(packageId);
        //        }
        //        else
        //        {
        //            stepModelHelper.Model.ErrorList = errors;
        //            TaskManager.Current().SetValid(false);
        //            TaskManager.Current().SetStatus(StepStatus.error);
        //        }
        //    }
        //}

        private List<Error> validateStep(AbstractMetadataStepModel pModel)
        {
            List<Error> errorList = new List<Error>();

            if (pModel != null)
            {
                foreach (MetadataAttributeModel m in pModel.MetadataAttributeModels)
                {
                    List<Error> temp = validateAttribute(m);
                    if (temp != null)
                        errorList.AddRange(temp);
                }
            }

            if (errorList.Count == 0)
                return null;
            else
                return errorList;
        }

        private List<Error> validateAttribute(MetadataAttributeModel aModel)
        {
            List<Error> errors = new List<Error>();
            //optional check
            if (aModel.MinCardinality > 0 && aModel.Value == null)
                errors.Add(new Error(ErrorType.MetadataAttribute, "is not optional", new object[] { aModel.DisplayName, aModel.Value, aModel.Number, aModel.ParentModelNumber, aModel.Parent.Label }));
            else
                if (aModel.MinCardinality > 0 && String.IsNullOrEmpty(aModel.Value.ToString()))
                    errors.Add(new Error(ErrorType.MetadataAttribute, "is not optional", new object[] { aModel.DisplayName, aModel.Value, aModel.Number, aModel.ParentModelNumber, aModel.Parent.Label }));

            //check datatype
            if (aModel.Value != null)
            {
                if (!DataTypeUtility.IsTypeOf(aModel.Value, aModel.SystemType))
                {
                    errors.Add(new Error(ErrorType.MetadataAttribute, "Value can´t convert to the type: " + aModel.SystemType + ".", new object[] { aModel.DisplayName, aModel.Value, aModel.Number, aModel.ParentModelNumber, aModel.Parent.Label }));
                }
            }

            // check Constraints
            foreach (Constraint constraint in aModel.GetMetadataAttribute().Constraints)
            {
                if (aModel.Value != null)
                {
                    if (!constraint.IsSatisfied(aModel.Value))
                    {
                        errors.Add(new Error(ErrorType.MetadataAttribute, constraint.ErrorMessage, new object[] { aModel.DisplayName, aModel.Value, aModel.Number, aModel.ParentModelNumber, aModel.Parent.Label }));
                    }
                }
            }


            if (errors.Count == 0)
                return null;
            else
                return errors;
        }


        #endregion

        #region xml

        private void AddPackageToXml(BaseUsage usage, int number, string xpath)
        {
            TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];

            XDocument metadataXml = (XDocument)TaskManager.Bus[CreateDatasetTaskmanager.METADATA_XML];

            XmlMetadataWriter xmlMetadataWriter = new XmlMetadataWriter(XmlNodeMode.xPath);


            metadataXml = xmlMetadataWriter.AddPackage(
                metadataXml,
                usage,
                number,
                UsageHelper.GetNameOfType(usage),
                UsageHelper.GetIdOfType(usage),
                UsageHelper.GetChildren(usage),
                BExIS.Xml.Helpers.XmlNodeType.MetadataPackage,
                BExIS.Xml.Helpers.XmlNodeType.MetadataPackageUsage,
                xpath);

            TaskManager.Bus[CreateDatasetTaskmanager.METADATA_XML] = metadataXml;

        }

        private void AddCompoundAttributeToXml(BaseUsage usage, int number, string xpath)
        {
            TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];

            XDocument metadataXml = (XDocument)TaskManager.Bus[CreateDatasetTaskmanager.METADATA_XML];

            XmlMetadataWriter xmlMetadataWriter = new XmlMetadataWriter(XmlNodeMode.xPath);


            metadataXml = xmlMetadataWriter.AddPackage(metadataXml, usage, number, UsageHelper.GetNameOfType(usage), UsageHelper.GetIdOfType(usage), UsageHelper.GetChildren(usage), BExIS.Xml.Helpers.XmlNodeType.MetadataAttribute, BExIS.Xml.Helpers.XmlNodeType.MetadataAttributeUsage, xpath);

            TaskManager.Bus[CreateDatasetTaskmanager.METADATA_XML] = metadataXml;

        }

        private void UpdateCompoundAttributeToXml(BaseUsage usage, int number, string xpath)
        {
            TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];

            XDocument metadataXml = (XDocument)TaskManager.Bus[CreateDatasetTaskmanager.METADATA_XML];

            XmlMetadataWriter xmlMetadataWriter = new XmlMetadataWriter(XmlNodeMode.xPath);


            metadataXml = xmlMetadataWriter.AddPackage(metadataXml, usage, number, UsageHelper.GetNameOfType(usage), UsageHelper.GetIdOfType(usage), UsageHelper.GetChildren(usage), BExIS.Xml.Helpers.XmlNodeType.MetadataAttribute, BExIS.Xml.Helpers.XmlNodeType.MetadataAttributeUsage, xpath);

            TaskManager.Bus[CreateDatasetTaskmanager.METADATA_XML] = metadataXml;

        }

        private void RemoveCompoundAttributeToXml(BaseUsage usage, int number)
        {
            TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];

            XDocument metadataXml = (XDocument)TaskManager.Bus[CreateDatasetTaskmanager.METADATA_XML];

            XmlMetadataWriter xmlMetadataWriter = new XmlMetadataWriter(XmlNodeMode.xPath);
            metadataXml = xmlMetadataWriter.RemovePackage(metadataXml, usage, number, UsageHelper.GetNameOfType(usage));

            TaskManager.Bus[CreateDatasetTaskmanager.METADATA_XML] = metadataXml;
        }

        private void RemoveFromXml(string xpath)
        {
            TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];

            XDocument metadataXml = (XDocument)TaskManager.Bus[CreateDatasetTaskmanager.METADATA_XML];

            XmlMetadataWriter xmlMetadataWriter = new XmlMetadataWriter(XmlNodeMode.xPath);
            metadataXml = xmlMetadataWriter.Remove(metadataXml, xpath);

            TaskManager.Bus[CreateDatasetTaskmanager.METADATA_XML] = metadataXml;
        }

        private void AddAttributeToXml(BaseUsage parentUsage, int parentNumber, BaseUsage attribute, int number)
        {
            TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];

            XDocument metadataXml = (XDocument)TaskManager.Bus[CreateDatasetTaskmanager.METADATA_XML];

            XmlMetadataWriter xmlMetadataWriter = new XmlMetadataWriter(XmlNodeMode.xPath);
            metadataXml = xmlMetadataWriter.AddAttribute(metadataXml, parentUsage, parentNumber, attribute, number, UsageHelper.GetNameOfType(parentUsage), UsageHelper.GetNameOfType(attribute), UsageHelper.GetIdOfType(attribute).ToString());

            TaskManager.Bus[CreateDatasetTaskmanager.METADATA_XML] = metadataXml;

            // locat path
            string path = Path.Combine(AppConfiguration.GetModuleWorkspacePath("DCM"), "metadataTemp.Xml");
            //metadataXml.Save
        }

        private void RemoveAttributeToXml(BaseUsage parentUsage, int packageNumber, BaseUsage attribute, int number, string metadataAttributeName)
        {
            TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];
            XDocument metadataXml = (XDocument)TaskManager.Bus[CreateDatasetTaskmanager.METADATA_XML];
            XmlMetadataWriter xmlMetadataWriter = new XmlMetadataWriter(XmlNodeMode.xPath);

            metadataXml = xmlMetadataWriter.RemoveAttribute(metadataXml, parentUsage, packageNumber, attribute, number, UsageHelper.GetNameOfType(parentUsage), metadataAttributeName);

            TaskManager.Bus[CreateDatasetTaskmanager.METADATA_XML] = metadataXml;
            // locat path
            //string path = Path.Combine(AppConfiguration.GetModuleWorkspacePath("DCM"), "metadataTemp.Xml");
            //metadataXml.Save
        }

        private void UpdateAttribute(BaseUsage parentUsage, int packageNumber, BaseUsage attribute, int number, object value)
        {
            TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];
            XDocument metadataXml = (XDocument)TaskManager.Bus[CreateDatasetTaskmanager.METADATA_XML];
            XmlMetadataWriter xmlMetadataWriter = new XmlMetadataWriter(XmlNodeMode.xPath);

            metadataXml = xmlMetadataWriter.Update(metadataXml, parentUsage, packageNumber, attribute, number, value, UsageHelper.GetNameOfType(parentUsage), UsageHelper.GetNameOfType(attribute));

            TaskManager.Bus[CreateDatasetTaskmanager.METADATA_XML] = metadataXml;
            // locat path
            string path = Path.Combine(AppConfiguration.GetModuleWorkspacePath("DCM"), "metadataTemp.Xml");
            metadataXml.Save(path);
        }


        #endregion

        #region attribute

        private StepModelHelper UpdateChildrens(StepModelHelper stepModelHelper)
        {
            int count = stepModelHelper.Model.MetadataAttributeModels.Count;

            for (int i = 0; i < count; i++)
            {
                MetadataAttributeModel child = stepModelHelper.Model.MetadataAttributeModels.ElementAt(i);
                child.NumberOfSourceInPackage = count;
                child.Number = i + 1;
            }

            stepModelHelper.Model.MetadataAttributeModels = UpdateFirstAndLast(stepModelHelper.Model.MetadataAttributeModels);

            return stepModelHelper;
        }

        private StepModelHelper Up(StepModelHelper stepModelHelperParent, long id, int number)
        {
            List<MetadataAttributeModel> list = stepModelHelperParent.Model.MetadataAttributeModels;

            MetadataAttributeModel temp = list.Where(m => m.Id.Equals(id) && m.Number.Equals(number)).FirstOrDefault();
            int index = list.IndexOf(temp);

            list.RemoveAt(index);
            list.Insert(number - 2, temp);

            return stepModelHelperParent;
        }

        private StepModelHelper Down(StepModelHelper stepModelHelperParent, long id, int number)
        {
            List<MetadataAttributeModel> list = stepModelHelperParent.Model.MetadataAttributeModels;

            MetadataAttributeModel temp = list.Where(m => m.Id.Equals(id) && m.Number.Equals(number)).FirstOrDefault();
            int index = list.IndexOf(temp);

            list.RemoveAt(index);
            list.Insert(number, temp);

            return stepModelHelperParent;
        }

        #endregion

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

        private StepModelHelper AddStepModelhelper(StepModelHelper stepModelHelper)
        {
            TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];
            if (TaskManager.Bus.ContainsKey(CreateDatasetTaskmanager.METADATA_STEP_MODEL_HELPER))
            {
                ((List<StepModelHelper>)TaskManager.Bus[CreateDatasetTaskmanager.METADATA_STEP_MODEL_HELPER]).Add(stepModelHelper);

                return stepModelHelper;
            }

            return stepModelHelper;
        }

        private StepModelHelper GetStepModelhelper(int stepId)
        {
            TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];
            if (TaskManager.Bus.ContainsKey(CreateDatasetTaskmanager.METADATA_STEP_MODEL_HELPER))
            {
                return ((List<StepModelHelper>)TaskManager.Bus[CreateDatasetTaskmanager.METADATA_STEP_MODEL_HELPER]).Where(s => s.StepId.Equals(stepId)).FirstOrDefault();
            }

            return null;
        }

        private StepModelHelper GetStepModelhelper(long usageId, int number)
        {
            TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];
            if (TaskManager.Bus.ContainsKey(CreateDatasetTaskmanager.METADATA_STEP_MODEL_HELPER))
            {
                return ((List<StepModelHelper>)TaskManager.Bus[CreateDatasetTaskmanager.METADATA_STEP_MODEL_HELPER]).Where(s => s.Usage.Id.Equals(usageId) && s.Number.Equals(number)).FirstOrDefault();
            }

            return null;
        }

        private int GetNumberOfUsageInStepModelHelper(long usageId)
        {
            TaskManager = (CreateDatasetTaskmanager)Session["CreateDatasetTaskmanager"];
            if (TaskManager.Bus.ContainsKey(CreateDatasetTaskmanager.METADATA_STEP_MODEL_HELPER))
            {
                return ((List<StepModelHelper>)TaskManager.Bus[CreateDatasetTaskmanager.METADATA_STEP_MODEL_HELPER]).Where(s => s.Usage.Id.Equals(usageId)).Count()-1;
            }

            return 0;
        }

        private void GenerateModelsForChildrens(StepModelHelper modelHelper, long metadataStructureId)
        {
            foreach (StepModelHelper item in modelHelper.Childrens)
            {
                if (item.Childrens.Count() > 0)
                {
                    GenerateModelsForChildrens(item, metadataStructureId);
                }

                if (item.Model == null)
                {
                    BaseUsage u = LoadUsage(item.Usage);
                    if (u is MetadataAttributeUsage || u is MetadataNestedAttributeUsage)
                    {
                        item.Model = MetadataCompoundAttributeModel.ConvertToModel(u, item.Number);
                        ((MetadataCompoundAttributeModel)item.Model).ConvertMetadataAttributeModels(u, metadataStructureId, item.StepId);
                    }

                    if (u is MetadataPackageUsage)
                    {
                        item.Model = MetadataPackageModel.Convert(u, item.Number);
                        ((MetadataPackageModel)item.Model).ConvertMetadataAttributeModels(u, metadataStructureId, item.StepId);
                    }
                }
            }
            
        }

        private StepModelHelper GenerateModelsForChildrens(StepInfo stepInfo, long metadataStructureId)
        {
            StepModelHelper stepModelHelper = GetStepModelhelper(stepInfo.Id);

            if (stepModelHelper.Model == null)
            {
                if (stepModelHelper.Usage is MetadataPackageUsage)
                    stepModelHelper.Model = CreatePackageModel(stepInfo.Id, false);

                if (stepModelHelper.Usage is MetadataNestedAttributeUsage)
                    stepModelHelper.Model = CreateCompoundModel(stepInfo.Id, false);

                getChildModelsHelper(stepModelHelper);
            }

            return stepModelHelper;
        }

        public List<ListViewItem> LoadMetadataStructureViewList()
        {
            MetadataStructureManager msm = new MetadataStructureManager();
            List<ListViewItem> temp = new List<ListViewItem>();

            foreach (MetadataStructure metadataStructure in msm.Repo.Get())
            {
                string title = metadataStructure.Name;

                temp.Add(new ListViewItem(metadataStructure.Id, title));
            }

            return temp.OrderBy(p => p.Title).ToList();
        }

        public List<ListViewItem> LoadDataStructureViewList()
        {
            DataStructureManager dsm = new DataStructureManager();
            List<ListViewItem> temp = new List<ListViewItem>();

            foreach (DataStructure dataStructure in dsm.AllTypesDataStructureRepo.Get())
            {
                string title = dataStructure.Name;

                temp.Add(new ListViewItem(dataStructure.Id, title));
            }

            return temp.OrderBy(p => p.Title).ToList();
        }

        private BaseUsage LoadUsage(BaseUsage usage)
        {
            if (usage is MetadataPackageUsage)
            {
                MetadataStructureManager msm = new MetadataStructureManager();
                return msm.PackageUsageRepo.Get(usage.Id);
            }

            if (usage is MetadataNestedAttributeUsage)
            {
                MetadataAttributeManager mam = new MetadataAttributeManager();

                var x = from c in mam.MetadataCompoundAttributeRepo.Get()
                        from u in c.Self.MetadataNestedAttributeUsages
                        where u.Id == usage.Id //&& c.Id.Equals(parentId)
                        select u;

                return x.FirstOrDefault();
            }

            if (usage is MetadataAttributeUsage)
            {
                MetadataPackageManager mpm = new MetadataPackageManager();

                var q = from p in mpm.MetadataPackageRepo.Get()
                        from u in p.MetadataAttributeUsages
                        where u.Id == usage.Id // && p.Id.Equals(parentId)
                        select u;

                return q.FirstOrDefault();
            }


            return usage;
        }


        #endregion

        #region load & update advanced steps

        private StepInfo AddStepsBasedOnUsage(BaseUsage usage, StepInfo current, string parentXpath)
        {
            // genertae action, controller base on usage
            string actionName = "";
            string childName = "";
            int min = usage.MinCardinality;

            if (usage is MetadataPackageUsage)
            {
                actionName = "SetMetadataPackageInstanze";
                childName = ((MetadataPackageUsage)usage).MetadataPackage.Name;
            }
            else
            {
                actionName = "SetMetadataCompoundAttributeInstanze";

                if (usage is MetadataNestedAttributeUsage)
                    childName = ((MetadataNestedAttributeUsage)usage).Member.Name;

                if (usage is MetadataAttributeUsage)
                    childName = ((MetadataAttributeUsage)usage).MetadataAttribute.Name;

            }

            List<StepInfo> list = new List<StepInfo>();
            List<StepModelHelper> stepHelperModelList = (List<StepModelHelper>)TaskManager.Bus[CreateDatasetTaskmanager.METADATA_STEP_MODEL_HELPER];

            if (TaskManager.Bus.ContainsKey(CreateDatasetTaskmanager.METADATA_XML))
            {
                XDocument xMetadata = (XDocument)TaskManager.Bus[CreateDatasetTaskmanager.METADATA_XML];

                var x = new XElement("null");
                List<XElement> elements = new List<XElement>();

                Dictionary<string, string> keyValueDic = new Dictionary<string, string>();
                keyValueDic.Add("id", usage.Id.ToString());

                if (usage is MetadataPackageUsage)
                {
                    keyValueDic.Add("type", BExIS.Xml.Helpers.XmlNodeType.MetadataPackageUsage.ToString());
                    elements = XmlUtility.GetXElementsByAttribute(usage.Label, keyValueDic, xMetadata).ToList();
                }
                else
                {
                    keyValueDic.Add("type", BExIS.Xml.Helpers.XmlNodeType.MetadataAttributeUsage.ToString());
                    elements = XmlUtility.GetXElementsByAttribute(usage.Label, keyValueDic, xMetadata).ToList();
                }

                x = elements.FirstOrDefault();

                if (x != null && !x.Name.Equals("null"))
                {
                    IEnumerable<XElement> xelements = x.Elements();

                    if (xelements.Count() > 0)
                    {
                        int counter = 0;

                        foreach (XElement element in xelements)
                        {
                            counter++;
                            string title = counter.ToString(); //usage.Label+" (" + counter + ")";
                            long id = Convert.ToInt64((element.Attribute("roleId")).Value.ToString());

                            StepInfo s = new StepInfo(title)
                            {
                                Id = TaskManager.GenerateStepId(),
                                Parent = current,
                                IsInstanze = true,
                                HasContent = UsageHelper.HasUsagesWithSimpleType(usage),
                                //GetActionInfo = new ActionInfo
                                //{
                                //    ActionName = actionName,
                                //    ControllerName = "CreateSetMetadataPackage",
                                //    AreaName = "DCM"
                                //},

                                //PostActionInfo = new ActionInfo
                                //{
                                //    ActionName = actionName,
                                //    ControllerName = "CreateSetMetadataPackage",
                                //    AreaName = "DCM"
                                //}
                            };

                            string xPath = parentXpath + "//" + childName.Replace(" ", string.Empty) + "[" + counter + "]";

                            s.Children = GetChildrenSteps(usage, s, xPath);

                            if (s.Children.Count == 0)
                            {

                            }

                            current.Children.Add(s);

                            if (TaskManager.Root.Children.Where(z => z.title.Equals(title)).Count() == 0)
                            {
                                stepHelperModelList.Add(new StepModelHelper(s.Id, 1, usage, xPath));
                            }

                        }
                    }
                }

                //TaskManager.AddToBus(CreateDatasetTaskmanager.METADATAPACKAGE_IDS, MetadataPackageDic);
            }
            return current;
        }

        private List<StepInfo> GetChildrenSteps(BaseUsage usage, StepInfo parent, string parentXpath)
        {
            List<StepInfo> childrenSteps = new List<StepInfo>();
            List<BaseUsage> childrenUsages = UsageHelper.GetCompoundChildrens(usage);
            List<StepModelHelper> stepHelperModelList = (List<StepModelHelper>)TaskManager.Bus[CreateDatasetTaskmanager.METADATA_STEP_MODEL_HELPER];

            if (childrenUsages.Count > 0)
            {
                foreach (BaseUsage u in childrenUsages)
                {

                    string xPath = parentXpath + "//" + u.Label.Replace(" ", string.Empty) + "[1]";

                    bool complex = false;

                    string actionName = "";
                    string attrName = "";

                    if (u is MetadataPackageUsage)
                    {
                        actionName = "SetMetadataPackage";
                    }
                    else
                    {
                        actionName = "SetMetadataCompoundAttribute";

                        if (u is MetadataAttributeUsage)
                        {
                            MetadataAttributeUsage mau = (MetadataAttributeUsage)u;
                            if (mau.MetadataAttribute.Self is MetadataCompoundAttribute)
                            {
                                complex = true;
                                attrName = mau.MetadataAttribute.Self.Name;
                            }
                        }

                        if (u is MetadataNestedAttributeUsage)
                        {
                            MetadataNestedAttributeUsage mau = (MetadataNestedAttributeUsage)u;
                            if (mau.Member.Self is MetadataCompoundAttribute)
                            {
                                complex = true;
                                attrName = mau.Member.Self.Name;
                            }
                        }

                    }

                    if (complex)
                    {
                        StepInfo s = new StepInfo(u.Label)
                        {
                            Id = TaskManager.GenerateStepId(),
                            parentTitle = attrName,
                            Parent = parent,
                            IsInstanze = false,
                            GetActionInfo = new ActionInfo
                            {
                                ActionName = actionName,
                                ControllerName = "CreateSetMetadataPackage",
                                AreaName = "DCM"
                            },

                            PostActionInfo = new ActionInfo
                            {
                                ActionName = actionName,
                                ControllerName = "CreateSetMetadataPackage",
                                AreaName = "DCM"
                            }
                        };

                        //only not optional
                        s = AddStepsBasedOnUsage(u, s, xPath);
                        childrenSteps.Add(s);

                        if (TaskManager.StepInfos.Where(z => z.Id.Equals(s.Id)).Count() == 0)
                        {
                            // MetadataPackageDic.Add(s.Id, u.Id);
                            stepHelperModelList.Add(new StepModelHelper(s.Id, 1, u, xPath));
                        }
                    }
                    //}

                }
            }


            return childrenSteps;
        }

        private List<StepInfo> UpdateStepsBasedOnUsage(BaseUsage usage, StepInfo currentSelected, string parentXpath)
        {

            // genertae action, controller base on usage
            string actionName = "";
            string childName = "";
            if (usage is MetadataPackageUsage)
            {
                actionName = "SetMetadataPackageInstanze";
                childName = ((MetadataPackageUsage)usage).MetadataPackage.Name;
            }
            else
            {
                actionName = "SetMetadataCompoundAttributeInstanze";

                if (usage is MetadataNestedAttributeUsage)
                    childName = ((MetadataNestedAttributeUsage)usage).Member.Name;

                if (usage is MetadataAttributeUsage)
                    childName = ((MetadataAttributeUsage)usage).MetadataAttribute.Name;

            }

            List<StepInfo> list = new List<StepInfo>();
            List<StepModelHelper> stepHelperModelList = (List<StepModelHelper>)TaskManager.Bus[CreateDatasetTaskmanager.METADATA_STEP_MODEL_HELPER];

            if (TaskManager.Bus.ContainsKey(CreateDatasetTaskmanager.METADATA_XML))
            {
                XDocument xMetadata = (XDocument)TaskManager.Bus[CreateDatasetTaskmanager.METADATA_XML];

                var x = XmlUtility.GetXElementByXPath(parentXpath, xMetadata);

                if (x != null && !x.Name.Equals("null"))
                {

                    StepInfo current = currentSelected;
                    IEnumerable<XElement> xelements = x.Elements();

                    if (xelements.Count() > 0)
                    {
                        int counter = 0;
                        foreach (XElement element in xelements)
                        {
                            counter++;
                            string title = counter.ToString();

                            if(current.Children.Where(s=>s.title.Equals(title)).Count()==0)
                            {
                                long id = Convert.ToInt64((element.Attribute("roleId")).Value.ToString());

                                StepInfo s = new StepInfo(title)
                                {
                                    Id = TaskManager.GenerateStepId(),
                                    Parent = current,
                                    IsInstanze = true,
                                    //GetActionInfo = new ActionInfo
                                    //{
                                    //    ActionName = actionName,
                                    //    ControllerName = "CreateSetMetadataPackage",
                                    //    AreaName = "DCM"
                                    //},

                                    //PostActionInfo = new ActionInfo
                                    //{
                                    //    ActionName = actionName,
                                    //    ControllerName = "CreateSetMetadataPackage",
                                    //    AreaName = "DCM"
                                    //}
                                };

                                string xPath = parentXpath + "//" + childName.Replace(" ",string.Empty) + "[" + counter + "]";

                                s.Children = GetChildrenStepsUpdated(usage, s, xPath);
                                list.Add(s);

                                if (TaskManager.Root.Children.Where(z => z.Id.Equals(s.Id)).Count() == 0)
                                {
                                    StepModelHelper newStepModelHelper = new StepModelHelper(s.Id, counter, usage, xPath);

                                    stepHelperModelList.Add(newStepModelHelper);
                                }
                            }//end if
                        }//end foreach
                    }//end if
                }

            }
            return list;
        }

        private List<StepInfo> GetChildrenStepsUpdated(BaseUsage usage, StepInfo parent, string parentXpath)
            {
                List<StepInfo> childrenSteps = new List<StepInfo>();
                List<BaseUsage> childrenUsages = UsageHelper.GetCompoundChildrens(usage);
                List<StepModelHelper> stepHelperModelList = (List<StepModelHelper>)TaskManager.Bus[CreateDatasetTaskmanager.METADATA_STEP_MODEL_HELPER];

                foreach (BaseUsage u in childrenUsages)
                {
                    string label = u.Label.Replace(" ", string.Empty);
                    string xPath = parentXpath + "//" + label + "[1]";
                    bool complex = false;

                    string actionName = "";

                    if (u is MetadataPackageUsage)
                    {
                        actionName = "SetMetadataPackage";
                    }
                    else
                    {
                        actionName = "SetMetadataCompoundAttribute";

                        if (u is MetadataAttributeUsage)
                        {
                            MetadataAttributeUsage mau = (MetadataAttributeUsage)u;
                            if (mau.MetadataAttribute.Self is MetadataCompoundAttribute)
                                complex = true;
                        }

                        if (u is MetadataNestedAttributeUsage)
                        {
                            MetadataNestedAttributeUsage mau = (MetadataNestedAttributeUsage)u;
                            if (mau.Member.Self is MetadataCompoundAttribute)
                                complex = true;
                        }

                    }

                    if (complex)
                    {
                        StepInfo s = new StepInfo(u.Label)
                        {
                            Id = TaskManager.GenerateStepId(),
                            Parent = parent,
                            IsInstanze = false,
                            //GetActionInfo = new ActionInfo
                            //{
                            //    ActionName = actionName,
                            //    ControllerName = "CreateSetMetadataPackage",
                            //    AreaName = "DCM"
                            //},

                            //PostActionInfo = new ActionInfo
                            //{
                            //    ActionName = actionName,
                            //    ControllerName = "CreateSetMetadataPackage",
                            //    AreaName = "DCM"
                            //}
                        };

                        s.Children = UpdateStepsBasedOnUsage(u, s, xPath).ToList();
                        childrenSteps.Add(s);

                        if (TaskManager.Root.Children.Where(z => z.title.Equals(s.title)).Count() == 0)
                        {
                            stepHelperModelList.Add(new StepModelHelper(s.Id, 1, u, xPath));
                        }
                    }

                }

                return childrenSteps;
            }

        #endregion

        #region bus functions

        private List<MetadataAttributeModel> UpdateFirstAndLast(List<MetadataAttributeModel> list)
        {
            foreach (MetadataAttributeModel x in list)
            {
                if (list.First().Equals(x)) x.first = true;
                else x.first = false;

                if (list.Last().Equals(x)) x.last = true;
                else x.last = false;
            }


            return list;
        }

        private long GetUsageId(int stepId)
        {

            if (TaskManager.Bus.ContainsKey(CreateDatasetTaskmanager.METADATA_STEP_MODEL_HELPER))
            {
                List<StepModelHelper> list = (List<StepModelHelper>)TaskManager.Bus[CreateDatasetTaskmanager.METADATA_STEP_MODEL_HELPER];
                return list.Where(s => s.StepId.Equals(stepId)).FirstOrDefault().Usage.Id;

            }

            return 0;

        }

        private BaseUsage GetMetadataCompoundAttributeUsage(long id)
        {
            //return UsageHelper.GetChildrenUsageById(Id);
            return UsageHelper.GetMetadataAttributeUsageById(id);
        }

        private BaseUsage GetPackageUsage(long Id)
        {
            MetadataStructureManager mpm = new MetadataStructureManager();
            return mpm.PackageUsageRepo.Get(Id);
        }

        #endregion

    }
}
