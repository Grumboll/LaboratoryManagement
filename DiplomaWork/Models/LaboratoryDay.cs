using System;
using System.Collections.Generic;

namespace DiplomaWork.Models
{
    public partial class LaboratoryDay
    {
        public uint Id { get; set; }
        public DateOnly Day { get; set; }
        public uint ProfileId { get; set; }
        public decimal MetersSquaredPerSample { get; set; }
        public uint PaintedSamplesCount { get; set; }
        public decimal PaintedMetersSquared { get; set; }
        public decimal? KilogramsPerMeter { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public uint CreatedBy { get; set; }
        public uint UpdatedBy { get; set; }

        public virtual User CreatedByNavigation { get; set; } = null!;
        public virtual Profile Profile { get; set; } = null!;
        public virtual User UpdatedByNavigation { get; set; } = null!;
        public virtual LaboratoryMonth LaboratoryMonth { get; set; } = null!;
    }
}
