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

        // this variable is a flag if the user requested the search or not to display the papers button
        public Boolean serachFlag = false ;
    }
}