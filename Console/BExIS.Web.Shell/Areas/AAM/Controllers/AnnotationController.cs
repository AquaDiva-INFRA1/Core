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

namespace BExIS.Modules.Bam.UI.Controllers
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
    }
}