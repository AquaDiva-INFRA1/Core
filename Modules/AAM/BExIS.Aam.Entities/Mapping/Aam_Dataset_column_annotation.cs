using BExIS.Dlm.Entities.Data;
using BExIS.Dlm.Entities.DataStructure;
using System;
using System.Collections.Generic;
using System.Xml;
using Vaiona.Entities.Common;

namespace BExIS.Aam.Entities.Mapping
{
    public class Aam_Dataset_column_annotation : BaseEntity
    {
        public Aam_Dataset_column_annotation(int id, Dataset dataset, DatasetVersion datasetVersion, Variable variable_id, int entity_id, int characteristic_id, int standard_id)
        {
            Id = id;
            Dataset = dataset ?? throw new ArgumentNullException(nameof(dataset));
            DatasetVersion = datasetVersion ?? throw new ArgumentNullException(nameof(datasetVersion));
            this.variable_id = variable_id ?? throw new ArgumentNullException(nameof(variable_id));
            this.entity_id = entity_id;
            this.characteristic_id = characteristic_id;
            this.standard_id = standard_id;
        }

        public Aam_Dataset_column_annotation(Aam_Dataset_column_annotation an)
        {
            Id = an.Id;
            Dataset = an.Dataset ?? throw new ArgumentNullException(nameof(an.Dataset));
            DatasetVersion = an.DatasetVersion ?? throw new ArgumentNullException(nameof(an.DatasetVersion));
            this.variable_id = an.variable_id ?? throw new ArgumentNullException(nameof(variable_id));
            this.entity_id = an.entity_id;
            this.characteristic_id = an.characteristic_id;
            this.standard_id = an.standard_id;
        }
        public Aam_Dataset_column_annotation(){}

        public Aam_Dataset_column_annotation(long id, Dataset dataset, DatasetVersion datasetVersion, Variable variable_id, int entity_id, int characteristic_id, int standard_id, int versionNo, XmlNode extra)
        {
            Id = id;
            Dataset = dataset;
            DatasetVersion = datasetVersion;
            this.variable_id = variable_id;
            this.entity_id = entity_id;
            this.characteristic_id = characteristic_id;
            this.standard_id = standard_id;
            VersionNo = versionNo;
            Extra = extra;
        }

        public override long Id { get => base.Id; set => base.Id = value; }
        public virtual Dataset Dataset { get; set; }
        public virtual DatasetVersion DatasetVersion { get; set; }
        public virtual Variable variable_id { get; set; }
        public virtual Int32 entity_id { get; set; }
        public virtual Int32 characteristic_id { get; set; }
        public virtual Int32 standard_id { get; set; }
        public override int VersionNo { get => base.VersionNo; set => base.VersionNo = value; }
        public override XmlNode Extra { get => base.Extra; set => base.Extra = value; }

        public override void Dematerialize(bool includeChildren = true)
        {
            base.Dematerialize(includeChildren);
        }

        public override bool Equals(object obj)
        {
            return obj is Aam_Dataset_column_annotation annotation &&
                   Id == annotation.Id &&
                   EqualityComparer<Dataset>.Default.Equals(Dataset, annotation.Dataset) &&
                   EqualityComparer<DatasetVersion>.Default.Equals(DatasetVersion, annotation.DatasetVersion) &&
                   EqualityComparer<Variable>.Default.Equals(variable_id, annotation.variable_id) &&
                   entity_id == annotation.entity_id &&
                   characteristic_id == annotation.characteristic_id &&
                   standard_id == annotation.standard_id;
        }

        public override int GetHashCode()
        {
            var hashCode = -487558095;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Dataset>.Default.GetHashCode(Dataset);
            hashCode = hashCode * -1521134295 + EqualityComparer<DatasetVersion>.Default.GetHashCode(DatasetVersion);
            hashCode = hashCode * -1521134295 + EqualityComparer<Variable>.Default.GetHashCode(variable_id);
            hashCode = hashCode * -1521134295 + entity_id.GetHashCode();
            hashCode = hashCode * -1521134295 + characteristic_id.GetHashCode();
            hashCode = hashCode * -1521134295 + standard_id.GetHashCode();
            return hashCode;
        }

        public override void Materialize(bool includeChildren = true)
        {
            base.Materialize(includeChildren);
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
