using System;
using System.Collections.Generic;

namespace DiplomaWork.Models
{
    public partial class RoleHasPermission
    {
        public uint Id { get; set; }
        public uint RoleId { get; set; }
        public uint PermissionId { get; set; }

        public virtual Permission Permission { get; set; } = null!;
        public virtual Role Role { get; set; } = null!;
    }
}
