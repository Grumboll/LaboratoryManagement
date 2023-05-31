using System;
using System.Collections.Generic;

namespace DiplomaWork.Models
{
    public partial class EmailCode
    {
        public uint Id { get; set; }
        public uint UserId { get; set; }
        public byte IsValid { get; set; }
        public DateTime ExpiredAt { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
