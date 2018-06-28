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

        public AnnotationResult(string entity, string characteristic, string standard)
        {
            this.Entity = entity;
            this.Characteristic = characteristic;
            this.Standard = standard;
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
