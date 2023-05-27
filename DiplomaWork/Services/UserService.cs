using DiplomaWork.DataItems;
using DiplomaWork.Models;
using iText.Kernel.Geom;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace DiplomaWork.Services
{
    public class UserService
    {
        public static List<UserItem> getUsersListFilteredByNamePaginated(int startIndex, int pageSize, string filterText)
        {
            using (var dbContext = new laboratory_2023Context())
            {
                IQueryable<User> query = dbContext.Users;

                if (!string.IsNullOrEmpty(filterText))
                {
                    query = query.Where(u => u.Username.Contains(filterText));
                }

                var users = query
                    .OrderBy(p => p.Id)
                    .Skip(startIndex)
                    .Take(pageSize)
                    .Select(u => new UserItem
                    {
                        Id = u.Id,
                        Username = u.Username,
                        EMail = u.EMail,
                        PhoneNumber = u.PhoneNumber,
                        DateOfBirth = u.DateOfBirth.ToString(),
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        IsLocked = u.IsLocked ? "Да" : "Не",
                    }).ToList();

                return users;
            }
        }

        public static int getTotalUsersCount()
        {
            using (var dbContext = new laboratory_2023Context())
            {
                int totalCount = dbContext.Users.Count();
                return totalCount;
            }
        }
    }
}