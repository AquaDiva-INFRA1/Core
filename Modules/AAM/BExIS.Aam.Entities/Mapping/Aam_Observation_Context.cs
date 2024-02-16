using System.Data.Entity;
using System.Xml;
using Vaiona.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BExIS.Dlm.Entities.Data;

namespace BExIS.Aam.Entities.Mapping
{
    public class Aam_Observation_Context : BaseEntity
    {
        public Aam_Observation_Context(Dataset dataset, DatasetVersion datasetVersion, Aam_Uri contextualized_entity, Aam_Uri contextualizing_entity)
        {
            Dataset = dataset;
            DatasetVersion = datasetVersion;
            Contextualized_entity = contextualized_entity;
            Contextualizing_entity = contextualizing_entity;
        }

        public Aam_Observation_Context()
        {
        }

        public class Aam_Observation_ContextContext : DbContext
        {
            public DbSet<Aam_Observation_Context> Aam_Observation_Contexts { get; set; }
        }


        [Required(ErrorMessage = "Must not be Empty")]
        [Key]
        [ForeignKey("Dataset")]
        public virtual Dataset Dataset { get; set; }
        [Required(ErrorMessage = "Must not be Empty")]
        [Key]
        [ForeignKey("DatasetVersion")]
        public virtual DatasetVersion DatasetVersion { get; set; }
        [Required(ErrorMessage = "Must not be Empty")]
        [ForeignKey("Aam_Uri")]
        public virtual Aam_Uri Contextualized_entity { get; set; }
        [Required(ErrorMessage = "Must not be Empty")]
        [ForeignKey("Aam_Uri")]
        public virtual Aam_Uri Contextualizing_entity { get; set; }

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
