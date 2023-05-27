using DiplomaWork.DataItems;
using DiplomaWork.Models;
using iText.Kernel.Geom;
using System;
using System.Collections.Generic;
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

        public static int getTotalProfilesCount()
        {
            using (var dbContext = new laboratory_2023Context())
            {
                int totalCount = dbContext.ProfileHasLengthsPerimeters.Count();
                return totalCount;
            }
        }
    }
}
