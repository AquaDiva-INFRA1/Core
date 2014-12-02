﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BExIS.Web.Shell.Areas.DCM.Models
{
    public class FinishUploadModel
    {
        public long DatasetId { get; set; }
        public string DatasetTitle { get; set; }
        public string Filename { get; set; }

        public FinishUploadModel()
        {
            DatasetTitle = "";
            Filename = "";
        }

    }
}