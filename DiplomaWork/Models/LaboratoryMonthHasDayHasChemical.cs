using System;
using System.Collections.Generic;

namespace DiplomaWork.Models
{
    public partial class LaboratoryMonthHasDayHasChemical
    {
        public uint Id { get; set; }
        public uint LaboratoryMonthHasDayId { get; set; }
        public string Name { get; set; } = null!;
        public float ExpensePerMeterSquared { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public uint CreatedBy { get; set; }
        public uint UpdatedBy { get; set; }

        public virtual User CreatedByNavigation { get; set; } = null!;
        public virtual LaboratoryMonthHasDay LaboratoryMonthHasDay { get; set; } = null!;
        public virtual User UpdatedByNavigation { get; set; } = null!;
    }
}
