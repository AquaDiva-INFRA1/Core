using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BExIS.Modules.Ddm.UI.Helpers
{
    public class Result
    {
        public string versionno { get; set; }
        public string dataset_id { get; set; }
        public string version_id { get; set; }
    }

    public class SemanticSearchResultIds
    {
        public List<Result> result { get; set; }
    }
}