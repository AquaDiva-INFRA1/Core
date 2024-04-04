using System;
using System.Data.Entity;
using System.Xml;
using Vaiona.Entities.Common;
using System.ComponentModel.DataAnnotations;

namespace BExIS.Aam.Entities.Mapping
{
    public class Aam_Uri : BaseEntity
    {
        public Aam_Uri( string uRI, string label, string type_uri)
        {
            URI = uRI ?? throw new ArgumentNullException(nameof(uRI));
            this.label = label ?? throw new ArgumentNullException(nameof(label));
            this.type_uri = type_uri ?? throw new ArgumentNullException(nameof(type_uri));
        }

        public Aam_Uri(Aam_Uri Aam_Uri)
        {
            URI = Aam_Uri.URI ;
            this.label = Aam_Uri.label ;
            this.type_uri = Aam_Uri.type_uri ;
        }


        public Aam_Uri() {}

        [RegularExpression(@"^http://.*", ErrorMessage = "URI must start with http://")]
        public virtual string URI { get; set; }
        [Required(ErrorMessage = "Must not be Empty")]
        public virtual string label { get; set; }
        [Required(ErrorMessage = "Must not be Empty")]
        public virtual string type_uri { get; set; }
        [Required(ErrorMessage = "Must not be Empty")]
        public override int VersionNo { get => base.VersionNo; set => base.VersionNo = value; }
        public override XmlNode Extra { get => base.Extra; set => base.Extra = value; }

        public class Aam_UriContext : DbContext
        {
            public DbSet<Aam_Uri> Aam_Uris { get; set; }
        }

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
