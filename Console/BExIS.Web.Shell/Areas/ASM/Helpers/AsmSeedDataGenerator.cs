
using BExIS.Security.Entities.Objects;
using BExIS.Security.Services.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BExIS.Modules.Asm.UI.Helpers
{
    public class AsmSeedDataGenerator : IDisposable
    {
        public void GenerateSeedData()
        {
            FeatureManager featureManager = new FeatureManager();
            OperationManager operationManager = new OperationManager();

            try
            {

                #region SECURITY
                //workflows = größere sachen, vielen operation
                //operations = einzelne actions

                //1.controller -> 1.Operation
                List<Feature> features = featureManager.FeatureRepository.Get().ToList();

                ////////////////

                Feature asm = features.FirstOrDefault(f => f.Name.Equals("analytics and statistics"));
                if (asm == null) asm = featureManager.Create("analytics and statistics", "analytics and statistics");

                Feature statistics = features.FirstOrDefault(f => f.Name.Equals("Portal Statistics"));
                if (statistics == null) statistics = featureManager.Create("Portal Statistics", "Portal Statistics", asm);

                operationManager.Create("ASM", "Analytics", "*", statistics);

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
            throw new NotImplementedException();
        }
    }
}