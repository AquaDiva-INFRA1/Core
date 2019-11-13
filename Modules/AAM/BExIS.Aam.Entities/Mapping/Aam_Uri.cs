using BExIS.Dlm.Entities.Data;
using BExIS.Dlm.Entities.DataStructure;
using System;
using System.Collections.Generic;
using System.Xml;
using Vaiona.Entities.Common;

namespace BExIS.Aam.Entities.Mapping
{
    public class Aam_Uri : BaseEntity
    {
        public Aam_Uri(long id, string uRI, string label, string type)
        {
            Id = id;
            URI = uRI ?? throw new ArgumentNullException(nameof(uRI));
            this.label = label ?? throw new ArgumentNullException(nameof(label));
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public Aam_Uri(long id, string uRI, string label, string type, int versionNo, XmlNode extra) : this(id, uRI, label, type)
        {
            VersionNo = versionNo;
            Extra = extra;
        }

        public override long Id { get => base.Id; set => base.Id = value; }
        public virtual String URI { get; set; }
        public virtual String label { get; set; }
        public virtual String type { get; set; }
        public override int VersionNo { get => base.VersionNo; set => base.VersionNo = value; }
        public override XmlNode Extra { get => base.Extra; set => base.Extra = value; }

        public override void Dematerialize(bool includeChildren = true)
        {
            base.Dematerialize(includeChildren);
        }

        public override bool Equals(object obj)
        {
            return obj is Aam_Uri uri &&
                   Id == uri.Id &&
                   URI == uri.URI &&
                   label == uri.label &&
                   type == uri.type;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
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
