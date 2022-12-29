using System;
using System.Collections.Generic;

namespace DiplomaWork.Models
{
    public partial class LaboratoryMonth
    {
        public uint Id { get; set; }
        public uint Month { get; set; }
        public uint Year { get; set; }

        public virtual LaboratoryMonthHasDay LaboratoryMonthHasDay { get; set; } = null!;
    }
}
