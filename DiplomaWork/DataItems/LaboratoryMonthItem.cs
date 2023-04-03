using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomaWork.DataItems
{
    public class LaboratoryMonthItem
    {
        public uint? Id { get; set; }
        public DateTime? LaboratoryDayDate { get; set; }
        public string Kilograms { get; set; }
        public string MetersSquared { get; set; }
        public uint? LaboratoryMonthChemicalId { get; set; }
        public string? LaboratoryMonthChemical { get; set; }
        public string? ExpensePerMonthMetersSquared { get; set; }
    }
}
