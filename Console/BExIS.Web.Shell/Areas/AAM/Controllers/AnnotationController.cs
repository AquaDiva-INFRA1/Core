using BExIS.Aam.Entities.Mapping;
using BExIS.Aam.Services;
using BExIS.Dlm.Entities.Data;
using BExIS.Dlm.Entities.DataStructure;
using BExIS.Dlm.Services.Data;
using BExIS.Modules.Aam.UI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vaiona.Persistence.Api;

namespace BExIS.Modules.Aam.UI.Controllers
{
    public class AnnotationController : Controller
    {
        // GET: Annotation
        public ActionResult Index()
        {
            AnnotationManager am = new AnnotationManager();

            #region Test functions
            //Testing get-functions
            //var test1 = am.GetAnnotations(); //Checking the count should be enough here
            //Debug.WriteLine(am.GetAnnotation(644));
            //Debug.WriteLine(am.GetAnnotation(160, 342, 1991));

            //Testing delete-functions (Script for adding the annotations is on my hard drive
            //am.DeleteAnnotation(12345);
            //am.DeleteAnnotation(new List<long>(new long[] {12346, 12347, 12348}));
            //am.DeleteAnnotationsForDataset(199);
            //am.DeleteAnnotationsForDatasetVersion(5000);

            //Testing Create functions
            /*DatasetManager dsm = new DatasetManager();
            Dataset testDataset = dsm.GetDataset(199);
            DatasetVersion testDsv = dsm.GetDatasetLatestVersion(testDataset);
            Variable testVariable;
            using(IUnitOfWork uow = this.GetUnitOfWork())
            {
                IReadOnlyRepository<Variable> repo = uow.GetReadOnlyRepository<Variable>();
                testVariable = repo.Get(2476);
            }*/
            //am.CreateAnnotation(testDataset, testDsv, testVariable, "Test Entity", "Test Characteristic", "Test Standard");
            //am.CreateAnnotation(199, 446, testVariable, "Test Entity", "Test Characteristic", "Test Standard");
            //am.CreateAnnotation(199, 446, testVariable, "Test Entity", "Test Characteristic");
            //am.CreateAnnotation(testDataset, testDsv, testVariable, "Test Entity", "Test Characteristic");
            #endregion

            List<Annotation> annotationList = am.GetAnnotations().OrderBy(an => an.Dataset.Id).ToList();
            List<AnnotationModel> model = new List<AnnotationModel>();
            foreach(Annotation an in annotationList)
            {
                model.Add(new AnnotationModel(an));
            }
            return View(model);
        }

        /// <summary>
        /// Create an annotation based on the given parameters and store it in the database.
        /// </summary>
        /// <param name="DatasetId">Id of the Dataset that the annotation is refering to</param>
        /// <param name="DatasetVersionId">Id of the DatasetVersion that the annotation is refering to</param>
        /// <param name="Variable">Variable that the annotation is refering to</param>
        /// <param name="Entity">URI-String of the entity that the annotation is refering to</param>
        /// <param name="Characteristic">URI-String of the characteristic that the annotation is refering to</param>
        /// <param name="Standard">URI-String of the standard that the annotation is refering to</param>
        /// <returns>True if everything went well</returns>
        public Boolean CreateAnnotation(long DatasetId, long DatasetVersionId, Variable Variable, String Entity, String Characteristic, String Standard = null)
        {
            AnnotationManager am = new AnnotationManager();
            if(Standard == null)
            {
                am.CreateAnnotation(DatasetId, DatasetVersionId, Variable, Entity, Characteristic);
            }
            else
            {
                am.CreateAnnotation(DatasetId, DatasetVersionId, Variable, Entity, Characteristic, Standard);
            }
            return true;
        }
    }
}