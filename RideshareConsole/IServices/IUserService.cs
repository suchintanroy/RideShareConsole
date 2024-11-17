using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RideshareConsole.Models;

namespace RideshareConsole.IServices
{
    public interface IUserService
    {
        User Authenticate(string userId, string password);
        void Logout();
        User GetCurrentUser();
        List<User> GetAllUsers();
        void AddUser(User user);
        void DeactivateUser(string userId);
    }

}
