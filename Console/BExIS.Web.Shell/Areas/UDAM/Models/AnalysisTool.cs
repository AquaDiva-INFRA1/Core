using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace BExIS.Modules.UDAM.UI.Models
{
    public class AnalysisTool : ScriptAndTool
    {
        public string serverURL;
        public new string description;
        public new FileInfo file;

        public AnalysisTool(string serverURL, string description, FileInfo file)
        {
            this.serverURL = serverURL;
            this.description = description;
            this.file = file;
        }

        public AnalysisTool()
        {
        }
    }
}