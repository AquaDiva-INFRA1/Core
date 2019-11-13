using BExIS.Security.Entities.Objects;
using BExIS.Security.Services.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BExIS.Modules.HomePage.UI.Helpers
{
    public class HomePageSeedDataGenerator : IDisposable
    {
        public void GenerateSeedData()
        {
            FeatureManager featureManager = new FeatureManager();
            OperationManager operationManager = new OperationManager();

            try
            {
                #region Security
                List<Feature> features = featureManager.FeatureRepository.Get().ToList();

                Feature HomePageFeature = features.FirstOrDefault(f => f.Name.Equals("Home Page"));
                if (HomePageFeature == null) HomePageFeature = featureManager.Create("Home Page", "Home Page");

                operationManager.Create("HomePage", "HomePage", "*", HomePageFeature);
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