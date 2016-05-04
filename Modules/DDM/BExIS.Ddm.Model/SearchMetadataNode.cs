﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BExIS.Ddm.Model
{
    public class SearchMetadataNode
    {
        public string MetadataStructureName { get; set; }
        public string XPath { get; set; }
        public string DisplayName { get; set; }
        public string DisplayNameLong { get; set; }

        public SearchMetadataNode(string metadataStructureName, string xPath)
        {
            this.MetadataStructureName = metadataStructureName;
            this.XPath = xPath;
            DisplayName = generateDisplayName(xPath);
            DisplayNameLong = "("+MetadataStructureName+") " + generateDisplayName(xPath);
        }

        private string generateDisplayName(string xPath)
        {
            string tmp = "";
            string[] tempArray = xPath.Split('/');

            for (int i = 1; i < tempArray.Length; i = i + 2)
            {
                tmp += tempArray[i]+"/" ;
            }

            return tmp.Remove(tmp.Length-1);
        }
    }
}
