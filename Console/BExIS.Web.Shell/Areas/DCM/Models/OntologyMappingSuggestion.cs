using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BExIS.Modules.Dcm.UI.Models
{
    public class OntologyMappingSuggestionModel
    {
        public string conceptURI;
        public string displayName;
        public double similarity;

        public OntologyMappingSuggestionModel(string uri, string label, double sim)
        {
            this.conceptURI = uri;
            this.displayName = label;
            this.similarity = sim;
        }
    }

    /*
     * Class for saving the selected annotation in the bus
     * */
    public class OntologyAnnotation
    {
        string conceptURI;
        public int headerID;

        public OntologyAnnotation(string conceptURI, int headerID)
        {
            this.conceptURI = conceptURI;
            this.headerID = headerID;
        }
    }
}