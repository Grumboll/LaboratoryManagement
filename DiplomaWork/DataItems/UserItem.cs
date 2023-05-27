using DiplomaWork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomaWork.DataItems
{ 
    public class UserItem
    {
        public uint Id { get; set; }
        public string Username { get; set; }
        public string EMail { get; set; }
        public string PhoneNumber { get; set; }
        public string DateOfBirth { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IsLocked { get; set; }
    }
}
