using BExIS.Security.Entities.Authorization;
using BExIS.Security.Entities.Objects;
using BExIS.Security.Services.Authorization;
using BExIS.Security.Services.Objects;
using System;
using System.Collections.Generic;
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

            
            OperationManager operationManager = new OperationManager();
            FeatureManager featureManager = new FeatureManager();

            try
            {
                //features
                List<Feature> features = featureManager.FeatureRepository.Get().ToList();

                Feature ASM = features.FirstOrDefault(f => f.Name.Equals("Statistics and analysis"));
                if (ASM == null) ASM = featureManager.Create("Statistics and analysis", "Statistics and analysis");


                Feature portalstatistics = features.FirstOrDefault(f =>
                    f.Name.Equals("Portal Statistics") &&
                    f.Parent != null &&
                    f.Parent.Id.Equals(ASM.Id));
                if (portalstatistics == null) portalstatistics = featureManager.Create("Portal Statistics", "Portal Statistics", ASM);
                if (!operationManager.Exists("ASM", "PortalStatistics", "*")) 
                    operationManager.Create("ASM", "PortalStatistics", "*", portalstatistics);


                Feature summary = features.FirstOrDefault(f =>
                    f.Name.Equals("Dataset Summary") &&
                    f.Parent != null &&
                    f.Parent.Id.Equals(ASM.Id));
                if (summary == null) summary = featureManager.Create("Dataset Summary", "Dataset Summary", ASM);
                if (!operationManager.Exists("ASM", "DataSetSummary", "*")) 
                    operationManager.Create("ASM", "DataSetSummary", "*", summary);


                Feature classification = features.FirstOrDefault(f =>
                    f.Name.Equals("Dataset Classification") &&
                    f.Parent != null &&
                    f.Parent.Id.Equals(ASM.Id));
                if (classification == null) classification = featureManager.Create("Dataset Classification", "Dataset Classification", ASM);
                if (!operationManager.Exists("ASM", "DataSetSummary", "classification")) 
                    operationManager.Create("ASM", "DataSetSummary", "classification", classification);

                //// APIs
                Feature API = featureManager.FeatureRepository.Get().FirstOrDefault(f => 
                    f.Name.Equals("API") &&
                    f.Parent != null &&
                    f.Parent.Equals(ASM));
                if (API == null) API = featureManager.Create("ASM API", "ASM API", ASM);
                if (!operationManager.Exists("api", "Statistics", "*"))
                    operationManager.Create("api", "Statistics", "*", API);

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