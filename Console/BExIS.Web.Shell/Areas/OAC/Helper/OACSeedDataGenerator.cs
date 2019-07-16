using BExIS.Security.Entities.Objects;
using BExIS.Security.Services.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BExIS.Modules.OAC.UI.Helpers
{
    public class OACSeedDataGenerator : IDisposable
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

                Feature SDUM = features.FirstOrDefault(f => f.Name.Equals("Sequence Data Upload"));
                if (SDUM == null) SDUM = featureManager.Create("Sequence Data Upload", "Sequence Data Upload");

                Feature OAC = features.FirstOrDefault(f => f.Name.Equals("Sequence Data Import"));
                if (OAC == null) OAC = featureManager.Create("Sequence Data Import", "Sequence Data Import", SDUM);


                operationManager.Create("SDUM", "SequenceDataUpload", "*", SDUM);
                operationManager.Create("OAC", "Home", "*", OAC);

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
            //throw new NotImplementedException();
        }
    }
}
