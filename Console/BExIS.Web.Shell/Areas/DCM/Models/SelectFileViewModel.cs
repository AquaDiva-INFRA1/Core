﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using BExIS.DCM.Transform.Validation.Exceptions;
using BExIS.DCM.UploadWizard;

namespace BExIS.Web.Shell.Areas.DCM.Models
{
    public class SelectFileViewModel
    {
        public HttpPostedFileBase file;
        public List<Error> ErrorList = new List<Error>();
        public String SelectedFileName = "";
        public Stream fileStream;

        public StepInfo StepInfo { get; set; }
    }
}