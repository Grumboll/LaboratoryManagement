using System;
using System.Collections.Generic;

namespace DiplomaWork.Models
{
    public partial class User
    {
        public User()
        {
            InverseCreatedByNavigation = new HashSet<User>();
            InverseUpdatedByNavigation = new HashSet<User>();
            LaboratoryDayHasProfileCreatedByNavigations = new HashSet<LaboratoryDayHasProfile>();
            LaboratoryDayHasProfileUpdatedByNavigations = new HashSet<LaboratoryDayHasProfile>();
            LaboratoryMonthHasDayHasChemicalCreatedByNavigations = new HashSet<LaboratoryMonthHasDayHasChemical>();
            LaboratoryMonthHasDayHasChemicalUpdatedByNavigations = new HashSet<LaboratoryMonthHasDayHasChemical>();
            ProfileCreatedByNavigations = new HashSet<Profile>();
            ProfileUpdatedByNavigations = new HashSet<Profile>();
            UserHasRoles = new HashSet<UserHasRole>();
        }

        public uint Id { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = null!;
        public bool IsLocked { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public uint CreatedBy { get; set; }
        public uint UpdatedBy { get; set; }

        public virtual User CreatedByNavigation { get; set; } = null!;
        public virtual User UpdatedByNavigation { get; set; } = null!;
        public virtual ICollection<User> InverseCreatedByNavigation { get; set; }
        public virtual ICollection<User> InverseUpdatedByNavigation { get; set; }
        public virtual ICollection<LaboratoryDayHasProfile> LaboratoryDayHasProfileCreatedByNavigations { get; set; }
        public virtual ICollection<LaboratoryDayHasProfile> LaboratoryDayHasProfileUpdatedByNavigations { get; set; }
        public virtual ICollection<LaboratoryMonthHasDayHasChemical> LaboratoryMonthHasDayHasChemicalCreatedByNavigations { get; set; }
        public virtual ICollection<LaboratoryMonthHasDayHasChemical> LaboratoryMonthHasDayHasChemicalUpdatedByNavigations { get; set; }
        public virtual ICollection<Profile> ProfileCreatedByNavigations { get; set; }
        public virtual ICollection<Profile> ProfileUpdatedByNavigations { get; set; }
        public virtual ICollection<UserHasRole> UserHasRoles { get; set; }
    }
}
