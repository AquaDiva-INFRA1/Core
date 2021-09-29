using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vaiona.Logging;

namespace BEXIS.OAC.Entities
{

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public class EXTERNALID
    {
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
            //x = "accession,alias,center name,title,description,sample name - scientific name , sample name - taxon id, sample links,samples attributes,identifier - primary id,identifier - external id,identifier - external text,identifier - internal id,identifier - internal text";
            try
            {
                x = unpack(model.Accession)+ " ,"
                + unpack(model.Alias) + " ,"
                + unpack(model.CenterName) + " ,"
                + unpack(model.TITLE) + " ,"
                + unpack(model.DESCRIPTION?.ToString())+ " ,"
                + unpack(model.SAMPLENAME) + " ,"
                + unpack(model.SAMPLELINKS) + " ,"
                + unpack(model.SAMPLEATTRIBUTES) + " ,"
                + unpack(model.IDENTIFIERS)
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
            //string data_csv = "accession,alias,center name,title,description,sample name - scientific name , sample name - taxon id, sample link - IDs, sample link - DBs,samples attributes - tags, samples attributes - values, samples attributes - units,identifier - primary id,identifier - external id,identifier - external text,identifier - internal id,identifier - internal text";
            string data_csv = "accession,alias,center_name,IDENTIFIERS__PRIMARY_ID,IDENTIFIERS__|,IDENTIFIERS__|__namespace,IDENTIFIERS__|__#text," +
                "TITLE,SAMPLE_NAME__TAXON_ID,SAMPLE_NAME__SCIENTIFIC_NAME,DESCRIPTION,SAMPLE_LINKS__SAMPLE_LINK__XREF_LINK__DB," +
                "SAMPLE_LINKS__SAMPLE_LINK__XREF_LINK__ID__#cdata-section,SAMPLE_LINKS__SAMPLE_LINK__XREF_LINK__ID,SAMPLE_ATTRIBUTES__SAMPLE_ATTRIBUTE__TAG," +
                "SAMPLE_ATTRIBUTES__SAMPLE_ATTRIBUTE__VALUE,SAMPLE_ATTRIBUTES__SAMPLE_ATTRIBUTE__UNITS";

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

        #region unpack / deserialise  classes
        private string unpack(SAMPLEATTRIBUTES t)
        {
            string tag = "";
            string value = "";
            string unit = "";
            //header = "samples attributes - tags, samples attributes - values, samples attributes - units"
            foreach (SAMPLEATTRIBUTE att in t.SAMPLEATTRIBUTE)
            {
                tag = tag + (att.TAG?.Replace(',', ' ').Replace('-', ' ') ?? " ");
                unit = unit + (att.UNITS?.Replace(',', ' ').Replace('-', ' ') ?? " ") ;
                value = value + (att.VALUE?.Replace(',', ' ').Replace('-', ' ') ?? " ") ;
            }
            tag = tag.TrimEnd('-');
            unit = unit.TrimEnd('-');
            value = value.TrimEnd('-');
            string x = tag + "," + value + "," + unit;
            return x;
        }

        private string unpack(SAMPLELINKS t)
        {
            string IDs = "";
            string DBs = "";
            string x = "";
            //header = 'sample link - IDs, sample link - DBs';
            foreach (SAMPLELINK link in t.SAMPLELINK)
            {
                DBs = DBs + (link.XREFLINK.DB?.Replace(',', ' ').Replace('-', ' ') ?? " ") ;
                IDs = IDs + (link.XREFLINK.ID?.ToString().Replace(',', ' ').Replace('-', ' ') ?? " ");
            }
            IDs = IDs.TrimEnd('-');
            DBs = DBs.TrimEnd('-');
            x = DBs + "," + IDs;
            return x;
        }

        private string unpack(IDENTIFIERS t)
        {
            string x = "";
            //header = "identifier - primary id,identifier - external id,identifier - external text,identifier - internal id,identifier - internal text"
            x = (t.PRIMARYID?.Replace(',', ' ') ?? " ") + " ,"
                + (t.EXTERNALID.Namespace?.Replace(',', ' ') ?? " ") + " ,"
                + (t.EXTERNALID.Text?.Replace(',', ' ') ?? " ") + " ,"
                + (t.SUBMITTERID.Namespace?.Replace(',', ' ') ?? " ") + " ,"
                + (t.SUBMITTERID.Text?.Replace(',', ' ') ?? " ")
                ;
            return x;
        }

        private string unpack(SAMPLENAME t)
        {
            string x = "";
            //header = "sample name - scientific name, sample name - taxon id"
            x = (t.SCIENTIFICNAME?.Replace(',', ' ') ?? " ") + " ,"
                + (t.TAXONID?.Replace(',', ' ') ?? " ")
                ;
            return x;
        }

        private string unpack(string prop)
        {
            //header = "accession,alias,center name,title,description"
            string x = prop?.Replace(',', ' ') ?? " ";
            return x;
        }

        #endregion
    }
}

