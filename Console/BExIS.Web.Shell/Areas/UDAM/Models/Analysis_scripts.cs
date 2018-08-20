using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BExIS.Modules.UDAM.UI.Models
{
    public class Analysis_scripts
    {
        //contains the file paths of the python / r scripts to analyse and the title of the script
        //1st item is the path and the second is the title
        public Dictionary<string,string> script_paths;
        public List<string> tools_list;

        public Analysis_scripts()
        {
            this.script_paths = new Dictionary<string, string>();
            this.tools_list = new List<string>();
            initialize_analysis_scripts();
        }

        private Analysis_scripts(Dictionary<string, string> script_paths, List<string> tools_list)
        {
            this.script_paths = script_paths;
            this.tools_list = tools_list;
        }

        private void initialize_analysis_scripts()
        {
            // initializing the tools for analysis
            this.tools_list.Add("vrap");
            this.tools_list.Add("tool2");
            this.tools_list.Add("tool3");
            this.tools_list.Add("tool4");

            // initializing the scripts
            this.script_paths.Add("filepath1", "R script 1");
            this.script_paths.Add("filepath2", "R script 2");
            this.script_paths.Add("filepath3", "python script 3");
        }
        
        public string get_file_path(int index)
        {
            int i = 0;
            foreach (KeyValuePair<string, string> kvp in this.script_paths)
            {
                if (i == index)
                {
                    return kvp.Key;
                }
                i++;
            }
            return "";
        }

        public string get_file_path(string key)
        {
            string value = "";
            this.script_paths.TryGetValue(key, out value);
            return value;
        }

        public string get_file_title(int index)
        {
            int i = 0;
            foreach (KeyValuePair<string, string> kvp in this.script_paths)
            {
                if (i == index)
                {
                    return kvp.Value;
                }
                i++;
            }
            return "";
        }
    }
}