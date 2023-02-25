using System;
using System.Collections.Generic;

namespace DiplomaWork.Models
{
    public partial class LaboratoryMonthHasChemical
    {
        public uint Id { get; set; }
        public uint LaboratoryMonthId { get; set; }
        public string Name { get; set; } = null!;
        public decimal ExpensePerMeterSquared { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public uint CreatedBy { get; set; }
        public uint UpdatedBy { get; set; }

        public virtual User CreatedByNavigation { get; set; } = null!;
        public virtual LaboratoryMonth LaboratoryMonth { get; set; } = null!;
        public virtual User UpdatedByNavigation { get; set; } = null!;
    }
}
