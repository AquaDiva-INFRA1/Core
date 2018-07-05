using BExIS.Aam.Entities.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BExIS.Modules.Aam.UI.Models
{
    public class AnnotationResult : IEquatable<AnnotationResult>
    {
        public String Entity;
        public String Characteristic;
        public String Standard;
        public int Occurences;

        public AnnotationResult(string entity, string characteristic, string standard)
        {
            this.Entity = entity;
            this.Characteristic = characteristic;
            this.Standard = standard;
            this.Occurences = 0;
        }

        public AnnotationResult(string entity, string characteristic, string standard, int occurences)
        {
            this.Entity = entity;
            this.Characteristic = characteristic;
            this.Standard = standard;
            this.Occurences = occurences;
        }

        public AnnotationResult(Annotation an)
        {
            this.Entity = an.Entity;
            this.Characteristic = an.Characteristic;
            this.Standard = an.Standard;
            this.Occurences = 1;
        }

        public AnnotationResult(Annotation an, int occ)
        {
            this.Entity = an.Entity;
            this.Characteristic = an.Characteristic;
            this.Standard = an.Standard;
            this.Occurences = occ;
        }

        public bool Equals(Annotation an)
        {
            return this.Entity.Equals(an.Entity) && this.Characteristic.Equals(an.Characteristic) && this.Standard.Equals(an.Standard);
        }

        /// <summary>
        /// Test, if an instance of AnnotationResult equals another instance.
        /// </summary>
        /// <param name="other">The other AnnotationResult instance</param>
        /// <returns>True, if the Entity, Characteristic and Standard are equal, false otherwise</returns>
        public bool Equals(AnnotationResult other)
        {
            return (this.Entity.Equals(other.Entity) && this.Characteristic.Equals(other.Characteristic) && this.Standard.Equals(other.Standard));
        }

        public override int GetHashCode()
        {
            return Entity.GetHashCode() ^ Characteristic.GetHashCode() ^ Standard.GetHashCode();
        }
    }
}
