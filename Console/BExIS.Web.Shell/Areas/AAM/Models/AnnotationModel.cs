using BExIS.Aam.Entities.Mapping;
using System;

namespace BExIS.Modules.Aam.UI.Models
{
    public class AnnotationModel
    {
        public long DatasetId;
        public long DatasetVersionId;
        public long VariableId;
        public String VariableLabel;
        public String Entity;
        public String EntityLabel;
        public String Characteristic;
        public String CharacteristicLabel;
        public String Standard;
        public String StandardLabel;

        public AnnotationModel(Annotation an)
        {
            this.DatasetId = an.Dataset.Id;
            this.DatasetVersionId = an.DatasetVersion.Id;
            this.VariableId = an.Variable.Id;
            this.VariableLabel = an.Variable.Label;
            this.Entity = an.Entity;
            this.EntityLabel = an.Entity_Label;
            this.Characteristic = an.Characteristic;
            this.CharacteristicLabel = an.Characteristic_Label;
            this.Standard = an.Standard;
            this.StandardLabel = an.Standard_Label;
        }
    }
}