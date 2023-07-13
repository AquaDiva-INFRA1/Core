using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BExIS.Modules.Ddm.UI.Models
{
    public class SemedicoResultDetailsModel
    {
        public List<Author> Authors { get; set; }
        public List<String> FormattedAuthors { get; set; }
        public String Title { get; set; }
        public String AbstractText { get; set; }
        public String Publication { get; set; }
        public List<Object> Links { get; set; }
    }
}