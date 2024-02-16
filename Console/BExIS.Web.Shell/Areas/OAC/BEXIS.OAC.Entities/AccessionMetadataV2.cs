using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
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

        [JsonProperty("TITLE")]
        public readonly string TITLE;

        [JsonProperty("DESCRIPTION")]
        public readonly string DESCRIPTION;

        [JsonProperty("IDENTIFIERS")]
        public readonly IDENTIFIERS IDENTIFIERS;

        [JsonProperty("SAMPLE_NAME")]
        public readonly SAMPLENAME SAMPLENAME;

        [JsonProperty("SAMPLE_LINKS")]
        public readonly SAMPLELINKS SAMPLELINKS;

        [JsonProperty("SAMPLE_ATTRIBUTES")]
        public readonly SAMPLEATTRIBUTES SAMPLEATTRIBUTES;

        public string convertToCSV(AccessionMetadataV2 model, string tempfile, bool save_output = true)
        {

            string x = "";
            //x = "accession,alias,center name,title,description,sample name - scientific name , sample name - taxon id, sample links,samples attributes,identifier - primary id,identifier - external id,identifier - external text,identifier - internal id,identifier - internal text";
            try
            {
                x = Unpack(model.Accession)+ " ,"      //returns: 1 attibute
                + Unpack(model.Alias) + " ,"                    //1
                + Unpack(model.CenterName) + " ,"               //1
                + Unpack(model.IDENTIFIERS) + " ,"              //3
                + Unpack(model.TITLE) + " ,"                    //1
                + Unpack(model.SAMPLENAME) + " ,"               //2
                + Unpack(model.DESCRIPTION?.ToString())+ " ,"   //1
                + Unpack(model.SAMPLELINKS) + " ,"              //3
                + Unpack(model.SAMPLEATTRIBUTES)                //3
                ;
            }
            catch (Exception e) {
                LoggerFactory.GetFileLogger().LogCustom(e.Message);
                LoggerFactory.GetFileLogger().LogCustom(e.StackTrace);
            }
            if (!save_output) return x;

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

        public string Initialise_header(string tempfile, bool save_output = false)
        {

            string data_csv = "Accession,Alias,Center Name, Primary ID (IDENTIFIERS),Namespace (IDENTIFIERS),Text (IDENTIFIERS)," +
                "Title,Taxon ID (SAMPLE_NAME),Scientific Name (SAMPLE_NAME),Description,DB (XREF_LINK)," +
                "cData (XREF_LINK),ID (XREF_LINK),Tag(SAMPLE_ATTRIBUTE)," +
                "Value(SAMPLE_ATTRIBUTE),Units(SAMPLE_ATTRIBUTE)";
            
            if (!save_output) return data_csv;

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
        private string Unpack(SAMPLEATTRIBUTES t)
        {
            string tag = "";
            string value = "";
            string unit = "";
            //header = "samples attributes - tags, samples attributes - values, samples attributes - units"
            foreach (SAMPLEATTRIBUTE att in t.SAMPLEATTRIBUTE)
            {
                tag = tag + (att.TAG?.Replace(',', ' ').Replace(';', ' ') ?? " ") + "; ";
                unit = unit + (att.UNITS?.Replace(',', ' ').Replace(';', ' ') ?? " ") + "; ";
                value = value + (att.VALUE?.Replace(',', ' ').Replace(';', ' ') ?? " ") + "; ";
            }
            tag = tag.TrimEnd(' ').TrimEnd(';');
            unit = unit.TrimEnd(' ').TrimEnd(';');
            value = value.TrimEnd(' ').TrimEnd(';');
            string x = tag + "," + value + "," + unit;
            return x;
        }

        private string Unpack(SAMPLELINKS t)
        {
            string IDs = "";
            string DBs = "";
            string cData = "";
            string temp_ID, temp_cData = "";
            //header = 'sample link - DBs, sample link - cdata, sample link - IDs ';
            foreach (SAMPLELINK link in t.SAMPLELINK)
            {
                DBs = DBs + (link.XREFLINK.DB?.Replace(',', ' ').Replace(';', ' ') ?? " ") + "; ";
                (temp_ID, temp_cData) = ParseIDs(link.XREFLINK.ID?.ToString() ?? " ");
                IDs = IDs + (temp_ID?.Replace(',', ' ') ?? " ");
                cData = cData + (temp_cData?.Replace(',', ' ') ?? " ");
            }
            IDs = IDs.TrimEnd(' ').TrimEnd(';');
            DBs = DBs.TrimEnd(' ').TrimEnd(';');
            cData = cData.TrimEnd(' ').TrimEnd(';');
            return DBs + "," + cData + "," + IDs;
        }

        private (string,string) ParseIDs(string json)
        {
            try
            {
                var jObject = new JObject();
                jObject = JObject.Parse(json);
                string target = "";
                foreach (JProperty x in (JToken)jObject)
                {
                    string name = x.Name;
                    string value = x.Value.ToString();
                    target = target + value + ";";
                }
                target = target.TrimEnd(';').Replace(';',' ');
                return ("", target + "; ");
            }
            catch
            {
                return (json.Replace(';', ' ') + "; ", "");
            }
        }


        private string Unpack(IDENTIFIERS t)
        {
            string x = "";
            //header = "identifier - primary id,identifier - namespace,identifier - text"
            x = (t.PRIMARYID?.Replace(',', ' ') ?? " ") + " ,"
                + (t.EXTERNALID.Namespace?.Replace(',', ' ') ?? " ") + "; "
                + (t.SUBMITTERID.Namespace?.Replace(',', ' ') ?? " ") + " ,"
                + (t.EXTERNALID.Text?.Replace(',', ' ') ?? " ") + "; "
                + (t.SUBMITTERID.Text?.Replace(',', ' ') ?? " ")
                ;
            return x;
        }

        private string Unpack(SAMPLENAME t)
        {
            string x = "";
            //header = "sample name - taxon id, sample name - scientific name"   
            x = (t.TAXONID?.Replace(',', ' ') ?? " ") + " ,"
                + (t.SCIENTIFICNAME?.Replace(',', ' ') ?? " ")
                ;
            return x;
        }

        private string Unpack(string prop)
        {
            //header = "accession,alias,center name,title,description"
            string x = prop?.Replace(',', ' ') ?? " ";
            return x;
        }

        #endregion
    }
}

