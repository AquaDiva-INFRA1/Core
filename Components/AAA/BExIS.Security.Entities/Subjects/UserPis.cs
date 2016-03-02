using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vaiona.Entities.Common;

namespace BExIS.Security.Entities.Subjects
{
    public class UserPi : BaseEntity
    {
        public virtual long Id { get; set; }
        public virtual long UserId { get; set; }
        public virtual long PiId { get; set; }

        public UserPi(long UserId, long PiId)
        {
            this.UserId = UserId;
            this.PiId = PiId;
        }

        public UserPi(long Id, long UserId, long PiId)
        {
            this.Id = Id;
            this.UserId = UserId;
            this.PiId = PiId;
        }

        public UserPi() { }
    }
}
