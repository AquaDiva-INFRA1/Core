﻿
using BExIS.Dlm.Entities.Data;
using BExIS.Dlm.Services.Data;
using BExIS.IO;
using BExIS.Modules.Dcm.UI.Models.Attachments;
using BExIS.Xml.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using Vaiona.Utils.Cfg;
using Vaiona.Web.Extensions;
using Vaiona.Web.Mvc.Models;

namespace BExIS.Modules.Dcm.UI.Controllers
{
    public class AttachmentsController : Controller
    {
        // GET: Attachments
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult DatasetAttachements(long datasetId)
        {
            ViewBag.datasetId = datasetId;
            return PartialView("_datasetAttachements", LoadDatasetModel(datasetId));
        }

        public ActionResult Download(long datasetId, String fileName)
        {
            var filePath = Path.Combine(AppConfiguration.DataPath, "Datasets", datasetId.ToString(), "Attachments", fileName);
            return File(filePath, MimeMapping.GetMimeMapping(fileName), Path.GetFileName(filePath));
        }

        public ActionResult Delete(long datasetId, String fileName)
        {
            var filePath = Path.Combine(AppConfiguration.DataPath, "Datasets", datasetId.ToString(), "Attachments", fileName);
            FileHelper.Delete(filePath);
            var dm = new DatasetManager();
            var dataset = dm.GetDataset(datasetId);
            var datasetVersion = dm.GetDatasetLatestVersion(dataset);

            dm.CheckOutDataset(dataset.Id, GetUsernameOrDefault());
            dm.EditDatasetVersion(datasetVersion, null, null, null);
            dm.CheckInDataset(dataset.Id, "upload dataset attachements", GetUsernameOrDefault(), ViewCreationBehavior.None);
            return RedirectToAction("showdata", "data", new { area = "ddm", id = datasetId });
        }

        private DatasetFilesModel LoadDatasetModel(long datasetId)
        {
            var model = new DatasetFilesModel
            {
                ServerFileList = GetDatasetFileList(datasetId),
                FileSize = this.Session.GetTenant().MaximumUploadSize
            };
            return model;
        }

        /// <summary>
        /// read filenames from datapath/Datasets/id
        /// </summary>
        /// <returns>return a list with all names from FileStream in the folder</returns>
        private Dictionary<BasicFileInfo, String> GetDatasetFileList(long datasetId)
        {
            var fileList = new Dictionary<BasicFileInfo, String>();
            var dm = new DatasetManager();
            var dataset = dm.GetDataset(datasetId);
            var datasetDataPath = Path.Combine(AppConfiguration.DataPath, "Datasets", datasetId.ToString(), "Attachments");
            var datasetVersion = dm.GetDatasetLatestVersion(dataset);
            foreach (var contentDescriptor in datasetVersion.ContentDescriptors.OrderBy(c => c.OrderNo))
                if (System.IO.File.Exists(contentDescriptor.URI))
                {
                    fileList.Add(new BasicFileInfo(contentDescriptor.Name, contentDescriptor.URI, contentDescriptor.MimeType, Path.GetExtension(contentDescriptor.URI), 0), GetDescription(contentDescriptor.Extra));
                }
            return fileList;
        }

        [HttpPost]
        public ActionResult ProcessSubmit(IEnumerable<HttpPostedFileBase> attachments, long datasetId, String description)
        {
            ViewBag.Title = PresentationModel.GetViewTitleForTenant("Attach file to dataset", this.Session.GetTenant());
            // The Name of the Upload component is "attachments"                            
            if (attachments != null)
            {
                Session["FileInfos"] = attachments;
                uploadFiles(attachments, datasetId, description);
            }
            // Redirect to a view showing the result of the form submission.
            return RedirectToAction("showdata", "data", new { area = "ddm", id = datasetId });
        }

        public void uploadFiles(IEnumerable<HttpPostedFileBase> attachments, long datasetId, String description)
        {
            var filemNames = "";
            var dm = new DatasetManager();
            var dataset = dm.GetDataset(datasetId);
            var datasetVersion = dm.GetDatasetLatestVersion(dataset);
            foreach (var file in attachments)
            {
                var fileName = Path.GetFileName(file.FileName);
                filemNames += fileName.ToString() + ",";
                var dataPath = AppConfiguration.DataPath;
                if (!Directory.Exists(Path.Combine(dataPath, "Datasets", datasetId.ToString(), "Attachments")))
                    Directory.CreateDirectory(Path.Combine(dataPath, "Datasets", datasetId.ToString(), "Attachments"));
                var destinationPath = Path.Combine(dataPath, "Datasets", datasetId.ToString(), "Attachments", fileName);
                file.SaveAs(destinationPath);
                AddFileInContentDiscriptor(datasetVersion, fileName, description);
            }
            dm.CheckOutDataset(dataset.Id, GetUsernameOrDefault());
            dm.EditDatasetVersion(datasetVersion, null, null, null);
            dm.CheckInDataset(dataset.Id, "upload dataset attachements", GetUsernameOrDefault(), ViewCreationBehavior.None);
        }

        private string AddFileInContentDiscriptor(DatasetVersion datasetVersion, String fileName, String description)
        {

            string dataPath = AppConfiguration.DataPath;
            string storePath = Path.Combine(dataPath, "Datasets", datasetVersion.Dataset.Id.ToString(), "Attachments");
            int lastOrderContentDescriptor = 0;

            if (datasetVersion.ContentDescriptors.Any())
                lastOrderContentDescriptor = datasetVersion.ContentDescriptors.Max(cc => cc.OrderNo);
            ContentDescriptor originalDescriptor = new ContentDescriptor()
            {
                OrderNo = lastOrderContentDescriptor + 1,
                Name = fileName,
                MimeType = MimeMapping.GetMimeMapping(fileName),
                URI = Path.Combine(storePath, fileName),
                DatasetVersion = datasetVersion,

            };
            // replace the URI and description in case they have a same name
            if (datasetVersion.ContentDescriptors.Count(p => p.Name.Equals(originalDescriptor.Name)) > 0)
            {
                //
                foreach (ContentDescriptor cd in datasetVersion.ContentDescriptors)
                {
                    if (cd.Name == originalDescriptor.Name)
                    {
                        cd.URI = originalDescriptor.URI;
                        cd.Extra = SetDescription(cd.Extra, description);
                    }
                }
            }
            else
            {
                // add file description Node
                XmlDocument doc = SetDescription(originalDescriptor.Extra, description);
                originalDescriptor.Extra = doc;
                //Add current contentdesciptor to list
                datasetVersion.ContentDescriptors.Add(originalDescriptor);
            }

            return storePath;
        }
        private XmlDocument SetDescription(XmlNode extraField, string description)
        {
            XmlNode newExtra;
            var source = (XmlDocument)extraField;
            if (source == null)
            {
                source = new XmlDocument();
                source.LoadXml("<extra><fileDescription>" + description + "</fileDescription></extra>");
            }
            else
            {
                if (XmlUtility.GetXmlNodeByName(extraField, "fileDescription") == null)
                {
                    XmlNode t = XmlUtility.CreateNode("fileDescription", source);
                    t.InnerText = description;
                    source.DocumentElement.AppendChild(t);
                }
                else
                {
                    var descNodes = source.SelectNodes("/extra/fileDescription");
                    descNodes[0].InnerText = description;
                }
            }
            return source;
        }
        private string GetDescription(XmlNode extra)
        {
            if ((XmlDocument)extra != null)
            {
                var descNode = extra.SelectNodes("/extra/fileDescription");
                if (descNode != null)
                {
                    return descNode[0].InnerText;
                }
            }
            return "";
        }
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
    }


}