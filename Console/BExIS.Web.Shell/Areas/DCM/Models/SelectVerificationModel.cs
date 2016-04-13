using System.Collections.Generic;
using BExIS.IO.Transform.Validation.Exceptions;
using BExIS.Dcm.Wizard;
using System;
using BExIS.Dlm.Entities.DataStructure;

namespace BExIS.Web.Shell.Areas.DCM.Models
{
    public class SelectVerificationModel
    {
        public StepInfo StepInfo { get; set; }
        public String[] HeaderFields { get; set; }
        public List<Unit> AvailableUnits { get; set; }
        public List<Tuple<int, string, Unit>> AssignedHeaderUnits { get; set; }
      
        public List<Error> ErrorList { get; set; }

        public SelectVerificationModel()
        {
            ErrorList = new List<Error>();
            AvailableUnits = new List<Unit>();
            AssignedHeaderUnits = new List<Tuple<int, string, Unit>>();
        }
    }
}