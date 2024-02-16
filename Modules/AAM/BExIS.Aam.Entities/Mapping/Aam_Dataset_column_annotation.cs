using System;
using System.Xml;
using System.ComponentModel.DataAnnotations;
using Vaiona.Entities.Common;
using BExIS.Dlm.Entities.Data;
using BExIS.Dlm.Entities.DataStructure;
using System.Data.Entity;

namespace BExIS.Aam.Entities.Mapping
{
    public class Aam_Dataset_column_annotation : BaseEntity
    {
        public Aam_Dataset_column_annotation(Aam_Dataset_column_annotation an)
        {
            Dataset = an.Dataset ?? throw new ArgumentNullException(nameof(an.Dataset));
            DatasetVersion = an.DatasetVersion ?? throw new ArgumentNullException(nameof(an.DatasetVersion));
            this.variable_id = an.variable_id ?? throw new ArgumentNullException(nameof(variable_id));
            this.entity_id = an.entity_id;
            this.characteristic_id = an.characteristic_id;
            this.standard_id = an.standard_id;
        }

        public Aam_Dataset_column_annotation(Dataset dataset, DatasetVersion datasetVersion, Variable variable_id, Aam_Uri entity_id, Aam_Uri characteristic_id, Aam_Uri standard_id)
        {
            Dataset = dataset;
            DatasetVersion = datasetVersion;
            this.variable_id = variable_id;
            this.entity_id = entity_id;
            this.characteristic_id = characteristic_id;
            this.standard_id = standard_id;
        }

        public Aam_Dataset_column_annotation() { }

        public class Aam_Dataset_column_annotationContext : DbContext
        {
            public DbSet<Aam_Dataset_column_annotation> Aam_Dataset_column_annotations { get; set; }
        }

        [Required(ErrorMessage = "Must not be Empty")]
        public virtual Dataset Dataset { get; set; }
        [Required(ErrorMessage = "Must not be Empty")]
        public virtual DatasetVersion DatasetVersion { get; set; }
        [Required(ErrorMessage = "Must not be Empty")]
        public virtual Variable variable_id { get; set; }
        [Required(ErrorMessage = "Must not be Empty")]
        public virtual Aam_Uri entity_id { get; set; }
        [Required(ErrorMessage = "Must not be Empty")]
        public virtual Aam_Uri characteristic_id { get; set; }
        [Required(ErrorMessage = "Must not be Empty")]
        public virtual Aam_Uri standard_id { get; set; }
        public override int VersionNo { get => base.VersionNo; set => base.VersionNo = value; }
        public override XmlNode Extra { get => base.Extra; set => base.Extra = value; }

        public override string ToString()
        {
            return base.ToString();
        }

        public override void Dematerialize(bool includeChildren = true)
        {
            base.Dematerialize(includeChildren);
        }
    }
}
