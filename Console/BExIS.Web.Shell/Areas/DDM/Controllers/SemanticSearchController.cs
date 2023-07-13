using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vaiona.Web.Mvc.Models;
using Vaiona.Web.Extensions;
using System.IO;
using System.Diagnostics;
using Vaiona.IoC;
using BExIS.Ddm.Api;
using BExIS.Utils.Models;
using BExIS.Modules.Ddm.UI.Models;
using System.Text;
using Newtonsoft.Json;
using Telerik.Web.Mvc;
using System.Data;
using BExIS.Xml.Helpers;
using System.Net.Http;
using System.Net.Sockets;
using VDS.RDF;
using VDS.RDF.Query;
using BExIS.Modules.Ddm.UI.Helpers;
using Vaiona.Utils.Cfg;
using Vaiona.Persistence.Api;
using System.Web.Configuration;
using System.Configuration;
using Newtonsoft.Json.Linq;
using BExIS.Dlm.Entities.Data;
using BExIS.Aam.Services;
using BExIS.Aam.Entities.Mapping;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BExIS.Modules.Ddm.UI.Controllers
{
    public class SemanticSearchController : Controller
    {
        static string Conx = ConfigurationManager.ConnectionStrings[1].ConnectionString;

        static ShowSemanticResultModel model;
        static List<HeaderItem> headerItems;

        static String semanticExtractionURL = WebConfigurationManager.AppSettings["semanticExtractionURL"];//"http://localhost:2607/bexis-0.1/search/";
        static String semanticSearchURL = WebConfigurationManager.AppSettings["semanticSearchURL"];//"http://localhost:2607/bexis-0.1/search/";
        static String ad_ontology_merged_obda = WebConfigurationManager.AppSettings["ad-ontology-merged.obda"];
        static String ad_ontology_merged_owl = WebConfigurationManager.AppSettings["ad-ontology-merged.owl"];
        String ADOntologyPath = Path.Combine(AppConfiguration.GetModuleWorkspacePath("DDM"), "Semantic Search", "Ontologies", ad_ontology_merged_owl);

        static String semedicoSearchURL = WebConfigurationManager.AppSettings["semedicoSearchURL"];//"http://aquadiva-semeddev.inf-bb.uni-jena.de:8080/";
        static String semedicoSearchURLAD = WebConfigurationManager.AppSettings["semedicoSearchURLAD"];//"http://aquadiva-semeddev.inf-bb.uni-jena.de:8080/";
        static HeaderItem idHeader;

        static Dictionary<String, List<OntologyMapping>> mappingDic;

        static String mappingDictionaryFilePath = Path.Combine(AppConfiguration.GetModuleWorkspacePath("DDM"), "Semantic Search", "mappings.txt");
        static String autocompletionFilePath = Path.Combine(AppConfiguration.GetModuleWorkspacePath("DDM"), "Semantic Search", "autocompletion.txt");
        static String extendedautocompletionFilePath = Path.Combine(AppConfiguration.GetModuleWorkspacePath("DDM"), "Semantic Search", "extendedAutocompletion.txt");
        static String standardsFilePath = Path.Combine(AppConfiguration.GetModuleWorkspacePath("DDM"), "Semantic Search", "standards.txt");
        static String DebugFilePath = Path.Combine(AppConfiguration.GetModuleWorkspacePath("DDM"), "Semantic Search", "Debug.txt");
        static String informationSeparator = "+=+=+=+";

        static string userName = WebConfigurationManager.AppSettings["connectionStrings"];

        private void setSessions()
        {
            //Initially hide the semedico details window
            if (Session["Window"] == null)
                Session["Window"] = false;
        }

        // GET: DDM/SemanticSearch
        public ActionResult Index()
        {
            model = new ShowSemanticResultModel(CreateDataTable(makeHeader()));

            ViewBag.Title = PresentationModel.GetViewTitleForTenant("Semantic Search", this.Session.GetTenant());

            setSessions();

            //Initially show all possible Datasets
            model = new ShowSemanticResultModel(CreateDataTable(makeHeader()));
            model.semanticComponent = searchAndMerge(CreateDataTable(headerItems), "", false);
            model.semedicoServerError = "";
            return View(model);
        }

        /* Is called when the Search button in the SemanticSearch-View is used
            * Parameter:
            *      String searchTerm = content of the search bar
            *      String searchSemedico = value of a checkBox = "on" when checked, NULL otherwise
            *      String searchSemantically = value of a checkBox = "on" when checked, NULL otherwise
            * */
        [HttpPost]
        public ActionResult Index(String searchTerm, string Seamntic_depth, string Error_distance)
        {
            //debugging file
            using (StreamWriter sw = System.IO.File.AppendText(DebugFilePath))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-ddThh:mm:ssTZD") + " : Search Terms passed : " + searchTerm);
            }

            ViewBag.Title = PresentationModel.GetViewTitleForTenant("Semantic Search", this.Session.GetTenant());
            Session["Window"] = false;
            model = null;

            searchTerm = searchTerm.Trim();
            //Semedico Search
            if ((searchTerm != null) && (!searchTerm.Equals("")))
            {
                model = new ShowSemanticResultModel(CreateDataTable(makeHeader()));
                model.detailsComponent = null;
                ViewData.Model = model;
            }

            if (model == null || model.semanticComponent == null)
            {
                model = new ShowSemanticResultModel(CreateDataTable(makeHeader()));
            }

            //Semantic Search
            if (searchTerm != null)
            {
                //Merge the results from the semantic search with the results from the normal bexis-search
                //Alternative: Skip the merging: 
                model.semanticComponent = semanticSearchAsync(searchTerm, Seamntic_depth, Error_distance).Result;
                //model.semanticComponent = searchAndMerge(semanticSearch(searchTerm), searchTerm);
            }

            headerItems = makeHeader();
            ViewData["DefaultHeaderList"] = headerItems;
            ViewData["serachFlag"] = model.serachFlag;
            return View(model);
        }

        /*
            * Is called when the "Next"-Button in the _semedicoSearchConent View is pressed
            * Loads the next page by calling the semedico REST-service
            * */
        public ActionResult nextPage()
        {
            int currentSubsetSize = model.resultListComponent.subsetsize;
            int currentSubsetStart = model.resultListComponent.subsetstart;
            string searchTermString = model.resultListComponent.searchTermString;

            int newSubsetStart = currentSubsetStart + currentSubsetSize;
            int newSubsetSize = currentSubsetSize;

            //debugging file
            using (StreamWriter sw = System.IO.File.AppendText(DebugFilePath))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-ddThh:mm:ssTZD") + " : Next Page is pressed for semedico search results : from - " + newSubsetStart + "-->" + newSubsetSize);
            }

            string result = consumeSemedicoREST(searchTermString, newSubsetStart, newSubsetSize);

            if (result == null)
            {
                model.semedicoServerError = "An error occured when trying to connect to Semedico. Please try again later.";
            }
            else
            {
                model.resultListComponent = JsonConvert.DeserializeObject<SemedicoResultModel>(result);
                model.resultListComponent.searchTermString = searchTermString;
            }
            return PartialView("_semedicoSearchContent", model);
        }

        /*
            * Is called when the "Previous"-Button in the _semedicoSearchContent View is pressed
            * Loads the previous page by calling the semedico REST-service
            * */
        public ActionResult prevPage()
        {
            int currentSubsetSize = model.resultListComponent.subsetsize;
            int currentSubsetStart = model.resultListComponent.subsetstart;
            string searchTermString = model.resultListComponent.searchTermString;

            int newSubsetStart = currentSubsetStart - currentSubsetSize;
            int newSubsetSize = currentSubsetSize;

            //debugging file
            using (StreamWriter sw = System.IO.File.AppendText(DebugFilePath))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-ddThh:mm:ssTZD") + " : Previous Page is pressed for semedico search results : from - " + newSubsetStart + "-->" + newSubsetSize);
            }

            string result = consumeSemedicoREST(searchTermString, newSubsetStart, newSubsetSize);
            if (result == null)
            {
                model.semedicoServerError = "An error occured when trying to connect to Semedico. Please try again later.";
            }
            model.resultListComponent = JsonConvert.DeserializeObject<SemedicoResultModel>(result);
            model.resultListComponent.searchTermString = searchTermString;

            return PartialView("_semedicoSearchContent", model);
        }

        /*
            * Refreshes the display of the semantic search after hiding it because of the details window popup
            * */
        public ActionResult refreshGridDisplay()
        {
            ViewData["DefaultHeaderList"] = headerItems;
            ViewData["ID"] = idHeader;
            return PartialView("_metaDataResultGridViewSemantic", model.semanticComponent);
        }

        /*
            * Uses the Bexis-Search with the same search term and merges the result
            * to the given data table
            * if parameter ignoreEmptySearchTerms is false, this results in a table with all datasets due to the bexis search provider
            * */
        private System.Data.DataTable searchAndMerge(System.Data.DataTable semanticResultTable, String searchTerm, Boolean ignoreEmptySearchTerms = true)
        {
            //Split searchTerm into Tokens
            List<String> tokens = searchTerm.Split(',').ToList();
            //Trim tokens
            List<String> trimmedTokens = tokens.Select(x => x.Trim()).ToList<String>();
            //Filter out empty Tokens if ignoreEmptySearchTerms is true
            if (ignoreEmptySearchTerms)
                trimmedTokens = trimmedTokens.Where(x => x != "").ToList<String>();
            //Lower case for all tokens
            trimmedTokens = trimmedTokens.Select(x => x.ToLower()).ToList<String>();

            #region search
            foreach (String token in trimmedTokens)
            {
                //Same as HomeController Index [HttpPost]
                ISearchProvider provider = IoCFactory.Container.ResolveForSession<ISearchProvider>() as ISearchProvider;
                String FilterList = "All";
                String searchType = null;

                SetSearchType(searchType);

                if (!provider.WorkingSearchModel.CriteriaComponent.ContainsSearchCriterion(FilterList, token, SearchComponentBaseType.Category))
                {
                    provider.WorkingSearchModel.UpdateSearchCriteria(FilterList, token, SearchComponentBaseType.Category);
                }

                provider.SearchAndUpdate(provider.WorkingSearchModel.CriteriaComponent);

                //reset searchType
                // after every search - searchType must be based on
                SetSearchType("basedon");

                #endregion

                //Get Result-Table
                System.Data.DataTable bexisResult = provider.WorkingSearchModel.ResultComponent.ConvertToDataTable();

                //Add primary key to eliminate duplicates
                bexisResult.PrimaryKey = new DataColumn[] { bexisResult.Columns["ID"] };

                //Merge the two DataTables
                semanticResultTable.Merge(bexisResult, true, MissingSchemaAction.Ignore);
            }

            return semanticResultTable;
        }

        private void SetSearchType(string filter)
        {
            Session["SearchType"] = filter;
        }

        /* Is called when the user clicks on one of the Semedico-Results
            * Fills the detailsComponent of the Model
            * */
        public ActionResult ShowDetails(int id)
        {
            if (model != null)
            {
                Bibliographylist pub = model.resultListComponent.bibliographylist.ElementAt(id);
                model.detailsComponent = new SemedicoResultDetailsModel();
                model.detailsComponent.Title = pub.articleTitle;
                model.detailsComponent.Authors = pub.authors;
                model.detailsComponent.AbstractText = pub.abstractText;
                model.detailsComponent.Links = pub.externalLinks;

                //Extract year from date and put publication title and date together
                DateTime date = new DateTime();
                bool foundDate = false;
                if (pub.publication.date != null)
                {
                    date = DateTime.Parse(pub.publication.date);
                    foundDate = true;
                }

                StringBuilder sb = new StringBuilder();
                if (!(String.IsNullOrWhiteSpace(pub.publication.title)))
                {
                    sb.Append(pub.publication.title);
                    if (foundDate)
                        sb.Append(", ");
                }

                if (foundDate)
                    sb.Append(date.Year.ToString());
                model.detailsComponent.Publication = sb.ToString();

                //Format each Author into FirstName, LastName[, Affiliation]
                List<String> auth = new List<string>();
                foreach (Author author in model.detailsComponent.Authors)
                {
                    sb = new StringBuilder();
                    sb.Append(author.firstname + " " + author.lastname);

                    auth.Add(sb.ToString());
                }

                //Put the whole author-list into a list with some formatting
                if (auth.ElementAt(0) != null)
                {
                    List<String> newAuthList = new List<string>();
                    for (int i = 0; i < auth.Count; i++)
                    {
                        if (i == 0)
                        {
                            newAuthList.Add("by " + auth.ElementAt(i) + ", ");
                        }
                        else if (i == (auth.Count - 2))
                        {
                            newAuthList.Add(auth.ElementAt(i) + " and ");
                        }
                        else if (i == (auth.Count - 1))
                        {
                            newAuthList.Add(auth.ElementAt(i));
                        }
                        else
                        {
                            newAuthList.Add(auth.ElementAt(i) + ", ");
                        }

                    }

                    model.detailsComponent.FormattedAuthors = newAuthList;
                }
                Session["Window"] = true;
                ViewData["DefaultHeaderList"] = headerItems;
            }
            return View("Index", model);
        }

        /*
            * Makes a List of autocompletion items (Strings) from the dictionary
            * */
        public ActionResult getAutocompletionFromFile()
        {
            List<String> autocompletionList = new List<string>();

            if (global::System.IO.File.Exists(autocompletionFilePath))
            {
                string[] autocompletionTerms = global::System.IO.File.ReadAllLines(autocompletionFilePath);
                autocompletionList = autocompletionTerms.ToList<String>();
            }

            return Json(autocompletionList);
        }

        //private static void buildDictionary()
        public ActionResult buildDictionary()
        {
            mappingDic = new Dictionary<string, List<OntologyMapping>>();
            List<OntologyNamePair> ontologies = new List<OntologyNamePair>();


            ontologies.Add(new OntologyNamePair(ADOntologyPath, "ADOntology"));

            //Just for testing purposes
            StringBuilder sb = new StringBuilder();

            foreach (OntologyNamePair ontology in ontologies)
            {
                String ontologyPath = ontology.getPath();
                //Load the ontology as a graph
                using (IGraph g = new Graph())
                {
                    g.LoadFromFile(ontologyPath);

                    //Debugging output

                    /*foreach(Triple t in g.Triples){
                        Console.WriteLine(t.ToString());
                    }*/

                    //Create a new queryString
                    SparqlParameterizedString queryString = new SparqlParameterizedString();

                    //Add some important namespaces
                    queryString.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
                    queryString.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
                    queryString.Namespaces.AddNamespace("owl", new Uri("http://www.w3.org/2002/07/owl#"));
                    queryString.Namespaces.AddNamespace("oboe.1.0", new Uri("http://ecoinformatics.org/oboe/oboe.1.2/oboe-core.owl#"));
                    queryString.Namespaces.AddNamespace("oboe.1.2", new Uri("http://ecoinformatics.org/oboe/oboe.1.2/oboe-core.owl#"));


                    #region Characteristics
                    //Grab all subclasses (transitively) of oboe-core:Characteristic
                    queryString.CommandText =
                        "SELECT DISTINCT ?s ?label WHERE " +
                        "{?s rdfs:subClassOf* oboe.1.2:Characteristic ." +
                        "?s rdfs:label ?label . }";

                    //Debugging output
                    //Console.WriteLine(queryString.ToString());

                    //Execute the query & Insert results in Dictionary with ConceptGroup "Characteristic"
                    SparqlResultSet results = (SparqlResultSet)g.ExecuteQuery(queryString);

                    foreach (SparqlResult res in results.Results)
                    {
                        //Create the mapping and build output for the log file
                        OntologyMapping mapping = new OntologyMapping();
                        mapping.setBaseUri(g.BaseUri);
                        mapping.setMappedConceptGroup("Characteristic");

                        sb.Append(res["s"].ToString() + " ");
                        mapping.setMappedConceptUri(new Uri(res["s"].ToString()));

                        //Process the language tags - for now, just throw them away
                        String s = res["label"].ToString();

                        if (s.Contains("^^"))
                        {
                            s = s.Split(new String[] { "^^" }, StringSplitOptions.None)[0];
                        }
                        if (s.Contains("@"))
                        {
                            sb.Append(s.Split('@')[0] + "\n");
                            mapping.setDisplayName(s.Split('@')[0]);
                            //Add to mapping only if no other mapping for that string ---to be changed later
                            if (!mappingDic.ContainsKey(s.Split('@')[0].ToLower()))
                            {
                                List<OntologyMapping> mappingList = new List<OntologyMapping>();
                                mappingList.Add(mapping);
                                mappingDic.Add(s.Split('@')[0].ToLower(), new List<OntologyMapping>(mappingList));
                            }
                            else
                            {
                                mappingDic[s.Split('@')[0].ToLower()].Add(mapping);
                            }
                        }
                        else
                        {
                            sb.Append(s + "\n");
                            mapping.setDisplayName(s);
                            if (!mappingDic.ContainsKey(s.ToString().ToLower()))
                            {
                                List<OntologyMapping> mappingList = new List<OntologyMapping>();
                                mappingList.Add(mapping);
                                mappingDic.Add(s.ToString().ToLower(), mappingList);
                            }
                            else
                            {
                                mappingDic[s.ToString().ToLower()].Add(mapping);
                            }

                        }
                    }
                    #endregion

                    #region Entities

                    //Grab all subclasses (transitively) of oboe-core:Entity
                    queryString.CommandText =
                        "SELECT DISTINCT ?s ?label WHERE " +
                        "{?s rdfs:subClassOf* oboe.1.2:Entity ." +
                        "?s rdfs:label ?label . }";

                    //Execute the query & Insert results in Dictionary with ConceptGroup "Entity"
                    results = (SparqlResultSet)g.ExecuteQuery(queryString);

                    foreach (SparqlResult res in results.Results)
                    {
                        //Create the mapping and build output for the log file
                        OntologyMapping mapping = new OntologyMapping();
                        mapping.setBaseUri(g.BaseUri);
                        mapping.setMappedConceptGroup("Entity");

                        sb.Append(res["s"].ToString() + " ");
                        mapping.setMappedConceptUri(new Uri(res["s"].ToString()));

                        //Process the language tags - for now, just throw them away
                        String s = res["label"].ToString();

                        if (s.Contains("^^"))
                        {
                            s = s.Split(new String[] { "^^" }, StringSplitOptions.None)[0];
                        }
                        if (s.Contains("@"))
                        {
                            sb.Append(s.Split('@')[0] + "\n");
                            mapping.setDisplayName(s.Split('@')[0]);
                            if (!mappingDic.ContainsKey(s.Split('@')[0].ToLower()))
                            {
                                List<OntologyMapping> mappingList = new List<OntologyMapping>();
                                mappingList.Add(mapping);
                                mappingDic.Add(s.Split('@')[0].ToLower(), mappingList);
                            }
                            else
                            {
                                mappingDic[s.Split('@')[0].ToLower()].Add(mapping);
                            }

                        }
                        else
                        {
                            sb.Append(s + "\n");
                            mapping.setDisplayName(s);
                            if (!mappingDic.ContainsKey(s.ToString().ToLower()))
                            {
                                List<OntologyMapping> mappingList = new List<OntologyMapping>();
                                mappingList.Add(mapping);
                                mappingDic.Add(s.ToString().ToLower(), mappingList);
                            }
                            else
                            {
                                mappingDic[s.ToString().ToLower()].Add(mapping);
                            }

                        }
                    }
                    #endregion

                    #region Standards
                    //Grab all subclasses (transitively) of oboe-core:Entity
                    queryString.CommandText =
                        "SELECT DISTINCT ?s ?label WHERE " +
                        "{?s rdfs:subClassOf* oboe.1.2:Standard ." +
                        "?s rdfs:label ?label . }";

                    //Execute the query & Insert results in Dictionary with ConceptGroup "Characteristic"
                    results = (SparqlResultSet)g.ExecuteQuery(queryString);

                    List<String> standards = new List<string>();
                    foreach (SparqlResult res in results.Results)
                    {
                        //Process the language tags - for now, just throw them away
                        String s = res["label"].ToString();

                        if (s.Contains("^^"))
                        {
                            s = s.Split(new String[] { "^^" }, StringSplitOptions.None)[0];
                        }
                        if (s.Contains("@"))
                        {
                            s = s.Split('@')[0];
                        }

                        standards.Add(s + informationSeparator + res["s"].ToString());
                    }
                    #region Store standards in file
                    using (StreamWriter writer = new StreamWriter(standardsFilePath, false))
                    {
                        writer.WriteLine(informationSeparator);
                        foreach (String standard in standards)
                        {
                            writer.WriteLine(standard);
                        }
                    }
                }
                #endregion
                #endregion

                #region Store dictionary in file
                String serializedDic = JsonConvert.SerializeObject(mappingDic, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    TypeNameAssemblyFormat = global::System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
                });
                global::System.IO.File.WriteAllText(mappingDictionaryFilePath, serializedDic);
                #endregion

                #region Store autocompletion terms in file

                using (StreamWriter writer = new StreamWriter(autocompletionFilePath, false))
                {
                    foreach (KeyValuePair<String, List<OntologyMapping>> kvp in mappingDic)
                    {
                        if (kvp.Value.Count >= 1)
                        {
                            writer.WriteLine(kvp.Value.ElementAt(0).getDisplayName());
                        }

                    }
                }

                #endregion

                #region Store extended autocompletion terms in file
                using (StreamWriter writer = new StreamWriter(extendedautocompletionFilePath, false))
                {
                    writer.WriteLine(informationSeparator);
                    foreach (KeyValuePair<String, List<OntologyMapping>> kvp in mappingDic)
                    {
                        foreach (OntologyMapping map in kvp.Value)
                        {
                            StringBuilder outputBuilder = new StringBuilder();
                            outputBuilder.Append(map.getDisplayName());
                            outputBuilder.Append(informationSeparator);
                            outputBuilder.Append(map.getMappedConceptUri());
                            outputBuilder.Append(informationSeparator);
                            outputBuilder.Append(map.getMappedConceptGroup());
                            writer.WriteLine(outputBuilder.ToString());
                        }
                    }
                }
                #endregion

            }

            return RedirectToAction("Index");
        }

        /*
            * Calls the Semedico API and parse the results
            * */
        private SemedicoResultModel semedicoSearch(String searchTerm)
        {
            //JSON-Example for offline-tests
            //String json = "{  \"inputstring\": \"mtor\",  \"tokens\": [    \"mtor\"  ],  \"sortcriterium\": \"DATE\",  \"subsetsize\": 1,  \"subsetstart\": 1,  \"subsetend\": 10,  \"countallresults\": 103,  \"bibliographylist\": [    {      \"articleTitle\": \"mTOR, S6 and AKT expression in relation to proliferation and apoptosis/autophagy in glioma.\",      \"abstractText\": \"The mammalian target of rapamycin (mTOR) controls cell growth through protein synthesis regulation. It can be activated by protein kinase B (AKT) or through ribosomal S6 kinase (S6K1). In malignant glioma, mTOR inhibitors have antiproliferative and proapoptotic effects and mTOR has been suggested as a target of therapies, thus it is worthwhile to verify its relations with the phosphatidylinositol-3-kinase (PI3)/AKT cascade, proliferation and apoptosis in human gliomas.\nIn a series of 64 gliomas, including high- and low-grade tumors, AKT, mTOR, S6, caspase-3, poly(ADP-ribose) polymerase 1 (PARP1) and cleaved PARP1, Ki-67/MIB.1 and beclin 1 were studied by molecular biology techniques, quantitative immunohistochemistry and Western blotting.\nmTOR (phospho-mTOR), S6 (phospho-S6), AKT (phospho-AKT) levels and Ki-67/MIB.1 labelling index (LI) increased with increasing grade of malignancy. Epithelial growth factor receptor (EGFR) amplification correlated with EGFRwt and EGFRvIII immunohistochemistry, and with AKT expression; the latter correlated with mTOR expression, whereas S6 expression correlated with Ki-67/MIB.1 LI. Within the category of glioblastoma, S6 but not mTOR correlated with proliferation. mTOR did not show correlation with apoptosis, whereas it was inversely correlated with beclin 1, in line with the observation that autophagy is not activated in many malignancies.\nThe relationship of S6 with the proliferation markers emphasizes the importance of the position of S6K1 downstream AKT in the PI3/AKT pathway.\",      \"docId\": \"19661320\",      \"pmid\": \"19661320\",      \"authors\": [        {          \"firstname\": \" Laura\",          \"lastname\": \"Annovazzi\",          \"affiliation\": \"Neuro-bio-oncology Center of Policlinico di Monza Foundation/University of Turin, Vercelli, Italy.\"        },        {          \"firstname\": \" Marta\",          \"lastname\": \"Mellai\"        },        {          \"firstname\": \" Valentina\",          \"lastname\": \"Caldera\"        },        {          \"firstname\": \" Guido\",          \"lastname\": \"Valente\"        },        {          \"firstname\": \" Luciana\",          \"lastname\": \"Tessitore\"        },        {          \"firstname\": \" Davide\",          \"lastname\": \"Schiffer\"        }      ],      \"publication\": {        \"title\": \"Anticancer research\",        \"volume\": \"29\",        \"issue\": \"8\",        \"pages\": \"3087-94\",        \"date\": \"Aug 1, 2009 12:00:00 AM\",        \"dateComplete\": true      },      \"externalLinks\": [\"https://www.google.de/\", \"https://msdn.microsoft.com/\"],      \"type\": 1,      \"review\": false,      \"indextype\": \"medline\"    },    {      \"articleTitle\": \"Coordinated time-dependent modulation of AMPK/Akt/mTOR signaling and autophagy controls osteogenic differentiation of human mesenchymal stem cells.\",      \"abstractText\": \"We investigated the role of AMP-activated protein kinase (AMPK), Akt, mammalian target of rapamycin (mTOR), autophagy and their interplay in osteogenic differentiation of human dental pulp mesenchymal stem cells. The activation of various members of AMPK, Akt and mTOR signaling pathways and autophagy was analyzed by immunoblotting, while osteogenic differentiation was assessed by alkaline phosphatase staining and real-time RT-PCR/immunoblot quantification of osteocalcin, Runt-related transcription factor 2 and bone morphogenetic protein 2 mRNA and/or protein levels. Osteogenic differentiation of mesenchymal stem cells was associated with early (day 1) activation of AMPK and its target Raptor, coinciding with the inhibition of mTOR and its substrate p70S6 kinase. The early induction of autophagy was demonstrated by accumulation of autophagosome-bound LC3-II, upregulation of proautophagic beclin-1 and a decrease in the selective autophagic target p62. This was followed by the late activation of Akt/mTOR at days 3-7 of differentiation. The RNA interference-mediated silencing of AMPK, mTOR or autophagy-essential LC3β, as well as the pharmacological inhibitors of AMPK (compound C), Akt (10-DEBC hydrochloride), mTOR (rapamycin) and autophagy (bafilomycin A1, chloroquine and ammonium chloride), each suppressed mesenchymal stem cell differentiation to osteoblasts. AMPK knockdown prevented early mTOR inhibition and autophagy induction, as well as late activation of Akt/mTOR signaling, while Akt inhibition suppressed mTOR activation without affecting AMPK phosphorylation. Our data indicate that AMPK controls osteogenic differentiation of human mesenchymal stem cells through both early mTOR inhibition-mediated autophagy and late activation of Akt/mTOR signaling axis.\",      \"docId\": \"23111315\",      \"pmid\": \"23111315\",      \"authors\": [        {          \"firstname\": \" Aleksandar\",          \"lastname\": \"Pantovic\",          \"affiliation\": \"Institute of Microbiology and Immunology, School of Medicine, University of Belgrade, Dr Subotica 1, 11000 Belgrade, Serbia.\"        },        {          \"firstname\": \" Aleksandra\",          \"lastname\": \"Krstic\"        },        {          \"firstname\": \" Kristina\",          \"lastname\": \"Janjetovic\"        },        {          \"firstname\": \" Jelena\",          \"lastname\": \"Kocic\"        },        {          \"firstname\": \" Ljubica\",          \"lastname\": \"Harhaji-Trajkovic\"        },        {          \"firstname\": \" Diana\",          \"lastname\": \"Bugarski\"        },        {          \"firstname\": \" Vladimir\",          \"lastname\": \"Trajkovic\"        }      ],      \"publication\": {        \"title\": \"Bone\",        \"volume\": \"52\",        \"issue\": \"1\",        \"pages\": \"524-31\",        \"date\": \"Jan 1, 2013 12:00:00 AM\",        \"dateComplete\": true      },      \"externalLinks\": [],      \"type\": 1,      \"review\": false,      \"indextype\": \"medline\"    },    {      \"articleTitle\": \"HBx induces HepG-2 cells autophagy through PI3K/Akt-mTOR pathway.\",      \"abstractText\": \"Chronic hepatitis B virus infection is the dominant global cause of hepatocellular carcinoma (HCC), especially hepatitis B virus-X (HBx) plays a major role in this process. HBx protein promotes cell cycle progression, inactivates negative growth regulators, and binds to and inhibits the expression of p53 tumor suppressor gene and other tumor suppressor genes and senescence-related factors. However, the relationship between HBx and autophagy during the HCC development is poorly known. Previous studies found that autophagy functions as a survival mechanism in liver cancer cells. We suggest that autophagy plays a possible role in the pathogenesis of HBx-induced HCC. The present study showed that HBx transfection brought about an increase in the formation of autophagosomes and autolysosomes. Microtubule-associated protein light chain 3, Beclin 1, and lysosome-associated membrane protein 2a were up-regulated after HBx transfection. HBx-induced increase in the autophagic level was increased by mTOR inhibitor rapamycin and was blocked by treatment with the PI3K-Akt inhibitor LY294002. The same results can also be found in HepG2.2.15 cells. These results suggest that HBx activates the autophagic lysosome pathway in HepG-2 cells through the PI3K-Akt-mTOR pathway.\",      \"docId\": \"23001846\",      \"pmid\": \"23001846\",      \"authors\": [        {          \"firstname\": \" Peng\",          \"lastname\": \"Wang\",          \"affiliation\": \"Department of Hepatobiliary Surgery, First Affiliated Hospital of Soochow University, Suzhou 215004, People\u0027s Republic of China. wyc2578@sina.com\"        },        {          \"firstname\": \" Qing-Song\",          \"lastname\": \"Guo\"        },        {          \"firstname\": \" Zhi-Wei\",          \"lastname\": \"Wang\"        },        {          \"firstname\": \" Hai-Xin\",          \"lastname\": \"Qian\"        }      ],      \"publication\": {        \"title\": \"Molecular and cellular biochemistry\",        \"volume\": \"372\",        \"issue\": \"1-2\",        \"pages\": \"161-8\",        \"date\": \"Jan 1, 2013 12:00:00 AM\",        \"dateComplete\": true      },      \"externalLinks\": [],      \"type\": 1,      \"review\": false,      \"indextype\": \"medline\"    },    {      \"articleTitle\": \"Crosstalk between Bak/Bax and mTOR signaling regulates radiation-induced autophagy.\",      \"abstractText\": \"Bax and Bak, act as a gateway for caspase-mediated cell death. mTOR, an Akt downstream effector, plays a critical role in cell proliferation, growth and survival. The inhibition of mTOR induces autophagy, whereas apoptosis is a minor cell death mechanism in irradiated solid tumors. We explored possible alternative pathways for cell death induced by radiation in Bax/Bak-/- double knockout (DKO) MEF cells and wild-type cells, and we compared the cell survival: the Bax/Bak-/- cells were more radiosensitive than the wild-type cells. The irradiated cells displayed an increase in the pro-autophagic proteins ATG5-ATG12 and Beclin-1. These results are surprising in the fact that the inhibition of apoptosis resulted in increasing radiosensitivity; indicating that perhaps autophagy is the cornerstone in the cell radiation sensitivity regulation. Furthermore, irradiation upregulates autophagic programmed cell death in cells that are unable to undergo Bax/Bak-mediated apoptosis. We hypothesize the presence of a phosphatase-possibly PTEN, an Akt/mTOR negative regulator that can be inhibited by Bax/Bak. This fits with our hypothesis of Bax/Bak as a downregulator of autophagy. We are currently conducting experiments to explore the relationship between apoptosis and autophagy. Future directions in research include strategies targeting Bax/Bak in cancer xenografts and exploring novel radiosensitizers targeting autophagy pathways.\",      \"docId\": \"17204849\",      \"pmid\": \"17204849\",      \"authors\": [        {          \"firstname\": \" Luigi\",          \"lastname\": \"Moretti\",          \"affiliation\": \"Department of Radiation Oncology, Vanderbilt Ingram Cancer Center, Vanderbilt University School of Medicine, Nashville, Tennessee 37232-5671, USA.\"        },        {          \"firstname\": \" Albert\",          \"lastname\": \"Attia\"        },        {          \"firstname\": \" Kwang Woon\",          \"lastname\": \"Kim\"        },        {          \"firstname\": \" Bo\",          \"lastname\": \"Lu\"        }      ],      \"publication\": {        \"title\": \"Autophagy\",        \"volume\": \"3\",        \"issue\": \"2\",        \"pages\": \"142-4\",        \"date\": \"Mar 1, 2007 12:00:00 AM\",        \"dateComplete\": true      },      \"externalLinks\": [],      \"type\": 1,      \"review\": false,      \"indextype\": \"medline\"    },    {      \"articleTitle\": \"DNA damaging agents and p53 do not cause senescence in quiescent cells, while consecutive re-activation of mTOR is associated with conversion to senescence.\",      \"abstractText\": \"When the cell cycle is arrested, growth-promoting pathways such as mTOR (Target of Rapamycin) drive cellular senescence, characterized by cellular hyper-activation, hypertrophy and permanent loss of the proliferative potential. While arresting cell cycle, p53 (under certain conditions) can inhibit the mTOR pathway. Senescence occurs when p53 fails to inhibit mTOR. Low concentrations of DNA-damaging drugs induce p53 at levels that do not inhibit mTOR, thus causing senescence. In quiescence caused by serum starvation, mTOR is deactivated. This predicts that induction of p53 will not cause senescence in such quiescent cells. Here we tested this prediction. In proliferating normal cells, etoposide caused senescence (cells could not resume proliferation after removal of etoposide). Serum starvation prevented induction of senescence, but not of p53, by etoposide. When etoposide was removed, such cells resumed proliferation upon addition of serum. Also, doxorubicin did not cause senescent morphology in the absence of serum. Re-addition of serum caused mTOR-dependent senescence in the presence of etoposide or doxorubicin. Also, serum-starvation prevented senescent morphology caused by nutlin-3a in MCF-7 and Mel-10 cells. We conclude that induction of p53 does not activate the senescence program in quiescent cells. In cells with induced p53, re-activation of mTOR by serum stimulation causes senescence, as an equivalent of cellular growth.\",      \"docId\": \"21212465\",      \"pmid\": \"21212465\",      \"pmcid\": \"3034181\",      \"authors\": [        {          \"firstname\": \" Olga V\",          \"lastname\": \"Leontieva\",          \"affiliation\": \"Department of Cell Stress Biology, Roswell Park Cancer Institute, BLSC, L3-312, Elm and Carlton Streets, Buffalo, NY 14263, USA.\"        },        {          \"firstname\": \" Mikhail V\",          \"lastname\": \"Blagosklonny\"        }      ],      \"publication\": {        \"title\": \"Aging\",        \"volume\": \"2\",        \"issue\": \"12\",        \"pages\": \"924-35\",        \"date\": \"Dec 1, 2010 12:00:00 AM\",        \"dateComplete\": true      },      \"externalLinks\": [],      \"type\": 1,      \"review\": false,      \"indextype\": \"medline\"    },    {      \"articleTitle\": \"Avian influenza A virus H5N1 causes autophagy-mediated cell death through suppression of mTOR signaling.\",      \"abstractText\": \"Of the few avian influenza viruses that have crossed the species barrier to infect humans, the highly pathogenic influenza A (H5N1) strain has claimed the lives of more than half of the infected patients. With largely unknown mechanism of lung injury by H5N1 infection, acute respiratory distress syndrome (ARDS) is the major cause of death among the victims. Here we present the fact that H5N1 caused autophagic cell death through suppression of mTOR signaling. Inhibition of autophagy, either by depletion of autophagy gene Beclin1 or by autophagy inhibitor 3-methyladenine (3-MA), significantly reduced H5N1 mediated cell death. We suggest that autophagic cell death may contribute to the development of ARDS in H5N1 influenza patients and inhibition of autophagy could therefore become a novel strategy for the treatment of H5N1 infection.\",      \"docId\": \"22133684\",      \"pmid\": \"22133684\",      \"authors\": [        {          \"firstname\": \" Jianhui\",          \"lastname\": \"Ma\",          \"affiliation\": \"Department of Physiology and Pathophysiology, State Key Laboratory of Medical Molecular Biology, Institute of Basic Medical Sciences and School of Basic Medicine, Peking Union Medical College and Chinese Academy of Medical Sciences, Beijing.\"        },        {          \"firstname\": \" Qian\",          \"lastname\": \"Sun\"        },        {          \"firstname\": \" Ruifang\",          \"lastname\": \"Mi\"        },        {          \"firstname\": \" Hongbing\",          \"lastname\": \"Zhang\"        }      ],      \"publication\": {        \"title\": \"Journal of genetics and genomics \u003d Yi chuan xue bao\",        \"volume\": \"38\",        \"issue\": \"11\",        \"pages\": \"533-7\",        \"date\": \"Nov 20, 2011 12:00:00 AM\",        \"dateComplete\": true      },      \"externalLinks\": [],      \"type\": 1,      \"review\": false,      \"indextype\": \"medline\"    },    {      \"articleTitle\": \"Autophagy upregulation by inhibitors of caspase-3 and mTOR enhances radiotherapy in a mouse model of lung cancer.\",      \"abstractText\": \"Autophagy has been reported to be increased in irradiated cancer cells resistant to various apoptotic stimuli. We therefore hypothesized that induction of autophagy via mTOR inhibition could enhance radiosensitization in apoptosis-inhibited H460 lung cancer cells in vitro and in a lung cancer xenograft model. To test this hypothesis, combinations of Z-DEVD (caspase-3 inhibitor), RAD001 (mTOR inhibitor) and irradiation were tested in cell and mouse models. The combination of Z-DEVD and RAD001 more potently radiosensitized H460 cells than individual treatment alone. The enhancement in radiation response was not only evident in clonogenic survival assays, but also was demonstrated through markedly reduced tumor growth, cellular proliferation (Ki67 staining), apoptosis (TUNEL staining) and angiogenesis (vWF staining) in vivo. Additionally, upregulation of autophagy as measured by increased GFP-LC3-tagged autophagosome formation accompanied the noted radiosensitization in vitro and in vivo. The greatest induction of autophagy and associated radiation toxicity was exhibited in the tri-modality treatment group. Autophagy marker, LC-3-II, was reduced by 3-methyladenine (3-MA), a known inhibitor of autophagy, but further increased by the addition of lysosomal protease inhibitors (pepstatin A and E64d), demonstrating that there is autophagic induction through type III PI3 kinase during the combined therapy. Knocking down of ATG5 and beclin-1, two essential autophagic molecules, resulted in radiation resistance of lung cancer cells. Our report suggests that combined inhibition of apoptosis and mTOR during radiotherapy is a potential therapeutic strategy to enhance radiation therapy in patients with non-small cell lung cancer.\",      \"docId\": \"18424912\",      \"pmid\": \"18424912\",      \"pmcid\": \"3073356\",      \"authors\": [        {          \"firstname\": \" Kwang Woon\",          \"lastname\": \"Kim\",          \"affiliation\": \"Department of Radiation Oncology, Vanderbilt Ingram Cancer Center, Vanderbilt University School of Medicine, Nashville, Tennessee 37232, USA.\"        },        {          \"firstname\": \" Misun\",          \"lastname\": \"Hwang\"        },        {          \"firstname\": \" Luigi\",          \"lastname\": \"Moretti\"        },        {          \"firstname\": \" Jerry J\",          \"lastname\": \"Jaboin\"        },        {          \"firstname\": \" Yong I\",          \"lastname\": \"Cha\"        },        {          \"firstname\": \" Bo\",          \"lastname\": \"Lu\"        }      ],      \"publication\": {        \"title\": \"Autophagy\",        \"volume\": \"4\",        \"issue\": \"5\",        \"pages\": \"659-68\",        \"date\": \"Jul 1, 2008 12:00:00 AM\",        \"dateComplete\": true      },      \"externalLinks\": [],      \"type\": 1,      \"review\": false,      \"indextype\": \"medline\"    },    {      \"articleTitle\": \"XLID CUL4B mutants are defective in promoting TSC2 degradation and positively regulating mTOR signaling in neocortical neurons.\",      \"abstractText\": \"Truncating or missense mutation of cullin 4B (CUL4B) is one of the most prevalent causes underlying X-linked intellectual disability (XLID). CUL4B-RING E3 ubiquitin ligase promotes ubiquitination and degradation of various proteins. Consistent with previous studies, overexpression of wild-type CUL4B in 293 cells enhanced ubiquitylation and degradation of TSC2 or cyclin E. The present study shows that XLID mutant (R388X), (R572C) or (V745A) CULB failed to promote ubiquitination and degradation of TSC2 or cyclin E. Adenoviruses-mediated expression of wild-type CUL4B decreased protein level of TSC2 or cyclin E in cultured neocortical neurons of frontal lobe. Furthermore, shRNA-mediated CUL4B knockdown caused an upregulation of TSC2 or cyclin E. XLID mutant (R388X), (R572C) or (V745A) CUL4B did not downregulate protein expression of TSC2 or cyclin E in neocortical neurons. By promoting TSC2 degradation, CUL4B could positively regulate mTOR activity in neocortical neurons of frontal cortex. Consistent with this hypothesis, CUL4B knockdown-induced upregulation of TSC2 in neocortical neurons resulted in a decreased protein level of active phospho-mTOR(Ser2448) and a reduced expression of active phospho-p70S6K(Thr389) and phospho-4E-BP1(Thr37/46), two main substrates of mTOR-mediated phosphorylation. Wild-type CUL4B also increased protein level of active phospho-mTOR(Ser2448), phospho-p70S6K(Thr389) or phospho-4E-BP1(Thr37/46). XLID CUL4B mutants did not affect protein level of active phospho-mTOR(Ser2448), phospho-p70S6K(Thr389) or phospho-4E-BP1(Thr37/46). Our results suggest that XLID CUL4B mutants are defective in promoting TSC2 degradation and positively regulating mTOR signaling in neocortical neurons.\",      \"docId\": \"23348097\",      \"pmid\": \"23348097\",      \"authors\": [        {          \"firstname\": \" Hung-Li\",          \"lastname\": \"Wang\",          \"affiliation\": \"Department of Physiology, Chang Gung University School of Medicine, Taiwan, Republic of China. hlwns@mail.cgu.edu.tw\"        },        {          \"firstname\": \" Ning-Chun\",          \"lastname\": \"Chang\"        },        {          \"firstname\": \" Yi-Hsin\",          \"lastname\": \"Weng\"        },        {          \"firstname\": \" Tu-Hsueh\",          \"lastname\": \"Yeh\"        }      ],      \"publication\": {        \"title\": \"Biochimica et biophysica acta\",        \"volume\": \"1832\",        \"issue\": \"4\",        \"pages\": \"585-93\",        \"date\": \"Apr 1, 2013 12:00:00 AM\",        \"dateComplete\": true      },      \"externalLinks\": [],      \"type\": 1,      \"review\": false,      \"indextype\": \"medline\"    },    {      \"articleTitle\": \"Arenobufagin, a natural bufadienolide from toad venom, induces apoptosis and autophagy in human hepatocellular carcinoma cells through inhibition of PI3K/Akt/mTOR pathway.\",      \"abstractText\": \"Hepatocellular carcinoma (HCC) is a deadly form of cancer without effective chemotherapy so far. Currently, only sorafenib, a multitargeted tyrosine kinase inhibitor, slightly improves survival in HCC patients. In searching for natural anti-HCC components from toad venom, which is frequently used in the treatment of liver cancer in traditional Chinese medicine, we discovered that arenobufagin, a bufadienolide from toad venom, had potent antineoplastic activity against HCC HepG2 cells as well as corresponding multidrug-resistant HepG2/ADM cells. We found that arenobufagin induced mitochondria-mediated apoptosis in HCC cells, with decreasing mitochondrial potential, as well as increasing Bax/Bcl-2 expression ratio, Bax translocation from cytosol to mitochondria. Arenobufagin also induced autophagy in HepG2/ADM cells. Autophagy-specific inhibitors (3-methyladenine, chloroquine and bafilomycin A1) or Beclin1 and Atg 5 small interfering RNAs (siRNAs) enhanced arenobufagin-induced apoptosis, indicating that arenobufagin-mediated autophagy may protect HepG2/ADM cells from undergoing apoptotic cell death. In addition, we observed the inhibition of phosphatidylinositol 3-kinase (PI3K)/Akt/mammalian target of rapamycin (mTOR) pathway by arenobufagin. Interestingly, inhibition of mTOR by rapamycin or siRNA duplexes augmented arenobufagin-induced apoptosis and autophagy. Finally, arenobufagin inhibited the growth of HepG2/ADM xenograft tumors, which were associated with poly (ADP-ribose) polymerase cleavage, light chain 3-II activation and mTOR inhibition. In summary, we first demonstrated the antineoplastic effect of arenobufagin on HCC cells both in vitro and in vivo. We elucidated the underlying antineoplastic mechanisms of arenobufagin that involve cross talk between apoptosis and autophagy via inhibition of the PI3K/Akt/mTOR pathway. This study may provide a rationale for future clinical application using arenobufagin as a chemotherapeutic agent for HCC.\",      \"docId\": \"23393227\",      \"pmid\": \"23393227\",      \"authors\": [        {          \"firstname\": \" Dong-Mei\",          \"lastname\": \"Zhang\",          \"affiliation\": \"Institute of Traditional Chinese Medicine and Natural Products, College of Pharmacy, Jinan University, Guangzhou 510632, China.\"        },        {          \"firstname\": \" Jun-Shan\",          \"lastname\": \"Liu\"        },        {          \"firstname\": \" Li-Juan\",          \"lastname\": \"Deng\"        },        {          \"firstname\": \" Min-Feng\",          \"lastname\": \"Chen\"        },        {          \"firstname\": \" Anita\",          \"lastname\": \"Yiu\"        },        {          \"firstname\": \" Hui-Hui\",          \"lastname\": \"Cao\"        },        {          \"firstname\": \" Hai-Yan\",          \"lastname\": \"Tian\"        },        {          \"firstname\": \" Kwok-Pui\",          \"lastname\": \"Fung\"        },        {          \"firstname\": \" Hiroshi\",          \"lastname\": \"Kurihara\"        },        {          \"firstname\": \" Jing-Xuan\",          \"lastname\": \"Pan\"        },        {          \"firstname\": \" Wen-Cai\",          \"lastname\": \"Ye\"        }      ],      \"publication\": {        \"title\": \"Carcinogenesis\",        \"volume\": \"34\",        \"issue\": \"6\",        \"pages\": \"1331-42\",        \"date\": \"Jun 1, 2013 12:00:00 AM\",        \"dateComplete\": true      },      \"externalLinks\": [],      \"type\": 1,      \"review\": false,      \"indextype\": \"medline\"    },    {      \"articleTitle\": \"Nifedipine induced autophagy through Beclin1 and mTOR pathway in endometrial carcinoma cells.\",      \"abstractText\": \"Endometrial carcinoma is one of the most common female tract genital malignant tumors. Nifedipine, an L-type calcium channel antagonist can inhibit cell proliferation of carcinomas. Recent studies indicated that a rise in the free cytosolic calcium ([Ca(2+)](c)) was a potent inducer of autophagy. Here, we investigated the relationship between nifedipine and autophagy in Hec-1A cells.\nCells were cultured with nifedipine (10 µmol/L) and harvested at different times for counting cell number. MTT assay was applied to evaluate the cell viability and transwell assay to reveal cell migration. Apoptotic cells were detected with annexin V/PI assay. Then cells were treated with 3-methyladenine (3-MA) (2.5 mmol/L) for 0, 5, 15, 30, 60, and 120 minutes and the expression of the L-type calcium channel alpha1D (Cav1.3) protein was detected. At last, cells were cultured and assigned to four groups with different treatment: untreated (control group), 10 µmol/L nifedipine (N group), 2.5 mmol/L 3-MA (3-MA group), and 10 µmol/L nifedipine plus 2.5 mmol/L 3-MA (N+3MA group). Autophagy was detected with GFP-LC3 modulation by fluorescent microscopy, and expression of the autophagy-associated proteins (LC3, Beclin1 and P70s6K) by Western blotting and monodansylcadaverine (MDC) labeled visualization.\nProliferation of Hec-1A cells was obviously suppressed by nifedipine compared with that of the untreated cells for 24, 48, and 96 hours (P \u003d 0.000 for each day). The suppression of migration ability of the nifedipine-treated cells (94.0 ± 8.2) was significantly different from that of the untreated cells (160.00 ± 9.50, P \u003d 0.021). The level of early period cell apoptosis induced by nifedipine was (2.21 ± 0.19)%, which was (2.90 ± 0.13)% in control group (P \u003d 0.052), whereas the late period apoptosis level reached (10.38 ± 0.96)% and (4.40 ± 0.60)% (P \u003d 0.020), respectively. The 3-MA group induced a slight increase in the Cav1.3 levels within 15 minutes, but significantly attenuated the Cav1.3 levels after 30 minutes. There were more autophagic vacuoles labeled by MDC in the N group (20.63 ± 3.36) than the control group (6.29 ± 0.16, P \u003d 0.015). GFP-LC3 localization revealed that the LC3 levels of cells in 3-MA group, N+3MA group, 3-MA group were 2.80 ± 0.29, 2.30 ± 0.17, and 1.80 ± 0.21, respectively. Cells in the N group showed significant augmentation of autophagy (P \u003c 0.05). Western blotting analysis confirmed the down-regulation of LC3 levels in 3-MA group (0.85 ± 0.21) and N+3MA group (1.21 ± 0.12) compared with nifedipine treatment (2.64 ± 0.15, P \u003c 0.05). The annexin-V-FITC/PI assay showed that the level of early period cell apoptosis induced in the N+3-MA group ((11.22 ± 0.91)%) differed significantly from that of the control group ((2.51 ± 0.70)%) and N group ((3.47 ± 0.39)%). Similarly, the late period level of the N+3-MA group ((55.19 ± 2.51)%) differed significantly from that of the control group ((15.81 ± 1.36)%) and the N group ((22.09 ± 2.48)%, P \u003c 0.05). The down-regulated expression of P70s6k and up-regulated expression of the Beclin1 revealed significant differences between the N+3-MA group and control group (P \u003d 0.025; Beclin1: P \u003d 0.015).\nProliferation and migration in vitro of endometrial carcinoma Hec-1A cells are significantly suppressed by nifedipine. The nifedipine leads autophagy to oppose Hec-1A cells apoptosis. Autophagy inhibition by 3-MA leads down-regulation of Cav1.3 and enhances nifedipine-induced cell death. The nifedipine-induced autophagy is linked to Beclin1 and mTOR pathways.\",      \"docId\": \"22932192\",      \"pmid\": \"22932192\",      \"authors\": [        {          \"firstname\": \" Xiao-Xia\",          \"lastname\": \"Bao\",          \"affiliation\": \"Department of Obstetrics and Gynecology, Peking University People\u0027s Hospital, Beijing 100044, China.\"        },        {          \"firstname\": \" Bu-Shan\",          \"lastname\": \"Xie\"        },        {          \"firstname\": \" Qi\",          \"lastname\": \"Li\"        },        {          \"firstname\": \" Xiao-Ping\",          \"lastname\": \"Li\"        },        {          \"firstname\": \" Li-Hui\",          \"lastname\": \"Wei\"        },        {          \"firstname\": \" Jian-Liu\",          \"lastname\": \"Wang\"        }      ],      \"publication\": {        \"title\": \"Chinese medical journal\",        \"volume\": \"125\",        \"issue\": \"17\",        \"pages\": \"3120-6\",        \"date\": \"Sep 1, 2012 12:00:00 AM\",        \"dateComplete\": true      },      \"externalLinks\": [],      \"type\": 1,      \"review\": false,      \"indextype\": \"medline\"    }  ]}";

            //Consume REST-Service
            String output = consumeSemedicoREST(searchTerm);

            //Parse the returned JSON and populate Model
            //For offline-tests: SemedicoResultModel model = JsonConvert.DeserializeObject<SemedicoResultModel>(json);
            if (output == null)
                return null;
            SemedicoResultModel model = JsonConvert.DeserializeObject<SemedicoResultModel>(output);

            model.searchTermString = searchTerm;

            //Return populated Model
            return model;
        }

        /*
            * Creates a DataTable for given HeaderItems
            * */
        private System.Data.DataTable CreateDataTable(List<HeaderItem> items)
        {
            System.Data.DataTable table = new System.Data.DataTable();

            foreach (HeaderItem item in items)
            {
                table.Columns.Add(new DataColumn()
                {
                    ColumnName = item.Name,
                    Caption = item.DisplayName,
                    DataType = getDataType(item.DataType)
                });
            }
            table.PrimaryKey = new DataColumn[] { table.Columns["ID"] };

            return table;
        }

        /*
            * Needed to create the DataTable
            * Called from within CreateDataTable(List<HeaderItem> items)
            * */
        private Type getDataType(string dataType)
        {
            switch (dataType)
            {
                case "String":
                    {
                        return Type.GetType("System.String");
                    }

                case "Double":
                    {
                        return Type.GetType("System.Double");
                    }

                case "Int16":
                    {
                        return Type.GetType("System.Int16");
                    }

                case "Int32":
                    {
                        return Type.GetType("System.Int32");
                    }

                case "Int64":
                    {
                        return Type.GetType("System.Int64");
                    }

                case "Decimal":
                    {
                        return Type.GetType("System.Decimal");
                    }

                case "DateTime":
                    {
                        return Type.GetType("System.DateTime");
                    }

                default:
                    {
                        return Type.GetType("System.String");
                    }
            }
        }

        /* Calls the search-Server which returns DatasetIDs and VersionIDs
            * Then looks for the specified DatasetVersion and displays it
            * */
        private async Task<DataTable> semanticSearchAsync(String searchTerm, string Seamntic_depth, string Error_distance)
        {
            using (var client = new HttpClient())
            {
                Dictionary<string, string> dict_data = new Dictionary<string, string>();
                dict_data.Add("key", string.Join(" , ", searchTerm.ToLower()));
                dict_data.Add("depth", Seamntic_depth);
                dict_data.Add("errorDistance", Error_distance);

                var json_ = JsonConvert.SerializeObject(dict_data, Newtonsoft.Json.Formatting.Indented);
                using (var stringContent = new StringContent("[" + json_ + "]", Encoding.UTF8, "application/json"))
                {
                    string output = "";
                    try
                    {
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var responseTask = client.PostAsync(semanticSearchURL, stringContent).Result;
                        output = await responseTask.Content.ReadAsStringAsync();
                    }
                    catch (SocketException e)
                    {
                        model.semanticSearchServerError = "An error occured when trying to connect to the Semantic Search Server. Please try again later.";
                    }
                    catch (AggregateException e)
                    {
                        model.semanticSearchServerError = "An error occured when trying to connect to the Semantic Search Server. Please try again later.";
                    }
                    //JObject json_class = JObject.Parse((string)JToken.Parse(output));
                    model.serachFlag = true;
                    model.semanticComponent = CreateDataTable(headerItems);
                    //Parse the search-output
                    object obj = null;
                    try
                    {
                        obj = JsonConvert.DeserializeObject<object>(output);

                    }
                    catch (Newtonsoft.Json.JsonSerializationException)
                    {
                        //Do nothing when the Server states that it didn't find any results
                        model.semanticSearchServerError = "No results found.";
                    }

                    System.Data.DataTable m;
                    m = CreateDataTable(headerItems);

                    #region find metadata and fill DataTable
                    if (obj != null)
                    {
                        #region cleaning ids of datasets so they can be put in the table as id should be unique
                        List<JObject> clean_ids = new List<JObject>();
                        foreach (JObject res in (JArray)obj)
                        {
                            Boolean b = false;
                            foreach (JObject r in clean_ids)
                            {
                                if (r["dataset_id"] == res["dataset_id"])
                                {
                                    b = true;
                                    break;
                                }
                            }
                            if (!b) clean_ids.Add(res);
                        }
                        #endregion

                        foreach (JObject r in (JArray)obj)
                        {
                            DataRow row = m.NewRow();
                            row["ID"] = Int64.Parse((string)r["dataset_id"]).ToString().Trim();
                            //row["VersionID"] = Int64.Parse(r.versionno);

                            //Grab the Metadata of the current ID
                            long datasetID = long.Parse(r["dataset_id"].ToString().Trim());
                            string description = "";
                            string title = "";
                            string owner = "";

                            Dataset dataset = null;
                            using (IUnitOfWork uow = this.GetUnitOfWork())
                            {
                                var datasetRepo = uow.GetReadOnlyRepository<Dataset>();
                                dataset = datasetRepo.Get(datasetID);
                            }


                            if (dataset != null)
                            {
                                try
                                {
                                    //Grab the Metadata
                                    XmlDatasetHelper helper = new XmlDatasetHelper();

                                    row["Datasetdescription"] = helper.GetInformation(datasetID, NameAttributeValues.description).Trim();
                                    row["Title"] = helper.GetInformation(datasetID, NameAttributeValues.title).Trim();
                                    row["Owner"] = helper.GetInformation(datasetID, NameAttributeValues.owner).Trim();
                                    
                                }
                                catch (Exception exc)
                                {
                                    Console.WriteLine(exc.Message);
                                }
                                try
                                {
                                    m.Rows.Add(row);
                                }
                                catch (Exception exc)
                                {
                                    Console.WriteLine(exc.Message);
                                }
                            }
                        }
                    }
                    #endregion
                    if (m.Rows.Count > 0)
                        return m;
                    return searchAndMerge(m, searchTerm);
                }
            }
        }

        /*
            * Defines the list of HeaderItems that will be used to create the DataTable
            * */
        private List<HeaderItem> makeHeader()
        {
            headerItems = new List<HeaderItem>();

            HeaderItem headerItem = new HeaderItem()
            {
                Name = "ID",
                DisplayName = "ID",
                DataType = "Int64"
            };
            headerItems.Add(headerItem);

            idHeader = headerItem;
            ViewData["ID"] = headerItem;

            /*headerItem = new HeaderItem()
            {
                Name = "VersionID",
                DisplayName = "Version No.",
                DataType = "Int64"
            };
            headerItems.Add(headerItem);*/

            headerItem = new HeaderItem()
            {
                Name = "Title",
                DisplayName = "Title",
                DataType = "String"
            };
            headerItems.Add(headerItem);

            headerItem = new HeaderItem()
            {
                Name = "Owner",
                DisplayName = "Owner",
                DataType = "String"
            };
            headerItems.Add(headerItem);

            headerItem = new HeaderItem()
            {
                Name = "Datasetdescription",
                DisplayName = "Description",
                DataType = "String"
            };
            headerItems.Add(headerItem);

            ViewData["DefaultHeaderList"] = headerItems;

            return headerItems;
        }



        /*
            * Calls the semedico API with the specified search term and subset
            * */
        private String consumeSemedicoREST(String searchTerm, int subsetStart = 1, int subsetSize = 10)
        {
            #region Http-Request
            //Construct a HttpClient for the search-Server
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(semedicoSearchURL);
                client.Timeout = TimeSpan.FromSeconds(30);
                //Set the searchTerm as query-String
                String param = ("?inputstring=" + searchTerm.Replace(", ", "+") + "&subsetstart=" + subsetStart + "&subsetsize=" + subsetSize);

                //debugging file
                using (StreamWriter sw = System.IO.File.AppendText(DebugFilePath))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-ddThh:mm:ssTZD") + " parameters for the Semedico API : " + param);
                }
                String output = null;

                try
                {
                    HttpResponseMessage response = client.GetAsync(param).Result;  // Blocking call!
                    if (response.IsSuccessStatusCode)
                    {
                        // Get the response body. Blocking!
                        output = response.Content.ReadAsStringAsync().Result;
                        //debugging file
                        using (StreamWriter sw = System.IO.File.AppendText(DebugFilePath))
                        {
                            sw.WriteLine(DateTime.Now.ToString("yyyy-MM-ddThh:mm:ssTZD") + " Response success from Semedico : " + output);
                        }
                    }
                }
                catch (SocketException e)
                {
                    //debugging file
                    using (StreamWriter sw = System.IO.File.AppendText(DebugFilePath))
                    {
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-ddThh:mm:ssTZD") + " Semedico Socket Exception  " + e.Message);
                    }
                }
                catch (AggregateException e)
                {
                    //debugging file
                    using (StreamWriter sw = System.IO.File.AppendText(DebugFilePath))
                    {
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-ddThh:mm:ssTZD") + " Semedico Aggregate Exception   " + e.Message);
                    }
                    //Returning null if the timeout triggers
                    return null;

                }

                #endregion

                return output;
            }
        }

        [GridAction]
        public ActionResult _CustomBinding(GridCommand command)
        {
            if (model != null)
            {
                return View(new GridModel(model.semanticComponent));
            }
            return View();
        }


        public ActionResult SetResultViewVar(string key, string value)
        {
            Session[key] = value;

            return this.Json(new { success = true });
        }



        /*
         * this is the Semedico API consumption for the preposed papers
         * */
        private String consumeSemedicoREST_v2(string url, String query_String, int subsetStart = 0, int subsetSize = 9)
        {
            #region Http-Request
            //Construct a HttpClient for the search-Server
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                client.Timeout = TimeSpan.FromSeconds(30);
                //Set the searchTerm as query-String
                string encoded_param = "";
                foreach (string ont_pair in query_String.Split(',').ToList<string>())
                {
                    foreach (string pair_elem in ont_pair.Split(';').ToList<string>())
                    {
                        encoded_param = encoded_param + HttpUtility.UrlEncode(pair_elem.Trim()) + ";";
                    }
                    encoded_param = encoded_param.Substring(0, encoded_param.Length - 1) + ",";
                }
                encoded_param = encoded_param.Substring(0, encoded_param.Length - 1);
                String param = ("?request=" + encoded_param + "&start=" + subsetStart + "&size=" + subsetSize);
                String output = null;

                try
                {
                    HttpResponseMessage response = client.GetAsync(param).Result;  // Blocking call!
                    if (response.IsSuccessStatusCode)
                    {
                        // Get the response body. Blocking!
                        output = response.Content.ReadAsStringAsync().Result;

                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                    return null;
                }
                #endregion

                return output;
            }
        }

        private String get_observations_contextualized_contextualizing(String id)
        {

            String request_string = " ";

            using (Aam_Dataset_column_annotationManager aam = new Aam_Dataset_column_annotationManager())
            {
                //List<Aam_Dataset_column_annotation> annots = (List < Aam_Dataset_column_annotation > ) aam.get_all_dataset_column_annotation().Where(x => x.Dataset.Id == long.Parse(id));
                foreach (Aam_Dataset_column_annotation ann in aam.get_all_dataset_column_annotation().Where(x => x.Dataset.Id == long.Parse(id)))
                    //request_string = request_string + clean_labels(ann.characteristic_id.label) + clean_labels(ann.entity_id.label) + ",";
                    request_string = request_string + ann.entity_id.URI.ToString() + ";" + ann.characteristic_id.URI.ToString() + ",";
            }

            return request_string.Substring(0, request_string.Length - 1);
        }

        public String get_dataset_related_papers_by_ID(String id, String flag)
        {
            String Semedico_Result = "";
            string Semedico_Result_AD = "";
            Semedico_Result = "";
            String Query_4_API;
            if (id != "")
            {
                Query_4_API = get_observations_contextualized_contextualizing(id);
                if (Query_4_API.Length < 3)
                {
                    return "";
                }
                Debug.WriteLine("API Request for Dataset ID : " + id + " => " + Query_4_API);

                Semedico_Result = consumeSemedicoREST_v2(semedicoSearchURL, Query_4_API, 0, 9);
                Semedico_Result_AD = consumeSemedicoREST_v2(semedicoSearchURLAD, Query_4_API, 0, 9);
                if (model == null)
                {
                    model = new ShowSemanticResultModel(CreateDataTable(makeHeader()));
                    model.resultListComponent = new SemedicoResultModel();
                }
                model.resultListComponent = new SemedicoResultModel();
                model.resultListComponent.searchTermString = Query_4_API;
                if (flag == null) model.resultListComponent.subsetstart = 0;
            }
            else
            {
                if (flag == "nextpage")
                {
                    model.resultListComponent.subsetstart = model.resultListComponent.subsetstart + 10;
                    Semedico_Result = consumeSemedicoREST_v2(semedicoSearchURL, model.resultListComponent.searchTermString, model.resultListComponent.subsetstart, 9);
                    Semedico_Result_AD = consumeSemedicoREST_v2(semedicoSearchURLAD, model.resultListComponent.searchTermString, model.resultListComponent.subsetstart, 9);
                }
                else if (flag == "prevpage")
                {
                    model.resultListComponent.subsetstart = model.resultListComponent.subsetstart - 10;
                    if (model.resultListComponent.subsetstart < 0)
                    {
                        model.resultListComponent.subsetstart = 0;
                    }
                    Semedico_Result = consumeSemedicoREST_v2(semedicoSearchURL, model.resultListComponent.searchTermString, model.resultListComponent.subsetstart, 9);
                    Semedico_Result_AD = consumeSemedicoREST_v2(semedicoSearchURLAD, model.resultListComponent.searchTermString, model.resultListComponent.subsetstart, 9);
                }
            }
            ViewData["page"] = model.resultListComponent.subsetstart;

            Debug.WriteLine("====> Semedico result " + Semedico_Result, Semedico_Result_AD);
            var res = new
            {
                subsetstart = model.resultListComponent.subsetstart,
                Semedico_Result = Semedico_Result,
                Semedico_Result_AD = Semedico_Result_AD
            };

            return JsonConvert.SerializeObject(res);

        }


        #region refresh the observation_contexts table to the observation_contexts_URI_label 
        /*
         * refresh the observation_contexts table to the observation_contexts_URI_label + populate the table observation_contexts_URI_label in the database
          -- Table: observation_contexts_uri_label
            -- DROP TABLE observation_contexts_uri_label;

            CREATE TABLE observation_contexts_uri_label
            (
              datasets_id bigint NOT NULL,
              version_id integer NOT NULL,
              contextualized_entity character varying NOT NULL,
              contextualized_entity_label character varying NOT NULL,
              contextualizing_entity character varying NOT NULL,
              contextualizing_entity_label character varying NOT NULL,
              contextualized_entity_id bigint,
              contextualizing_entity_id bigint,
              CONSTRAINT observation_contexts_uri_label_pkey PRIMARY KEY (datasets_id, version_id, contextualized_entity, contextualizing_entity)
            )
            WITH (
              OIDS=FALSE
            );
            ALTER TABLE observation_contexts_uri_label
              OWNER TO postgres;
        
        public void insert_into_DB_URI_Label()
        {
            DatasetManager dsm = new DatasetManager();
            List<Int64>  ds_ids = dsm.GetDatasetLatestIds(true);
            
            //to load the graph one time and set the sparql query
            SparqlParameterizedString queryString = new SparqlParameterizedString();
            queryString.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            queryString.Namespaces.AddNamespace("owl", new Uri("http://www.w3.org/2002/07/owl#"));
            queryString.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            IGraph g = new Graph();
            g.LoadFromFile(Path.Combine(AppConfiguration.GetModuleWorkspacePath("DDM"), "Semantic Search", "Ontologies", "ad-ontology-merged.owl"));
            //end of loading the graph one time and set the sparql query

            foreach (Int64 ds_id in ds_ids)
            {
                NpgsqlConnection MyCnx = new NpgsqlConnection(Conx);
                MyCnx.Open();

                string select = "SELECT * FROM \"observation_contexts\" WHERE datasets_id=" + ds_id;
                NpgsqlCommand MyCmd = new NpgsqlCommand(select, MyCnx);

                NpgsqlDataReader dr = MyCmd.ExecuteReader();
                int line = 0;
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        if (dr["datasets_id"] != System.DBNull.Value)
                        {
                            var Datasetref = dr["datasets_id"].ToSafeString();
                            var version_id = dr["version_id"].ToSafeString();
                            var contextualized_entity = (String)dr["contextualized_entity"].ToSafeString();
                            var contextualizing_entity = (String)dr["contextualizing_entity"].ToSafeString();
                            var contextualized_entity_id = dr["contextualized_entity_id"].ToSafeString();
                            var contextualizing_entity_id = dr["contextualizing_entity_id"].ToSafeString();

                            //set the entity to search through the graph
                            queryString.Namespaces.AddNamespace("entity", new Uri(contextualized_entity.Trim()));
                            queryString.CommandText = "SELECT ?label WHERE" +
                                " { " +
                                "<" + contextualized_entity.Trim() + "> rdfs:label ?label " +
                                "} ";
                            // end ofthe settings

                            var contextualized_entity_label = this.Get_Label_from_entity_rdf(contextualized_entity.Trim(), g, queryString);
                            var contextualizing_entity_label = this.Get_Label_from_entity_rdf(contextualizing_entity.Trim(), g, queryString);

                            if (contextualized_entity_id == "")
                                contextualized_entity_id = "0";
                            if (contextualizing_entity_id == "")
                                contextualizing_entity_id = "0";

                            Debug.WriteLine("Row processed  number : " + line); line++;

                            string insert = "INSERT INTO observation_contexts_uri_label " +
                                "VALUES (" + Datasetref + ", " + version_id + ",  \'" + clean_entity_URI_for_insert(contextualized_entity) + 
                                "\' ,  \'" + contextualized_entity_label+ "\' ,  \'" + clean_entity_URI_for_insert(contextualizing_entity) + 
                                "\' ,  \'" +contextualizing_entity_label+ "\' , " + contextualized_entity_id+ " , " + contextualizing_entity_id+ " )";

                            NpgsqlConnection MyCnx2 = new NpgsqlConnection(Conx);
                            MyCnx2.Open();
                            NpgsqlCommand MyCmd2 = new NpgsqlCommand(insert, MyCnx2);
                            MyCmd2.ExecuteNonQuery();
                            MyCnx2.Close();
                        }
                    }
                }
                MyCnx.Close();
            }
        }

        //Takes a JSON-Serialization of a List<String> of URIs and find the labels of these URIs in the AD-ontology
        //Result contains null entries for whitespace or null string inputs
        public ContentResult FindOntologyLabels(string serializedURIList)
        {
            // debugging file
            using (StreamWriter sw = System.IO.File.AppendText(DebugFilePath))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-ddThh:mm:ssTZD") + " : FindOntologyLabels called : with parameter : "+serializedURIList );
            }

            List<String> uriList = JsonConvert.DeserializeObject<List<String>>(serializedURIList);
            List<String> labelList = new List<string>();
            //Load the ontology as a graph
            IGraph g = new Graph();
            g.LoadFromFile(ADOntologyPath);
            //Create a new queryString
            SparqlParameterizedString queryString = new SparqlParameterizedString();
            //Add some important namespaces
            queryString.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            queryString.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            String commandTextTemplate = "SELECT ?label WHERE {{<{0}> rdfs:label ?label}}";

            foreach (String uri_ in uriList)
            {
                string uri = clean_entity_URI_for_insert(uri_);
                if (String.IsNullOrWhiteSpace(uri))
                {
                    labelList.Add(null);
                }
                else
                {
                    //Add current uri to CommandText
                    queryString.CommandText = String.Format(commandTextTemplate, uri);
                    SparqlResultSet results = new SparqlResultSet();
                    try
                    {
                        results = (SparqlResultSet)g.ExecuteQuery(queryString);
                    }
                    catch(Exception ex)
                    {
                        // debugging file
                        using (StreamWriter sw = System.IO.File.AppendText(DebugFilePath))
                        {
                            sw.WriteLine(DateTime.Now.ToString("yyyy-MM-ddThh:mm:ssTZD") + " : FindOntologyLabels error :  " + ex.Message);
                        }
                        //throw (ex);
                    }
                    //Execute the query
                    

                    string labelOutput = "";
                    foreach (SparqlResult res in results.Results)
                    {
                        string label = res["label"].ToString();

                        String s = clean_labels(label);
                        if (s == "" )  s = clean_labels(uri);

                        //Remove the ^^xsd:String
                        if (s.Contains("^^"))
                        {
                            s = s.Split(new String[] { "^^" }, StringSplitOptions.None)[0];
                        }

                        if (labelOutput.Equals(""))
                        {
                            labelOutput += s;
                        }
                        else
                        {
                            labelOutput += " / " + s;
                        }
                    }
                    if (labelOutput.Equals(""))
                        labelOutput = "";
                    
                    labelList.Add(labelOutput);
                }
            }
            ContentResult result = new ContentResult();
            result.Content = JsonConvert.SerializeObject(labelList);
            using (StreamWriter sw = System.IO.File.AppendText(DebugFilePath))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-ddThh:mm:ssTZD") + " : FindOntologyLabels label outputs : " + String.Join(" ", labelList.ToArray()));
            }
            return result;
        }

        
        */
        public string clean_entity_URI_for_insert(string uri)
        {
            // debugging file
            using (StreamWriter sw = System.IO.File.AppendText(DebugFilePath))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-ddThh:mm:ssTZD") + " : clean_entity_URI_for_insert called : " + uri + " ==> " + uri.Replace("'", "''").Replace(System.Environment.NewLine, "").Replace("\n", ""));
            }
            return uri.Replace("'", "''").Replace(System.Environment.NewLine, "").Replace("\n", "");
        }

        public String Get_Label_from_entity_rdf(String entity, IGraph g, SparqlParameterizedString queryString)
        {
            SparqlResultSet results = (SparqlResultSet)g.ExecuteQuery(queryString);
            String res = "";
            if (results.Count != 0)
            {
                res = results[0]["label"].ToString().Split('^')[0];
            }
            if (res.Contains("@"))
                return res.Substring(0, res.IndexOf("@"));
            return res;
        }

        public String clean_labels(String label)
        {
            string k = label;

            if (label.Contains("@"))
            {
                return label.Substring(0, label.IndexOf("@"));
            }
            if (label.Contains("#"))
            {
                return label.Substring(label.IndexOf("@"), label.Length - 1);
            }
            else
            {
                return label;
            }


        }
        #endregion


        #region fill the annotation from csv file
        /*
        public void Fill_annotations_from_csv_file()
        {
            //var lines = System.IO.File.ReadLines("C:/Users/admin/Desktop/AnnotationFile(4740).csv");
            //
            //foreach (string line in lines)
            //{
            //    Debug.WriteLine("This is the line : ====>   " + line);
            //}
            //

            string filePath = Request["filePath"];// "C:/Users/admin/Desktop/CopyofAnnotationFile.xlsx";

            // debugging file
            using (StreamWriter sw = System.IO.File.AppendText(DebugFilePath))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-ddThh:mm:ssTZD") + " : Fill_annotations_from_csv_file called : import from " + filePath);
            }

            //FileStream for the users file
            FileStream fis = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            //Grab the sheet format from the bus
            string sheetFormatString = Convert.ToString("TopDown");
            SheetFormat CurrentSheetFormat = 0;
            Enum.TryParse<SheetFormat>(sheetFormatString, true, out CurrentSheetFormat);

            //Transforms the content of the file into a 2d-json-array
            JsonTableGenerator EUEReader = new JsonTableGenerator(fis);
            //If the active worksheet was never changed, we default to the first one
            string activeWorksheet = EUEReader.GetFirstWorksheetUri().ToString(); 
            //Generate the table for the active worksheet
            string jsonTable = EUEReader.GenerateJsonTable(CurrentSheetFormat, activeWorksheet);
            JArray textArray = JArray.Parse(jsonTable);

            int index = 0;

            string[] ds_id = new string[2];

            for (int k = 1; k< textArray.Count; k++)
            {
                if (textArray[k].ToString().Length > 1)
                {
                    var excelline = textArray[k];
                    JArray excellineJson = JArray.Parse(excelline.ToString());

                    if (excellineJson[0].ToString().Length > 1)
                    {
                        ds_id = excellineJson[0].ToString().Split('.');
                    }

                    string attribute_name = excellineJson[3].ToString();
                    string var_id = excellineJson[2].ToString();
                    string entity_uri = clean_entity_URI_for_insert(excellineJson[6].ToString());
                    string entity_label = clean_entity_URI_for_insert(excellineJson[4].ToString());
                    string charac_uri = clean_entity_URI_for_insert(excellineJson[7].ToString());
                    string charac_label = clean_entity_URI_for_insert(excellineJson[5].ToString());

                    AnnotationManager AM = new AnnotationManager();
                    Variable variable = new Variable();

                    //try
                    //{
                    DataStructureManager dataStructureManager = new DataStructureManager();
                    var structureRepo = dataStructureManager.GetUnitOfWork().GetReadOnlyRepository<StructuredDataStructure>();
                    StructuredDataStructure dataStructure = structureRepo.Get(new DatasetManager().GetDataset(Int64.Parse(ds_id[0])).DataStructure.Id);

                    if (var_id != "")
                    {
                        foreach (Variable var in dataStructure.Variables)
                        {
                            if (var.Id == Int64.Parse(var_id)) variable = var;
                        }
                    }

                    if (variable != null)
                    {
                        try
                        {
                            AM.CreateAnnotation(
                                new DatasetManager().GetDataset(Int64.Parse(ds_id[0])), new DatasetManager().GetDatasetLatestVersion(Int64.Parse(ds_id[0])),
                                variable,
                                entity_uri, entity_label,
                                charac_uri, charac_label,
                                "http://ecoinformatics.org/oboe/oboe.1.2/oboe-core.owl#Standard", "");

                            index++;
                        }
                        catch (Exception exc)
                        {
                            Debug.WriteLine(exc.Message);
                            // debugging file
                            using (StreamWriter sw = System.IO.File.AppendText(DebugFilePath))
                            {
                                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-ddThh:mm:ssTZD") + " : Fill_annotations_from_csv_file  : Exception occured : " + exc.Message);
                            }
                        }

                        if (ds_id.Length > 1)
                        {
                            //for (int i = Int32.Parse(ds_id[0].ToString())+1 ; i <= (Int32.Parse(ds_id[ds_id.Length - 1].ToString())); i++ )
                            for (int i = 1; i < ds_id.Count(); i++)
                            {
                                string dataset_id = ds_id[i].ToString();

                                try
                                {
                                    AM.CreateAnnotation(new DatasetManager().GetDataset(Int64.Parse(dataset_id)), new DatasetManager().GetDatasetLatestVersion(Int64.Parse(dataset_id)), variable,
                                        entity_uri, entity_label,
                                        charac_uri, charac_label,
                                        "http://ecoinformatics.org/oboe/oboe.1.2/oboe-core.owl#Standard", "");
                                    index++;
                                }
                                catch (Exception exc)
                                {
                                    Debug.WriteLine(exc.Message);
                                    // debugging file
                                    using (StreamWriter sw = System.IO.File.AppendText(DebugFilePath))
                                    {
                                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-ddThh:mm:ssTZD") + " : Fill_annotations_from_csv_file  : Exception occured : " +exc.Message);
                                    }
                                }
                                

                            }
                        }

                    }

                    //}
                    //catch (Exception ex)
                    //{
                    //    Debug.WriteLine(ex.ToString());
                    //}

                }
            }
        }
        */
        #endregion


    }
}
