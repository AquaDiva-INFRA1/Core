﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using BExIS.Io.Transform.Validation.Exceptions;
using BExIS.Dcm.Wizard;

namespace BExIS.Web.Shell.Areas.DCM.Models
{
    public class SelectFileViewModel
    {
        public HttpPostedFileBase file;
        public List<string> serverFileList = new List<string>();
        public List<Error> ErrorList = new List<Error>();
        public String SelectedFileName = "";
        public String SelectedServerFileName = "";
        public Stream fileStream;

        public StepInfo StepInfo { get; set; }
    }
}