using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BExIS.Utils.Models
{
    public class MetadataNameGroup
    {
        [Display(Name = "Metadata Node")]
        [Required(ErrorMessage = "Please select a Metadata link.")]
        public List<string> metadataNames { get; set; }

        public MetadataNameGroup()
        {
            this.metadataNames = new List<string>();
        }

        public MetadataNameGroup(List<string> metadataNames)
        {
            this.metadataNames = metadataNames;
        }
    }
}
