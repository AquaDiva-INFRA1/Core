﻿using System.Collections.Generic;
using BExIS.DCM.Transform.Validation.Exceptions;
using BExIS.DCM.UploadWizard;

namespace BExIS.Web.Shell.Areas.DCM.Models
{
    public class ChooseDatasetViewModel
    {
        public StepInfo StepInfo { get; set; }
        public int SelectedDatasetId { get; set; }
        public int SelectedDatastructureId { get; set; }
        public string DatasetTitle { get; set; }
      
        public List<long> Datasets { get; set; }
        public List<ListViewItem> DatasetsViewList { get; set; }


        public List<Error> ErrorList { get; set; }

        public CreateDatasetViewModel DatasetViewModel { get; set; }

        public ChooseDatasetViewModel()
        {
            Datasets = new List<long>();
            ErrorList = new List<Error>();
            DatasetsViewList = new List<ListViewItem>();
            DatasetTitle = "";
        }

        

        
    }
}