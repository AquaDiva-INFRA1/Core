using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace BExIS.Modules.Ddm.UI.Helpers
{
    class OntologyMapping
    {
        public String displayName;
        public String mappedConceptGroup;
        public Uri baseUri;
        public Uri mappedConceptUri;

        public OntologyMapping(String displayName, String mappedConceptGroup, Uri baseUri, Uri mappedConceptUri)
        {
            this.displayName = displayName;
            this.mappedConceptGroup = mappedConceptGroup;
            this.baseUri = baseUri;
            this.mappedConceptUri = mappedConceptUri;
        }

        public OntologyMapping()
        {
            this.displayName = null;
            this.mappedConceptGroup = null;
            this.baseUri = null;
            this.mappedConceptUri = null;
        }

        public String toString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(displayName + "  -  ");
            sb.Append(mappedConceptGroup + "  -  ");
            sb.Append(mappedConceptUri.ToString() + "  -  ");
            sb.Append(baseUri.ToString());
            return sb.ToString();
        }

        #region Getter
        public Uri getBaseUri()
        {
            return baseUri;
        }

        public Uri getMappedConceptUri()
        {
            return mappedConceptUri;
        }

        public String getDisplayName()
        {
            return this.displayName;
        }

        public String getMappedConceptGroup()
        {
            return this.mappedConceptGroup;
        }
        #endregion

        #region Setter
        public void setDisplayName(String s)
        {
            this.displayName = s;
        }

        public void setBaseUri(Uri uri)
        {
            this.baseUri = uri;
        }

        public void setMappedConceptUri(Uri uri)
        {
            this.mappedConceptUri = uri;
        }

        public void setMappedConceptGroup(String group)
        {
            this.mappedConceptGroup = group;
        }
        #endregion
    }

    class OntologyNamePair
    {
        private String ontologyPath;
        private String contentName;

        public OntologyNamePair(String path, String name)
        {
            this.ontologyPath = path;
            this.contentName = name;
        }

        internal string getPath()
        {
            return this.ontologyPath;
        }

        internal string getContentName()
        {
            return contentName;
        }
    }
}