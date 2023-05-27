using System;
using System.Collections.Generic;

namespace DiplomaWork.Models
{
    public partial class ProfileHasLengthsPerimeter
    {
        public ProfileHasLengthsPerimeter()
        {
            LaboratoryDays = new HashSet<LaboratoryDay>();
        }

        public uint Id { get; set; }
        public uint ProfileId { get; set; }
        public decimal? Length { get; set; }
        public decimal? Perimeter { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public uint CreatedBy { get; set; }
        public uint UpdatedBy { get; set; }

        public virtual User CreatedByNavigation { get; set; } = null!;
        public virtual Profile Profile { get; set; } = null!;
        public virtual User UpdatedByNavigation { get; set; } = null!;
        public virtual ICollection<LaboratoryDay> LaboratoryDays { get; set; }
    }
}
