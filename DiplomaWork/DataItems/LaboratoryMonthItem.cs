using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomaWork.DataItems
{
    class LaboratoryMonthItem
    {
        public uint Id { get; set; }
        public uint? ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string ProfileLength { get; set; }
        public string ProfilePerimeter { get; set; }
        public string MetersSquaredPerSample { get; set; }
        public string PaintedSamplesCount { get; set; }
        public string PaintedMetersSquared { get; set; }
        public string KilogramsPerMeter { get; set; }
    }
}
