﻿using BExIS.Security.Entities.Authorization;
using System.Collections.Generic;
using Vaiona.Entities.Common;

namespace BExIS.Security.Entities.Subjects
{
    public abstract class Subject : BaseEntity
    {
        public virtual string Name { get; set; }
        public virtual ICollection<Permission> Permissions { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
    }
}