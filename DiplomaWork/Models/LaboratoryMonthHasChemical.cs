using System;
using System.Collections.Generic;

namespace DiplomaWork.Models
{
    public partial class LaboratoryMonthHasChemical
    {
        public uint Id { get; set; }
        public uint MonthId { get; set; }
        public ushort Year { get; set; }
        public string Name { get; set; } = null!;
        public decimal ChemicalExpenditure { get; set; }
        public decimal ExpensePerMeterSquared { get; set; }

        public virtual LaboratoryMonth Month { get; set; } = null!;
    }
}
