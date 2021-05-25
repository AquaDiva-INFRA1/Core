using BExIS.Security.Entities.Objects;
using BExIS.Security.Services.Authorization;
using BExIS.Security.Services.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using Vaiona.Persistence.Api;
using Vaiona.Web.Mvc.Modularity;

namespace BExIS.Modules.OAC.UI.Helper
{
    public class OACSeedDataGenerator : IModuleSeedDataGenerator
    {
        public void GenerateSeedData()
        {
            FeatureManager featureManager = new FeatureManager();
            OperationManager operationManager = new OperationManager();
            FeaturePermissionManager featurePermissionManager = new FeaturePermissionManager();

            try
            {
                Feature DataCollectionFeature = featureManager.FeatureRepository.Get().FirstOrDefault(f => f.Name.Equals("Data Collection"));
                if (DataCollectionFeature != null)
                {
                    Feature OAC = featureManager.FeatureRepository.Get().FirstOrDefault(f => f.Name.Equals("OMIC Archives Import"));
                    if (OAC == null) DataCollectionFeature = featureManager.Create("OMIC Archives Import", "OMIC Archives Import", DataCollectionFeature);
                    operationManager.Create("OAC", "*", "*", DataCollectionFeature);

                    Feature API = featureManager.FeatureRepository.Get().FirstOrDefault(f => f.Name.Equals("API OMIC") && f.Parent.Equals(DataCollectionFeature));
                    if (API == null) API = featureManager.Create("API OMIC", "API OMIC", DataCollectionFeature);
                    if (!operationManager.Exists("api", "SampleAccession", "*")) operationManager.Create("API", "SampleAccession", "*", API);
                    //set api public
                    featurePermissionManager.Create(null, API.Id, Security.Entities.Authorization.PermissionType.Grant);
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
                featurePermissionManager.Dispose();
            }
            

        }

        public void Dispose()
        {

        }
    }
}
