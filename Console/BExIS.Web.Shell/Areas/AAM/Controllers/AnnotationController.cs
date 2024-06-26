using BExIS.Dlm.Entities.DataStructure;
using BExIS.Modules.Aam.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using BExIS.Aam.Services;
using BExIS.Aam.Entities.Mapping;

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

            //var unicorn = this.GetExistingAnnotationsByVariableLabel("Date");

            /*List<Test> tests = new List<Test>();
            tests.Add(new Test("Blabla", "Blublub", "Blibli", 1));
            tests.Add(new Test("Bim", "Bam", "Bum", 2));
            AnnotationResultList list = new AnnotationResultList(tests);
            string serialized = JsonConvert.SerializeObject(list);
            dynamic deserialized = JsonConvert.DeserializeObject(serialized);
            Debug.WriteLine(deserialized);*/

            #endregion

            List<Annotation> annotationList = am.GetAnnotations().OrderBy(an => an.Dataset.Id).ThenBy(an => an.DatasetVersion).ThenBy(an => an.Variable.Id).ToList();
            List<AnnotationModel> model = new List<AnnotationModel>();
            foreach(Annotation an in annotationList)
            {
                model.Add(new AnnotationModel(an));
            }
            return View(model);
        }

        /// <summary>
        /// Internal API to create an annotation and store it in the database.
        /// </summary>
        /// <param name="DatasetId">Id of the Dataset that the annotation is refering to</param>
        /// <param name="DatasetVersionId">Id of the DatasetVersion that the annotation is refering to</param>
        /// <param name="Variable">Variable that the annotation is refering to</param>
        /// <param name="Entity">URI-String of the entity that the annotation is refering to</param>
        /// <param name="Characteristic">URI-String of the characteristic that the annotation is refering to</param>
        /// <param name="Standard">URI-String of the standard that the annotation is refering to</param>
        /// <returns>True if everything went well</returns>
        public Boolean CreateAnnotationWithoutStandard(long DatasetId, long DatasetVersionId, Variable Variable, String Entity, String EntityLabel, String Characteristic, String CharacteristicLabel)
        {
            AnnotationManager am = new AnnotationManager();
            IEnumerable<Annotation> allAnnotations = am.GetAnnotations();

            am.CreateAnnotation(DatasetId, DatasetVersionId, Variable, Entity, EntityLabel, Characteristic, CharacteristicLabel);
            return true;
        }

        public Boolean CreateAnnotation(long DatasetId, long DatasetVersionId, Variable Variable, String Entity, String EntityLabel, String Characteristic, String CharacteristicLabel, String Standard, String StandardLabel)
        {
            AnnotationManager am = new AnnotationManager();
            am.CreateAnnotation(DatasetId, DatasetVersionId, Variable, Entity, EntityLabel, Characteristic, CharacteristicLabel, Standard, StandardLabel);
            return true;
        }

        /// <summary>
        /// Internal API to get all annotations from the database that are assigned to variables having the given label.
        /// </summary>
        /// <param name="variableLabel">Label of the Variable</param>
        /// <returns>A Dictionary containing the matching annotations as Keys and the number of occurences as Values.</returns>
        public ContentResult GetExistingAnnotationsByVariableLabel(String variableLabel)
        {
            //Dictionary<AnnotationResult, int> output = new Dictionary<AnnotationResult, int>();
            AnnotationManager am = new AnnotationManager();
            List<Annotation> matchingAnnotations = am.GetAnnotationsByVariableLabel(variableLabel);

            
            //Create a distinct list of Annotations including the number of occurences in our annotations table
            List<AnnotationResult> output = new List<AnnotationResult>();
            foreach(Annotation match in matchingAnnotations)
            {
                AnnotationResult unicorn = output.Where(an => an.Equals(match)).FirstOrDefault();
                if(unicorn != null)
                {
                    //We already have this annotation in the List, just increase the occurences
                    unicorn.Occurences++;
                }
                else
                {
                    output.Add(new AnnotationResult(match, 1));
                }
            }
            output.OrderByDescending(el => el.Occurences);
            return Content(JsonConvert.SerializeObject(output), "application/json");
        }

        /// <summary>
        /// Internal API to get all annotations from the database that are assigned to variables having the given unit and datatype.
        /// </summary>
        /// <param name="unitID">ID of the unit</param>
        /// <param name="datatypeID">ID of the datatype</param>
        /// <returns>A Dictionary containing the matching annotations as Keys and the number of occurences as Values.</returns>
        public ContentResult GetExistingAnnotationsByUnitAndDatatype(long unitID, long datatypeID)
        {
            AnnotationManager am = new AnnotationManager();
            List<Annotation> allAnnotations = am.GetAnnotations().ToList();

            List<AnnotationResult> output = new List<AnnotationResult>();

            //First look for full matches
            List<Annotation> fullMatches = allAnnotations.Where(an => an.Variable.Unit.Id == unitID && an.Variable.DataType.Id == datatypeID).ToList();

            if(fullMatches.Count != 0)
            {
                foreach(Annotation an in fullMatches)
                {
                    output.Add(new AnnotationResult(an));
                }
            }
            else
            {
                //If we didn't find any full matches, look for partial matches
                List<Annotation> matchingUnit = allAnnotations.Where(an => an.Variable.Unit.Id == unitID).ToList();
                List<Annotation> matchingDatatype = allAnnotations.Where(an => an.Variable.DataType.Id == datatypeID).ToList();

                foreach(Annotation an in matchingUnit)
                {
                    output.Add(new AnnotationResult(an));
                }

                foreach(Annotation an in matchingDatatype)
                {
                    output.Add(new AnnotationResult(an));
                }
            }

            return Content(JsonConvert.SerializeObject(output), "application/json");
        }

        public ActionResult FillLabelsInAnnotationTable()
        {
            AnnotationManager am = new AnnotationManager();
            List<Annotation> allAnnotations = am.GetAnnotations().ToList();

            //Build lists with all entity/characteristic/standard URIs
            List<String> entityURIs = new List<string>();
            List<String> charURIs = new List<string>();
            List<String> standardURIs = new List<string>();
            foreach(Annotation an in allAnnotations)
            {
                if (!entityURIs.Contains(an.Entity) && !String.IsNullOrWhiteSpace(an.Entity))
                    entityURIs.Add(an.Entity);
                if (!charURIs.Contains(an.Characteristic) && !String.IsNullOrWhiteSpace(an.Characteristic))
                    charURIs.Add(an.Characteristic);
                if (!standardURIs.Contains(an.Standard) && !String.IsNullOrWhiteSpace(an.Standard))
                    standardURIs.Add(an.Standard);
            }

            //Send each of the lists to the SemanticSearchController to find the labels from the ontology
            //if (this.IsAccessible("DDM", "SemanticSearch", "FindOntologyLabels"))
            //{
            //    ContentResult entityLabelsRes = (ContentResult) this.Run("DDM", "SemanticSearch", "FindOntologyLabels", new RouteValueDictionary() { { "serializedURIList", JsonConvert.SerializeObject(entityURIs) } });
            //    ContentResult charLabelsRes = (ContentResult) this.Run("DDM", "SemanticSearch", "FindOntologyLabels", new RouteValueDictionary() { { "serializedURIList", JsonConvert.SerializeObject(charURIs) } });
            //    ContentResult standardLabelsRes = (ContentResult) this.Run("DDM", "SemanticSearch", "FindOntologyLabels", new RouteValueDictionary() { { "serializedURIList", JsonConvert.SerializeObject(standardURIs) } });
            //
            //    List<String> entityLabels = JsonConvert.DeserializeObject<List<String>>(entityLabelsRes.Content);
            //    List<String> charLabels = JsonConvert.DeserializeObject<List<String>>(charLabelsRes.Content);
            //    List<String> standardLabels = JsonConvert.DeserializeObject<List<String>>(standardLabelsRes.Content);
            //
            //    int numberOfModifiedLabels = am.EditLabels(entityURIs.Concat(charURIs).Concat(standardURIs).ToList(), entityLabels.Concat(charLabels).Concat(standardLabels).ToList());
            //    
            //    //return Content("Number of modified labels: " + numberOfModifiedLabels.ToString()); //For Debugging purposes
            //}

            return RedirectToAction("Index");
        }
    }
}