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

        public virtual Month Month { get; set; } = null!;
        public virtual LaboratoryMonthHasChemical LaboratoryMonthHasChemical { get; set; } = null!;
    }
}
