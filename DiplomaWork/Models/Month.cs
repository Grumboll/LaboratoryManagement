using System;
using System.Collections.Generic;

namespace DiplomaWork.Models
{
    public partial class Month
    {
        public Month()
        {
            LaboratoryDays = new HashSet<LaboratoryDay>();
        }

        public uint Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;

        public virtual LaboratoryMonth LaboratoryMonth { get; set; } = null!;
        public virtual ICollection<LaboratoryDay> LaboratoryDays { get; set; }
    }
}
