using System;
using System.Collections.Generic;

namespace DiplomaWork.Models
{
    public partial class Role
    {
        public Role()
        {
            RoleHasPermissions = new HashSet<RoleHasPermission>();
            UserHasRoles = new HashSet<UserHasRole>();
        }

        public uint Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;

        public virtual ICollection<RoleHasPermission> RoleHasPermissions { get; set; }
        public virtual ICollection<UserHasRole> UserHasRoles { get; set; }
    }
}
