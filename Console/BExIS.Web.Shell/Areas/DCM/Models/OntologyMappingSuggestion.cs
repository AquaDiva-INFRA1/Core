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
        public Boolean selected;

        public OntologyMappingSuggestionModel(string uri, string label, double sim, bool selected = false)
        {
            this.conceptURI = uri;
            this.displayName = label;
            this.similarity = sim;
            this.selected = selected;
        }
    }

    /*
     * Class for saving the selected annotation in the bus
     * */
    public class OntologyAnnotation
    {
        string conceptURI;
        string conceptCategory;
        public int headerID;

        public OntologyAnnotation(string conceptURI, string conceptCategory, int headerID)
        {
            this.conceptURI = conceptURI;
            this.conceptCategory = conceptCategory;
            this.headerID = headerID;
        }
    }
}