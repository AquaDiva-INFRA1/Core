using BExIS.Aam.Entities.Mapping;
using BExIS.Aam.Services;
using BExIS.Dlm.Services.Data;
using BExIS.Modules.Asm.UI.Models;
using BExIS.Security.Services.Subjects;
using F23.StringSimilarity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Vaiona.Logging;
using Vaiona.Utils.Cfg;

namespace BExIS.Modules.Asm.UI.Controllers
{
    public class SummaryAnalysisController : Controller
    {
        static string BaseAdress = WebConfigurationManager.AppSettings["BaseAdress"];

        List<string> domains = new List<string>() { "Sites", "BioGeoChemichals", "Cycles", "Matter Cycles",
            "Signals", "Phages", "Surface Inputs", "Gases", "Tree Matter", "Groundwater BioGeoChem", "Viruses", "Pathways" };
        static String projectTerminolgies = Path.Combine(AppConfiguration.GetModuleWorkspacePath("ASM"), "Project-terminologies.csv");

        public static Dictionary<string, List<string>> dict_ = new Dictionary<string, List<string>>();

        #region classification
        public ActionResult Summary(long id)
        {
            return PartialView("classify", id);
        }
        public async System.Threading.Tasks.Task<ActionResult> classify(string dataset)
        {
            string username = this.ControllerContext.HttpContext.User.Identity.Name;
            Dictionary<string, string> dict_data = new Dictionary<string, string>();
            dict_data.Add("username", username );
            dict_data.Add("data", dataset);

            try
            {
                List<string> nodes = new List<string>();
                List<List<int>> paths = new List<List<int>>();
                List<string> terminologies = new List<string>();
                List<string> predictions = new List<string>();
                List<string> keywords = new List<string>();
                List<Predict_results> classification_results = new List<Predict_results>();

                using (var client = new HttpClient())
                {
                    string url = BaseAdress + "/api/Summary/getSummary";
                    client.BaseAddress = new Uri(url);

                    var json = JsonConvert.SerializeObject(dict_data, Newtonsoft.Json.Formatting.Indented);
                    var stringContent = new StringContent(json);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var responseTask = await client.PostAsync(url, stringContent);
                    string result = await responseTask.Content.ReadAsStringAsync();
                    //result = "{\"sampling campaign (monthly)\":{\"input\":{\"charachteristic\":\"http://www.aquadiva.uni-jena.de/ad-ontology/ad-ontology.0.0/ad-ontology-characteristics.owl#Date\",\"entity\":\"http://purl.obolibrary.org/obo/OBI_0000747\",\"dataset_title\":\"Fungi_454_DNA_Water BIODIV3 - A03\",\"type\":\"String\",\"unit\":\"aquadiva\",\"variable_id_from_table\":\"sampling campaign (monthly)\",\"variable_value\":\"Sep-14-\"},\"class_score\":{\"0\":-10.398930549621582,\"1\":-169.6646270751953,\"2\":-112.41615295410156,\"3\":-158.6976318359375,\"4\":-71.62468719482422,\"5\":-90.60425567626953,\"6\":-229.56483459472656,\"7\":-214.24143981933594,\"8\":-213.2483367919922,\"9\":-81.06129455566406,\"10\":-208.5798797607422,\"11\":-175.37454223632812},\"predicted_class\":\" ; 0 ; 4 ; 9 ; 5\",\"onto_match\":[],\"onto_no_path\":[],\"onto_no_node\":[],\"db_match\":[\"['http://purl.obolibrary.org/obo/OBI_0000747', 'http://purl.obolibrary.org/obo/OBI_0100051', 'http://purl.obolibrary.org/obo/BFO_0000040', 'http://ecoinformatics.org/oboe/oboe.1.2/oboe-core.owl#Entity', 'http://ecoinformatics.org/oboe/oboe.1.2/oboe-core.owl#Measurement', '_:b5', 'http://ecoinformatics.org/oboe/oboe.1.2/oboe-core.owl#MeasuredValue']\",\"['http://www.aquadiva.uni-jena.de/ad-ontology/ad-ontology.0.0/ad-ontology-characteristics.owl#Date', 'http://ecoinformatics.org/oboe/oboe.1.2/oboe-core.owl#Characteristic', 'http://ecoinformatics.org/oboe/oboe.1.2/oboe-core.owl#Measurement']\",\"['http://www.aquadiva.uni-jena.de/ad-ontology/ad-ontology.0.0/ad-ontology-characteristics.owl#Date', 'http://ecoinformatics.org/oboe/oboe.1.2/oboe-core.owl#Characteristic', 'http://ecoinformatics.org/oboe/oboe.1.2/oboe-core.owl#Standard']\",\"['http://purl.obolibrary.org/obo/OBI_0000747', 'http://purl.obolibrary.org/obo/OBI_0100051', 'http://purl.obolibrary.org/obo/BFO_0000040', 'http://ecoinformatics.org/oboe/oboe.1.2/oboe-core.owl#Entity', 'http://ecoinformatics.org/oboe/oboe.1.2/oboe-core.owl#Standard']\",\"['http://purl.obolibrary.org/obo/OBI_0000747', 'http://purl.obolibrary.org/obo/OBI_0100051', 'http://purl.obolibrary.org/obo/BFO_0000040', 'http://ecoinformatics.org/oboe/oboe.1.2/oboe-core.owl#Entity', 'http://ecoinformatics.org/oboe/oboe.1.2/oboe-core.owl#Measurement']\",\"['http://www.aquadiva.uni-jena.de/ad-ontology/ad-ontology.0.0/ad-ontology-characteristics.owl#Date', 'http://ecoinformatics.org/oboe/oboe.1.2/oboe-core.owl#Characteristic', 'http://ecoinformatics.org/oboe/oboe.1.2/oboe-core.owl#MeasuredValue']\"],\"db_no_path\":[\"http://purl.obolibrary.org/obo/ENVO_01000002\",\"http://purl.obolibrary.org/obo/CHEBI_49936\",\"http://purl.obolibrary.org/obo/OBI_0200121\",\"http://purl.obolibrary.org/obo/ENVO_01000760 \",\"http://purl.obolibrary.org/obo/ENVO_00002000\",\"http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#C40098\",\"http://purl.obolibrary.org/obo/ENVO_00012408\",\"http://purl.obolibrary.org/obo/PATO_0000122 \",\"http://purl.obolibrary.org/obo/ENVO_00002005 \",\"http://purl.obolibrary.org/obo/CHEBI_18248\",\"http://purl.obolibrary.org/obo/GAZ_00000448\",\"http://purl.obolibrary.org/obo/PATO_0001025\",\"http://purl.obolibrary.org/obo/PATO_0000033 \",\"http://purl.obolibrary.org/obo/OBI_0000427\",\"http://purl.obolibrary.org/obo/CHEBI_60272\",\"http://purl.obolibrary.org/obo/PATO_0000970\",\"http://purl.obolibrary.org/obo/OBI_0001968\",\"http://purl.obolibrary.org/obo/OBI_0000426\",\"http://purl.obolibrary.org/obo/CHEBI_26833\",\"http://www.aquadiva.uni-jena.de/ad-ontology/ad-ontology.0.0/ad-ontology-characteristics.owl#SoilMoisture\",\"http://purl.obolibrary.org/obo/OBI_0000744\",\"http://www.aquadiva.uni-jena.de/ad-ontology/ad-ontology.0.0/ad-ontology-entities.owl#GroundWater\",\"http://purl.obolibrary.org/obo/CHEBI_16301\",\"http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#C45998\",\"http://www.aquadiva.uni-jena.de/ad-ontology/ad-ontology.0.0/ad-ontology-characteristics.owl#Date \",\"http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#C45290\",\"http://purl.obolibrary.org/obo/ENVO_00002001\",\"http://purl.obolibrary.org/obo/PATO_0000165\",\"http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#C45293 \",\"http://purl.obolibrary.org/obo/CHEBI_27363\",\"http://purl.obolibrary.org/obo/ENVO_00001999\",\"http://purl.obolibrary.org/obo/CHEBI_26710\",\"http://purl.obolibrary.org/obo/CHEBI_35170\",\"http://purl.obolibrary.org/obo/CHEBI_30513\",\"http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#C45277 \",\"http://purl.obolibrary.org/obo/ENVO_01000281\",\"http://purl.obolibrary.org/obo/ENVO_00001998 \r\n\",\"http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#C25447\",\"http://purl.obolibrary.org/obo/CHEBI_22977\",\"http://purl.obolibrary.org/obo/PATO_0000165\r\n\",\"http://purl.obolibrary.org/obo/OBI_0000968 \",\"http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#C45276\",\"http://purl.obolibrary.org/obo/GAZ_00000448 \",\"http://purl.obolibrary.org/obo/PATO_0000146\",\"http://purl.obolibrary.org/obo/CHEBI_28112\",\"http://www.aquadiva.uni-jena.de/ad-ontology/ad-ontology.0.0/ad-ontology-entities.owl#TemporalEntity \r\n\",\"http://purl.obolibrary.org/obo/CHEBI_27573\",\"http://purl.obolibrary.org/obo/CHEBI_16189\",\"http://purl.obolibrary.org/obo/PR_000000001\",\"http://purl.obolibrary.org/obo/ENVO_00000101\",\"http://purl.obolibrary.org/obo/CHEBI_28694\",\"http://purl.obolibrary.org/obo/CHEBI_29033 \",\"http://purl.obolibrary.org/obo/CHEBI_33376\",\"http://purl.obolibrary.org/obo/CHEBI_27594\",\"http://purl.obolibrary.org/obo/CHEBI_26708\",\"http://purl.obolibrary.org/obo/ENVO_01000635\",\"http://www.aquadiva.uni-jena.de/ad-ontology/ad-ontology.0.0/ad-ontology-entities.owl#TemporalEntity\",\"http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#C45280 \",\"http://purl.obolibrary.org/obo/ENVO_09100001\",\"http://purl.obolibrary.org/obo/PATO_0002269\",\"http://www.aquadiva.uni-jena.de/ad-ontology/ad-ontology.0.0/ad-ontology-characteristics.owl#Name \",\"http://purl.obolibrary.org/obo/PATO_0000165 \r\n\",\"http://purl.obolibrary.org/obo/CHEBI_28938 \",\"http://purl.obolibrary.org/obo/OBI_0001898\",\"http://purl.obolibrary.org/obo/CHEBI_17632 \",\"http://purl.obolibrary.org/obo/PATO_0001323\",\"http://www.aquadiva.uni-jena.de/ad-ontology/ad-ontology.0.0/ad-ontology-characteristics.owl#BaseCapacity\",\"http://purl.obolibrary.org/obo/CHEBI_22984\",\"http://purl.obolibrary.org/obo/CHEBI_33324\",\"http://www.aquadiva.uni-jena.de/ad-ontology/ad-ontology.0.0/ad-ontology-characteristics.owl#Date\",\"http://www.aquadiva.uni-jena.de/ad-ontology/ad-ontology.0.0/ad-ontology-characteristics.owl#Name\",\"http://purl.obolibrary.org/obo/OBI_0000963\",\"http://www.aquadiva.uni-jena.de/ad-ontology/ad-ontology.0.0/ad-ontology-characteristics.owl#AcidCapacity\",\"http://purl.obolibrary.org/obo/CHEBI_24431\",\"http://www.aquadiva.uni-jena.de/ad-ontology/ad-ontology.0.0/ad-ontology-entities.owl#GroundWater \",\"http://www.aquadiva.uni-jena.de/ad-ontology/ad-ontology.0.0/ad-ontology-entities.owl#Water \",\"http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#C45292\",\"http://purl.obolibrary.org/obo/ENVO_01000830 \",\"http://purl.obolibrary.org/obo/PATO_0001595 \",\"http://purl.obolibrary.org/obo/PATO_0000146 \",\"http://purl.obolibrary.org/obo/PATO_0000918\",\"http://purl.obolibrary.org/obo/ENVO_02500033\",\"http://purl.obolibrary.org/obo/CHEBI_30514\",\"http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#C45287 \",\"http://purl.obolibrary.org/obo/CHEBI_18291\",\"http://purl.obolibrary.org/obo/CHEBI_25555\",\"http://purl.obolibrary.org/obo/CHEBI_27563\",\"http://purl.obolibrary.org/obo/OBI_0200122\",\"http://www.aquadiva.uni-jena.de/ad-ontology/ad-ontology.0.0/ad-ontology-entities.owl#TemporalEntity \",\"http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#C44445 \r\n\",\"http://purl.obolibrary.org/obo/CHEBI_26216\",\"http://purl.obolibrary.org/obo/OBI_0000973\",\"http://purl.obolibrary.org/obo/OBI_0000747\",\"http://purl.obolibrary.org/obo/CHEBI_33375\",\"http://purl.obolibrary.org/obo/ENVO_00000026\",\"http://purl.obolibrary.org/obo/ENVO_00002005\",\"http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#C25337\",\"http://purl.obolibrary.org/obo/OBI_0000065\",\"http://purl.obolibrary.org/obo/CHEBI_26822\",\"http://purl.obolibrary.org/obo/ENVO_00000176\",\"http://www.aquadiva.uni-jena.de/ad-ontology/ad-ontology.0.0/ad-ontology-entities.owl#'dissolved_oxygen_atom_in_environmental_material'\",\"http://purl.obolibrary.org/obo/CHEBI_50906\",\"http://purl.obolibrary.org/obo/BFO_0000029\",\"http://purl.obolibrary.org/obo/CHEBI_17996\",\"http://purl.obolibrary.org/obo/CHEBI_25107\",\"http://purl.obolibrary.org/obo/CHEBI_16526\",\"http://purl.obolibrary.org/obo/CHEBI_17996 \",\"http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#C44445\",\"http://purl.obolibrary.org/obo/GO_0043565\",\"http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#C25364\",\" http://www.aquadiva.uni-jena.de/ad-ontology/ad-ontology.0.0/ad-ontology-characteristics.owl#Name \",\"http://purl.obolibrary.org/obo/PATO_0000122\",\"http://purl.obolibrary.org/obo/CHEBI_25016\",\"http://purl.obolibrary.org/obo/OBI_0000968\",\"http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#C45997\",\"http://purl.obolibrary.org/obo/CHEBI_28938\",\"http://purl.obolibrary.org/obo/CHEBI_27560\",\"http://purl.obolibrary.org/obo/ENVO_00002003\",\"http://purl.obolibrary.org/obo/PATO_0001595\",\"http://purl.obolibrary.org/obo/CHEBI_33696\",\"http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#C88206 \",\"http://purl.obolibrary.org/obo/ENVO_01000793\",\"http://purl.obolibrary.org/obo/OBI_0600034\",\"http://purl.obolibrary.org/obo/CHEBI_28659\",\"http://purl.obolibrary.org/obo/OBI_0000747 \",\"http://purl.obolibrary.org/obo/OBI_0000963 \",\"http://purl.obolibrary.org/obo/ENVO_00001995\",\"http://purl.obolibrary.org/obo/CHEBI_26020\",\" http://purl.obolibrary.org/obo/ENVO_01000002 \",\"http://purl.obolibrary.org/obo/ENVO_01000406\",\"http://purl.obolibrary.org/obo/PATO_0000973\",\"http://purl.obolibrary.org/obo/CHEBI_24870\",\"http://purl.obolibrary.org/obo/CHEBI_17051\",\"http://purl.obolibrary.org/obo/PATO_0000125\",\"http://purl.obolibrary.org/obo/ENVO_00002002\",\"http://purl.obolibrary.org/obo/CHEBI_28073\",\"http://purl.obolibrary.org/obo/CHEBI_16301 \",\"http://purl.obolibrary.org/obo/ENVO_01000793 \",\"http://purl.obolibrary.org/obo/ENVO_00001998\",\"http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#C88206\",\"http://www.aquadiva.uni-jena.de/ad-ontology/ad-ontology.0.0/ad-ontology-characteristics.owl#Watertable\",\"http://purl.obolibrary.org/obo/CHEBI_17632\"],\"db_no_node\":[],\"onto_target_file\":[]},\"H3_1\":{\"input\":{\"charachteristic\":\"http://purl.obolibrary.org/obo/PATO_0000070\",\"entity\":\"http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#C14209\",\"dataset_title\":\"Fungi_454_DNA_Water BIODIV3 - A03\",\"type\":\"Number\",\"unit\":\"Amount\",\"variable_id_from_table\":\"H3_1\",\"variable_value\":\"1185-1524-0-568-1639-1399-2887-186-666-36-349-20-134-91-46-23-30-37-27-22-10-14-\"},\"class_score\":{\"0\":-24.420162200927734,\"1\":-144.7536163330078,\"2\":-96.42914581298828,\"3\":-102.6191177368164,\"4\":-59.93063735961914,\"5\":-80.13410949707031,\"6\":-188.01722717285156,\"7\":-174.66627502441406,\"8\":-180.8800048828125,\"9\":-67.95101165771484,\"10\":-180.00311279296875,\"11\":-159.05343627929688},\"predicted_class\":\" ; 0 ; 4 ; 9 ; 5\",\"onto_match\":[],\"onto_no_path\":[],\"onto_no_node\":[],\"db_match\":[],\"db_no_path\":[],\"db_no_node\":[],\"onto_target_file\":[]},\"H3_2\":{\"input\":{\"charachteristic\":\"http://purl.obolibrary.org/obo/PATO_0000070\",\"entity\":\"http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#C14209\",\"dataset_title\":\"Fungi_454_DNA_Water BIODIV3 - A03\",\"type\":\"Number\",\"unit\":\"Amount\",\"variable_id_from_table\":\"H3_2\",\"variable_value\":\"1069-2071-2498-209-111-27-58-662-658-467-410-149-279-25-32-28-16-29-12-13-\"},\"class_score\":{\"0\":-24.420162200927734,\"1\":-144.7536163330078,\"2\":-96.42914581298828,\"3\":-102.6191177368164,\"4\":-59.93063735961914,\"5\":-80.13410949707031,\"6\":-188.01722717285156,\"7\":-174.66627502441406,\"8\":-180.8800048828125,\"9\":-67.95101165771484,\"10\":-180.00311279296875,\"11\":-159.05343627929688},\"predicted_class\":\" ; 0 ; 4 ; 9 ; 5\",\"onto_match\":[],\"onto_no_path\":[],\"onto_no_node\":[],\"db_match\":[],\"db_no_path\":[],\"db_no_node\":[],\"onto_target_file\":[]},\"H4_1\":{\"input\":{\"charachteristic\":\"http://purl.obolibrary.org/obo/PATO_0000070\",\"entity\":\"http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#C14209\",\"dataset_title\":\"Fungi_454_DNA_Water BIODIV3 - A03\",\"type\":\"Number\",\"unit\":\"Amount\",\"variable_id_from_table\":\"H4_1\",\"variable_value\":\"1287-61-1094-227-1037-136-252-1148-67-177-60-437-235-53-33-289-189-86-173-167-90-153-42-111-93-57-74-65-38-62-\"},\"class_score\":{\"0\":-24.420162200927734,\"1\":-144.7536163330078,\"2\":-96.42914581298828,\"3\":-102.6191177368164,\"4\":-59.93063735961914,\"5\":-80.13410949707031,\"6\":-188.01722717285156,\"7\":-174.66627502441406,\"8\":-180.8800048828125,\"9\":-67.95101165771484,\"10\":-180.00311279296875,\"11\":-159.05343627929688},\"predicted_class\":\" ; 0 ; 4 ; 9 ; 5\",\"onto_match\":[],\"onto_no_path\":[],\"onto_no_node\":[],\"db_match\":[],\"db_no_path\":[],\"db_no_node\":[],\"onto_target_file\":[]},\"H4_3\":{\"input\":{\"charachteristic\":\"http://purl.obolibrary.org/obo/PATO_0000070\",\"entity\":\"http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#C14209\",\"dataset_title\":\"Fungi_454_DNA_Water BIODIV3 - A03\",\"type\":\"Number\",\"unit\":\"Amount\",\"variable_id_from_table\":\"H4_3\",\"variable_value\":\"2075-659-177-323-208-375-868-1757-475-952-152-94-740-165-417-399-327-250-183-35-12-10-22-30-43-14-11-\"},\"class_score\":{\"0\":-24.420162200927734,\"1\":-144.7536163330078,\"2\":-96.42914581298828,\"3\":-102.6191177368164,\"4\":-59.93063735961914,\"5\":-80.13410949707031,\"6\":-188.01722717285156,\"7\":-174.66627502441406,\"8\":-180.8800048828125,\"9\":-67.95101165771484,\"10\":-180.00311279296875,\"11\":-159.05343627929688},\"predicted_class\":\" ; 0 ; 4 ; 9 ; 5\",\"onto_match\":[],\"onto_no_path\":[],\"onto_no_node\":[],\"db_match\":[],\"db_no_path\":[],\"db_no_node\":[],\"onto_target_file\":[]},\"H5_1\":{\"input\":{\"charachteristic\":\"http://purl.obolibrary.org/obo/PATO_0000070\",\"entity\":\"http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#C14209\",\"dataset_title\":\"Fungi_454_DNA_Water BIODIV3 - A03\",\"type\":\"Number\",\"unit\":\"Amount\",\"variable_id_from_table\":\"H5_1\",\"variable_value\":\"1289-0-352-249-3177-2013-42-891-686-175-65-172-48-11-135-129-109-70-63-59-56-54-46-44-43-39-25-21-18-16-\"},\"class_score\":{\"0\":-24.420162200927734,\"1\":-144.7536163330078,\"2\":-96.42914581298828,\"3\":-102.6191177368164,\"4\":-59.93063735961914,\"5\":-80.13410949707031,\"6\":-188.01722717285156,\"7\":-174.66627502441406,\"8\":-180.8800048828125,\"9\":-67.95101165771484,\"10\":-180.00311279296875,\"11\":-159.05343627929688},\"predicted_class\":\" ; 0 ; 4 ; 9 ; 5\",\"onto_match\":[],\"onto_no_path\":[],\"onto_no_node\":[],\"db_match\":[],\"db_no_path\":[],\"db_no_node\":[],\"onto_target_file\":[]},\"H5_2\":{\"input\":{\"charachteristic\":\"http://purl.obolibrary.org/obo/PATO_0000070\",\"entity\":\"http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#C14209\",\"dataset_title\":\"Fungi_454_DNA_Water BIODIV3 - A03\",\"type\":\"Number\",\"unit\":\"Amount\",\"variable_id_from_table\":\"H5_2\",\"variable_value\":\"695-73-45-81-4149-557-115-805-105-902-321-434-154-64-16-255-109-88-130-98-84-68-56-71-67-26-63-51-27-18-\"},\"class_score\":{\"0\":-24.420162200927734,\"1\":-144.7536163330078,\"2\":-96.42914581298828,\"3\":-102.6191177368164,\"4\":-59.93063735961914,\"5\":-80.13410949707031,\"6\":-188.01722717285156,\"7\":-174.66627502441406,\"8\":-180.8800048828125,\"9\":-67.95101165771484,\"10\":-180.00311279296875,\"11\":-159.05343627929688},\"predicted_class\":\" ; 0 ; 4 ; 9 ; 5\",\"onto_match\":[],\"onto_no_path\":[],\"onto_no_node\":[],\"db_match\":[],\"db_no_path\":[],\"db_no_node\":[],\"onto_target_file\":[]},\"H5_3\":{\"input\":{\"charachteristic\":\"http://purl.obolibrary.org/obo/PATO_0000070\",\"entity\":\"http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#C14209\",\"dataset_title\":\"Fungi_454_DNA_Water BIODIV3 - A03\",\"type\":\"Number\",\"unit\":\"Amount\",\"variable_id_from_table\":\"H5_3\",\"variable_value\":\"293-0-5230-257-141-68-26-46-748-31-637-599-353-126-154-32-144-94-78-18-17-11-\"},\"class_score\":{\"0\":-24.420162200927734,\"1\":-144.7536163330078,\"2\":-96.42914581298828,\"3\":-102.6191177368164,\"4\":-59.93063735961914,\"5\":-80.13410949707031,\"6\":-188.01722717285156,\"7\":-174.66627502441406,\"8\":-180.8800048828125,\"9\":-67.95101165771484,\"10\":-180.00311279296875,\"11\":-159.05343627929688},\"predicted_class\":\" ; 0 ; 4 ; 9 ; 5\",\"onto_match\":[],\"onto_no_path\":[],\"onto_no_node\":[],\"db_match\":[],\"db_no_path\":[],\"db_no_node\":[],\"onto_target_file\":[]},\"Reprerentative_sequence\":{\"input\":{\"charachteristic\":\"0\",\"entity\":\"0\",\"dataset_title\":\"Fungi_454_DNA_Water BIODIV3 - A03\",\"type\":\"Text\",\"unit\":\"none\",\"variable_id_from_table\":\"Reprerentative_sequence\",\"variable_value\":\"IPTIW0E02D74BL-IPTIW0E02DDJIS-IPTIW0E02DLMF4-IPTIW0E02CYS8Y-IPTIW0E02DWAK5-IPTIW0E02DNX6U-IPTIW0E02DMLHR-IPTIW0E02DNT9U-IPTIW0E02DDZEI-IPTIW0E02DEO5C-IPTIW0E02EKFNN-IPTIW0E02DVUE1-IPTIW0E02D1Y42-IPTIW0E02DH0Z9-IPTIW0E02D4PQD-IPTIW0E02D1YXT-IPTIW0E02DGMJV-IPTIW0E02DJOX4-IPTIW0E02DNLND-IPTIW0E02DXHNO-IPTIW0E02DPOEU-IPTIW0E02EB2RW-IPTIW0E02C8G1Q-IPTIW0E02DAA9H-IPTIW0E02DWPGJ-IPTIW0E02D7BV3-IPTIW0E02DC6YY-IPTIW0E02DEVGP-IPTIW0E02DTPVM-IPTIW0E02DHPVI-\"},\"class_score\":{\"0\":-14.157430648803711,\"1\":-69.66995239257812,\"2\":-45.444313049316406,\"3\":-79.72266387939453,\"4\":-41.197471618652344,\"5\":-34.36906433105469,\"6\":-106.65391540527344,\"7\":-98.92076873779297,\"8\":-98.90592193603516,\"9\":-45.48395919799805,\"10\":-90.64178466796875,\"11\":-63.12458801269531},\"predicted_class\":\" ; 0 ; 5 ; 4 ; 2\",\"onto_match\":[],\"onto_no_path\":[],\"onto_no_node\":[],\"db_match\":[],\"db_no_path\":[],\"db_no_node\":[],\"onto_target_file\":[]},\"Domain\":{\"input\":{\"charachteristic\":\"http://www.aquadiva.uni-jena.de/ad-ontology/ad-ontology.0.0/ad-ontology-characteristics.owl#Name\",\"entity\":\"http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#C62289\",\"dataset_title\":\"Fungi_454_DNA_Water BIODIV3 - A03\",\"type\":\"Text\",\"unit\":\"none\",\"variable_id_from_table\":\"Domain\",\"variable_value\":\"Eukaryota-\"},\"class_score\":{\"0\":-21.388832092285156,\"1\":-173.80215454101562,\"2\":-110.54634857177734,\"3\":-177.40867614746094,\"4\":-86.39814758300781,\"5\":-98.41116333007812,\"6\":-258.6026611328125,\"7\":-243.90689086914062,\"8\":-241.28433227539062,\"9\":-93.68749237060547,\"10\":-236.7564697265625,\"11\":-174.23809814453125},\"predicted_class\":\" ; 0 ; 4 ; 9 ; 5\",\"onto_match\":[],\"onto_no_path\":[],\"onto_no_node\":[],\"db_match\":[],\"db_no_path\":[],\"db_no_node\":[],\"onto_target_file\":[]},\"Kingdom\":{\"input\":{\"charachteristic\":\"http://www.aquadiva.uni-jena.de/ad-ontology/ad-ontology.0.0/ad-ontology-characteristics.owl#Name\",\"entity\":\"http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#C45276\",\"dataset_title\":\"Fungi_454_DNA_Water BIODIV3 - A03\",\"type\":\"Text\",\"unit\":\"none\",\"variable_id_from_table\":\"Kingdom\",\"variable_value\":\"Fungi-\"},\"class_score\":{\"0\":-21.388832092285156,\"1\":-173.80215454101562,\"2\":-110.54634857177734,\"3\":-177.40867614746094,\"4\":-86.39814758300781,\"5\":-98.41116333007812,\"6\":-258.6026611328125,\"7\":-243.90689086914062,\"8\":-241.28433227539062,\"9\":-93.68749237060547,\"10\":-236.7564697265625,\"11\":-174.23809814453125},\"predicted_class\":\" ; 0 ; 4 ; 9 ; 5\",\"onto_match\":[],\"onto_no_path\":[],\"onto_no_node\":[],\"db_match\":[],\"db_no_path\":[],\"db_no_node\":[],\"onto_target_file\":[]},\"Phylum\":{\"input\":{\"charachteristic\":\"http://www.aquadiva.uni-jena.de/ad-ontology/ad-ontology.0.0/ad-ontology-characteristics.owl#Name\",\"entity\":\"http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#C45277\",\"dataset_title\":\"Fungi_454_DNA_Water BIODIV3 - A03\",\"type\":\"Text\",\"unit\":\"none\",\"variable_id_from_table\":\"Phylum\",\"variable_value\":\"Ascomycota-Basidiomycota-NA-Zygomycota-Chytridiomycota-\"},\"class_score\":{\"0\":-21.388832092285156,\"1\":-173.80215454101562,\"2\":-110.54634857177734,\"3\":-177.40867614746094,\"4\":-86.39814758300781,\"5\":-98.41116333007812,\"6\":-258.6026611328125,\"7\":-243.90689086914062,\"8\":-241.28433227539062,\"9\":-93.68749237060547,\"10\":-236.7564697265625,\"11\":-174.23809814453125},\"predicted_class\":\" ; 0 ; 4 ; 9 ; 5\",\"onto_match\":[],\"onto_no_path\":[],\"onto_no_node\":[],\"db_match\":[],\"db_no_path\":[],\"db_no_node\":[],\"onto_target_file\":[]},\"Class\":{\"input\":{\"charachteristic\":\"http://www.aquadiva.uni-jena.de/ad-ontology/ad-ontology.0.0/ad-ontology-characteristics.owl#Name\",\"entity\":\"http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#C45280\",\"dataset_title\":\"Fungi_454_DNA_Water BIODIV3 - A03\",\"type\":\"Text\",\"unit\":\"none\",\"variable_id_from_table\":\"Class\",\"variable_value\":\"Dothideomycetes-Basidiomycota_class_Incertae_sedis-NA-Tremellomycetes-Sordariomycetes-Zygomycota_class_Incertae_sedis-Leotiomycetes-Agaricomycetes-Eurotiomycetes-Lecanoromycetes-Microbotryomycetes-Ustilaginomycetes-Saccharomycetes-Ascomycota_class_Incertae_sedis-Agaricostilbomycetes-Chytridiomycetes-Taphrinomycetes-\"},\"class_score\":{\"0\":-21.388832092285156,\"1\":-173.80215454101562,\"2\":-110.54634857177734,\"3\":-177.40867614746094,\"4\":-86.39814758300781,\"5\":-98.41116333007812,\"6\":-258.6026611328125,\"7\":-243.90689086914062,\"8\":-241.28433227539062,\"9\":-93.68749237060547,\"10\":-236.7564697265625,\"11\":-174.23809814453125},\"predicted_class\":\" ; 0 ; 4 ; 9 ; 5\",\"onto_match\":[],\"onto_no_path\":[],\"onto_no_node\":[],\"db_match\":[],\"db_no_path\":[],\"db_no_node\":[],\"onto_target_file\":[]},\"Order\":{\"input\":{\"charachteristic\":\"http://www.aquadiva.uni-jena.de/ad-ontology/ad-ontology.0.0/ad-ontology-characteristics.owl#Name\",\"entity\":\"http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#C45287\",\"dataset_title\":\"Fungi_454_DNA_Water BIODIV3 - A03\",\"type\":\"Text\",\"unit\":\"none\",\"variable_id_from_table\":\"Order\",\"variable_value\":\"Capnodiales-Malasseziales-NA-Tremellales-Pleosporales-Mortierellales-Helotiales-Russulales-Erysiphales-Eurotiales-Filobasidiales-Holtermanniales-Agaricales-Teloschistales-Hypocreales-Auriculariales-Sporidiobolales-Xylariales-Ustilaginales-Sordariales-Saccharomycetales-Cantharellales-Ascomycota_order_Incertae_sedis-Chaetothyriales-Polyporales-Agaricostilbales-Sordariomycetes_order_Incertae_sedis-Cystofilobasidiales-Lecanorales-Diaporthales-\"},\"class_score\":{\"0\":-21.388832092285156,\"1\":-173.80215454101562,\"2\":-110.54634857177734,\"3\":-177.40867614746094,\"4\":-86.39814758300781,\"5\":-98.41116333007812,\"6\":-258.6026611328125,\"7\":-243.90689086914062,\"8\":-241.28433227539062,\"9\":-93.68749237060547,\"10\":-236.7564697265625,\"11\":-174.23809814453125},\"predicted_class\":\" ; 0 ; 4 ; 9 ; 5\",\"onto_match\":[],\"onto_no_path\":[],\"onto_no_node\":[],\"db_match\":[],\"db_no_path\":[],\"db_no_node\":[],\"onto_target_file\":[]},\"Family\":{\"input\":{\"charachteristic\":\"http://www.aquadiva.uni-jena.de/ad-ontology/ad-ontology.0.0/ad-ontology-characteristics.owl#Name\",\"entity\":\"http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#C45290\",\"dataset_title\":\"Fungi_454_DNA_Water BIODIV3 - A03\",\"type\":\"Text\",\"unit\":\"none\",\"variable_id_from_table\":\"Family\",\"variable_value\":\"Davidiellaceae-NA-Tremellales_family_Incertae_sedis-Malasseziales_family_Incertae_sedis-Mortierellaceae-Sclerotiniaceae-Stereaceae-Erysiphaceae-Trichocomaceae-Filobasidiaceae-Holtermanniales_family_Incertae_sedis-Mycenaceae-Pleosporaceae-Teloschistaceae-Peniophoraceae-Clavicipitaceae-Sporidiobolales_family_Incertae_sedis-Xylariales_family_Incertae_sedis-Ustilaginaceae-Lachnocladiaceae-Chaetomiaceae-Pichiaceae-Pleosporales_family_Incertae_sedis-Helotiales_family_Incertae_sedis-Nectriaceae-Phaeosphaeriaceae-Ceratobasidiaceae-Typhulaceae-Ascomycota_family_Incertae_sedis-Herpotrichiellaceae-\"},\"class_score\":{\"0\":-21.388832092285156,\"1\":-173.80215454101562,\"2\":-110.54634857177734,\"3\":-177.40867614746094,\"4\":-86.39814758300781,\"5\":-98.41116333007812,\"6\":-258.6026611328125,\"7\":-243.90689086914062,\"8\":-241.28433227539062,\"9\":-93.68749237060547,\"10\":-236.7564697265625,\"11\":-174.23809814453125},\"predicted_class\":\" ; 0 ; 4 ; 9 ; 5\",\"onto_match\":[],\"onto_no_path\":[],\"onto_no_node\":[],\"db_match\":[],\"db_no_path\":[],\"db_no_node\":[],\"onto_target_file\":[]},\"Genus\":{\"input\":{\"charachteristic\":\"http://www.aquadiva.uni-jena.de/ad-ontology/ad-ontology.0.0/ad-ontology-characteristics.owl#Name\",\"entity\":\"http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#C45292\",\"dataset_title\":\"Fungi_454_DNA_Water BIODIV3 - A03\",\"type\":\"Text\",\"unit\":\"none\",\"variable_id_from_table\":\"Genus\",\"variable_value\":\"Davidiella-NA-Cryptococcus-Malassezia-Monilinia-Stereum-Blumeria-Aspergillus-Holtermanniella-Mycena-Paecilomyces-Bullera-Alternaria-Penicillium-Xanthoria-Dioszegia-Claviceps-Sporobolomyces-Monographella-Anthracocystis-Humicola-Pichia-Leptosphaerulina-Oculimacula-Sawadaea-Erysiphe-Mycocentrospora-Typhula-Cladosporium-Chalara-\"},\"class_score\":{\"0\":-21.388832092285156,\"1\":-173.80215454101562,\"2\":-110.54634857177734,\"3\":-177.40867614746094,\"4\":-86.39814758300781,\"5\":-98.41116333007812,\"6\":-258.6026611328125,\"7\":-243.90689086914062,\"8\":-241.28433227539062,\"9\":-93.68749237060547,\"10\":-236.7564697265625,\"11\":-174.23809814453125},\"predicted_class\":\" ; 0 ; 4 ; 9 ; 5\",\"onto_match\":[],\"onto_no_path\":[],\"onto_no_node\":[],\"db_match\":[],\"db_no_path\":[],\"db_no_node\":[],\"onto_target_file\":[]},\"Species\":{\"input\":{\"charachteristic\":\"http://www.aquadiva.uni-jena.de/ad-ontology/ad-ontology.0.0/ad-ontology-characteristics.owl#Name\",\"entity\":\"http://ncicb.nci.nih.gov/xml/owl/EVS/Thesaurus.owl#C45293\",\"dataset_title\":\"Fungi_454_DNA_Water BIODIV3 - A03\",\"type\":\"Text\",\"unit\":\"none\",\"variable_id_from_table\":\"Species\",\"variable_value\":\"NA-Tremellales_sp-Cryptococcus_victoriae-Malassezia_restricta-Pleosporales_sp_1_MU_2012-Sordariomycetes_sp_WF151-Monilinia_fructigena-Stereum_sp_Po50-Blumeria_graminis-Aspergillus_sydowii-Cryptococcus_stepposus-Cryptococcus_festucosus-Paecilomyces_sp_MTFA02-Bullera_globospora-Xanthoria_coomae-Dioszegia_buhagiarii-Claviceps_purpurea-Sporobolomyces_roseus-Sporobolomyces_foliicola-Microdochium_nivale-Anthracocystis_sp_YL_2013-Humicola_fuscoatra-Pichia_jadinii-Basidiomycota_sp_CBS_10026-Leptosphaerulina_sp_BF19-Cryptococcus_albidosimilis-Oculimacula_yallundae-Sawadaea_bicornis-Erysiphe_sp-Cryptococcus_wieringae-\"},\"class_score\":{\"0\":-21.388832092285156,\"1\":-173.80215454101562,\"2\":-110.54634857177734,\"3\":-177.40867614746094,\"4\":-86.39814758300781,\"5\":-98.41116333007812,\"6\":-258.6026611328125,\"7\":-243.90689086914062,\"8\":-241.28433227539062,\"9\":-93.68749237060547,\"10\":-236.7564697265625,\"11\":-174.23809814453125},\"predicted_class\":\" ; 0 ; 4 ; 9 ; 5\",\"onto_match\":[],\"onto_no_path\":[],\"onto_no_node\":[],\"db_match\":[],\"db_no_path\":[],\"db_no_node\":[],\"onto_target_file\":[]},\"Fungal_OTU\":{\"input\":{\"charachteristic\":\"http://www.aquadiva.uni-jena.de/ad-ontology/ad-ontology.0.0/ad-ontology-characteristics.owl#Name\",\"entity\":\"http://purl.obolibrary.org/obo/OBI_0001968\",\"dataset_title\":\"Fungi_454_DNA_Water BIODIV3 - A03\",\"type\":\"Text\",\"unit\":\"none\",\"variable_id_from_table\":\"Fungal_OTU\",\"variable_value\":\"Otu_0001-Otu_0002-Otu_0003-Otu_0004-Otu_0005-Otu_0006-Otu_0007-Otu_0008-Otu_0009-Otu_0010-Otu_0011-Otu_0012-Otu_0016-Otu_0017-Otu_0018-Otu_0019-Otu_0020-Otu_0021-Otu_0022-Otu_0023-Otu_0024-Otu_0025-Otu_0026-Otu_0027-Otu_0028-Otu_0029-Otu_0030-Otu_0031-Otu_0032-Otu_0033-\"},\"class_score\":{\"0\":-21.388832092285156,\"1\":-173.80215454101562,\"2\":-110.54634857177734,\"3\":-177.40867614746094,\"4\":-86.39814758300781,\"5\":-98.41116333007812,\"6\":-258.6026611328125,\"7\":-243.90689086914062,\"8\":-241.28433227539062,\"9\":-93.68749237060547,\"10\":-236.7564697265625,\"11\":-174.23809814453125},\"predicted_class\":\" ; 0 ; 4 ; 9 ; 5\",\"onto_match\":[],\"onto_no_path\":[],\"onto_no_node\":[],\"db_match\":[],\"db_no_path\":[],\"db_no_node\":[],\"onto_target_file\":[]}}";

                    JObject json_class = JObject.Parse((string)JToken.Parse(result));

                    // parsing results - matches to the ontologies
                    int index = 0;
                    foreach (JProperty xx in (JToken)json_class)
                    {
                        Predict_results analysisResults = new Predict_results(xx.Value, xx.Name);

                        classification_results.Add(analysisResults);
                        foreach (string ss in analysisResults.predition_bestMatches.Split(';'))
                        {
                            if ((ss != "\" \"") && (ss != " ") && (!predictions.Contains(domains[Int32.Parse(ss)])))
                            {
                                predictions.Add(domains[Int32.Parse(ss)]);
                            }
                        }
                        index++;
                    }

                    DatasetManager dm = new DatasetManager();
                    DataTable table = dm.GetLatestDatasetVersionTuples(Int64.Parse(dataset), 0, 0, true);
                    dm.Dispose();

                    foreach (string pred in predictions)
                    {
                        using (var reader = new StreamReader(projectTerminolgies))
                        {
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                List<string> tmp = line.Split(',').ToList<string>();
                                if (tmp.Count > 1)
                                {
                                    if (tmp[1].Trim() == pred.Trim())
                                    {
                                        foreach (string keyword in tmp[2].Replace("  ", " ").Split('"'))
                                        {
                                            foreach (DataColumn dc in table.Columns)
                                            {
                                                if ((!keywords.Contains(keyword.Replace("\"", "").Trim())) &&
                                                    (similarity(dc.Caption.Trim(), keyword.Replace("\"", "").Trim()) > 0.5))
                                                {
                                                    keywords.Add(keyword.Replace("\"", "").Trim());
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }

                    Aam_UriManager aam_manag = new Aam_UriManager();
                    foreach (Predict_results inp in classification_results)
                    {
                        if (inp.db_match != null)
                        {
                            foreach (string s in inp.db_match)
                            {
                                List<int> path = new List<int>();
                                foreach (string el in s.Replace("[", "").Replace("]", "").Replace("'", "").Replace("\"", "").Replace(" ", "").Split(','))
                                {
                                    if (el.Contains("http"))
                                    {
                                        Aam_Uri aam_uri = aam_manag.get_Aam_Uri_by_uri(el);
                                        string label = (aam_uri != null) ? (aam_uri.label) : el;
                                        if (nodes.FindAll(x => x == label).Count == 0)
                                        {
                                            nodes.Add(label);
                                        }
                                        int index_in_list = nodes.FindIndex(x => x == label);
                                        path.Add(index_in_list);
                                    }
                                }
                                if (path.Count() > 0) paths.Add(path);
                            }
                        }
                        if (inp.onto_match != null)
                        {
                            foreach (string s in inp.onto_match)
                            {
                                List<int> path = new List<int>();
                                foreach (string el in s.Replace("[", "").Replace("]", "").Replace("'", "").Replace("\"", "").Replace(" ", "").Split(','))
                                {
                                    if (el.Contains("http"))
                                    {
                                        Aam_Uri aam_uri = aam_manag.get_Aam_Uri_by_uri(el);
                                        string label = (aam_uri != null) ? (aam_uri.label) : el;
                                        if (nodes.FindAll(x => x == label).Count == 0)
                                        {
                                            nodes.Add(label);
                                        }
                                        int index_in_list = nodes.FindIndex(x => x == label);
                                        path.Add(index_in_list);
                                    }
                                }
                                if (path.Count() > 0) paths.Add(path);
                            }
                        }
                    }
                    var json_ = new JavaScriptSerializer().Serialize(
                        new
                        {
                            nodes = nodes,
                            links = paths,
                            id=dataset,
                            keywords = keywords,
                            class_results = classification_results
                        }
                        );
                    return Json(json_, JsonRequestBehavior.AllowGet);

                    //ViewData["id"] = dataset;
                    //ViewData["label"] = "";
                    //ViewData["keywords"] = keywords;
                    //classification.keywords = keywords;
                    //classification.class_results = classification_results;
                }
            }
            catch (Exception e)
            {
                LoggerFactory.GetFileLogger().LogCustom(e.Message);
                LoggerFactory.GetFileLogger().LogCustom(e.StackTrace);
            }
            return Json(new JavaScriptSerializer().Serialize(new
                        {
                            nodes = "[]",
                            links = "[]",
                            id = dataset,
                            keywords = "[]",
                            class_results = "[]"
                        }
                        ), JsonRequestBehavior.AllowGet);
        }
        private double similarity(string a, string b)
        {
            List<double> similarities = new List<double>();
            double output = 0.0;

            var l = new NormalizedLevenshtein();
            similarities.Add(l.Similarity(a, b));
            var jw = new JaroWinkler();
            similarities.Add(jw.Similarity(a, b));
            var jac = new Jaccard();
            similarities.Add(jac.Similarity(a, b));

            foreach (double sim in similarities)
            {
                output += sim;
            }

            return output / similarities.Count;
        }
        #endregion

        #region categorical analysis
        public async System.Threading.Tasks.Task<ActionResult> CategoralAnalysisAsync(long id)
        {
            string result = "";
            using (var client = new HttpClient())
            {
                string username = this.ControllerContext.HttpContext.User.Identity.Name;
                Dictionary<string, string> dict_data = new Dictionary<string, string>();
                dict_data.Add("username", username);
                dict_data.Add("data", id.ToString());

                string url = BaseAdress + "/api/Summary/getCategrocialAnalysis";
                client.BaseAddress = new Uri(url);

                var json = JsonConvert.SerializeObject(dict_data, Newtonsoft.Json.Formatting.Indented);
                var stringContent = new StringContent(json);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var responseTask = await client.PostAsync(url, stringContent);
                result = await responseTask.Content.ReadAsStringAsync();
            }
            return PartialView("classify", JObject.Parse(result));
        }
        #endregion

        #region sampling summary
        public async System.Threading.Tasks.Task<ActionResult> Filter_ApplyAsync(
            string welllocation = "", string year = "", string filtersize = "", 
            string GroupName = "", string NameFIlter = "",
            String Season_dict = "", string column = "-1", string row = "-1", Boolean flag = false)
        {
            string row_ = "";
            dict_ = dict_.OrderByDescending(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            if (row != "-1")
                row_ = dict_.ElementAt(dict_.Count - Int32.Parse(row) - 1).Key;
            string col = "";
            if (column != "-1")
                col = dict_.ElementAt(dict_.Count - 1).Value[Int32.Parse(column) - 1];
            string param = "?year=" + year + "&filtersize=" + filtersize + "&GroupName=" + GroupName + "&Season_dict=" + Season_dict + "&column=" + col + "&row=" + row_;
            if (welllocation != "")
                param = param + "&welllocation=" + parse_Json_location(welllocation);
            if (NameFIlter != "")
                param = param + "&PIName=" + NameFIlter;

            string result = "";
            using (var client = new HttpClient())
            {
                string username = this.ControllerContext.HttpContext.User.Identity.Name;
                Dictionary<string, string> dict_data = new Dictionary<string, string>();
                dict_data.Add("username", username);
                dict_data.Add("data", param);

                string url = BaseAdress + "/api/Summary/getSamplingSummary";
                client.BaseAddress = new Uri(url);

                var json = JsonConvert.SerializeObject(dict_data, Newtonsoft.Json.Formatting.Indented);
                var stringContent = new StringContent(json);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var responseTask = await client.PostAsync(url, stringContent);
                result = await responseTask.Content.ReadAsStringAsync();
            }
            return PartialView("classify", JObject.Parse(result));
        }

        private String parse_Json_location(String location_coordinates)
        {
            String Gps_coordinates_for_wells = Path.Combine(AppConfiguration.GetModuleWorkspacePath("DDM"), "Interactive Search", "D03_well coordinates_20180525.json");
            //"LatLng(51.080258, 10.42626)"
            using (StreamReader r = new StreamReader(Gps_coordinates_for_wells))
            {
                string json = r.ReadToEnd();
                List<coordinates_GPS> items = JsonConvert.DeserializeObject<List<coordinates_GPS>>(json);
                if (location_coordinates.Length > 0)
                {
                    string lon = location_coordinates.Substring(location_coordinates.IndexOf('(') + 1, location_coordinates.IndexOf(',') - location_coordinates.IndexOf('(') - 1);
                    string lat = location_coordinates.Substring(location_coordinates.IndexOf(", ") + 2, location_coordinates.IndexOf(')') - location_coordinates.IndexOf(',') - 2);

                    foreach (coordinates_GPS item in items)
                    {
                        try
                        {
                            if ((item.Lat.ToString().IndexOf(lon.Substring(0, lon.Length - 1)) > -1) && (item.Lon.ToString().IndexOf(lat.Substring(0, lat.Length - 1)) > -1))
                            {
                                return item.Well_name;
                            }
                        }
                        catch (NullReferenceException e)
                        {
                            LoggerFactory.GetFileLogger().LogCustom(e.Message);
                            LoggerFactory.GetFileLogger().LogCustom(e.StackTrace);
                        }

                    }
                }
                else
                {
                    return json;
                }
            }
            return "";
        }
        #endregion
    }
}