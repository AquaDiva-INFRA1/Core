/*

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Reflection;

namespace BEXIS.OAC.Entities
{
    public class AccessionMetadata
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

        public AccessionMetadata() { }

        public AccessionMetadata(JObject json)
        {
            this.accession = json["accession"] == null ? "" : json["accession"].ToString();
            this.name = json["name"] == null ? "" : json["name"].ToString();
            this.releaseDate = json["releaseDate"] == null ? "" : json["releaseDate"].ToString();
            this.updateDate = json["updateDate"] == null ? "" : json["updateDate"].ToString();
            this.description = json["DESCRIPTION"] == null ? "" : json["DESCRIPTION"].ToString() ;

            this.geographicLocationDepth = new List<string>();
            if (json["characteristics"]["geographicLocationDepth"] != null)
                foreach (var x in json["characteristics"]["geographicLocationDepth"])
                {
                    geographicLocationDepth.Add(x["text"].ToString().Replace(",","."));
                }

            this.organismtext = new List<string>();
            this.organismontologyTerms = new List<string>();
            if (json["characteristics"]["organism"] != null)
                foreach (var x in json["characteristics"]["organism"])
                {
                    organismtext.Add(x["text"].ToString());
                    foreach (var y in x["ontologyTerms"])
                        organismontologyTerms.Add(y.ToString());
                }

            this.environmentMaterial = new List<string>();
            if (json["characteristics"]["environmentMaterial"] != null)
                foreach (var x in json["characteristics"]["environmentMaterial"])
                {
                    environmentMaterial.Add(x["text"].ToString());
                }


            this.insdcFirstPublic = new List<string>();
            if (json["characteristics"]["environmentMaterial"] != null)
                foreach (var x in json["characteristics"]["insdcFirstPublic"])
                {
                    insdcFirstPublic.Add(x["text"].ToString());
                }


            this.enaChecklist = new List<string>();
            if (json["characteristics"]["enaChecklist"] != null)
                foreach (var x in json["characteristics"]["enaChecklist"])
                {
                    enaChecklist.Add(x["text"].ToString());
                }


            this.collectionDate = new List<string>();
            if (json["characteristics"]["collectionDate"] != null)
                foreach (var x in json["characteristics"]["collectionDate"])
                {
                    collectionDate.Add(x["text"].ToString());
                }

            this.geographicLocationLongitude = new List<string>();
            if (json["characteristics"]["geographicLocationLongitude"] != null)
                foreach (var x in json["characteristics"]["geographicLocationLongitude"])
                {
                    geographicLocationLongitude.Add(x["text"].ToString().Replace(",","."));
                }

            this.titletext = new List<string>();
            if (json["characteristics"]["title"] != null)
                foreach (var x in json["characteristics"]["title"])
                {
                    titletext.Add(x["text"].ToString());
                }

            this.geographicLocationLatitude = new List<string>();
            if (json["characteristics"]["geographicLocationLatitude"] != null)
                foreach (var x in json["characteristics"]["geographicLocationLatitude"])
                {
                    geographicLocationLatitude.Add(x["text"].ToString().Replace(",","."));
                }

            this.insdcLastUpdate = new List<string>();
            if (json["characteristics"]["insdcLastUpdate"] != null)
                foreach (var x in json["characteristics"]["insdcLastUpdate"])
                {
                    insdcLastUpdate.Add(x["text"].ToString());
                }


            this.waterEnvironmentalPackage = new List<string>();
            this.waterEnvironmentalPackageontologyTerms = new List<string>();
            if (json["characteristics"]["waterEnvironmentalPackage"] != null)
                foreach (var x in json["characteristics"]["waterEnvironmentalPackage"])
                {
                    waterEnvironmentalPackage.Add(x["text"].ToString());
                    foreach (var y in x["ontologyTerms"])
                        waterEnvironmentalPackageontologyTerms.Add(y.ToString());
                }


            this.investigationType = new List<string>();
            if (json["characteristics"]["investigationType"] != null)
                foreach (var x in json["characteristics"]["investigationType"])
                {
                    investigationType.Add(x["text"].ToString());
                }


            this.synonym = new List<string>();
            if (json["characteristics"]["synonym"] != null)
                foreach (var x in json["characteristics"]["synonym"])
                {
                    synonym.Add(x["text"].ToString());
                }

            this.insdcStatus = new List<string>();
            if (json["characteristics"]["insdcStatus"] != null)
                foreach (var x in json["characteristics"]["insdcStatus"])
                {
                    insdcStatus.Add(x["text"].ToString());
                }

            this.sequencingMethod = new List<string>();
            if (json["characteristics"]["sequencingMethod"] != null)
                foreach (var x in json["characteristics"]["sequencingMethod"])
                {
                    sequencingMethod.Add(x["text"].ToString());
                }

            this.projectName = new List<string>();
            if (json["characteristics"]["projectName"] != null)
                foreach (var x in json["characteristics"]["projectName"])
                {
                    projectName.Add(x["text"].ToString());
                }


            this.sraAccession = new List<string>();
            if (json["characteristics"]["sraAccession"] != null)
                foreach (var x in json["characteristics"]["sraAccession"])
                {
                    sraAccession.Add(x["text"].ToString());
                }

            this.alias = new List<string>();
            if (json["characteristics"]["alias"] != null)
                foreach (var x in json["characteristics"]["alias"])
                {
                    alias.Add(x["text"].ToString());
                }


            this.environmentBiome = new List<string>();
            if (json["characteristics"]["environmentBiome"] != null)
                foreach (var x in json["characteristics"]["environmentBiome"])
                {
                    environmentBiome.Add(x["text"].ToString());
                }

            this.environmentFeature = new List<string>();
            if (json["characteristics"]["environmentFeature"] != null)
                foreach (var x in json["characteristics"]["environmentFeature"])
                {
                    environmentFeature.Add(x["text"].ToString());
                }

            this.insdcCenterName = new List<string>();
            if (json["characteristics"]["insdcCenterName"] != null)
                foreach (var x in json["characteristics"]["insdcCenterName"])
                {
                    insdcCenterName.Add(x["text"].ToString());
                }

            this.geographicLocationCountryAndOrSea = new List<string>();
            if (json["characteristics"]["geographicLocationCountryAndOrSea"] != null)
                foreach (var x in json["characteristics"]["geographicLocationCountryAndOrSea"])
                {
                    geographicLocationCountryAndOrSea.Add(x["text"].ToString());
                }

            this.externalReferences = new List<string>();
            this.externalReferencesacc = new List<string>();
            this.externalReferencesurl = new List<string>();
            if (json["externalReferences"] != null)
                foreach (var x in json["externalReferences"])
                {
                    externalReferences.Add(x["name"].ToString());
                    externalReferencesacc.Add(x["acc"].ToString());
                    externalReferencesurl.Add(x["url"].ToString());
                }

            this._linksselfhref = json["_links"]["self"]["href"] == null ? "" : json["_links"]["self"]["href"].ToString();
            this._linkssamplehref = json["_links"]["sample"]["href"] == null ? "" : json["_links"]["sample"]["href"].ToString();
            this._linksrelationshref = json["_links"]["relations"]["href"] == null ? "" : json["_links"]["relations"]["href"].ToString();
        }

        public XmlDocument ConvertToXML(string jsonString)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(JsonReaderWriterFactory.CreateJsonReader(
                Encoding.ASCII.GetBytes(jsonString), new XmlDictionaryReaderQuotas()));
            return xml;
        }

        public string ConvertTocsv(AccessionMetadata model, string tempfile)
        {
            string x = "";
            //x = "accession,name,releaseDate,updateDate,description,_linksselfhref,_linkssamplehref,_linksrelationshref,geographicLocationDepth,organismtext,organismontologyTerms,environmentMaterial,insdcFirstPublic,enaChecklist,collectionDate,geographicLocationLongitude,titletext,geographicLocationLatitude,insdcLastUpdate,waterEnvironmentalPackage,waterEnvironmentalPackageontologyTerms,investigationType,synonym,insdcStatus,sequencingMethod,projectName,sraAccession,alias,environmentBiome,environmentFeature,insdcCenterName,geographicLocationCountryAndOrSea,externalReferences,externalReferencesacc,externalReferencesurl";
            //sw.WriteLine(x);

            x = model.accession.Replace(',', ' ').Replace(',', ' ') + " ," + model.name.Replace(',', ' ').Replace(',', ' ') + " ," + model.releaseDate.Replace(',', ' ').Replace(',', ' ') + " ," + model.updateDate.Replace(',', ' ').Replace(',', ' ') + " ," + model.description.Replace(',', ' ').Replace(',', ' ') + " ," + model._linksselfhref.Replace(',', ' ').Replace(',', ' ') + " ," + model._linkssamplehref.Replace(',', ' ').Replace(',', ' ') + ","
                + model._linksrelationshref.Replace(',', ' ').Replace(',', ' ') + " ," + string.Join("-", model.geographicLocationDepth).Replace(',', ' ').Replace(',', ' ') + " ," + string.Join("-", model.organismtext).Replace(',', ' ').Replace(',', ' ') + " ," + string.Join("-", model.organismontologyTerms).Replace(',', ' ') + ","
                + string.Join("-", model.environmentMaterial).Replace(',', ' ') + " ," + string.Join("-", model.insdcFirstPublic).Replace(',', ' ') + ","
                + string.Join("-", model.enaChecklist).Replace(',', ' ') + " ," + string.Join("-", model.collectionDate).Replace(',', ' ') + " ," + string.Join("-", model.geographicLocationLongitude).Replace(',', ' ') + " ," + string.Join("-", model.titletext).Replace(',', ' ') + ","
                + string.Join("-", model.geographicLocationLatitude).Replace(',', ' ') + " ," + string.Join("-", model.insdcLastUpdate).Replace(',', ' ') + " ," + string.Join("-", model.waterEnvironmentalPackage).Replace(',', ' ') + " ," + string.Join("-", model.waterEnvironmentalPackageontologyTerms).Replace(',', ' ') + ","
                + string.Join("-", model.investigationType).Replace(',', ' ') + " ," + string.Join("-", model.synonym).Replace(',', ' ') + " ," + string.Join("-", model.insdcStatus).Replace(',', ' ') + " ," + string.Join("-", model.sequencingMethod).Replace(',', ' ') + ","
                + string.Join("-", model.projectName).Replace(',', ' ') + " ," + string.Join("-", model.sraAccession).Replace(',', ' ') + " ," + string.Join("-", model.alias).Replace(',', ' ') + ","
                + string.Join("-", model.environmentBiome).Replace(',', ' ') + " ," + string.Join("-", model.environmentFeature).Replace(',', ' ') + " ," + string.Join("-", model.insdcCenterName).Replace(',', ' ') + ","
                + string.Join("-", model.geographicLocationCountryAndOrSea).Replace(',', ' ') + " ," + string.Join("-", model.externalReferences).Replace(',', ' ') + " ," + string.Join("-", model.externalReferencesacc).Replace(',', ' ') + "," + string.Join("-", model.externalReferencesurl)
                ;

            if (tempfile != "")
            {
                if (!File.Exists(tempfile))
                {
                    StreamWriter sw = File.CreateText(tempfile);
                    sw.Close();
                }

                using (StreamWriter sw = File.AppendText(tempfile))
                {
                    sw.WriteLine(x);
                    sw.Close();
                }
            }
            return x;
        }

        public string ConvertTocsv(AccessionMetadata model)
        {
            string x = "";
            //x = "accession,name,releaseDate,updateDate,description,_linksselfhref,_linkssamplehref,_linksrelationshref,geographicLocationDepth,organismtext,organismontologyTerms,environmentMaterial,insdcFirstPublic,enaChecklist,collectionDate,geographicLocationLongitude,titletext,geographicLocationLatitude,insdcLastUpdate,waterEnvironmentalPackage,waterEnvironmentalPackageontologyTerms,investigationType,synonym,insdcStatus,sequencingMethod,projectName,sraAccession,alias,environmentBiome,environmentFeature,insdcCenterName,geographicLocationCountryAndOrSea,externalReferences,externalReferencesacc,externalReferencesurl";
            //sw.WriteLine(x);

            x = model.accession.Replace(',', ' ').Replace(',', ' ') + " ," + model.name.Replace(',', ' ').Replace(',', ' ') + " ," + model.releaseDate.Replace(',', ' ').Replace(',', ' ') + " ," + model.updateDate.Replace(',', ' ').Replace(',', ' ') + " ," + model.description.Replace(',', ' ').Replace(',', ' ') + " ," + model._linksselfhref.Replace(',', ' ').Replace(',', ' ') + " ," + model._linkssamplehref.Replace(',', ' ').Replace(',', ' ') + ","
                + model._linksrelationshref.Replace(',', ' ').Replace(',', ' ') + " ," + string.Join("-", model.geographicLocationDepth).Replace(',', ' ').Replace(',', ' ') + " ," + string.Join("-", model.organismtext).Replace(',', ' ').Replace(',', ' ') + " ," + string.Join("-", model.organismontologyTerms).Replace(',', ' ') + ","
                + string.Join("-", model.environmentMaterial).Replace(',', ' ') + " ," + string.Join("-", model.insdcFirstPublic).Replace(',', ' ') + ","
                + string.Join("-", model.enaChecklist).Replace(',', ' ') + " ," + string.Join("-", model.collectionDate).Replace(',', ' ') + " ," + string.Join("-", model.geographicLocationLongitude).Replace(',', ' ') + " ," + string.Join("-", model.titletext).Replace(',', ' ') + ","
                + string.Join("-", model.geographicLocationLatitude).Replace(',', ' ') + " ," + string.Join("-", model.insdcLastUpdate).Replace(',', ' ') + " ," + string.Join("-", model.waterEnvironmentalPackage).Replace(',', ' ') + " ," + string.Join("-", model.waterEnvironmentalPackageontologyTerms).Replace(',', ' ') + ","
                + string.Join("-", model.investigationType).Replace(',', ' ') + " ," + string.Join("-", model.synonym).Replace(',', ' ') + " ," + string.Join("-", model.insdcStatus).Replace(',', ' ') + " ," + string.Join("-", model.sequencingMethod).Replace(',', ' ') + ","
                + string.Join("-", model.projectName).Replace(',', ' ') + " ," + string.Join("-", model.sraAccession).Replace(',', ' ') + " ," + string.Join("-", model.alias).Replace(',', ' ') + ","
                + string.Join("-", model.environmentBiome).Replace(',', ' ') + " ," + string.Join("-", model.environmentFeature).Replace(',', ' ') + " ," + string.Join("-", model.insdcCenterName).Replace(',', ' ') + ","
                + string.Join("-", model.geographicLocationCountryAndOrSea).Replace(',', ' ') + " ," + string.Join("-", model.externalReferences).Replace(',', ' ') + " ," + string.Join("-", model.externalReferencesacc).Replace(',', ' ') + "," + string.Join("-", model.externalReferencesurl)
                ;

            return x;
        }

        public string Initialise_header(string tempfile)
        {
            string data_csv = "accession,name,releaseDate,updateDate,description,_linksselfhref,_linkssamplehref," +
                "_linksrelationshref,geographicLocationDepth,organismtext,organismontologyTerms,environmentMaterial," +
                "insdcFirstPublic,enaChecklist,collectionDate,geographicLocationLongitude,titletext,geographicLocationLatitude," +
                "insdcLastUpdate,waterEnvironmentalPackage,waterEnvironmentalPackageontologyTerms,investigationType,synonym," +
                "insdcStatus,sequencingMethod,projectName,sraAccession,alias,environmentBiome,environmentFeature" +
                ",insdcCenterName,geographicLocationCountryAndOrSea,externalReferences," +
                "externalReferencesacc,externalReferencesurl";
            
            if (tempfile != "")
            {
                if (!File.Exists(tempfile))
                {
                    StreamWriter sw = File.CreateText(tempfile);
                    sw.Close();
                }
                    
                
                using (StreamWriter sw = File.AppendText(tempfile))
                {
                    sw.WriteLine(data_csv);sw.Close();
                }
            }
            
            return data_csv;
        }

        public string Initialise_header()
        {
            string data_csv = "accession,name,releaseDate,updateDate,description,_linksselfhref,_linkssamplehref," +
                "_linksrelationshref,geographicLocationDepth,organismtext,organismontologyTerms,environmentMaterial," +
                "insdcFirstPublic,enaChecklist,collectionDate,geographicLocationLongitude,titletext,geographicLocationLatitude," +
                "insdcLastUpdate,waterEnvironmentalPackage,waterEnvironmentalPackageontologyTerms,investigationType,synonym," +
                "insdcStatus,sequencingMethod,projectName,sraAccession,alias,environmentBiome,environmentFeature" +
                ",insdcCenterName,geographicLocationCountryAndOrSea,externalReferences," +
                "externalReferencesacc,externalReferencesurl";
            
            return data_csv;
        }

    }
}

*/