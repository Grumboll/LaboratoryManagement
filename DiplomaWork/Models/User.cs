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
            LaboratoryDayCreatedByNavigations = new HashSet<LaboratoryDay>();
            LaboratoryDayUpdatedByNavigations = new HashSet<LaboratoryDay>();
            LaboratoryMonthChemicalCreatedByNavigations = new HashSet<LaboratoryMonthChemical>();
            LaboratoryMonthChemicalUpdatedByNavigations = new HashSet<LaboratoryMonthChemical>();
            LaboratoryMonthCreatedByNavigations = new HashSet<LaboratoryMonth>();
            LaboratoryMonthUpdatedByNavigations = new HashSet<LaboratoryMonth>();
            ProfileCreatedByNavigations = new HashSet<Profile>();
            ProfileUpdatedByNavigations = new HashSet<Profile>();
            UserHasRoles = new HashSet<UserHasRole>();
        }

        public uint Id { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string PasswordSalt { get; set; } = null!;
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
        public virtual ICollection<LaboratoryDay> LaboratoryDayCreatedByNavigations { get; set; }
        public virtual ICollection<LaboratoryDay> LaboratoryDayUpdatedByNavigations { get; set; }
        public virtual ICollection<LaboratoryMonthChemical> LaboratoryMonthChemicalCreatedByNavigations { get; set; }
        public virtual ICollection<LaboratoryMonthChemical> LaboratoryMonthChemicalUpdatedByNavigations { get; set; }
        public virtual ICollection<LaboratoryMonth> LaboratoryMonthCreatedByNavigations { get; set; }
        public virtual ICollection<LaboratoryMonth> LaboratoryMonthUpdatedByNavigations { get; set; }
        public virtual ICollection<Profile> ProfileCreatedByNavigations { get; set; }
        public virtual ICollection<Profile> ProfileUpdatedByNavigations { get; set; }
        public virtual ICollection<UserHasRole> UserHasRoles { get; set; }
    }
}
