using BExIS.Security.Entities.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Vaiona.Utils.Cfg;

namespace BExIS.Modules.UDAM.UI.Helpers
{
    public class UdamSeedDataGenerator
    {
        public static string scriptPathR = Path.Combine(AppConfiguration.GetModuleWorkspacePath("UDAM"), "R_Scripts");
        public static string scriptPathPython = Path.Combine(AppConfiguration.GetModuleWorkspacePath("UDAM"), "Python_Scripts");
        public static string analysis_tools = Path.Combine(AppConfiguration.GetModuleWorkspacePath("UDAM"), "Analysis_tools");

        public static void GenerateSeedData()
        {
            if (!System.IO.Directory.Exists(scriptPathR))
                System.IO.Directory.CreateDirectory(scriptPathR);

            if (!System.IO.Directory.Exists(scriptPathPython))
                System.IO.Directory.CreateDirectory(scriptPathPython);

            if (!System.IO.Directory.Exists(analysis_tools))
                System.IO.Directory.CreateDirectory(analysis_tools);
        }
    }
}