using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Vaiona.Utils.Cfg;

namespace BExIS.Modules.UDAM.UI.Models
{
    public class Analysis_Model
    {
        public static string scriptPathR = Path.Combine(AppConfiguration.GetModuleWorkspacePath("UDAM"), "R_Scripts");
        public static string scriptPathPython = Path.Combine(AppConfiguration.GetModuleWorkspacePath("UDAM"), "Python_Scripts");
        public static string analysis_tools = Path.Combine(AppConfiguration.GetModuleWorkspacePath("UDAM"), "Analysis_tools");

        public Dictionary<Scripts, string> R_script_paths;
        public Dictionary<Scripts, string> Python_script_paths;
        public Dictionary<AnalysisTool, string> tools_list;
        

        public Analysis_Model()
        {
            this.R_script_paths = new Dictionary<Scripts, string>();
            this.Python_script_paths = new Dictionary<Scripts, string>();
            this.tools_list = new Dictionary<AnalysisTool, string>();
            initialize_analysis_scripts();
        }

        private void initialize_analysis_scripts()
        {
            try
            {
                DirectoryInfo d = new DirectoryInfo(scriptPathR);//Assuming Test is your Folder
                FileInfo[] Files = d.GetFiles("*.*"); //Getting Text files
                foreach (FileInfo file in Files)
                {
                    string text = System.IO.File.ReadAllText(file.FullName);
                    int p1 = text.IndexOf("<Description>") + "<Description>".Length;
                    int p2 = text.IndexOf("</Description>");
                    int p3 = text.IndexOf("<content>") + "<content>".Length;
                    int p4 = text.IndexOf("</content>");
                    Scripts script = new Scripts(text.Substring(p1, p2 - p1), text.Substring(p3, p4 - p3), file, Scripts.Script_type.r);
                    R_script_paths.Add(script, file.Name);
                }

                d = new DirectoryInfo(scriptPathPython);//Assuming Test is your Folder
                Files = d.GetFiles("*.*"); //Getting Text files
                foreach (FileInfo file in Files)
                {
                    string text = System.IO.File.ReadAllText(file.FullName);
                    int p1 = text.IndexOf("<Description>") + "<Description>".Length;
                    int p2 = text.IndexOf("</Description>");
                    int p3 = text.IndexOf("<content>") + "<content>".Length;
                    int p4 = text.IndexOf("</content>");
                    Scripts script = new Scripts(text.Substring(p1, p2 - p1), text.Substring(p3, p4 - p3), file, Scripts.Script_type.python);
                    Python_script_paths.Add(script, file.Name);
                }

                d = new DirectoryInfo(analysis_tools);//Assuming Test is your Folder
                Files = d.GetFiles("*.*"); //Getting Text files
                foreach (FileInfo file in Files)
                {
                    string text = System.IO.File.ReadAllText(file.FullName);
                    int p1 = text.IndexOf("<server>") + "<server>".Length;
                    int p2 = text.IndexOf("</server>");
                    int p3 = text.IndexOf("<Description>") + "<Description>".Length;
                    int p4 = text.IndexOf("</Description>");
                    AnalysisTool analysisTool = new AnalysisTool(text.Substring(p1, p2 - p1), text.Substring(p3, p4 - p3), file);
                    tools_list.Add(analysisTool, file.Name);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
        


        public KeyValuePair<Scripts, string> get_key(int index , Dictionary<Scripts, string> dict)
        {
            int i = 0;
            foreach (KeyValuePair<Scripts, string> kvp in dict)
            {
                if (i == index)
                {
                    return kvp;
                }
                i++;
            }
            return new KeyValuePair<Scripts, string>();
        }


        public KeyValuePair<AnalysisTool, string> get_key(int index, Dictionary<AnalysisTool, string> dict)
        {
            int i = 0;
            foreach (KeyValuePair<AnalysisTool, string> kvp in dict)
            {
                if (i == index)
                {
                    return kvp;
                }
                i++;
            }
            return new KeyValuePair<AnalysisTool, string>();
        }
        
    }
}