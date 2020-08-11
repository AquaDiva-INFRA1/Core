using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BExIS.Modules.Asm.UI.Models
{
    public class Variable_analytics
    {
        public long dataset_id;
        public string owner;
        public string datastructure_id;
        public string project;
        public List<string> variable_id;
        public List<string> variable_label;
        public List<string> variable_concept_entity;
        public List<string> variable_concept_caracteristic;
        public List<string> dataType;
        public List<string> unit;

        public Variable_analytics(long dataset_id, string datastructure_id, string owner, string project, List<string> variable_id, List<string> variable_label, List<string> variable_concept_entity, List<string> variable_concept_caracteristic, List<string> dataType, List<string> unit)
        {
            this.dataset_id = dataset_id;
            this.datastructure_id = datastructure_id;
            this.owner = owner;
            this.project = project;
            this.variable_id = variable_id;
            this.variable_label = variable_label;
            this.variable_concept_entity = variable_concept_entity;
            this.variable_concept_caracteristic = variable_concept_caracteristic;
            this.dataType = dataType;
            this.unit = unit;
        }
    }
}