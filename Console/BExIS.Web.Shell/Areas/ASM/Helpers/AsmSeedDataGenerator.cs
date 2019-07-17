using BExIS.Security.Entities.Authorization;
using BExIS.Security.Entities.Objects;
using BExIS.Security.Services.Authorization;
using BExIS.Security.Services.Objects;
using System;
using System.Linq;

namespace BExIS.Modules.Asm.UI.Helpers
{
    public class AsmSeedDataGenerator : IDisposable
    {
        public static void GenerateSeedData()
        {
            // Javad:
            // 1) all the create operations should check for existence of the record
            // 2) failure on creating any record should rollback the whole seed data generation. It is one transaction.
            // 3) failues should throw an exception with enough information to pin point the root cause
            // 4) only seed data related to the functions of this modules should be genereated here.
            // BUG: seed data creation is not working because of the changes that were done in the entities and services.
            // TODO: reimplement the seed data creation method.

            //#region Security

            //// Tasks

            
            OperationManager operationManager = null;
            FeatureManager featureManager = null;

            try
            {
                Feature ASM = featureManager.FeatureRepository.Get().FirstOrDefault(f => f.Name.Equals("Statistics and analysis"));
                if (ASM == null) ASM = featureManager.Create("Statistics and analysis", "Statistics and analysis");

                Feature portalstatistics = featureManager.FeatureRepository.Get().FirstOrDefault(f => f.Name.Equals("Portal Statistics"));
                if (portalstatistics == null) portalstatistics = featureManager.Create("Portal Statistics", "Portal Statistics", ASM);
                operationManager.Create("ASM", "Analytics", "Index", portalstatistics);

                Feature summary = featureManager.FeatureRepository.Get().FirstOrDefault(f => f.Name.Equals("Dataset Summary"));
                if (summary == null) summary = featureManager.Create("Dataset Summary", "Dataset Summary", ASM);
                operationManager.Create("ASM", "Analytics", "CategoralAnalysis", summary);

            }
            finally
            {
                featureManager?.Dispose();
                operationManager?.Dispose();
            }
            
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}