using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BExIS.Modules.Asm.UI.Models
{
    public class SpecialDataset_analysis
    {
        public SpecialDataset_analysis(JToken jtok)
        {
            foreach (JProperty xx in jtok)
            {
                var name = xx.Name;
                if (xx.Name == "key1")
                    key1 = JsonConvert.DeserializeObject<List<Key1>>(xx.Value.ToString());
                else if (xx.Name == "key2")
                    if (xx.Value.ToString() == "")
                        key2 = JsonConvert.DeserializeObject<List<Key2>>("[]");
                    else
                        key2 = JsonConvert.DeserializeObject<List<Key2>>(xx.Value.ToString());
            }
        }
        public List<Key1> key1 { get; set; }
        public List<Key2> key2 { get; set; }
    }

    public class Key1
    {
        [JsonProperty("Analysis Type")]
        public string AnalysisType { get; set; }

        [JsonProperty("H1-3")]
        public int H13 { get; set; }

        [JsonProperty("H1-4")]
        public int H14 { get; set; }

        [JsonProperty("H2-1")]
        public int H21 { get; set; }

        [JsonProperty("H2-3")]
        public int H23 { get; set; }

        [JsonProperty("H3-1")]
        public int H31 { get; set; }

        [JsonProperty("H3-2")]
        public int H32 { get; set; }

        [JsonProperty("H4-1")]
        public int H41 { get; set; }

        [JsonProperty("H4-2")]
        public int H42 { get; set; }

        [JsonProperty("H4-3")]
        public int H43 { get; set; }

        [JsonProperty("H5-1")]
        public int H51 { get; set; }

        [JsonProperty("H5-2")]
        public int H52 { get; set; }

        [JsonProperty("H5-3")]
        public int H53 { get; set; }
        public int id { get; set; }
    }

     
    public class Key2
    {
        [JsonProperty("PNK Number")]
        public string PNKNumber { get; set; }

        [JsonProperty("H4-2")]
        public int H42 { get; set; }
        public int id { get; set; }
    }



}