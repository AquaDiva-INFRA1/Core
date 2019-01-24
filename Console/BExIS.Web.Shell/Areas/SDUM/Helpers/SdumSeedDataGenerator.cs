﻿
using BExIS.Security.Entities.Objects;
using BExIS.Security.Services.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BExIS.Modules.Sdum.UI.Helpers
{
    public class SdumSeedDataGenerator : IDisposable
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

                Feature SDUM = features.FirstOrDefault(f => f.Name.Equals("analytics and statistics"));
                if (SDUM == null) SDUM = featureManager.Create("analytics and statistics", "analytics and statistics");

                Feature statistics = features.FirstOrDefault(f => f.Name.Equals("Portal Statistics"));
                if (statistics == null) statistics = featureManager.Create("Portal Statistics", "Portal Statistics", SDUM);

                operationManager.Create("SDUM", "SequenceDataUpload", "*", statistics);

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