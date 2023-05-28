using DiplomaWork.DataItems;
using DiplomaWork.Models;
using iText.Kernel.Geom;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomaWork.Services
{
    public class ProfileService
    {
        public static List<ProfileSettingsItem> getProfilesListFilteredByNamePaginated(int startIndex, int pageSize, string filterText)
        {
            using (var dbContext = new laboratory_2023Context())
            {
                IQueryable<ProfileHasLengthsPerimeter> query = dbContext.ProfileHasLengthsPerimeters;

                if (!string.IsNullOrEmpty(filterText))
                {
                    query = query.Where(p => p.Profile.Name.Contains(filterText));
                }

                var profiles = query
                    .OrderBy(p => p.Id)
                    .Skip(startIndex)
                    .Take(pageSize)
                    .Where(p => p.DeletedAt == null)
                    .Select(p => new ProfileSettingsItem
                    {
                        Id = p.Id,
                        Name = p.Profile.Name,
                        Length = p.Length.ToString().TrimEnd('0').TrimEnd('.'),
                        Perimeter = p.Perimeter.ToString().TrimEnd('0').TrimEnd('.'),
                    })
                    .ToList();

                return profiles;
            }
        }

        public static ProfileSettingsItem GetProfileSettingsItemById(uint profileId)
        {
            using (var dbContext = new laboratory_2023Context())
            {
                var profile = dbContext.ProfileHasLengthsPerimeters
                    .Select(p => new ProfileSettingsItem
                    {
                        Id = p.Id,
                        Name = p.Profile.Name,
                        Length = p.Length.ToString().TrimEnd('0').TrimEnd('.'),
                        Perimeter = p.Perimeter.ToString().TrimEnd('0').TrimEnd('.'),
                    })
                    .FirstOrDefault(x => x.Id == profileId);

                return profile;
            }
        }

        public static int getTotalProfilesCount()
        {
            using (var dbContext = new laboratory_2023Context())
            {
                int totalCount = dbContext.ProfileHasLengthsPerimeters.Count();
                return totalCount;
            }
        }
        public static uint createProfile(string profileName, decimal profileLength, decimal profilePerimeter)
        {
            using (var dbContext = new laboratory_2023Context())
            {
                try
                {
                    var profile = dbContext.Profiles.FirstOrDefault(x => x.Name == profileName);
                    uint newProfileId = 0;

                    if (profile == null)
                    {
                        Profile newProfile = new Profile
                        {
                            Name = profileName,
                        };

                        dbContext.Profiles.Add(newProfile);

                        dbContext.SaveChanges();

                        newProfileId = newProfile.Id;
                    }
                    else
                    {
                        newProfileId = profile.Id;
                    }

                    ProfileHasLengthsPerimeter newProfileHasLengthPerimeter = new ProfileHasLengthsPerimeter
                    {
                        ProfileId = newProfileId,
                        Length = profileLength,
                        Perimeter = profilePerimeter,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        DeletedAt = null,
                        CreatedBy = App.CurrentUser.Id,
                        UpdatedBy = App.CurrentUser.Id,
                    };

                    dbContext.ProfileHasLengthsPerimeters.Add(newProfileHasLengthPerimeter);

                    dbContext.SaveChanges();

                    return newProfileHasLengthPerimeter.Id;
                }
                catch (Exception ex)
                {
                    dbContext.Database.CurrentTransaction?.Rollback();

                    Log.Error(ex, "An error occurred while saving changes.");
                    return 0;
                }
            }
        }

        public static ProfileHasLengthsPerimeter GetProfileHasLengthsPerimeterById(uint? profileHasLengthsPerimeterId)
        {
            using (var dbContext = new laboratory_2023Context())
            {
                return dbContext.ProfileHasLengthsPerimeters.Include(x => x.Profile).FirstOrDefault(x => x.Id == profileHasLengthsPerimeterId);
            }
        }
        
        public static Profile GetProfileById(uint? profileId)
        {
            using (var dbContext = new laboratory_2023Context())
            {
                return dbContext.Profiles.FirstOrDefault(x => x.Id == profileId);
            }
        }
        
        public static int editProfileHasLengthsPerimeter(uint? profileHasLengthsPerimeterId, string profileName, decimal profileLength, decimal profilePerimeter, bool? isChecked)
        {
            using (var dbContext = new laboratory_2023Context())
            {
                try
                {
                    var profileHasLengthsPerimeterToUpdate = GetProfileHasLengthsPerimeterById(profileHasLengthsPerimeterId);

                    //Name has changed, either create new Profile and ProfileHasLengthsPerimeter or update existing
                    if (profileHasLengthsPerimeterToUpdate.Profile.Name != profileName)
                    {
                        if ((bool)isChecked)
                        {
                            var profileToUpdate = GetProfileById(profileHasLengthsPerimeterToUpdate.Profile.Id);
                            profileToUpdate.Name = profileName;
                            dbContext.Entry(profileToUpdate).State = EntityState.Modified;
                        }
                        else
                        {
                            createProfile(profileName, profileLength, profilePerimeter);
                        }
                    }

                    profileHasLengthsPerimeterToUpdate.Length = profileLength;
                    profileHasLengthsPerimeterToUpdate.Perimeter = profilePerimeter;
                    profileHasLengthsPerimeterToUpdate.UpdatedAt = DateTime.Now;
                    profileHasLengthsPerimeterToUpdate.UpdatedBy = App.CurrentUser.Id;

                    dbContext.Entry(profileHasLengthsPerimeterToUpdate).State = EntityState.Modified;
                    dbContext.SaveChanges();

                    return 1;
                }
                catch (Exception ex)
                {
                    dbContext.Database.CurrentTransaction?.Rollback();

                    Log.Error(ex, "An error occurred while saving changes.");
                    return 0;
                }
            }
        }
    }
}
