using BExIS.Aam.Entities.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BExIS.Modules.Aam.UI.Models
{
    public class AnnotationModel
    {
        public long DatasetId;
        public long DatasetVersionId;
        public long VariableId;
        public String VariableLabel;
        public String Entity;
        public String Characteristic;
        public String Standard;

        public AnnotationModel(Annotation an)
        {
            this.DatasetId = an.Dataset.Id;
            this.DatasetVersionId = an.DatasetVersion.Id;
            this.VariableId = an.Variable.Id;
            this.VariableLabel = an.Variable.Label;
            this.Entity = an.Entity;
            this.Characteristic = an.Characteristic;
            this.Standard = an.Standard;
        }
    }
}