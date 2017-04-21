﻿using BExIS.Security.Entities.Subjects;
using System;
using System.Collections.Generic;
using Vaiona.Entities.Common;

namespace BExIS.Security.Entities.Requests
{
    public abstract class Request : BaseEntity
    {
        public virtual ICollection<Decision> Decisions { get; set; }
        public virtual DateTime RequestDate { get; set; }
        public virtual User Requester { get; set; }
        public virtual RequestStatus Status { get; set; }

        public Request()
        {
            Decisions = new List<Decision>();
        }
    }

    public enum RequestStatus
    {
        Open = 0,
        Accepted = 1,
        Rejected = 2
    }
}
