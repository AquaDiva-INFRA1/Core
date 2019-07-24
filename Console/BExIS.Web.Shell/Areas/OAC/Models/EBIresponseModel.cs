using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using Newtonsoft.Json.Linq;

namespace BExIS.Modules.OAC.UI.Models
{
    public class EBIresponseModel
    {
        public string accession { get; set; }
        public string name { get; set; }
        public string releaseDate { get; set; }
        public string updateDate { get; set; }
        public string description { get; set; }
        public string _linksselfhref { get; set; }
        public string _linkssamplehref { get; set; }
        public string _linksrelationshref { get; set; }
        public List<string> geographicLocationDepth { get; set; }
        public List<string> organismtext { get; set; }
        public List<string> organismontologyTerms { get; set; }
        public List<string> environmentMaterial { get; set; }
        public List<string> insdcFirstPublic { get; set; }
        public List<string> enaChecklist { get; set; }
        public List<string> collectionDate { get; set; }
        public List<string> geographicLocationLongitude { get; set; }
        public List<string> titletext { get; set; }
        public List<string> geographicLocationLatitude { get; set; }
        public List<string> insdcLastUpdate { get; set; }
        public List<string> waterEnvironmentalPackage { get; set; }
        public List<String> waterEnvironmentalPackageontologyTerms { get; set; }
        public List<string> investigationType { get; set; }
        public List<string> synonym { get; set; }
        public List<string> insdcStatus { get; set; }
        public List<string> sequencingMethod { get; set; }
        public List<string> projectName { get; set; }
        public List<string> sraAccession { get; set; }
        public List<string> alias { get; set; }
        public List<string> environmentBiome { get; set; }
        public List<string> environmentFeature { get; set; }
        public List<string> insdcCenterName { get; set; }
        public List<string> geographicLocationCountryAndOrSea { get; set; }
        public List<string> externalReferences { get; set; }
        public List<string> externalReferencesacc { get; set; }
        public List<string> externalReferencesurl { get; set; }

        public EBIresponseModel() { }
        
        public EBIresponseModel(JObject json)
        {
            this.accession = json["accession"].ToString();
            this.name = json["name"].ToString();
            this.releaseDate = json["releaseDate"].ToString();
            this.updateDate = json["updateDate"].ToString();
            this.description = json["description"].ToString();

            this.geographicLocationDepth = new List<string>();
            foreach (var x in json["characteristics"]["geographicLocationDepth"])
            {
                geographicLocationDepth.Add(x["text"].ToString());
            }
            
            this.organismtext = new List<string>();
            this.organismontologyTerms = new List<string>();
            foreach (var x in json["characteristics"]["organism"])
            {
                organismtext.Add(x["text"].ToString());
                foreach (var y in x["ontologyTerms"])
                    organismontologyTerms.Add(y.ToString());
            }

            this.environmentMaterial = new List<string>();
            foreach (var x in json["characteristics"]["environmentMaterial"])
            {
                environmentMaterial.Add(x["text"].ToString());
            }


            this.insdcFirstPublic = new List<string>();
            foreach (var x in json["characteristics"]["insdcFirstPublic"])
            {
                insdcFirstPublic.Add(x["text"].ToString());
            }


            this.enaChecklist = new List<string>();
            foreach (var x in json["characteristics"]["enaChecklist"])
            {
                enaChecklist.Add(x["text"].ToString());
            }


            this.collectionDate = new List<string>();
            foreach (var x in json["characteristics"]["collectionDate"])
            {
                collectionDate.Add(x["text"].ToString());
            }

            this.geographicLocationLongitude = new List<string>();
            foreach (var x in json["characteristics"]["geographicLocationLongitude"])
            {
                geographicLocationLongitude.Add(x["text"].ToString());
            }

            this.titletext = new List<string>();
            foreach (var x in json["characteristics"]["title"])
            {
                titletext.Add(x["text"].ToString());
            }

            this.geographicLocationLatitude = new List<string>();
            foreach (var x in json["characteristics"]["geographicLocationLatitude"])
            {
                geographicLocationLatitude.Add(x["text"].ToString());
            }

            this.insdcLastUpdate = new List<string>();
            foreach (var x in json["characteristics"]["insdcLastUpdate"])
            {
                insdcLastUpdate.Add(x["text"].ToString());
            }


            this.waterEnvironmentalPackage = new List<string>();
            this.waterEnvironmentalPackageontologyTerms = new List<string>();
            foreach (var x in json["characteristics"]["waterEnvironmentalPackage"])
            {
                waterEnvironmentalPackage.Add(x["text"].ToString());
                foreach (var y in x["ontologyTerms"])
                    waterEnvironmentalPackageontologyTerms.Add(y.ToString());
            }


            this.investigationType = new List<string>();
            foreach (var x in json["characteristics"]["investigationType"])
            {
                investigationType.Add(x["text"].ToString());
            }


            this.synonym = new List<string>();
            foreach (var x in json["characteristics"]["synonym"])
            {
                synonym.Add(x["text"].ToString());
            }

            this.insdcStatus = new List<string>();
            foreach (var x in json["characteristics"]["insdcStatus"])
            {
                insdcStatus.Add(x["text"].ToString());
            }

            this.sequencingMethod = new List<string>();
            foreach (var x in json["characteristics"]["sequencingMethod"])
            {
                sequencingMethod.Add(x["text"].ToString());
            }

            this.projectName = new List<string>();
            foreach (var x in json["characteristics"]["projectName"])
            {
                projectName.Add(x["text"].ToString());
            }


            this.sraAccession = new List<string>();
            foreach (var x in json["characteristics"]["sraAccession"])
            {
                sraAccession.Add(x["text"].ToString());
            }

            this.alias = new List<string>();
            foreach (var x in json["characteristics"]["alias"])
            {
                alias.Add(x["text"].ToString());
            }


            this.environmentBiome = new List<string>();
            foreach (var x in json["characteristics"]["environmentBiome"])
            {
                environmentBiome.Add(x["text"].ToString());
            }

            this.environmentFeature = new List<string>();
            foreach (var x in json["characteristics"]["environmentFeature"])
            {
                environmentFeature.Add(x["text"].ToString());
            }

            this.insdcCenterName = new List<string>();
            foreach (var x in json["characteristics"]["insdcCenterName"])
            {
                insdcCenterName.Add(x["text"].ToString());
            }

            this.geographicLocationCountryAndOrSea = new List<string>();
            foreach (var x in json["characteristics"]["geographicLocationCountryAndOrSea"])
            {
                geographicLocationCountryAndOrSea.Add(x["text"].ToString());
            }

            this.externalReferences = new List<string>();
            this.externalReferencesacc = new List<string>();
            this.externalReferencesurl = new List<string>();
            foreach (var x in json["externalReferences"])
            {
                externalReferences.Add(x["name"].ToString());
                externalReferencesacc.Add(x["acc"].ToString());
                externalReferencesurl.Add(x["url"].ToString());
            }

            this._linksselfhref = json["_links"]["self"]["href"].ToString();
            this._linkssamplehref = json["_links"]["sample"]["href"].ToString();
            this._linksrelationshref = json["_links"]["relations"]["href"].ToString();
        }

        public XmlDocument ConvertToXML(string jsonString)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(JsonReaderWriterFactory.CreateJsonReader(
                Encoding.ASCII.GetBytes(jsonString), new XmlDictionaryReaderQuotas()));
            return xml;
        }

    }
}