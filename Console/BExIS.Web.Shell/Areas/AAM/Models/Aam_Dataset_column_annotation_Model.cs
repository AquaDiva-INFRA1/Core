using BExIS.Aam.Entities.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BExIS.Modules.Aam.UI.Models
{

    public class Aam_Dataset_column_annotation_Model
    {

        public Dictionary<long, string> datasets = new Dictionary<long, string>();
        public Dictionary<long, string> DataAttributes = new Dictionary<long, string>();
        public Dictionary<long, string> entites = new Dictionary<long, string>();
        public Dictionary<long, string> characs = new Dictionary<long, string>();
        public Dictionary<long, string> standards = new Dictionary<long, string>();

    }
}