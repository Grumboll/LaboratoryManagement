using System;
using System.Collections.Generic;

namespace DiplomaWork.Models
{
    public partial class LaboratoryDayHasProfile
    {
        public uint Id { get; set; }
        public uint LaboratoryDayId { get; set; }
        public uint ProfileId { get; set; }
        public float MetersSquaredPerSample { get; set; }
        public uint PaintedSamplesCount { get; set; }
        public float PaintedMetersSquared { get; set; }
        public float? KilogramsPerMeter { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public uint CreatedBy { get; set; }
        public uint UpdatedBy { get; set; }

        public virtual User CreatedByNavigation { get; set; } = null!;
        public virtual Profile Profile { get; set; } = null!;
        public virtual User UpdatedByNavigation { get; set; } = null!;
    }
}
