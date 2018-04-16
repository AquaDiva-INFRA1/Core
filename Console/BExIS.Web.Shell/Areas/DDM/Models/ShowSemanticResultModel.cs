using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BExIS.Modules.Ddm.UI.Models
{
    public class ShowSemanticResultModel
    {
        public ShowSemanticResultModel(DataTable table)
        {
            semanticComponent = table;
            resultListComponent = null;
            detailsComponent = null;
            semanticSearchServerError = null;
        }

        public SemedicoResultModel resultListComponent;
        public SemedicoResultDetailsModel detailsComponent;
        public DataTable semanticComponent;
        public String semanticSearchServerError;
        public String semedicoServerError;
    }
}