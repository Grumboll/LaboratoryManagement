using System;
using System.Collections.Generic;

namespace DiplomaWork.Models
{
    public partial class ProfileHasLengthsPerimeter
    {
        public uint Id { get; set; }
        public uint ProfileId { get; set; }
        public decimal? Length { get; set; }
        public decimal? Perimeter { get; set; }

        public virtual Profile Profile { get; set; } = null!;
    }
}
