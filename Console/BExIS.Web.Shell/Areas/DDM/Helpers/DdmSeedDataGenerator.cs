
using BExIS.Security.Entities.Objects;
using BExIS.Security.Services.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vaiona.Utils.Cfg;

namespace BExIS.Modules.Ddm.UI.Helpers
{
    public class DdmSeedDataGenerator
    {
        static String DebugFilePath = System.IO.Path.Combine(AppConfiguration.GetModuleWorkspacePath("DDM"), "Semantic Search", "Debug.txt");

        public static void GenerateSeedData()
        {
            FeatureManager featureManager = new FeatureManager();
            OperationManager operationManager = new OperationManager();
            
            try
            {
                #region debug for semantic search and Semedico
                //debugging file
                if (!System.IO.File.Exists(DebugFilePath))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = System.IO.File.CreateText(DebugFilePath))
                    {
                    }
                }
                #endregion

                #region SECURITY
                //workflows = größere sachen, vielen operation
                //operations = einzelne actions

                //1.controller -> 1.Operation
                List<Feature> features = featureManager.FeatureRepository.Get().ToList();

                Feature DataDiscovery = features.FirstOrDefault(f => f.Name.Equals("Data Discovery"));
                if (DataDiscovery == null) DataDiscovery = featureManager.Create("Data Discovery", "Data Discovery");

                Feature SearchFeature = features.FirstOrDefault(f =>
                    f.Name.Equals("Search") &&
                    f.Parent != null &&
                    f.Parent.Id.Equals(DataDiscovery.Id));

                if (SearchFeature == null) SearchFeature = featureManager.Create("Search", "Search", DataDiscovery);

                Feature SearchManagementFeature = features.FirstOrDefault(f =>
                    f.Name.Equals("Search Managment") &&
                    f.Parent != null &&
                    f.Parent.Id.Equals(DataDiscovery.Id));

                if (SearchManagementFeature == null) SearchManagementFeature = featureManager.Create("Search Management", "Search Management", DataDiscovery);

                Feature Dashboard = features.FirstOrDefault(f =>
                    f.Name.Equals("Dashboard") &&
                    f.Parent != null &&
                    f.Parent.Id.Equals(DataDiscovery.Id));

                if (Dashboard == null) Dashboard = featureManager.Create("Dashboard", "Dashboard", DataDiscovery);



                //worklfows -> create dataset ->
                //WorkflowManager workflowManager = new WorkflowManager();

                //var operation = new Operation();
                //Workflow workflow = new Workflow();

                //List<Workflow> workflows = workflowManager.WorkflowRepository.Get().ToList();

                #region Help Workflow

                //workflow =
                //    workflows.FirstOrDefault(w =>
                //    w.Name.Equals("Search Help") &&
                //    w.Feature != null &&
                //    w.Feature.Id.Equals(DataDiscovery.Id));

                //if (workflow == null) workflow = workflowManager.Create("Search Help", "", DataDiscovery);

                //operationManager.Create("DDM", "Help", "*", null, workflow);
                operationManager.Create("DDM", "Help", "*");

                #endregion

                #region Search Workflow

                // ToDo -> David, Sven
                // [Sven / 2017-08-21]
                // I had to remove the feature to get dashboard running without DDM feature permissions.
                // We have to think about how we can fix it in a long run. Maybe "DDM/Home" is not the proper
                // place for dashboard!?
                operationManager.Create("DDM", "Home", "*"); //, SearchFeature);
                operationManager.Create("DDM", "Data", "*", SearchFeature);
                operationManager.Create("DDM", "SemanticSearch", "*", SearchFeature);

                #endregion

                #region Search Admin Workflow

                operationManager.Create("DDM", "Admin", "*", SearchManagementFeature);

                #endregion



                #region  Dashboard
                operationManager.Create("DDM", "Dashboard", "*", Dashboard);

                #endregion

                #endregion
            }
            finally
            {
                featureManager.Dispose();
                operationManager.Dispose();
            }

        }
    }
}