using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RideshareConsole.Enum;
using RideshareConsole.IServices;
using RideshareConsole.Models;

namespace RideshareConsole.Services
{
    public class UserService : IUserService
    {
        private User _currentUser;
        private readonly Dictionary<string, User> _users;

        public UserService()
        {
            _users = new Dictionary<string, User>
            {
                {
                    "20001@hexaware.com", new User {
                        Id = "1",
                        UserId = "20001@hexaware.com",
                        Password = "pass123",
                        Name = "Roy",
                        Role = UserRole.Rider,
                        IsActive = true
                    }
                },
                {
                    "20002@hexaware.com", new User {
                        Id = "2",
                        UserId = "20002@hexaware.com",
                        Password = "pass123",
                        Name = "Satya",
                        Role = UserRole.Driver,
                        IsActive = true
                    }
                },
                {
                    "20003@hexaware.com", new User {
                        Id = "3",
                        UserId = "20003@hexaware.com",
                        Password = "pass123",
                        Name = "Ganesh",
                        Role = UserRole.Admin,
                        IsActive = true
                    }
                }
            };
        }

        public User Authenticate(string userId, string password)
        {
            if (!_users.TryGetValue(userId, out User user))
            {
                throw new Exception("User ID not found");
            }

            if (!user.IsActive)
            {
                throw new Exception("User account is deactivated");
            }

            if (user.Password != password)
            {
                throw new Exception("Invalid password");
            }

            _currentUser = user;
            return user;
        }

        public void Logout()
        {
            _currentUser = null;
        }

        public User GetCurrentUser()
        {
            return _currentUser;
        }

        public List<User> GetAllUsers()
        {
            return _users.Values.ToList();
        }

        public void AddUser(User user)
        {
            if (_users.ContainsKey(user.UserId))
            {
                throw new Exception("User ID already exists");
            }
            _users.Add(user.UserId, user);
        }

        public void DeactivateUser(string userId)
        {
            if (_users.TryGetValue(userId, out User user))
            {
                user.IsActive = false;
            }
        }
    }
}
