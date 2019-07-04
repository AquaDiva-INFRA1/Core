﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BExIS.Web.Shell.Areas.DCM.Helpers
{
    public class EasyUploadSuggestion
    {
        public string attributeName;
        public long unitID;
        public long dataTypeID;
        public string unitName;
        public string datatypeName;
        public Boolean show;

        public string datatypeDescription;
        public string conceptAnnotation;
        public string characteristicAnnotation;

        public EasyUploadSuggestion(string attributeName, long unitID, long dataTypeID, string unitName, string datatypeName, Boolean show)
        {
            this.attributeName = attributeName;
            this.unitID = unitID;
            this.dataTypeID = dataTypeID;
            this.unitName = unitName;
            this.datatypeName = datatypeName;
            this.show = show;
        }
        
        public void finish_suggestion_block (string desc, string concept, string charac)
        {
            this.conceptAnnotation = concept;
            this.characteristicAnnotation = charac;
            this.datatypeDescription = desc;
        }
    }
}