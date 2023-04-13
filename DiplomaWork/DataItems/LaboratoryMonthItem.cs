using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomaWork.DataItems
{
    public class LaboratoryMonthItem
    {
        public uint? Id { get; set; } = 0;
        public DateTime? LaboratoryDayDate { get; set; }
        public string Kilograms { get; set; }
        public string MetersSquared { get; set; }
    }
}
