using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BExIS.Modules.Asm.UI.Models
{
    public class Dataset_analysis
    {
        public Dataset_analysis(JToken jtok, int id)
        {
            this.id = id;
            foreach (JProperty xx in jtok)
            {
                var name = xx.Name;
                if (xx.Name == "classification")
                    category_Classifications = JsonConvert.DeserializeObject<List<Category_classification>>(xx.Value.ToString());
                else if (xx.Name == "categorical")
                    categorical = JsonConvert.DeserializeObject<List<Categorical>>(xx.Value.ToString());
                else if (xx.Name == "non_categorical")
                    non_Categorical = JsonConvert.DeserializeObject<List<Non_categorical>>(xx.Value.ToString());
            }

            category_Classifications = JsonConvert.DeserializeObject<List<Category_classification>>(jtok["classification"].ToString());
            categorical = JsonConvert.DeserializeObject<List<Categorical>>(jtok["categorical"].ToString()); 
            non_Categorical = JsonConvert.DeserializeObject<List<Non_categorical>>(jtok["non_categorical"].ToString()); 
        }

        public int id;
        public List<Category_classification> category_Classifications { get; set; }
        public List<Categorical> categorical { get; set; }
        public List<Non_categorical> non_Categorical { get; set; } 
    }

    public class Categorical{

        public List<int> counts { get; set; }
        public string name { get; set; }
        public List<float> values { get; set; }
    }

    public class Non_categorical
    {

        public List<int> counts { get; set; }
        public string name { get; set; }
        public List<float> values { get; set; }
    }
    public class Category_classification
    {
        public string category { get; set; }
        public bool drawable { get; set; }

        [JsonProperty(PropertyName = "Average Categorization Probability")]
        public float categorization_probability { get; set; }

        [JsonProperty(PropertyName = "Attribute Name")]
        public string name { get; set; }
    }
}