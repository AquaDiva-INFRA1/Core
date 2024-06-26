using BExIS.Modules.Dcm.UI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BExIS.Modules.OAC.UI.Models
{
    public class ViewFormModel
    {
        internal string project;

        // for getting input from the user
        [Display(Name = "Sample ID / Study ID")]
        [Required(ErrorMessage = "Please enter the sample ID , e.g. SAMEA0123456789. or Study Number , e.g. PRJEB25133")]
        public string Identifier { get; set; }

        [Display(Name = "Metadata Structure")]
        [Required(ErrorMessage = "Please select a metadata structure.")]
        public long SelectedMetadataStructureId { get; set; }

        [Display(Name = "Data Structure")]
        [Required(ErrorMessage = "Please select a data structure.")]
        public long SelectedDataStructureId { get; set; }

        [Display(Name = "Sequence Data Portal")]
        [Required(ErrorMessage = "Please select a sequence data portal.")]
        public long SelectedDataSourceId { get; set; }

        // error in case sth unexpected happened
        public string Error { get; set; }

        // internal
        public List<ListViewItem> MetadataStructureViewList { get; set; }
        public List<ListViewItem> DataSourceViewList { get; set; }
        public List<ListViewItemWithType> DataStructureViewList { get; set; }
        public Dictionary<string, string> Accessions { get; set; }
        //acession contains key respresting the accession number and the project number separated by space , and the value is the Sample (not project) metadata extracted from the EBI
        public ViewFormModel() { }

        public ViewFormModel(List<ListViewItem> MetadataStructureViewList, List<ListViewItemWithType> DataStructureViewList,
            List<ListViewItem> DataSourceViewList)
        {
            this.MetadataStructureViewList = MetadataStructureViewList;
            this.DataStructureViewList = DataStructureViewList;
            this.DataSourceViewList = DataSourceViewList;
            this.Accessions = new Dictionary<string, string>();
        }

        public List<ListViewItem> GetDataSourceList()
        {
            List<ListViewItem> temp = new List<ListViewItem>();

            Type type = typeof(DataSource);

            foreach (DataSource source in Enum.GetValues(type))
            {
                temp.Add(new ListViewItem((long)source, Enum.GetName(type, source)));
            }

            return temp;
        }
        public enum DataSource : long
        {
            BioGPS = 1,
            EBI = 2, 
            NCBI = 3 // the same in our examples
        }
    }
}
