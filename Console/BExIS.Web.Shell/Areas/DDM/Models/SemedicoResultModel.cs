using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BExIS.Modules.Ddm.UI.Models
{
    public class SemedicoResultModel
    {
        public string searchTermString { get; set; }
        public string inputstring { get; set; }
        public List<string> tokens { get; set; }
        public string sortcriterium { get; set; }
        public int subsetsize { get; set; }
        public int subsetstart { get; set; }
        public int countallresults { get; set; }
        public List<Bibliographylist> bibliographylist { get; set; }

        public SemedicoResultModel()
        {
            bibliographylist = new List<Bibliographylist>();
        }
    }


    public class Author
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string affiliation { get; set; }
    }

    public class Publication
    {
        public string title { get; set; }
        public string volume { get; set; }
        public string issue { get; set; }
        public string pages { get; set; }
        public string date { get; set; }
        public bool dateComplete { get; set; }
    }

    public class Bibliographylist
    {
        public string articleTitle { get; set; }
        public string abstractText { get; set; }
        public string docId { get; set; }
        public string pmid { get; set; }
        public List<Author> authors { get; set; }
        public Publication publication { get; set; }
        public List<object> externalLinks { get; set; }
        public int type { get; set; }
        public bool review { get; set; }
        public string indextype { get; set; }
        public string pmcid { get; set; }
    }

    public class RootObject
    {
        public string inputstring { get; set; }
        public List<string> tokens { get; set; }
        public string sortcriterium { get; set; }
        public int subsetsize { get; set; }
        public int subsetstart { get; set; }
        public int countallresults { get; set; }
        public List<Bibliographylist> bibliographylist { get; set; }
    }
}