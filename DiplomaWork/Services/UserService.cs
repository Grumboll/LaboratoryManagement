using DiplomaWork.DataItems;
using DiplomaWork.Models;
using iText.Kernel.Geom;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Cryptography;
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

        public static void lockUnlockUserById(uint? userId)
        {
            using (var dbContext = new laboratory_2023Context())
            {
                var userToLock = dbContext.Users.FirstOrDefault(x => x.Id == userId);

                if (userToLock.IsLocked)
                {
                    userToLock.IsLocked = false;
                }
                else
                {
                    userToLock.IsLocked = true;
                }

                dbContext.Entry(userToLock).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }
        
        public static void forcePasswordResetToUserById(uint? userId)
        {
            using (var dbContext = new laboratory_2023Context())
            {
                var userToLock = dbContext.Users.FirstOrDefault(x => x.Id == userId);

                userToLock.PasswordResetRequired = true;

                dbContext.Entry(userToLock).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public static string GenerateSalt()
        {
            const string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(allowedChars, 16)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string HashPassword(string password, string salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password + salt);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public static uint createUser(string username, string? email, DateOnly? dateOfBirth, string? phoneNumber, string password, string firstName, string lastName, uint roleId)
        {
            using (var dbContext = new laboratory_2023Context())
            {
                try
                {
                    string passwordSalt = GenerateSalt();

                    User newUser = new User
                    {
                        Username = username,
                        EMail = email,
                        DateOfBirth = dateOfBirth,
                        PhoneNumber = phoneNumber,
                        Password = HashPassword(password, passwordSalt),
                        PasswordSalt = passwordSalt,
                        FirstName = firstName,
                        LastName = lastName,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        DeletedAt = null,
                        CreatedBy = App.CurrentUser.Id,
                        UpdatedBy = App.CurrentUser.Id,
                    };

                    dbContext.Users.Add(newUser);

                    dbContext.SaveChanges();

                    UserHasRole newUserHasRole = new UserHasRole
                    {
                        UserId = newUser.Id,
                        RoleId = roleId
                    };

                    dbContext.UserHasRoles.Add(newUserHasRole);
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