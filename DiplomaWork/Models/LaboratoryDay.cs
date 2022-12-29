using System;
using System.Collections.Generic;

namespace DiplomaWork.Models
{
    public partial class LaboratoryDay
    {
        public uint Id { get; set; }
        public DateTime Day { get; set; }

        public virtual LaboratoryMonthHasDay LaboratoryMonthHasDay { get; set; } = null!;
    }
}
