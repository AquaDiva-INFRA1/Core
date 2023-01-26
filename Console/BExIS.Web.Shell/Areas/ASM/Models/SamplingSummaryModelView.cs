using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BExIS.Modules.Asm.UI.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class SamplingSummaryModelView
    {
        [JsonConstructor]
        public SamplingSummaryModelView(
            [JsonProperty("key1")] string key1,
            [JsonProperty("key2")] object key2
        )
        {
            this.Key1 = key1;
            this.Key2 = key2;
        }

        [JsonProperty("key1")]
        public readonly string Key1;

        [JsonProperty("key2")]
        public readonly object Key2;
    }
}