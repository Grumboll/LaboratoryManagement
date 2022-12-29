using System;
using System.Collections.Generic;

namespace DiplomaWork.Models
{
    public partial class ProfileHasLengthsPerimeter
    {
        public uint Id { get; set; }
        public uint ProfileId { get; set; }
        public float? Length { get; set; }
        public float? Perimeter { get; set; }

        public virtual Profile Profile { get; set; } = null!;
    }
}
