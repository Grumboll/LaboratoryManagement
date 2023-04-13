using System;
using System.Collections.Generic;

namespace DiplomaWork.Models
{
    public partial class LaboratoryMonthChemical
    {
        public uint Id { get; set; }
        public uint MonthId { get; set; }
        public ushort Year { get; set; }
        public string Name { get; set; } = null!;
        public decimal ChemicalExpenditure { get; set; }
        public decimal ExpensePerMeterSquared { get; set; }
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
