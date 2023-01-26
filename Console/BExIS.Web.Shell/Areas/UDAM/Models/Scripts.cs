using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace BExIS.Modules.UDAM.UI.Models
{
    public class Scripts : ScriptAndTool
    {
        public enum Script_type
        {
            r,
            python
        };

        public string description;
        public string script;
        public FileInfo file;
        public Script_type script_Type;

        public Scripts(string description, string script, FileInfo file, Script_type script_Type)
        {
            this.description = description;
            this.script = script;
            this.file = file;
            this.script_Type = script_Type;
        }

        public Scripts()
        {
        }
    }
}