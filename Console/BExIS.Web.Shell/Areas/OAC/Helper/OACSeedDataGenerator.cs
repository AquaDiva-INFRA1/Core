using BExIS.Security.Entities.Objects;
using BExIS.Security.Services.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using Vaiona.Persistence.Api;

namespace BExIS.Modules.OAC.UI.Helper
{
    public class OACSeedDataGenerator : IDisposable
    {
        public void GenerateSeedData()
        {
            FeatureManager featureManager = new FeatureManager();
            OperationManager operationManager = new OperationManager();

            try
            {
                Feature DataCollectionFeature = featureManager.FeatureRepository.Get().FirstOrDefault(f => f.Name.Equals("Data Collection"));
                if (DataCollectionFeature != null)
                {
                    Feature OAC = featureManager.FeatureRepository.Get().FirstOrDefault(f => f.Name.Equals("OMIC Archives Import"));
                    if (OAC == null) DataCollectionFeature = featureManager.Create("OMIC Archives Import", "OMIC Archives Import", DataCollectionFeature);
                    operationManager.Create("OAC", "Home", "*", DataCollectionFeature);
                }


            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                featureManager.Dispose();
                operationManager.Dispose();
            }
            

        }

        public void Dispose()
        {

        }
    }
}
