using System.Collections.Generic;

namespace BExIS.Modules.Aam.UI.Models
{

    public class Aam_Observation_Context_Model
    {
        public Dictionary<long, string> datasets = new Dictionary<long, string>();
        public Dictionary<long, string> Contextualized_entity = new Dictionary<long, string>();
        public Dictionary<long, string> Contextualizing_entity = new Dictionary<long, string>();

    }
}