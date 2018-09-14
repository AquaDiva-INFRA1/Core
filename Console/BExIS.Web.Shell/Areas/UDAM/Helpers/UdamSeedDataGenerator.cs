using BExIS.Security.Entities.Objects;
using BExIS.Security.Services.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Vaiona.Utils.Cfg;

namespace BExIS.Modules.UDAM.UI.Helpers
{
    public class UdamSeedDataGenerator : IDisposable
    {
        public static string scriptPathR = Path.Combine(AppConfiguration.GetModuleWorkspacePath("UDAM"), "R_Scripts");
        public static string scriptPathPython = Path.Combine(AppConfiguration.GetModuleWorkspacePath("UDAM"), "Python_Scripts");
        public static string analysis_tools = Path.Combine(AppConfiguration.GetModuleWorkspacePath("UDAM"), "Analysis_tools");
        
        public void GenerateSeedData()
        {
            FeatureManager featureManager = new FeatureManager();
            OperationManager operationManager = new OperationManager();

            try
            {
                if (!System.IO.Directory.Exists(scriptPathR))
                    System.IO.Directory.CreateDirectory(scriptPathR);

                if (!System.IO.Directory.Exists(scriptPathPython))
                    System.IO.Directory.CreateDirectory(scriptPathPython);

                if (!System.IO.Directory.Exists(analysis_tools))
                    System.IO.Directory.CreateDirectory(analysis_tools);
                
                List<Feature> features = featureManager.FeatureRepository.Get().ToList();

                Feature udam = features.FirstOrDefault(f => f.Name.Equals("Unstructred Data Analysis"));
                if (udam == null) udam = featureManager.Create("Unstructred Data Analysis", "Unstructred Data Analysis");

                Feature analytics = features.FirstOrDefault(f => f.Name.Equals("Sequence Data analysis"));
                if (analytics == null) analytics = featureManager.Create("Sequence Data analysis", "Sequence Data analysis", udam);

                operationManager.Create("UDAM", "Home", "*", analytics);
            }
            finally
            {
                featureManager.Dispose();
                operationManager.Dispose();
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}