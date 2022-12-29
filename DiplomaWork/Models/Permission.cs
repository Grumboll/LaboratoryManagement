using System;
using System.Collections.Generic;

namespace DiplomaWork.Models
{
    public partial class Permission
    {
        public Permission()
        {
            RoleHasPermissions = new HashSet<RoleHasPermission>();
        }

        public uint Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;

        public virtual ICollection<RoleHasPermission> RoleHasPermissions { get; set; }
    }
}
