using RideshareConsole.IServices;
using RideshareConsole.Services;
using RideshareConsole.UI;

namespace RideshareConsole
{
    class Program
    {
        private static IUserService _userService;
        private static IRideService _rideService;
        private static MenuManager _menuManager;

        static void Main(string[] args)
        {
            InitializeServices();
            RunApplication();
        }

        private static void InitializeServices()
        {
            _userService = new UserService();
            _rideService = new RideService(_userService);
            _menuManager = new MenuManager(_userService, _rideService); // Pass dependencies to MenuManager
        }

        private static void RunApplication()
        {
            while (true)
            {
                try
                {
                    if (_userService.GetCurrentUser() == null)
                    {
                        _menuManager.ShowLoginMenu(); // Redirect to Login Menu
                    }
                    else
                    {
                        _menuManager.ShowMainMenu(); // Redirect to Main Menu
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }
    }
}
