using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BExIS.Modules.OAC.UI.Models
{
    public class ProjAcc
    {
        public string project { get; set; }
        public List<KeyValuePair<string, string>> samples { get; set; }
        public ProjAcc()
        {
            this.samples = new List<KeyValuePair<string, string>>();
        }
        public ProjAcc(string project, List<KeyValuePair<string, string>> samples)
        {
            this.project = project;
            this.samples = samples;
        }

        public void add_sample_to_list(KeyValuePair<string, string> sample)
        {
            this.samples.Add(sample);
        }


    }
}