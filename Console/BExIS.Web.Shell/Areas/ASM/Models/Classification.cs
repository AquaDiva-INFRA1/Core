using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BExIS.Modules.Asm.UI.Models
{
    //delete classes classification and input after DataSetController.cs has been removed 
    public class Classification
    {
        //public Input var_name;
        public List<Input> class_results;
        public List<string> keywords;
        public Classification()
        {
            this.class_results = new List<Input>();
            this.keywords = new List<string>();
        }
    }

    public class Input
    {
        public string entity_id;
        public string entity;
        public string charachteristic_id;
        public string variable_id_from_table;
        public string variable_value;
        public string predicted_class;
        public List<string> onto_match;
        public List<string> onto_no_path;
        public List<string> onto_no_node;
        public List<string> db_match; 
        public List<string> db_no_node;
        public List<string> onto_target_file;
        public List<string> db_no_path;
    }


    public class Predict_input
    {
        public Predict_input(JToken ip)
        {
            characteristic = (string)ip["charachteristic"];
            entity = (string)ip["entity"];
            ds_title = (string)ip["dataset_title"];
            type = (string)ip["type"];
            unit = (string)ip["unit"];
            var_id = (string)ip["variable_id_from_table"];
            var_value = (string)ip["variable_value"];
        }
        public string characteristic { get; set; }
        public string entity { get; set; }
        public string ds_title{ get; set; }
        public string type { get; set; }
        public string unit { get; set; }
        public string var_id { get; set; }
        public string var_value { get; set; }
    }

    public class Predict_classScore
    {
        public Predict_classScore(JToken clScore)
        {
            score_0 = (double)clScore["0"];
            score_1 = (double)clScore["1"];
            score_2 = (double)clScore["2"];
            score_3 = (double)clScore["3"];
            score_4 = (double)clScore["4"];
            score_5 = (double)clScore["5"];
            score_6 = (double)clScore["6"];
            score_7 = (double)clScore["7"];
            score_8 = (double)clScore["8"];
            score_9 = (double)clScore["9"];
            score_10 = (double)clScore["10"];
            score_11 = (double)clScore["11"];
        }
        public double score_0 { get; set; }
        public double score_1 { get; set; }
        public double score_2 { get; set; }
        public double score_3 { get; set; }
        public double score_4 { get; set; }
        public double score_5 { get; set; }
        public double score_6 { get; set; }
        public double score_7 { get; set; }
        public double score_8 { get; set; }
        public double score_9 { get; set; }
        public double score_10 { get; set; }
        public double score_11 { get; set; }
    }


    public class Predict_results
    {
        public Predict_results(JToken jTok, string id)
        {
            this.id = id;
            input = new Predict_input(jTok["input"]);
            class_score = new Predict_classScore(jTok["class_score"]);
            predition_bestMatches = (string)jTok["predicted_class"];
            onto_match = JsonConvert.DeserializeObject<List<string>>(jTok["onto_match"].ToString());
            //onto_noPath = JsonConvert.DeserializeObject<List<string>>(jTok["onto_no_path"].ToString());
            //onto_noNode = JsonConvert.DeserializeObject<List<string>>(jTok["onto_no_node"].ToString());
            db_match = JsonConvert.DeserializeObject<List<string>>(jTok["db_match"].ToString());
            //db_noPath = JsonConvert.DeserializeObject<List<string>>(jTok["db_no_path"].ToString());
            //db_noNode = JsonConvert.DeserializeObject<List<string>>(jTok["db_no_node"].ToString());
            onto_targetFile = JsonConvert.DeserializeObject<List<string>>(jTok["onto_target_file"].ToString());
        }

        public string id { get; set; }
        public Predict_input input { get; set; }
        public Predict_classScore class_score { get; set; }
        public string predition_bestMatches { get; set; }
        public List<string> onto_match { get; set; }
        public List<string> onto_noPath { get; set; }
        public List<string> onto_noNode { get; set; }
        public List<string> db_match { get; set; }
        public List<string> db_noPath { get; set; }
        public List<string> db_noNode { get; set; }
        public List<string> onto_targetFile  { get; set; }

    }

}