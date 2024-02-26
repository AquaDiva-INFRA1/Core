using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using ChoETL ;

namespace BEXIS.OAC.Entities
{
    public class SequencedataNCBI
    {
        public SequencedataNCBI(
            string accession,
            string alias,
            string center_name,
            IDENTIFIERS IDENTIFIERS,
            string NAME,
            string TITLE,
            string DESCRIPTION,
            SUBMISSIONPROJECT SUBMISSION_PROJECT,
            PROJECTLINKS PROJECT_LINKS,
            PROJECTATTRIBUTES PROJECT_ATTRIBUTES
        )
        {
            this.accession = accession;
            this.alias = alias;
            this.center_name = center_name;
            this.IDENTIFIERS = IDENTIFIERS;
            this.NAME = NAME;
            this.TITLE = TITLE;
            this.DESCRIPTION = DESCRIPTION;
            this.SUBMISSION_PROJECT = SUBMISSION_PROJECT;
            this.PROJECT_LINKS = PROJECT_LINKS;
            this.PROJECT_ATTRIBUTES = PROJECT_ATTRIBUTES;
        }

        [JsonProperty("@accession", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("@accession")]
        public string accession { get; }

        [JsonProperty("@alias", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("@alias")]
        public string alias { get; }

        [JsonProperty("@center_name", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("@center_name")]
        public string center_name { get; }

        [JsonProperty("IDENTIFIERS", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("IDENTIFIERS")]
        public IDENTIFIERS IDENTIFIERS { get; }

        [JsonProperty("NAME", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("NAME")]
        public string NAME { get; }

        [JsonProperty("TITLE", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("TITLE")]
        public string TITLE { get; }

        [JsonProperty("DESCRIPTION", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("DESCRIPTION")]
        public string DESCRIPTION { get; }

        [JsonProperty("SUBMISSION_PROJECT", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("SUBMISSION_PROJECT")]
        public SUBMISSIONPROJECT SUBMISSION_PROJECT { get; }

        [JsonProperty("PROJECT_LINKS", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("PROJECT_LINKS")]
        public PROJECTLINKS PROJECT_LINKS { get; }

        [JsonProperty("PROJECT_ATTRIBUTES", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("PROJECT_ATTRIBUTES")]
        public PROJECTATTRIBUTES PROJECT_ATTRIBUTES { get; }

    }
    public class IDENTIFIERS
    {
        public IDENTIFIERS(
            string PRIMARY_ID,
            string SECONDARY_ID,
            SUBMITTERID SUBMITTER_ID
        )
        {
            this.PRIMARY_ID = PRIMARY_ID;
            this.SECONDARY_ID = SECONDARY_ID;
            this.SUBMITTER_ID = SUBMITTER_ID;
        }

        [JsonProperty("PRIMARY_ID", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("PRIMARY_ID")]
        public string PRIMARY_ID { get; }

        [JsonProperty("SECONDARY_ID", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("SECONDARY_ID")]
        public string SECONDARY_ID { get; }

        [JsonProperty("SUBMITTER_ID", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("SUBMITTER_ID")]
        public SUBMITTERID SUBMITTER_ID { get; }
    }

    public class PROJECTATTRIBUTE
    {
        public PROJECTATTRIBUTE(
            string TAG,
            string VALUE
        )
        {
            this.TAG = TAG;
            this.VALUE = VALUE;
        }

        [JsonProperty("TAG", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("TAG")]
        public string TAG { get; }

        [JsonProperty("VALUE", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("VALUE")]
        public string VALUE { get; }
    }

    public class PROJECTATTRIBUTES
    {
        public PROJECTATTRIBUTES(
            List<PROJECTATTRIBUTE> PROJECT_ATTRIBUTE
        )
        {
            this.PROJECT_ATTRIBUTE = PROJECT_ATTRIBUTE;
        }

        [JsonProperty("PROJECT_ATTRIBUTE", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("PROJECT_ATTRIBUTE")]
        public IReadOnlyList<PROJECTATTRIBUTE> PROJECT_ATTRIBUTE { get; }
    }

    public class PROJECTLINK
    {
        public PROJECTLINK(
            XREFLINK XREF_LINK
        )
        {
            this.XREF_LINK = XREF_LINK;
        }

        [JsonProperty("XREF_LINK", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("XREF_LINK")]
        public XREFLINK XREF_LINK { get; }
    }

    public class PROJECTLINKS
    {
        public PROJECTLINKS(
            List<PROJECTLINK> PROJECT_LINK
        )
        {
            this.PROJECT_LINK = PROJECT_LINK;
        }

        [JsonProperty("PROJECT_LINK", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("PROJECT_LINK")]
        public IReadOnlyList<PROJECTLINK> PROJECT_LINK { get; }
    }

    public class SUBMISSIONPROJECT
    {
        public SUBMISSIONPROJECT(
            object SEQUENCING_PROJECT
        )
        {
            this.SEQUENCING_PROJECT = SEQUENCING_PROJECT;
        }

        [JsonProperty("SEQUENCING_PROJECT", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("SEQUENCING_PROJECT")]
        public object SEQUENCING_PROJECT { get; }
    }

    public class SUBMITTERID
    {
        public SUBMITTERID(
            string namespace_,
            string text_
        )
        {
            this.namespace_ = namespace_;
            this.text_ = text_;
        }

        [JsonProperty("@namespace", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("@namespace")]
        public string namespace_ { get; }

        [JsonProperty("#text", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("#text")]
        public string text_ { get; }
    }

    public class XREFLINK
    {
        public XREFLINK(
            string DB,
            object ID
        )
        {
            this.DB = DB;
            this.ID = ID;
        }

        [JsonProperty("DB", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("DB")]
        public string DB { get; }

        [JsonProperty("ID", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("ID")]
        public object ID { get; }
    }

}
