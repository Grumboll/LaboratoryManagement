using System;
using System.Collections.Generic;

namespace DiplomaWork.Models
{
    public partial class LaboratoryMonthHasDay
    {
        public uint Id { get; set; }
        public uint LaboratoryMonthId { get; set; }
        public uint LaboratoryDayId { get; set; }
        public float? Kilograms { get; set; }
        public float MetersSquared { get; set; }

        public virtual LaboratoryDay LaboratoryDay { get; set; } = null!;
        public virtual LaboratoryMonth LaboratoryMonth { get; set; } = null!;
        public virtual LaboratoryMonthHasDayHasChemical LaboratoryMonthHasDayHasChemical { get; set; } = null!;
    }
}
