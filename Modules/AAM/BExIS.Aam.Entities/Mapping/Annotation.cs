using BExIS.Dlm.Entities.Data;
using BExIS.Dlm.Entities.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public virtual long EntityId { get; set; }
        public virtual long CharacteristicId { get; set; }
        public virtual long StandardId { get; set; }
        
        public Annotation(){ }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Annotation:");
            sb.AppendLine("\tDataset ID: " + this.Dataset.Id);
            sb.AppendLine("\tDatasetVersion ID: " + this.DatasetVersion.Id);
            sb.AppendLine("\tVariable: " + this.Variable.Id + " " + this.Variable.Label);
            sb.AppendLine("\tEntity: " + this.Entity);
            sb.AppendLine("\tCharacteristic: " + this.Characteristic);
            sb.AppendLine("\tStandard: " + this.Standard);
            return sb.ToString();
        }

        public Annotation(Dataset ds, DatasetVersion dsv, Variable var, String entity, String characteristic)
        {
            this.Dataset = ds;
            this.DatasetVersion = dsv;
            this.Variable = var;
            this.Entity = entity;
            this.Characteristic = characteristic;
            this.Standard = "http://ecoinformatics.org/oboe/oboe.1.2/oboe-core.owl#Standard"; //Default standard
        }

        public Annotation(Dataset ds, DatasetVersion dsv, Variable var, String entity, String characteristic, String standard)
        {
            this.Dataset = ds;
            this.DatasetVersion = dsv;
            this.Variable = var;
            this.Entity = entity;
            this.Characteristic = characteristic;
            this.Standard = standard;
        }
    }
}
