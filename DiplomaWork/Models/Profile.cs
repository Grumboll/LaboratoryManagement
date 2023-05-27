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

        public virtual ICollection<ProfileHasLengthsPerimeter> ProfileHasLengthsPerimeters { get; set; }
    }
}
