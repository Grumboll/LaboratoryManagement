using System;
using System.Collections.Generic;

namespace DiplomaWork.Models
{
    public partial class LaboratoryMonth
    {
        public uint Id { get; set; }
        public DateOnly Date { get; set; }
        public uint MonthId { get; set; }
        public short Year { get; set; }
        public decimal? Kilograms { get; set; }
        public decimal MetersSquared { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public uint CreatedBy { get; set; }
        public uint UpdatedBy { get; set; }

        public virtual User CreatedByNavigation { get; set; } = null!;
        public virtual Month Month { get; set; } = null!;
        public virtual User UpdatedByNavigation { get; set; } = null!;
    }
}
