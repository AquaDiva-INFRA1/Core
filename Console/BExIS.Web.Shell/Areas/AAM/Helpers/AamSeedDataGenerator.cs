using BExIS.Security.Entities.Objects;
using BExIS.Security.Services.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BExIS.Modules.Bam.UI.Helpers
{
    public class AamSeedDataGenerator
    {
        public static void GenerateSeedData()
        {
            FeatureManager featureManager = new FeatureManager();
            OperationManager operationManager = new OperationManager();

            try
            {
                #region Security
                List<Feature> features = featureManager.FeatureRepository.Get().ToList();

                Feature AquadivaAnnotationsFeature = features.FirstOrDefault(f => f.Name.Equals("Aquadiva Annotations"));
                if (AquadivaAnnotationsFeature == null) AquadivaAnnotationsFeature = featureManager.Create("Aquadiva Annotations", "Aquadiva Annotations");

                operationManager.Create("AAM", "Help", "*");
                operationManager.Create("AAM", "Annotation", "*", AquadivaAnnotationsFeature);
                #endregion
            }
            finally
            {
                featureManager.Dispose();
                operationManager.Dispose();
            }
        }
    }
}