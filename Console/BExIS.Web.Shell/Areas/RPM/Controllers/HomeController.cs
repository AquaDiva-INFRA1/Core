﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BExIS.Dlm.Entities.DataStructure;
using BExIS.Dlm.Services.DataStructure;
using BExIS.Dlm.Services.TypeSystem;
using BExIS.RPM.Model;
using BExIS.RPM.Output;
using Vaiona.Persistence.Api;
using Vaiona.Utils.Cfg;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace BExIS.Web.Shell.Areas.RPM.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Planing/Home/

        string templateName = "BExISppTemplate_Clean.xlsm";

        public ActionResult Index()
        {
            DataStructureManager dm = new DataStructureManager();
            dm.StructuredDataStructureRepo.Get();
            return View();
        }

        #region Data Structure Designer

        private void setSessions()
        {
            Session["Window"] = false;
            Session["VariableWindow"] = false;
            Session["DatasetWindow"] = false;
            Session["saveAsWindow"] = false;
        }

        public ActionResult DataStructureDesigner()
        {
            DataStructureDesignerModel DSDM = new DataStructureDesignerModel();
            DSDM.show = false;
            
            setSessions();

            return View("DataStructureDesigner", DSDM);
        }

        public ActionResult createStructuredDataStrukture()
        {
            DataStructureDesignerModel DSDM = new DataStructureDesignerModel();

            Session["Structured"] = true;
            setSessions();

            return View("DataStructureDesigner", DSDM); 
                }
        
        public ActionResult createUnStructuredDataStrukture()
                {
            DataStructureDesignerModel DSDM = new DataStructureDesignerModel();
            DSDM.structured = false;

            Session["Structured"] = false;
            setSessions();

            return View("DataStructureDesigner", DSDM);
        }

        #region Data Structure Info

        public ActionResult showDataStructure(string SelectedItem)
        {
            DataStructureDesignerModel DSDM = new DataStructureDesignerModel();
            string[] temp = SelectedItem.Split(',');

            DSDM.GetDataStructureByID(Convert.ToInt64(temp[0]), Convert.ToBoolean(temp[1]));
            Session["Structured"] = DSDM.structured;
            return View("DataStructureDesigner", DSDM);
        }

        #endregion

        #region save Datastructure

        public ActionResult saveDataStructure(DataStructureDesignerModel DSDM, string category, string[] varName, long[] optional, long[] varId, string[] varDesc)
        {
            DataStructureManager DSM = new DataStructureManager();
            DSDM.dataStructure.Name = cutSpaces(DSDM.dataStructure.Name);
            DSDM.dataStructure.Description = cutSpaces(DSDM.dataStructure.Description);
            DSDM.structured = (bool)Session["Structured"];
            List<string> errorMsg = new List<string>();

            if(DSDM.dataStructure.Id == 0)
            {
                if (dataStructureValidation(DSDM.dataStructure) != null)
                    errorMsg.Add(dataStructureValidation(DSDM.dataStructure));

                if (errorMsg.Count() > 0)
                {
                    ViewData["errorMsg"] = errorMsg;
                    return View("DataStructureDesigner", DSDM);
                }
                else
                {
                    if (DSDM.dataStructure.Name != "" && DSDM.dataStructure.Name != null)
                    {
                        DataStructureCategory DSC = new DataStructureCategory();
                        
                        foreach (DataStructureCategory dsc in Enum.GetValues(typeof(DataStructureCategory)))
                        {
                            if (dsc.ToString().Equals(category))
                            {
                                DSC = dsc;
                            }
                        }
                        if (DSDM.structured)
                        {
                            ExcelTemplateProvider provider = new ExcelTemplateProvider(templateName);
                            StructuredDataStructure DS = DSM.CreateStructuredDataStructure(DSDM.dataStructure.Name, DSDM.dataStructure.Description, "", "", DSC, null);
                            DSM.UpdateStructuredDataStructure(DS);
                            provider.CreateTemplate(DS.Id);
                            DSDM.GetDataStructureByID(DS.Id);
                            DSDM.dataStructureTree = DSDM.getDataStructureTree();
                        }
                        else
                        { 
                            DSDM.dataStructure = DSM.CreateUnStructuredDataStructure(DSDM.dataStructure.Name, DSDM.dataStructure.Description);
                            DSDM.GetDataStructureByID(DSDM.dataStructure.Id);
                            DSDM.dataStructureTree = DSDM.getDataStructureTree();
                        }
                    }
                    else
                    {
                        errorMsg.Add("Please type a Name");
                        ViewData["errorMsg"] = errorMsg;
                        return View("DataStructureDesigner", DSDM);
                    }
                }
            }
            else
            {   
                if (Request.Params["create"] == "save")
                {
                    if (varName != null)
                    {
                        bool opt = false;
                        string tempMsg = null;

                        for (int i = 0; i < varId.Count(); i++)
                        {
                            if (optional != null)
                            {
                                if (optional.Contains(varId[i]))
                                    opt = true;
                                else
                                    opt = false;
                            }
                            else
                            {
                                opt = false;
                            }

                            tempMsg = saveVariable(cutSpaces(varName[i]), varId[i],varDesc[i], DSDM.dataStructure.Id, opt);

                            if (tempMsg != null)
                                errorMsg.Add(tempMsg);
                        }
                    }
                    if (dataStructureValidation(DSDM.dataStructure) != null)
                        errorMsg.Add(dataStructureValidation(DSDM.dataStructure));
                }
                else if (Request.Params["create"] == "saveAs")
                {
                    if (openSaveAsWindow(DSDM.dataStructure))
                    {
                        DataStructure tempDS = new StructuredDataStructure();
                        tempDS.Name = DSDM.dataStructure.Name;
                        if (dataStructureValidation(tempDS) != null)
                            errorMsg.Add(dataStructureValidation(tempDS));

                        ViewData["errorMsg"] = errorMsg;
                        DSDM.GetDataStructureByID(DSDM.dataStructure.Id, DSDM.structured);
                        return View("DataStructureDesigner", DSDM);
                    }
                }

                if (errorMsg.Count() > 0)
                {
                    ViewData["errorMsg"] = errorMsg;
                    DSDM.GetDataStructureByID(DSDM.dataStructure.Id, DSDM.structured);
                    return View("DataStructureDesigner", DSDM);
                }
                else
                {
                    DataStructureCategory DSC = new DataStructureCategory();

                    foreach (DataStructureCategory dsc in Enum.GetValues(typeof(DataStructureCategory)))
                    {
                        if (dsc.ToString().Equals(category))
                        {
                            DSC = dsc;
                        }
                    }
                    if (DSDM.structured)
                    {
                        StructuredDataStructure DS = new StructuredDataStructure();
                        ExcelTemplateProvider provider = new ExcelTemplateProvider(templateName);
                        if (Request.Params["create"] == "save")
                        {
                            DS = DSM.StructuredDataStructureRepo.Get(DSDM.dataStructure.Id);
                            provider.deleteTemplate(DS.Id);
                            DS.Name = DSDM.dataStructure.Name;
                            DS.Description = DSDM.dataStructure.Description;
                            DS = DSM.UpdateStructuredDataStructure(DS);
                        }
                        else if (Request.Params["create"] == "saveAs")
                        {
                            StructuredDataStructure DsOld = DSM.StructuredDataStructureRepo.Get(DSDM.dataStructure.Id);
                            DS = DSM.CreateStructuredDataStructure(DSDM.dataStructure.Name, DSDM.dataStructure.Description, "", "", DSC, null);
                            List<Variable> variables = DSDM.getOrderedVariables(DSM.StructuredDataStructureRepo.Get(DsOld.Id));
                            XmlDocument doc = (XmlDocument)DS.Extra;
                            if (doc == null)
                            {
                                doc = new XmlDocument();
                                XmlNode root = doc.CreateNode(XmlNodeType.Element, "extra", null);
                                doc.AppendChild(root);
                            }
                            if (doc.GetElementsByTagName("original").Count == 0)
                            {
                                XmlNode original = doc.CreateNode(XmlNodeType.Element, "original", null);
                                XmlNode id = doc.CreateNode(XmlNodeType.Element, "id", null);
                                XmlNode versionNo = doc.CreateNode(XmlNodeType.Element, "versionNo", null);
                                id.InnerText = Convert.ToString(DsOld.Id);
                                versionNo.InnerText = Convert.ToString(DsOld.VersionNo);
                                original.AppendChild(id);
                                original.AppendChild(versionNo);
                                doc.FirstChild.AppendChild(original);
                            }
                            if(doc.GetElementsByTagName("order").Count == 0)
                                if (variables.Count != 0)
                                {
                                    XmlNode order = doc.CreateNode(XmlNodeType.Element, "order", null);
                                    Variable temp;
                                    bool opt = false;

                                    foreach (Variable v in variables)
                                    {
                                        if (varName != null)
                                        {
                                            for (int i = 0; i < varId.Count(); i++)
                                            {
                                                if (v.Id == varId[i])
                                                {
                                                    XmlNode variable = doc.CreateNode(XmlNodeType.Element, "variable", null);

                                                    if (optional != null)
                                                    {
                                                        if (optional.Contains(varId[i]))
                                                            opt = true;
                                                        else
                                                            opt = false;
                                                    }
                                                    else
                                                    {
                                                        opt = false;
                                                    }
                                                    if (DS.Variables.Where(p => cutSpaces(p.Label).ToLower().Equals(cutSpaces(varName[i]).ToLower())).Count() > 0 || cutSpaces(varName[i]) == "")
                                                    {
                                                        if (cutSpaces(varName[i]) == "")
                                                            errorMsg.Add("Can't rename Variable " + v.Label + ", invalid name");
                                                        else
                                                            errorMsg.Add("Can't rename Variable " + v.Label + ", name already exist");
                                                        temp = DSM.AddVariableUsage(DS, v.DataAttribute, opt, v.Label, null, null, v.Description);
                                                    }
                                                    else
                                                    {
                                                        temp = DSM.AddVariableUsage(DS, v.DataAttribute, opt, cutSpaces(varName[i]), null, null, varDesc[i]);
                                                    }
                                                    variable.InnerText = temp.Id.ToString();
                                                    order.AppendChild(variable);
                                                    ViewData["errorMsg"] = errorMsg;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            XmlNode variable = doc.CreateNode(XmlNodeType.Element, "variable", null);
                                            temp = DSM.AddVariableUsage(DS, v.DataAttribute, v.IsValueOptional, v.Label, null, null, v.Description);
                                            variable.InnerText = temp.Id.ToString();
                                            order.AppendChild(variable);
                                        }
                                    }
                                    doc.FirstChild.AppendChild(order);
                                    DS.Extra = doc;
                                }                                
                            DS = DSM.UpdateStructuredDataStructure(DS);
                        }
                        DSDM.GetDataStructureByID(DS.Id, DSDM.structured);
                        provider.CreateTemplate(DSM.StructuredDataStructureRepo.Get(DSDM.dataStructure.Id));
                        DSDM.dataStructureTree = DSDM.getDataStructureTree();
                    }
                    else
                    {
                        UnStructuredDataStructure DS = new UnStructuredDataStructure();
                        if (Request.Params["create"] == "save")
                        {
                            DS = DSM.UnStructuredDataStructureRepo.Get(DSDM.dataStructure.Id);
                            DS.Name = DSDM.dataStructure.Name;
                            DS.Description = DSDM.dataStructure.Description;
                            DS = DSM.UpdateUnStructuredDataStructure(DS);
                        }
                        else if (Request.Params["create"] == "saveAs")
                        {
                            DS = DSM.CreateUnStructuredDataStructure(DSDM.dataStructure.Name, DSDM.dataStructure.Description);                    
                        }
                        DSDM.GetDataStructureByID(DS.Id, DSDM.structured);
                        DSDM.dataStructureTree = DSDM.getDataStructureTree();
                    }
                }
            }

            Session["Window"] = false;
            Session["saveAsWindow"] = false;
            return View("DataStructureDesigner", DSDM);
        }

        private bool openSaveAsWindow(DataStructure dataStructure)
        {
            DataStructureManager dataStructureManager = new DataStructureManager();
            List<DataStructure> dataStructureList = new List<DataStructure>();
            
            List<StructuredDataStructure> StrTemp = dataStructureManager.StructuredDataStructureRepo.Get().ToList();
            foreach (DataStructure ds in StrTemp)
            {
                dataStructureList.Add(ds);
            }

            List<UnStructuredDataStructure> UnSTemp = dataStructureManager.UnStructuredDataStructureRepo.Get().ToList();
            foreach (DataStructure ds in UnSTemp)
            {
                dataStructureList.Add(ds);
            }

            if (cutSpaces(dataStructure.Name) == "" || cutSpaces(dataStructure.Name) == null)
            {
                Session["saveAsWindow"] = true;
                return true;
            }
            else if (dataStructureList.Where(p => cutSpaces(p.Name).ToLower().Equals(cutSpaces(dataStructure.Name).ToLower())).Count() > 0)
            {
                Session["saveAsWindow"] = true;
                return true;
            }
            Session["saveAsWindow"] = false;
            return false;
        }

        private string dataStructureValidation(DataStructure dataStructure)
        {
            DataStructureManager dataStructureManager = new DataStructureManager();
            List<DataStructure> dataStructureList = new List<DataStructure>();

                List<StructuredDataStructure> StrTemp = dataStructureManager.StructuredDataStructureRepo.Get().ToList();
                foreach (DataStructure ds in StrTemp)
                {
                    dataStructureList.Add(ds);
                }
            
                List<UnStructuredDataStructure> UnSTemp = dataStructureManager.UnStructuredDataStructureRepo.Get().ToList();
                foreach (DataStructure ds in UnSTemp)
                {
                    dataStructureList.Add(ds);
                }


                if (!(dataStructureList.Where(p => p.Id.Equals(dataStructure.Id)).Count() > 0) && dataStructure.Id != 0)
                {
                        return "Can\'t save Data Structure, doesn't exist anymore.";
                }
                if (dataStructure.Datasets.Count > 0)
                {
                    return "Can\'t save/rename Data Structure, is in use.";
                }
                if (cutSpaces(dataStructure.Name) == "" || cutSpaces(dataStructure.Name) == null)
                {
                    return "Can\'t save/rename Data Structure, invalid Name.";
                }
                else if (dataStructureList.Where(p => cutSpaces(p.Name).ToLower().Equals(cutSpaces(dataStructure.Name).ToLower())).Count() > 0)
                {
                    long newDataStructureId = dataStructureList.Where(p => cutSpaces(p.Name).ToLower().Equals(cutSpaces(dataStructure.Name).ToLower())).ToList().First().Id;
                    if (newDataStructureId != dataStructure.Id)
                            return "Can\'t save Data Structure, Name already exist.";
                    }
            return null;
                }

     
        public ActionResult deleteDataStructure(long id)
        {
            bool structured = (bool)Session["Structured"];


            //string message = "" ;

            //if (name != null && name != "")
            //    message = "Delete Data Structure " + name + "?";
            //else
            //    message = "Delete Data Structure?";

            //string caption = "Confirmation";
            //MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            //DialogResult result;
            //// Displays the MessageBox.
            //result = MessageBox.Show(message, caption, buttons);
            if (id != 0)
            {
                if (structured)
                {
                    DataStructureManager dataStructureManager = new DataStructureManager();
                    StructuredDataStructure dataStructure = new StructuredDataStructure();
                    dataStructure = dataStructureManager.StructuredDataStructureRepo.Get(id);

                    if (dataStructure != null)
                    {
                        if (dataStructure.Datasets.Count == 0)
                        {
                            DataStructureManager DSM = new DataStructureManager();
                            if (dataStructure.Variables.Count > 0)
                            {
                                foreach (Variable v in dataStructure.Variables)
                                {
                                    DSM.RemoveVariableUsage(v);
                                }
                            }
                            ExcelTemplateProvider provider = new ExcelTemplateProvider();
                            provider.deleteTemplate(id);
                            DSM.DeleteStructuredDataStructure(dataStructure);
                            return RedirectToAction("DataStructureDesigner");
                        }
                        else
                        {
                            DataStructureDesignerModel DSDM = new DataStructureDesignerModel();
                            DSDM.GetDataStructureByID(id);
                            return View("DataStructureDesigner", DSDM);
                        }
                    }
                    else
                    {
                        return RedirectToAction("DataStructureDesigner");
                    }

                }
                else
                {
                    DataStructureManager dataStructureManager = new DataStructureManager();
                    UnStructuredDataStructure dataStructure = new UnStructuredDataStructure();
                    dataStructure = dataStructureManager.UnStructuredDataStructureRepo.Get(id);
                    
                    if (dataStructure != null)
                    {
                        if (dataStructure.Datasets.Count == 0)
                        {
                            DataStructureManager DSM = new DataStructureManager();
                            DSM.DeleteUnStructuredDataStructure(dataStructure);
                            return RedirectToAction("DataStructureDesigner");
                        }
                        else
                        {
                            DataStructureDesignerModel DSDM = new DataStructureDesignerModel();
                            DSDM.GetDataStructureByID(id, false);
                            return View("DataStructureDesigner", DSDM);
                        }
                    }
                    else
                    {
                        return RedirectToAction("DataStructureDesigner");
                    }
                }
            }
            return RedirectToAction("DataStructureDesigner");
        }

        #endregion

        #region add Variable

        public ActionResult showVariables(long id)
        {
            DataStructureManager dataStructureManager = new DataStructureManager();
            DataStructureDesignerModel DSDM = new DataStructureDesignerModel();
            
            if (id != 0)
            {
                DSDM.GetDataStructureByID(id);
                DataContainerManager dataAttributeManager = new DataContainerManager();
                DSDM.dataAttributeList = dataAttributeManager.DataAttributeRepo.Get().ToList();
            }
                    
            if ((bool)Session["Window"] == false)
            {
                Session["Window"] = true;
                Session["dataStructureId"] = DSDM.dataStructure.Id;
            }
            else
            {
                Session["Window"] = false;
                Session["selected"] = null;
            }
            return View("DataStructureDesigner", DSDM);
        }

        public ActionResult AddVariables()
        {
            long id = (long)Session["dataStructureId"];
            long[][] selected = (long[][])Session["selected"];

            DataStructureManager dataStructureManager = new DataStructureManager();
            StructuredDataStructure dataStructure = dataStructureManager.StructuredDataStructureRepo.Get(id);
            //StructuredDataStructure dataStructure = DSDM.GetDataStructureByID(id);

            if (dataStructure != null)
            {
                if (!(dataStructure.Datasets.Count() > 0))
                {
                    if (selected != null)
                    {
                        DataContainerManager dataAttributeManager = new DataContainerManager();
                        DataAttribute temp = new DataAttribute();
                        XmlDocument doc = (XmlDocument)dataStructure.Extra;
                        XmlNode order;
                        if (doc == null)
                        {
                            doc = new XmlDocument();
                            XmlNode root = doc.CreateNode(XmlNodeType.Element, "extra", null);
                            doc.AppendChild(root);
                        }
                        if (doc.GetElementsByTagName("order").Count != 0)
                        {
                            order = doc.GetElementsByTagName("order")[0];
                        }
                        else
                        {
                            order = order = doc.CreateNode(XmlNodeType.Element, "order", null);
                            doc.FirstChild.AppendChild(order);
                        }

                        Variable var = new Variable();
                        int count = 0;
                        string tempName = null;

                        for (int i = 0; i < selected.Length; i++)
                        {
                            count = 0;
                            temp = dataAttributeManager.DataAttributeRepo.Get(selected[i][0]);
                            tempName = temp.Name;
                            if (temp != null)
                            {
                                for (int j = 0; j < selected[i][1]; j++)
                                {
                                    while (dataStructure.Variables.Where(p => cutSpaces(p.Label).ToLower().Equals(cutSpaces(tempName).ToLower())).Count() > 0)
                                    {
                                        count++;
                                        tempName = temp.Name + " (" + count + ")";                                            
                                    }
                                    var = dataStructureManager.AddVariableUsage(dataStructure, temp, true,tempName, null, null, temp.Description);

                                XmlNode variable = doc.CreateNode(XmlNodeType.Element, "variable", null);
                                variable.InnerText = var.Id.ToString();
                                    order.AppendChild(variable);                            
                            }
                        }
                        }
                        dataStructureManager.UpdateStructuredDataStructure(dataStructure);
                        ExcelTemplateProvider provider = new ExcelTemplateProvider(templateName);
                        provider.CreateTemplate(dataStructure);
                    }
                }
            }
            else
            {
                Session["selected"] = null;
                return RedirectToAction("DataStructureDesigner");
            }
            DataStructureDesignerModel DSDM = new DataStructureDesignerModel();
            DSDM.GetDataStructureByID(dataStructure.Id);
            Session["selected"] = null;
            return View("DataStructureDesigner", DSDM);
        }

        public ActionResult deleteVariable(long id, long dataStructureId)
        {
            if (dataStructureId != 0)
            {
                //string message = "Are you sure you want to delete the Varriable?";
                //string caption = "Confirmation";
                //MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                //DialogResult result;
                //// Displays the MessageBox.
                //result = MessageBox.Show(message, caption, buttons);

                //if (result == DialogResult.Yes)
                //{
                DataStructureManager DSM = new DataStructureManager();
                StructuredDataStructure dataStructure = DSM.StructuredDataStructureRepo.Get(dataStructureId);
                DataStructureDesignerModel DSDM = new DataStructureDesignerModel();

                if (!(dataStructure.Datasets.Count > 0))
                {
                    Variable variable = DSM.VariableRepo.Get(id);

                    if (variable != null)
                    {
                        XmlDocument doc = (XmlDocument)dataStructure.Extra;

                        if (doc.GetElementsByTagName("order").Count != 0)
                        {
                            XmlNode order = doc.GetElementsByTagName("order")[0];
                            foreach (XmlNode v in order)
                            {
                                if (Convert.ToInt64(v.InnerText) == variable.Id)
                                {
                                    order.RemoveChild(v);
                                    break;
                                }
                            }
                        }

                        DSM.RemoveVariableUsage(variable);
                        ExcelTemplateProvider provider = new ExcelTemplateProvider(templateName);
                        provider.CreateTemplate(dataStructure);
                    }
                    DSDM.GetDataStructureByID(dataStructure.Id);
                    return View("DataStructureDesigner", DSDM);
                }
                else
                {
                    DSDM.GetDataStructureByID(dataStructure.Id);
                    return View("DataStructureDesigner", DSDM);
                }
            
            }
            return RedirectToAction("DataStructureDesigner");
        }

        public string saveVariable(string name, long id,string description, long dataStructureId, bool optional)
        {
            DataStructureManager dataStructureManager = new DataStructureManager();
            StructuredDataStructure dataStructure = dataStructureManager.StructuredDataStructureRepo.Get(dataStructureId);
            DataStructureDesignerModel DSDM = new DataStructureDesignerModel();
            string errorMsg = null;

            name = cutSpaces(name);
            description = cutSpaces(description);
            if (!(dataStructure.Datasets.Count > 0))
            {
                if (id != 0)
                {
                    Variable var = dataStructureManager.VariableRepo.Get(id);

                    if (var != null)
                    {
                        if (name != null && name != "")
                        {
                            if (dataStructure.Variables.Where(p => cutSpaces(p.Label).ToLower().Equals(cutSpaces(name).ToLower())).Count() > 0)
                            {
                                long newVarId = dataStructure.Variables.Where(p => cutSpaces(p.Label).ToLower().Equals(cutSpaces(name).ToLower())).ToList().First().Id;
                                if (newVarId != var.Id)
                                {
                                    errorMsg = "Can't rename Variable "+var.Label+", name already exist";
                                }
                                else
                                {
                                    var.Label = name;
                                    var.IsValueOptional = optional;
                                    var.Description = description;
                                    dataStructureManager.UpdateStructuredDataStructure(var.DataStructure);

                                    ExcelTemplateProvider provider = new ExcelTemplateProvider(templateName);
                                }
                            }
                            else
                            {
                                var.Label = name;
                                var.IsValueOptional = optional;
                                var.Description = description;
                                dataStructureManager.UpdateStructuredDataStructure(var.DataStructure);

                                ExcelTemplateProvider provider = new ExcelTemplateProvider(templateName);
                            }
                        }

                        else
                        {
                            errorMsg = "Can't rename Variable " + var.Label + ", invalid Name";
                        }
                    }
                }
            }
            return errorMsg;
        }

        public ActionResult shiftVariableLeft(long id, long dataStructureId)
        {
            DataStructureManager dsm = new DataStructureManager();
            StructuredDataStructure ds = dsm.StructuredDataStructureRepo.Get(dataStructureId);
            XmlDocument doc = (XmlDocument)ds.Extra;
            XmlNodeList order = doc.GetElementsByTagName("order");
            List<long> tempList = new List<long>();

            foreach (XmlNode x in order[0])
            {
                tempList.Add(Convert.ToInt64(x.InnerText));
            }

            for(int i = 0;i<tempList.Count();i++)
            {
                if (tempList.ElementAt(i) == id)
                {
                    long temp = tempList.ElementAt(i);
                    tempList.RemoveAt(i);
                    tempList.Insert(i - 1,temp);
                    break;
                }     
            }

            order[0].RemoveAll();

            foreach (long l in tempList)
            {

                XmlNode variable = doc.CreateNode(XmlNodeType.Element, "variable", null);
                variable.InnerText = l.ToString();
                order[0].AppendChild(variable);
            }
            ds.Extra = doc;
            ds = dsm.UpdateStructuredDataStructure(ds);
            DataStructureDesignerModel Model = new DataStructureDesignerModel();
            Model.GetDataStructureByID(ds.Id);
            return View("DataStructureDesigner", Model);
        }

        public ActionResult shiftVariableRight(long id, long dataStructureId)
        {
            DataStructureManager dsm = new DataStructureManager();
            StructuredDataStructure ds = dsm.StructuredDataStructureRepo.Get(dataStructureId);
            XmlDocument doc = (XmlDocument)ds.Extra;
            XmlNodeList order = doc.GetElementsByTagName("order");
            List<long> tempList = new List<long>();

            foreach (XmlNode x in order[0])
            {
                tempList.Add(Convert.ToInt64(x.InnerText));
            }

            for (int i = 0; i < tempList.Count(); i++)
            {
                if (tempList.ElementAt(i) == id)
                {
                    long temp = tempList.ElementAt(i);
                    tempList.RemoveAt(i);
                    tempList.Insert(i + 1, temp);
                    break;
                }
            }

            order[0].RemoveAll();

            foreach (long l in tempList)
            {

                XmlNode variable = doc.CreateNode(XmlNodeType.Element, "variable", null);
                variable.InnerText = l.ToString();
                order[0].AppendChild(variable);
            }
            ds.Extra = doc;
            ds = dsm.UpdateStructuredDataStructure(ds);
            DataStructureDesignerModel Model = new DataStructureDesignerModel();
            Model.GetDataStructureByID(ds.Id);
            return View("DataStructureDesigner", Model);
        }

        public ActionResult openVariableWindow(long id, long dataStructureId)
        {

            DataStructureDesignerModel DSDM = new DataStructureDesignerModel();
            DSDM.GetDataStructureByID(dataStructureId);

            if (!(DSDM.inUse))
            {
                Session["VariableWindow"] = true;
                Session["variableId"] = id;
            }
            else
            {
                Session["VariableWindow"] = false;
            }
            return View("DataStructureDesigner", DSDM);
        }

        #endregion

        public ActionResult downloadTemplate(long id)
        {
            if (id != 0)
            {
                DataStructureManager dataStructureManager = new DataStructureManager();
                StructuredDataStructure dataStructure = new StructuredDataStructure();
                dataStructure = dataStructureManager.StructuredDataStructureRepo.Get(id);

                ExcelTemplateProvider provider = new ExcelTemplateProvider(templateName);
                provider.CreateTemplate(dataStructure);
                string path = "";

                XmlNode resources = dataStructure.TemplatePaths.FirstChild;

                XmlNodeList resource = resources.ChildNodes;

                foreach (XmlNode x in resource)
                {
                    if (x.Attributes.GetNamedItem("Type").Value == "Excel")
                        path = x.Attributes.GetNamedItem("Path").Value;
                    
                }
                string rgxPattern = "[<>?\":|\\\\/*]";
                string rgxReplace = "-";
                Regex rgx = new Regex(rgxPattern);

                string filename = rgx.Replace(dataStructure.Name, rgxReplace);

                if (filename.Length > 50)
                    filename = filename.Substring(0, 50);

                return File(Path.Combine(AppConfiguration.DataPath, path), "application/xlsm", "Template_" + dataStructure.Id + "_" + filename + ".xlsm");
            }
            DataStructureDesignerModel DSDM = new DataStructureDesignerModel();
            DSDM.GetDataStructureByID(id);
            return View("DataStructureDesigner", DSDM);
        }

        #endregion

        #region Unit Manager

        private List<Unit> GetUnitRepo()
        {
            UnitManager UM = new UnitManager();
            List<Unit> repo = UM.Repo.Get().Where(u => u.DataContainers.Count != null && u.AssociatedDataTypes.Count != null).ToList();
            
            foreach(Unit u in repo)
            {
                UM.Repo.LoadIfNot(u.AssociatedDataTypes);
            }
            return(repo);
        }

        public ActionResult UnitManager()
        {
            if (Session["Window"] == null)
                Session["Window"] = false;

            UnitManager unitManager = new UnitManager();
            List<Unit> unitList = unitManager.Repo.Get().Where(u => u.DataContainers.Count != null && u.AssociatedDataTypes.Count != null).ToList();

            DataTypeManager dataTypeManager = new DataTypeManager();
            Session["dataTypeList"] = dataTypeManager.Repo.Get().ToList();

            return View(unitList);
        }

        public ActionResult editUnit(Unit Model, long id, string measurementSystem, long[] checkedRecords)
        {
            UnitManager UM = new UnitManager();

            Model.Id = id;
            Model.Name = cutSpaces(Model.Name);
            Model.Abbreviation = cutSpaces(Model.Abbreviation);
            Model.Description = cutSpaces(Model.Description);
            Model.Dimension = cutSpaces(Model.Dimension);

            if( Model.Id == 0)
            {
                if (unitValidation(Model, checkedRecords))
                {
                    foreach (MeasurementSystem msCheck in Enum.GetValues(typeof(MeasurementSystem)))
                    {
                        if (msCheck.ToString().Equals(measurementSystem))
                        {
                            Model.MeasurementSystem = msCheck;
                        }
                    }
                    Unit unit = UM.Create(Model.Name, Model.Abbreviation, Model.Description, Model.Dimension, Model.MeasurementSystem);

                    updataAssociatedDataType(unit, checkedRecords);
                }
                else
                {
                    UnitManager unitManager = new UnitManager();
                    DataTypeManager dataTypeManager = new DataTypeManager();
                    List<Unit> unitList = unitManager.Repo.Get().Where(u => u.DataContainers.Count != null && u.AssociatedDataTypes.Count != null).ToList();

                    Session["Unit"] = Model;
                    Session["Window"] = true;
                    Session["dataTypeList"] = dataTypeManager.Repo.Get().ToList();

                    return View("UnitManager", unitList);
                }
            }
            else
            {
                if (unitValidation(Model, checkedRecords))
                {
                    UnitManager unitManager = new UnitManager();
                    Unit unit = unitManager.Repo.Get(Model.Id);

                    if (!(unit.DataContainers.Count() > 0))
                    {
                        unit.Name = Model.Name;
                        unit.Description = Model.Description;
                        unit.Abbreviation = Model.Abbreviation;
                        unit.Dimension = Model.Dimension;
                        foreach (MeasurementSystem msCheck in Enum.GetValues(typeof(MeasurementSystem)))
                        {
                            if (msCheck.ToString().Equals(measurementSystem))
                            {
                                unit.MeasurementSystem = msCheck;
                            }
                        }
                        unit = UM.Update(unit);
                        List<long> DataTypelIdList = new List<long>();

                        updataAssociatedDataType(unit, checkedRecords);
                    }
                }
                else
                {
                    UnitManager unitManager = new UnitManager();
                    DataTypeManager dataTypeManager = new DataTypeManager();
                    List<Unit> unitList = unitManager.Repo.Get().Where(u => u.DataContainers.Count != null && u.AssociatedDataTypes.Count != null).ToList();

                    Session["Unit"] = Model;
                    Session["Window"] = true;
                    Session["dataTypeList"] = dataTypeManager.Repo.Get().ToList();

                    return View("UnitManager", unitList);
                }
            }

            Session["Window"] = false;
            Session["checked"] = null;
            Session["Unit"] = new Unit();
            return RedirectToAction("UnitManager");

        }

        private bool unitValidation(Unit unit, long[] checkedRecords)
        {
            bool check = true;
            
            List<Unit> unitList = GetUnitRepo();

            if (unit.Name == null || unit.Name == "")
            {
                Session["nameMsg"] = "invalid Name";
                check = false;
            }
            else
            {
                bool nameExist = !(unitList.Where(p => p.Name.ToLower().Equals(unit.Name.ToLower())).Count().Equals(0));
                if (nameExist)
                {
                    Unit tempUnit = unitList.Where(p => p.Name.ToLower().Equals(unit.Name.ToLower())).ToList().First();
                    if (unit.Id != tempUnit.Id)
                    {
                        Session["nameMsg"] = "Name already exist";
                        check = false;
                    }
                    else
                    {
                        Session["nameMsg"] = null;
                    }
                }
                else
                {
                    Session["nameMsg"] = null;
                }
            }

            if (unit.Abbreviation == null || unit.Abbreviation == "")
            {
                Session["abbrMsg"] = "invalid Abbreviation";
                check = false;
            }
            else
            {
                bool abbreviationExist = !(unitList.Where(p => p.Abbreviation.ToLower().Equals(unit.Abbreviation.ToLower())).Count().Equals(0));
                if (abbreviationExist)
                {
                    Unit tempUnit = unitList.Where(p => p.Abbreviation.ToLower().Equals(unit.Abbreviation.ToLower())).ToList().First();
                    if (unit.Id != tempUnit.Id)
                    {
                        Session["abbrMsg"] = "Abbreviation already exist";
                        check = false;
                    }
                    else
                    {
                        Session["abbrMsg"] = null;
                    }
                }
                else
                {
                    Session["abbrMsg"] = null;
                }
            }

            if (checkedRecords != null)
            {
                Session["dataTypeMsg"] = null;
            }
            else
            {
                Session["dataTypeMsg"] = "Please choose at least one Data Type.";
                check = false;
            }
            
            return check;
        }

        private List<DataType> updataAssociatedDataType(Unit unit, long[] newDataTypelIds)
        {
            if (unit != null)
            {
                DataTypeManager dataTypeManger = new DataTypeManager();

                UnitManager unitManager = new UnitManager();

                unit = unitManager.Repo.Get(unit.Id);
                var existingDataTypes = unit.AssociatedDataTypes.ToList();                   
                var newDataTypes = newDataTypelIds == null? new List<DataType>() : dataTypeManger.Repo.Query().Where(p => newDataTypelIds.Contains(p.Id)).ToList();
                var tobeAddedDataTypes = newDataTypes.Except(existingDataTypes).ToList();

                if(tobeAddedDataTypes != null && tobeAddedDataTypes.Count() > 0)
                    unitManager.AddAssociatedDataType(unit, tobeAddedDataTypes);

                unit = unitManager.Repo.Get(unit.Id);
                existingDataTypes = unit.AssociatedDataTypes.ToList();
                var toBeRemoved = existingDataTypes.Except(newDataTypes).ToList();
                if (toBeRemoved != null && toBeRemoved.Count() > 0)
                    unitManager.RemoveAssociatedDataType(unit, toBeRemoved);

                unit = unitManager.Repo.Get(unit.Id);
                return unit.AssociatedDataTypes.ToList();
            }
            return null;
        }

        public ActionResult deletUnit(long id, string name)
        {

            if (id != 0)
            {
                //string message = "Are you sure you want to delete the Unit " + name + " ?";
                //string caption = "Confirmation";
                //MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                //DialogResult result;
                //// Displays the MessageBox.
                //result = MessageBox.Show(message, caption, buttons);

                //if (result == DialogResult.Yes)
                //{
                UnitManager UM = new UnitManager();
                Unit unit = UM.Repo.Get(id);
                if (unit != null)
                {
                    if (!unitInUse(unit))
                    {

                        UM.Delete(unit);
                    }
                
            }
            }
            return RedirectToAction("UnitManager");
        }

        public bool unitInUse(Unit unit)
        {
            List<DataAttribute> attributes = GetAttRepo();
            bool inUse = false;
            foreach (DataAttribute a in attributes)
            {
                if (a.Unit != null)
                {
                    if (a.Unit.Id == unit.Id)
                        inUse = true;
                }
            }
            return inUse;
        }

        public ActionResult openUnitWindow(long id)
        {
            UnitManager unitManager = new UnitManager();
            DataTypeManager dataTypeManager = new DataTypeManager();
            List<Unit> unitList = unitManager.Repo.Get().Where(u => u.DataContainers.Count != null && u.AssociatedDataTypes.Count != null).ToList();

            if (id != 0)
            {              
                Unit unit = unitManager.Repo.Get().Where(u => u.Id == id && u.AssociatedDataTypes.Count != null).FirstOrDefault();

                Session["nameMsg"] = null;
                Session["abbrMsg"] = null;
                Session["dataTypeMsg"] = null;
                if (Session["Unit"] != null)
                {
                    Unit temp = (Unit)Session["Unit"];
                    if(temp.Id != unit.Id)
                        Session["checked"] = null;
                }
                Session["Unit"] = unit;
                Session["Window"] = true;
                Session["dataTypeList"] = dataTypeManager.Repo.Get().ToList();
            }
            else
            {
                Unit unit = new Unit();

                Session["nameMsg"] = null;
                Session["abbrMsg"] = null;
                Session["dataTypeMsg"] = null;
                Session["Unit"] = new Unit();
                Session["Window"] = true;
                Session["dataTypeList"] = dataTypeManager.Repo.Get().ToList();
            }
            return View ("UnitManager", unitList);
        }

        #endregion

        #region Classification Manager

        private IReadOnlyRepository<Classifier> GetClassRepo()
        {
            ClassifierManager CM = new ClassifierManager();
            return (CM.Repo);
        }

        public ActionResult ClassificationManager()
        {

            if (Session["Class"] == null)
                Session["Class"] = new Classifier();

            if (Session["Window"] == null)
                Session["Window"] = false;

            List<Classifier> ClassList = GetClassRepo().Get().ToList();
            return View(ClassList);
        }

        public ActionResult editClassifier(Classifier Model, string parent, long id, string ParentClassifier)
        {
            IList<Classifier> classList = GetClassRepo().Get().ToList();
            Classifier Parent = new Classifier();
            if(classList.Where(p => p.Name.Equals(ParentClassifier)).Count().Equals(0))
                Parent = null;
            else
                Parent = classList.Where(p => p.Name.Equals(ParentClassifier)).ToList().First();               

            if (id == 0)
            {
                bool nameNotExist = classList.Where(p => p.Name.Equals(Model.Name)).Count().Equals(0);
                if (nameNotExist && (Model.Name != "" && Model.Name != null))
                {
                    ClassifierManager CM = new ClassifierManager();
                    CM.Create(Model.Name, Model.Description, Parent);
                    Session["Window"] = false;
                }
                else
                {
                    Session["errorMsg"] = "invalid Name";
                    Session["Window"] = true;
                }
            }
            else
            {       
                bool nameNotExist = classList.Where(p => p.Name.Equals(Model.Name)).Count().Equals(0);
                if (nameNotExist && (Model.Name != "" && Model.Name != null))
                {
                    Classifier classifier = classList.Where(p => p.Id.Equals(id)).ToList().First();
                    ClassifierManager CM = new ClassifierManager();
                    classifier.Name = Model.Name;
                    classifier.Description = Model.Description;
                    classifier.Parent = Parent;
                    CM.Update(classifier);
                    Session["Window"] = false;
                }
                else
                {
                    ViewData["errorMsg"] = "invalid Name";
                    Session["Window"] = true;
                }
            }
            
            Session["Class"] = new Classifier();
            return RedirectToAction(parent);
        }

        public ActionResult openClassWindow(long id)
        {

            if (id != 0)
            {
                ClassifierManager CM = new ClassifierManager();
                Classifier classifier = CM.Repo.Get(id);
                if (classifier != null)
                {
                    Session["Class"] = classifier;
                    Session["Window"] = true;
                }
                else
                {
                    Session["Class"] = new Classifier();
                    Session["Window"] = false ;
                }
            }
            else
            {
                Session["Class"] = new Classifier();
                Session["Window"] = true;
            }
            return RedirectToAction("ClassificationManager");
        }

        public ActionResult deletClass(long id)
        {

            if (id != 0)
            {
                IList<Classifier> classList = GetClassRepo().Get().ToList();
                Classifier classifier = classList.Where(p => p.Id.Equals(id)).ToList().First();
                ClassifierManager CM = new ClassifierManager();
                CM.Delete(classifier);
            }
            return RedirectToAction("ClassificationManager");
        }

        #endregion

        #region DataType Manager

        public ActionResult DataTypeManager()
        {
            if (Session["Window"] == null)
                Session["Window"] = false;

            DataTypeManager dataTypeManager = new DataTypeManager();
            List<DataType> datatypeList = dataTypeManager.Repo.Get().Where(d=> d.DataContainers.Count != null).ToList();

            return View(datatypeList);
        }

        public ActionResult editDataType(DataType Model, long id,string systemType, string parent)
        {
            DataTypeManager dataTypeManager = new DataTypeManager();
            IList<DataType> DataTypeList = dataTypeManager.Repo.Get();
            TypeCode typecode = new TypeCode();


            foreach (DataTypeCode tc in Enum.GetValues(typeof(DataTypeCode)))
            {
                if (tc.ToString() == systemType)
                {
                    typecode = (TypeCode)tc;
                    break;
                }
            }

            Model.Id = id;
            Model.Name = cutSpaces(Model.Name);
            Model.Description = cutSpaces(Model.Description);           

            if (Model.Name == "" | Model.Name == null)
            {
                Session["Window"] = true;
                Session["nameMsg"] = "invalid name";
                return RedirectToAction(parent);
            }
            else
            {
                bool nameExist = !(DataTypeList.Where(p => p.Name.ToLower().Equals(Model.Name.ToLower())).Count().Equals(0));

                if (Model.Id == 0)
                {
                    if (!nameExist)
                    {
                        DataType dt = dataTypeManager.Create(Model.Name, Model.Description, typecode);
                    }
                    else
                    {
                        Session["Window"] = true;
                        Session["nameMsg"] = "Name already exist";
                        return RedirectToAction(parent);
                    }
                }
                else
                {
                    DataType tempdataType = new DataType();
                    if(nameExist)
                        tempdataType = DataTypeList.Where(p => cutSpaces(p.Name).ToLower().Equals(cutSpaces(Model.Name.ToLower()))).ToList().First();
                    else
                        tempdataType = new DataType();
                    
                    if (!nameExist || Model.Id == tempdataType.Id)
                    {
                        DataType dataType = DataTypeList.Where(p => p.Id.Equals(id)).ToList().First();
                        if (!(dataType.DataContainers.Count() > 0))
                        {
                            DataTypeManager DTM = new DataTypeManager();
                            dataType.Name = Model.Name;
                            dataType.Description = Model.Description;
                            dataType.SystemType = typecode.ToString();
                            DTM.Update(dataType);
                        }
                    }
                    else
                    {
                        Session["Window"] = true;
                        Session["nameMsg"] = "Name already exist";
                        return RedirectToAction(parent);
                    }
                }
            }
           
            Session["Window"] = false;
            Session["DataType"] = new DataType();
            return RedirectToAction(parent);
        }

        public ActionResult openDataTypeWindow(long id)
        {

            if (id != 0)
            {

                DataTypeManager DTM = new DataTypeManager();
                DataType dataType = DTM.Repo.Get(id);

                Session["nameMsg"] = null;
                Session["DataType"] = dataType;
                Session["Window"] = true;              
            }
            else
            {
                Session["nameMsg"] = null;
                Session["DataType"] = new DataType();
                Session["Window"] = true;
            }
            return RedirectToAction("DataTypeManager");
        }

        public ActionResult deletDataType(long id, string name)
        {

            if (id != 0)
            {
                //string message = "Are you sure you want to delete the Data Type " + name +" ?";
                //string caption = "Confirmation";
                //MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                //DialogResult result;
                //// Displays the MessageBox.
                //result = MessageBox.Show(message, caption, buttons);

                //if (result == DialogResult.Yes)
                //{
                DataTypeManager dataTypeManager = new DataTypeManager();
                DataType dataType = dataTypeManager.Repo.Get(id);
                if (dataType != null)
                {
                    if (dataType.DataContainers.Count == 0)
                    {
                        DataTypeManager DTM = new DataTypeManager();
                        DTM.Delete(dataType);
                    }
                
                }
            }
            return RedirectToAction("DataTypeManager");
        }

        #endregion

        private List<DataAttribute> GetAttRepo()
        {
            DataContainerManager DAM = new DataContainerManager();
            List<DataAttribute> repo = DAM.DataAttributeRepo.Get().ToList();

            return (repo);
        }

        public ActionResult AttributeManager()
        {
            if (Session["Window"] == null)
                Session["Window"] = false;

            DataContainerManager dataAttributeManager = new DataContainerManager();
            List<DataAttribute> dataAttributeList = dataAttributeManager.DataAttributeRepo.Get().Where(a => a.UsagesAsVariable.Count != null && a.Unit.Name != null && a.DataType.Name != null).ToList();

            UnitManager unitManager = new UnitManager();
            DataTypeManager dataTypeManager = new DataTypeManager();
            Session["dataTypeList"] = dataTypeManager.Repo.Get().OrderBy(p => p.Name).ToList();
            Session["unitlist"] = unitManager.Repo.Get().OrderBy(p => p.Name).ToList();

            return View(dataAttributeList);
        }

        public ActionResult editAttribute(DataAttribute Model, long id, string unitId, string dataTypeId, string parent)
        {
            IList<DataAttribute> DataAttributeList = GetAttRepo();
            long tempUnitId = Convert.ToInt64(unitId);
            long tempDataTypeId = Convert.ToInt64(dataTypeId);

            Model.Id = id;
            Model.ShortName = cutSpaces(Model.ShortName);
            Model.Name = cutSpaces(Model.Name);
            Model.Description = cutSpaces(Model.Description);

            if (Model.Name == "" | Model.Name == null)
            {
                Session["nameMsg"] = "invalid Name";    
                Session["Window"] = true;
                return RedirectToAction(parent);
            }
            else
            {
                bool nameNotExist = DataAttributeList.Where(p => p.Name.ToLower().Equals(Model.Name.ToLower())).Count().Equals(0);

                if (Model.Id == 0)
                {
                    if (nameNotExist)
                    {
                        UnitManager UM = new UnitManager();
                        Unit unit = UM.Repo.Get(tempUnitId);
                        DataTypeManager DTM = new DataTypeManager();
                        DataType dataType = DTM.Repo.Get(tempDataTypeId);
                        DataContainerManager DAM = new DataContainerManager();

                        DataAttribute temp = new DataAttribute();
                        DAM.CreateDataAttribute(Model.ShortName, Model.Name, Model.Description, false, false, "", MeasurementScale.Categorial, DataContainerType.ReferenceType, "", dataType, unit, null, null, null, null, null, null);
                    }
                    else
                    {
                        Session["nameMsg"] = "Name already exist";  
                        Session["Window"] = true;
                        return RedirectToAction(parent);
                    }
                }
                else
                {
                    if (nameNotExist || DataAttributeList.Where(p => p.Name.Equals(Model.Name)).ToList().First().Id == Model.Id)
                    {
                        DataAttribute dataAttribute = DataAttributeList.Where(p => p.Id.Equals(Model.Id)).ToList().First();
                        if (!attributeInUse(dataAttribute))
                        {
                            DataContainerManager DAM = new DataContainerManager();
                            dataAttribute.Name = cutSpaces(Model.Name);
                            dataAttribute.ShortName = Model.ShortName;
                            dataAttribute.Description = Model.Description;
                            UnitManager UM = new UnitManager();
                            dataAttribute.Unit = UM.Repo.Get(tempUnitId);
                            DataTypeManager DTM = new DataTypeManager();
                            dataAttribute.DataType = DTM.Repo.Get(tempDataTypeId);
                            DAM.UpdateDataAttribute(dataAttribute);
                        }
                    }
                    else
                    {
                        Session["nameMsg"] = "Name already exist";
                        Session["Window"] = true;
                        return RedirectToAction(parent);
                    }
                }
            }

            Session["Window"] = false;
            Session["DataAttribute"] = new DataAttribute();
            return RedirectToAction(parent);
        }

        public JsonResult getDatatypeList(string Id)
        {
            long tempId = Convert.ToInt64(Id);
            UnitManager unitManager = new UnitManager();
            Unit tempUnit = unitManager.Repo.Get(tempId);
            List<DataType> dataTypeList = new List<DataType>();
            if (tempUnit.Name.ToLower() == "none")
            {
                DataTypeManager dataTypeManager = new DataTypeManager();
                dataTypeList = dataTypeManager.Repo.Get().ToList();
                dataTypeList = dataTypeList.OrderBy(p => p.Name).ToList();
            }
            else
            {
                dataTypeList = unitManager.Repo.Get(tempId).AssociatedDataTypes.ToList();
                dataTypeList = dataTypeList.OrderBy(p => p.Name).ToList();
            }

            return Json(new SelectList(dataTypeList.ToArray(), "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        public ActionResult openAttributeWindow(long id)
        {

            if (id != 0)
            {
                DataContainerManager DAM = new DataContainerManager();
                DataAttribute dataAttribute = DAM.DataAttributeRepo.Get(id);

                Session["nameMsg"] = null;
                Session["DataAttribute"] = dataAttribute;
                Session["Window"] = true;
               
            }
            else
            {
                Session["nameMsg"] = null;
                Session["DataAttribute"] = new DataAttribute();
                Session["Window"] = true;
            }
            return RedirectToAction("AttributeManager");
        }

        public ActionResult deletAttribute(long id, string name)
        {

            if (id != 0)
            {
                //string message = "Are you sure you want to delete the Data Attribute " + name + " ?";
                //string caption = "Confirmation";
                //MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                //DialogResult result;
                //// Displays the MessageBox.
                //result = MessageBox.Show(message, caption, buttons);

                //if (result == DialogResult.Yes)
                //{
                DataContainerManager DAM = new DataContainerManager();
                DataAttribute dataAttribute = DAM.DataAttributeRepo.Get(id);
                if (dataAttribute != null)
                {
                    if (!attributeInUse(dataAttribute))
                    {

                        DAM.DeleteDataAttribute(dataAttribute);
                    }
                
            }
            }
            return RedirectToAction("AttributeManager");
        }

        public bool attributeInUse(DataAttribute attribute)
        {         
           if (attribute.UsagesAsVariable.Count() == 0 && attribute.UsagesAsParameter.Count() == 0)
               return false;
           else
               return true;          
        }

        public ActionResult showDatasets(long id, bool structured)
        {
            DataStructureManager dataStructureManager = new DataStructureManager();
            DataStructureDesignerModel DSDM = new DataStructureDesignerModel();
            if (id != 0)
            {
                DSDM.GetDataStructureByID(id, structured);
            }

            if ((bool)Session["Window"] == false)
            {
                Session["DatasetWindow"] = true;
            }
            else
            {
                Session["DatasetWindow"] = false;
            }
            return View("DataStructureDesigner", DSDM);
        }

        private string cutSpaces(string str)
        {
            if (str != "" && str != null)
            {
                str = str.Trim();
                if(str.Length > 255)
                    str = str.Substring(0, 255);
            }
            return (str);
        }

        public JsonResult setSelected(long[][] selected)
        {
            if (selected != null)
                {
                Session["selected"] = selected;
                Session["dataStructureId"] = Session["dataStructureId"];
            }
            else
            {
                Session["selected"] = null;
                Session["dataStructureId"] = Session["dataStructureId"];
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult setChecked(long[] checkedIDs)
        {
            if (checkedIDs != null)
            {
                List<long> Ids = new List<long>();
                foreach (long l in checkedIDs)
                {
                    Ids.Add(Convert.ToInt64(l));
                }
                Session["checked"] = Ids;
            }
            else
                Session["checked"] = null;
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}


