using BExIS.Dlm.Entities.Data;
using BExIS.Dlm.Entities.DataStructure;
using System;
using System.Text;
using Vaiona.Entities.Common;

namespace BExIS.Aam.Entities.Mapping
{
    public class Annotation: BaseEntity
    {
        public virtual Dataset Dataset { get; set; }
        public virtual DatasetVersion DatasetVersion { get; set; }
        public virtual Variable Variable { get; set; }
        public virtual String Entity { get; set; }
        public virtual String Characteristic { get; set; }
        public virtual String Standard { get; set; }
        public virtual String Entity_Label { get; set; }
        public virtual String Characteristic_Label { get; set; }
        public virtual String Standard_Label { get; set; }
        public virtual long EntityId { get; set; }
        public virtual long CharacteristicId { get; set; }
        public virtual long StandardId { get; set; }
        
        public Annotation(System.Data.DataSet dataset) { }

        public Annotation() { }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Annotation:");
            sb.AppendLine("\tDataset ID: " + this.Dataset.Id);
            sb.AppendLine("\tDatasetVersion ID: " + this.DatasetVersion.Id);
            sb.AppendLine("\tVariable: " + this.Variable.Id + " " + this.Variable.Label);
            sb.AppendLine("\tEntity: " + this.Entity + " (" + this.Entity_Label + ")");
            sb.AppendLine("\tCharacteristic: " + this.Characteristic + " (" + this.Characteristic_Label + ")");
            sb.AppendLine("\tStandard: " + this.Standard + " (" + this.Standard_Label + ")");
            return sb.ToString();
        }

        public Annotation(Dataset ds, DatasetVersion dsv, Variable var, String entity, String entity_label, String characteristic, String characteristic_label)
        {
            this.Dataset = ds;
            this.DatasetVersion = dsv;
            this.Variable = var;
            this.Entity = entity;
            this.Entity_Label = entity_label;
            this.Characteristic = characteristic;
            this.Characteristic_Label = characteristic_label;
            this.Standard = "http://ecoinformatics.org/oboe/oboe.1.2/oboe-core.owl#Standard"; //Default standard
        }

        public Annotation(Dataset ds, DatasetVersion dsv, Variable var, String entity, String entity_label, String characteristic, String characteristic_label, String standard, String standard_label)
        {
            this.Dataset = ds;
            this.DatasetVersion = dsv;
            this.Variable = var;
            this.Entity = entity;
            this.Entity_Label = entity_label;
            this.Characteristic = characteristic;
            this.Characteristic_Label = characteristic_label;
            this.Standard = standard;
            this.Standard_Label = standard_label;
        }
    }
}
