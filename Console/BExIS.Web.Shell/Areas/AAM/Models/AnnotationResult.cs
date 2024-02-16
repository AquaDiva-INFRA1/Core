using BExIS.Aam.Entities.Mapping;
using System;

namespace BExIS.Modules.Aam.UI.Models
{
    public class AnnotationResult : IEquatable<AnnotationResult>
    {
        public String Entity;
        public String Entity_Label;
        public String Characteristic;
        public String Characteristic_Label;
        public String Standard;
        public String Standard_Label;
        public int Occurences;

        public AnnotationResult(string entity, string entity_label, string characteristic, string characteristic_label, string standard, string standard_label, int occurences = 0)
        {
            this.Entity = entity;
            this.Entity_Label = entity_label;
            this.Characteristic = characteristic;
            this.Characteristic_Label = characteristic_label;
            this.Standard = standard;
            this.Standard_Label = standard_label;
            this.Occurences = occurences;
        }

        public AnnotationResult(Annotation an, int occ = 1)
        {
            this.Entity = an.Entity;
            this.Entity_Label = an.Entity_Label;
            this.Characteristic = an.Characteristic;
            this.Characteristic_Label = an.Characteristic_Label;
            this.Standard = an.Standard;
            this.Standard_Label = an.Standard_Label;
            this.Occurences = occ;
        }

        public bool Equals(Annotation an)
        {
            return this.Entity == an.Entity && this.Characteristic == an.Characteristic && this.Standard == an.Standard;
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
