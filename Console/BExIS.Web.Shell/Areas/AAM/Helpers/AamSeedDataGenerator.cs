using BExIS.Security.Entities.Objects;
using BExIS.Security.Services.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BExIS.Modules.Aam.UI.Helpers
{
    public class AamSeedDataGenerator : IDisposable
    {
        public void GenerateSeedData()
        {
            FeatureManager featureManager = new FeatureManager();
            OperationManager operationManager = new OperationManager();

            try
            {
                #region Security
                List<Feature> features = featureManager.FeatureRepository.Get().ToList();

                Feature AquadivaAnnotationsbackendFeature = features.FirstOrDefault(f => f.Name.Equals("Aquadiva Annotations"));
                if (AquadivaAnnotationsbackendFeature == null) AquadivaAnnotationsbackendFeature = featureManager.Create("Aquadiva Annotations back end", "Aquadiva Annotations back end");

                operationManager.Create("AAM", "Annotation", "*", AquadivaAnnotationsbackendFeature);
                operationManager.Create("AAM", "Aam_Dataset_column_annotation", "*", AquadivaAnnotationsbackendFeature);
                operationManager.Create("AAM", "Aam_Observation_Context", "*", AquadivaAnnotationsbackendFeature);
                operationManager.Create("AAM", "Aam_Uri", "*", AquadivaAnnotationsbackendFeature);
                #endregion
            }
            finally
            {
                featureManager.Dispose();
                operationManager.Dispose();
            }
        }


        public void Dispose()
        {
            //Do nothing for now
        }
    }
}