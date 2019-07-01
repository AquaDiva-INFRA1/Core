﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BExIS.Dlm.Entities.MetadataStructure
{
    public class MetadataCompoundAttribute : MetadataAttribute
    {
        public virtual ICollection<MetadataNestedAttributeUsage> MetadataNestedAttributeUsages { get; set; }

        #region Methods

        public MetadataCompoundAttribute(): base()
        {
            MetadataNestedAttributeUsages = new List<MetadataNestedAttributeUsage>();
        }

        public virtual MetadataCompoundAttribute Self { get { return this; } }

        #endregion
    }
}
