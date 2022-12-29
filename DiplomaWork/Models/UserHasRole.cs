using System;
using System.Collections.Generic;

namespace DiplomaWork.Models
{
    public partial class UserHasRole
    {
        public uint Id { get; set; }
        public uint UserId { get; set; }
        public uint RoleId { get; set; }

        public virtual Role Role { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
