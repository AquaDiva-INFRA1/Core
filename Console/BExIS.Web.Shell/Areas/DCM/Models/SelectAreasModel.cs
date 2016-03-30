﻿using System.Collections.Generic;
using BExIS.IO.Transform.Validation.Exceptions;
using BExIS.Dcm.Wizard;
using BExIS.Web.Shell.Areas.DCM.Models.Metadata;

namespace BExIS.Web.Shell.Areas.DCM.Models
{
    public class SelectAreasModel
    {
        public StepInfo StepInfo { get; set; }
        public string jsonTableData { get; set; }
      
        public List<Error> ErrorList { get; set; }

        public SelectAreasModel()
        {
            ErrorList = new List<Error>();
        }
    }
}