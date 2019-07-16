using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;
using System.Net;
using System.Web.Mvc;
using System.Xml;
using Vaiona.Web.Extensions;
using Vaiona.Web.Mvc.Models;
using BExIS.Modules.Dcm.UI.Models;
using System.ComponentModel.DataAnnotations;

namespace BExIS.Modules.OAC.UI.Models
{
    public class SelectedImportOptionsModel
    {

        // for getting input from the user
        [Display(Name = "Dataset Identifier")]
        [Required(ErrorMessage = "Please enter the dataset identifier, e.g. SAMEA0123456789.")]
        public string Identifier { get; set; }

        [Display(Name = "Metadata Structure")]
        [Required(ErrorMessage = "Please select a metadata structure.")]
        public long SelectedMetadataStructureId { get; set; }

        [Display(Name = "Data Structure")]
        [Required(ErrorMessage = "Please select a data structure.")]
        public long SelectedDataStructureId { get; set; }

        [Display(Name = "Metadata Source")]
        [Required(ErrorMessage = "Please select a metadata source.")]
        public long SelectedDataSourceId { get; set; }

        // error in case sth unexpected happened
        public string Error { get; set; }

        // internal
        public List<ListViewItem> MetadataStructureViewList { get; set; }
        public List<ListViewItem> DataSourceViewList { get; set; }
        public List<ListViewItemWithType> DataStructureViewList { get; set; }

        public SelectedImportOptionsModel() { }
    }
}
