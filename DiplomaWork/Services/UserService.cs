using DiplomaWork;
using DiplomaWork.DataItems;
using DiplomaWork.Models;
using iText.Commons.Actions.Contexts;
using iText.Kernel.Geom;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using static Org.BouncyCastle.Utilities.Test.FixedSecureRandom;

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

        public static bool IsNameValid(string name)
        {
            string namePattern = @"^[A-Za-zА-Яа-я]+$";

            return Regex.IsMatch(name, namePattern);
        }

        public static bool IsPasswordValid(string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                return false;
            }

            bool isLengthValid = password.Length >= 8;
            bool hasSpecialSymbol = password.Any(c => !char.IsLetterOrDigit(c));
            bool hasUppercase = password.Any(c => char.IsUpper(c));
            bool hasNumber = password.Any(c => char.IsDigit(c));

            return isLengthValid && hasSpecialSymbol && hasUppercase && hasNumber;
        }

        public static int createUser(string username, string? email, DateOnly? dateOfBirth, string? phoneNumber, string password, string firstName, string lastName, uint roleId)
        {
            using (var dbContext = new laboratory_2023Context())
            {
                try
                {
                    string passwordSalt = GenerateSalt();

                    DateOnly? dateOnly = null;
                    if (!dateOfBirth.Equals(DateOnly.FromDateTime(DateTime.MinValue)))
                    {
                        dateOnly = dateOfBirth;
                    }

                    User newUser = new User
                    {
                        Username = username,
                        EMail = email,
                        DateOfBirth = dateOnly,
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
        
        public static int editUser(User user, string username, string email, DateOnly? dateOfBirth, string phoneNumber, string password, string firstName, string middleName, string lastName)
        {
            using (var dbContext = new laboratory_2023Context())
            {
                try
                {
                    user.Username = username;
                    user.EMail = email;
                    user.PhoneNumber = phoneNumber;
                    user.FirstName = firstName;
                    user.MiddleName = middleName;
                    user.LastName = lastName;

                    if (!dateOfBirth.Equals(DateOnly.FromDateTime(DateTime.MinValue)))
                    {
                        user.DateOfBirth = dateOfBirth;
                    }
                    else
                    {
                        user.DateOfBirth = null;
                    }

                    if (password != "")
                    {
                        string passwordSalt = GenerateSalt();
                        user.Password = HashPassword(password, passwordSalt);
                        user.PasswordSalt = passwordSalt;
                    }

                    user.UpdatedAt = DateTime.Now;
                    user.UpdatedBy = App.CurrentUser.Id;

                    dbContext.Entry(user).State = EntityState.Modified;
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

        public static User getUserById(uint userId)
        {
            using (var dbContext = new laboratory_2023Context())
            {
                return dbContext.Users.FirstOrDefault(x => x.Id == userId);
            }
        }
        
        public static User getUserByEmail(string email)
        {
            using (var dbContext = new laboratory_2023Context())
            {
                return dbContext.Users.FirstOrDefault(x => x.EMail == email);
            }
        }

        public static string getUserEmailCodeByUserId(uint userId)
        {
            using (var dbContext = new laboratory_2023Context())
            {
                return dbContext.EmailCodes
                    .Where(e => e.IsValid == 1 && e.User.Id == userId)
                    .Select(e => e.Code)
                    .First()
                    .ToString();
            }
        }
    }
}