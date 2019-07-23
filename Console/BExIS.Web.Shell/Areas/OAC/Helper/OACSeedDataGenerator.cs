using BExIS.Security.Entities.Objects;
using BExIS.Security.Services.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using Vaiona.Persistence.Api;

namespace BExIS.Modules.OAC.UI.Helpers
{
    public class OACSeedDataGenerator : IDisposable
    {
        public void GenerateSeedData()
        {

            // Features
            var featureManager = new FeatureManager();
            var bexisFeature = featureManager.FindByName("BExIS") ?? featureManager.Create("BExIS", "This is the root!");

            // Operations
            var operationManager = new OperationManager();
            var o1 = operationManager.Find("OAC", "Home", "*") ?? operationManager.Create("OAC", "Home", "*");

        }

        public void Dispose()
        {

        }
    }
}
