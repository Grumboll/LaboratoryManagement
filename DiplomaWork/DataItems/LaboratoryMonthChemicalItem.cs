using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomaWork.DataItems
{
    public class LaboratoryMonthChemicalItem
    {
        public uint? Id { get; set; } = 0;
        public string ChemicalName { get; set; }
        public string ChemicalExpenditure { get; set; }
        public string ExpensePerMeterSquared { get; set; }
    }
}
