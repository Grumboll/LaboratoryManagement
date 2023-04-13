using System;
using System.Collections.Generic;

namespace DiplomaWork.Models
{
    public partial class Month
    {
        public Month()
        {
            LaboratoryDays = new HashSet<LaboratoryDay>();
            LaboratoryMonthChemicals = new HashSet<LaboratoryMonthChemical>();
            LaboratoryMonths = new HashSet<LaboratoryMonth>();
        }

        public uint Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;

        public virtual ICollection<LaboratoryDay> LaboratoryDays { get; set; }
        public virtual ICollection<LaboratoryMonthChemical> LaboratoryMonthChemicals { get; set; }
        public virtual ICollection<LaboratoryMonth> LaboratoryMonths { get; set; }
    }
}
