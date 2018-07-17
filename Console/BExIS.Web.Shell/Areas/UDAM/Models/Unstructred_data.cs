using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BExIS.Modules.UDAM.UI.Models
{
    public class Unstructred_data
    {
        public string owner;
        public string description;
        string title;
        public int id;
        public DataTable result_table;

        public Unstructred_data(DataTable table)
        {
            this.result_table = table;
        }

        public Unstructred_data(string owner, string description, string title, int dataset_id)
        {
            this.owner = owner;
            this.description = description;
            this.id = dataset_id;
            this.title = title;
        }
        
    }
}