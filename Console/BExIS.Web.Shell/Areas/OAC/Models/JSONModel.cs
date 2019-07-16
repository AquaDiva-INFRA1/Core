using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace BExIS.Modules.OAC.UI.Models
{
    public class JSONModel
    {
        public string accession { get; set; }
        public JObject jo { get; set; }
        public JSONModel(string accession)
        {
            this.accession = accession;
            string url = "https://www.ebi.ac.uk/biosamples/api/samples/" + accession;
            this.jo = JObject.Parse(new WebClient().DownloadString(url));
        }
    }
}