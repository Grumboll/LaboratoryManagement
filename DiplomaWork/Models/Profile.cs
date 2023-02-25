using System;
using System.Collections.Generic;

namespace DiplomaWork.Models
{
    public partial class Profile
    {
        public Profile()
        {
            ProfileHasLengthsPerimeters = new HashSet<ProfileHasLengthsPerimeter>();
        }

        public uint Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public uint CreatedBy { get; set; }
        public uint UpdatedBy { get; set; }

        public virtual User CreatedByNavigation { get; set; } = null!;
        public virtual User UpdatedByNavigation { get; set; } = null!;
        public virtual LaboratoryDay LaboratoryDay { get; set; } = null!;
        public virtual ICollection<ProfileHasLengthsPerimeter> ProfileHasLengthsPerimeters { get; set; }
    }
}
