using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vaiona.Logging;

namespace BEXIS.OAC.Entities
{

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class EXTERNALID
    {
        [JsonConstructor]
        public EXTERNALID(
            [JsonProperty("@namespace")] string @namespace,
            [JsonProperty("#text")] string text
        )
        {
            this.Namespace = @namespace;
            this.Text = text;
        }

        [JsonProperty("@namespace")]
        public readonly string Namespace;

        [JsonProperty("#text")]
        public readonly string Text;
    }

    public class SUBMITTERID
    {
        [JsonConstructor]
        public SUBMITTERID(
            [JsonProperty("@namespace")] string @namespace,
            [JsonProperty("#text")] string text
        )
        {
            this.Namespace = @namespace;
            this.Text = text;
        }

        [JsonProperty("@namespace")]
        public readonly string Namespace;

        [JsonProperty("#text")]
        public readonly string Text;
    }

    public class IDENTIFIERS
    {
        [JsonConstructor]
        public IDENTIFIERS(
            [JsonProperty("PRIMARY_ID")] string pRIMARYID,
            [JsonProperty("EXTERNAL_ID")] EXTERNALID eXTERNALID,
            [JsonProperty("SUBMITTER_ID")] SUBMITTERID sUBMITTERID
        )
        {
            this.PRIMARYID = pRIMARYID;
            this.EXTERNALID = eXTERNALID;
            this.SUBMITTERID = sUBMITTERID;
        }

        [JsonProperty("PRIMARY_ID")]
        public readonly string PRIMARYID;

        [JsonProperty("EXTERNAL_ID")]
        public readonly EXTERNALID EXTERNALID;

        [JsonProperty("SUBMITTER_ID")]
        public readonly SUBMITTERID SUBMITTERID;
    }

    public class SAMPLENAME
    {
        [JsonConstructor]
        public SAMPLENAME(
            [JsonProperty("TAXON_ID")] string tAXONID,
            [JsonProperty("SCIENTIFIC_NAME")] string sCIENTIFICNAME
        )
        {
            this.TAXONID = tAXONID;
            this.SCIENTIFICNAME = sCIENTIFICNAME;
        }

        [JsonProperty("TAXON_ID")]
        public readonly string TAXONID;

        [JsonProperty("SCIENTIFIC_NAME")]
        public readonly string SCIENTIFICNAME;
    }

    public class XREFLINK
    {
        [JsonConstructor]
        public XREFLINK(
            [JsonProperty("DB")] string db,
            [JsonProperty("ID")] object id
        )
        {
            this.DB = db;
            this.ID = id;
        }

        [JsonProperty("DB")]
        public readonly string DB;

        [JsonProperty("ID")]
        public readonly object ID;
    }

    public class SAMPLELINK
    {
        [JsonConstructor]
        public SAMPLELINK(
            [JsonProperty("XREF_LINK")] XREFLINK xREFLINK
        )
        {
            this.XREFLINK = xREFLINK;
        }

        [JsonProperty("XREF_LINK")]
        public readonly XREFLINK XREFLINK;
    }

    public class SAMPLELINKS
    {
        [JsonConstructor]
        public SAMPLELINKS(
            [JsonProperty("SAMPLE_LINK")] List<SAMPLELINK> sAMPLELINK
        )
        {
            this.SAMPLELINK = sAMPLELINK;
        }

        [JsonProperty("SAMPLE_LINK")]
        public readonly List<SAMPLELINK> SAMPLELINK;
    }

    public class SAMPLEATTRIBUTE
    {
        [JsonConstructor]
        public SAMPLEATTRIBUTE(
            [JsonProperty("TAG")] string tAG,
            [JsonProperty("VALUE")] string vALUE,
            [JsonProperty("UNITS")] string uNITS
        )
        {
            this.TAG = tAG;
            this.VALUE = vALUE;
            this.UNITS = uNITS;
        }

        [JsonProperty("TAG")]
        public readonly string TAG;

        [JsonProperty("VALUE")]
        public readonly string VALUE;

        [JsonProperty("UNITS")]
        public readonly string UNITS;
    }

    public class SAMPLEATTRIBUTES
    {
        [JsonConstructor]
        public SAMPLEATTRIBUTES(
            [JsonProperty("SAMPLE_ATTRIBUTE")] List<SAMPLEATTRIBUTE> sAMPLEATTRIBUTE
        )
        {
            this.SAMPLEATTRIBUTE = sAMPLEATTRIBUTE;
        }

        [JsonProperty("SAMPLE_ATTRIBUTE")]
        public readonly List<SAMPLEATTRIBUTE> SAMPLEATTRIBUTE;
    }

    public class AccessionMetadataV2
    {
        [JsonConstructor]
        public AccessionMetadataV2(
            [JsonProperty("@accession")] string accession,
            [JsonProperty("@alias")] string alias,
            [JsonProperty("@center_name")] string centerName,
            [JsonProperty("IDENTIFIERS")] IDENTIFIERS iDENTIFIERS,
            [JsonProperty("TITLE")] string tITLE,
            [JsonProperty("SAMPLE_NAME")] SAMPLENAME sAMPLENAME,
            [JsonProperty("DESCRIPTION")] string dESCRIPTION,
            [JsonProperty("SAMPLE_LINKS")] SAMPLELINKS sAMPLELINKS,
            [JsonProperty("SAMPLE_ATTRIBUTES")] SAMPLEATTRIBUTES sAMPLEATTRIBUTES
        )
        {
            this.Accession = accession;
            this.Alias = alias;
            this.CenterName = centerName;
            this.IDENTIFIERS = iDENTIFIERS;
            this.TITLE = tITLE;
            this.SAMPLENAME = sAMPLENAME;
            this.DESCRIPTION = dESCRIPTION;
            this.SAMPLELINKS = sAMPLELINKS;
            this.SAMPLEATTRIBUTES = sAMPLEATTRIBUTES;
        }
        public AccessionMetadataV2() { }

        [JsonProperty("@accession")]
        public readonly string Accession;

        [JsonProperty("@alias")]
        public readonly string Alias;

        [JsonProperty("@center_name")]
        public readonly string CenterName;

        [JsonProperty("IDENTIFIERS")]
        public readonly IDENTIFIERS IDENTIFIERS;

        [JsonProperty("TITLE")]
        public readonly string TITLE;

        [JsonProperty("SAMPLE_NAME")]
        public readonly SAMPLENAME SAMPLENAME;

        [JsonProperty("DESCRIPTION")]
        public readonly string DESCRIPTION;

        [JsonProperty("SAMPLE_LINKS")]
        public readonly SAMPLELINKS SAMPLELINKS;

        [JsonProperty("SAMPLE_ATTRIBUTES")]
        public readonly SAMPLEATTRIBUTES SAMPLEATTRIBUTES;



        public string convertToCSV(AccessionMetadataV2 model, string tempfile)
        {

            string x = "";
            //x = "accession,name,releaseDate,updateDate,description,_linksselfhref,_linkssamplehref,_linksrelationshref,geographicLocationDepth,organismtext,organismontologyTerms,environmentMaterial,insdcFirstPublic,enaChecklist,collectionDate,geographicLocationLongitude,titletext,geographicLocationLatitude,insdcLastUpdate,waterEnvironmentalPackage,waterEnvironmentalPackageontologyTerms,investigationType,synonym,insdcStatus,sequencingMethod,projectName,sraAccession,alias,environmentBiome,environmentFeature,insdcCenterName,geographicLocationCountryAndOrSea,externalReferences,externalReferencesacc,externalReferencesurl";
            //sw.WriteLine(x);
            try
            {
                x = model.Accession.Replace(',', ' ').Replace(',', ' ') ?? " " + " ,"
                + model.Alias.Replace(',', ' ').Replace(',', ' ')?? " " + " ,"
                + model.CenterName.Replace(',', ' ').Replace(',', ' ') ?? " " + " ,"
                + model.TITLE.Replace(',', ' ').Replace(',', ' ') ?? " " + " ,"
                + model.DESCRIPTION.Replace(',', ' ').Replace(',', ' ') ?? " " + " ,"
                + model.SAMPLENAME.SCIENTIFICNAME.Replace(',', ' ').Replace(',', ' ') ?? " " + " ,"
                + model.SAMPLENAME.TAXONID.Replace(',', ' ').Replace(',', ' ') ?? " " + " ,"
                + string.Join("-", model.SAMPLELINKS.SAMPLELINK).Replace(',', ' ') ?? " " + " ,"
                + string.Join("-", model.SAMPLEATTRIBUTES.SAMPLEATTRIBUTE).Replace(',', ' ') ?? " "
                ;
            }
            catch (Exception e) {
                LoggerFactory.GetFileLogger().LogCustom(e.Message);
                LoggerFactory.GetFileLogger().LogCustom(e.StackTrace);
            }
            
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

        public string Initialise_header(string tempfile)
        {
            string data_csv = "accession,alias,center name,title,description,sample name - scientific name , sample name - taxon id, sample links,samples attributes";

            if (tempfile != "")
            {
                if (!File.Exists(tempfile))
                {
                    StreamWriter sw = File.CreateText(tempfile);
                    sw.Close();
                }


                using (StreamWriter sw = File.AppendText(tempfile))
                {
                    sw.WriteLine(data_csv); sw.Close();
                }
            }

            return data_csv;
        }
    }
}

