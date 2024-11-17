using System;
using RideshareConsole.Services;
using RideshareConsole.Enum;
using RideshareConsole.IServices;
using RideshareConsole.Models;

namespace RideshareConsole.UI
{
    public class MenuManager
    {
        private readonly IUserService _userService;
        private readonly IRideService _rideService;

        public MenuManager(IUserService userService, IRideService rideService)
        {
            _userService = userService;
            _rideService = rideService;
        }

        public void ShowLoginMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Login ===");

            Console.Write("Enter User ID: ");
            var userId = Console.ReadLine();
            Console.Write("Enter Password: ");
            var password = Console.ReadLine();

            try
            {
                _userService.Authenticate(userId, password);
                Console.WriteLine("Login successful!");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login failed: {ex.Message}");
                Console.WriteLine("Press any key to try again...");
                Console.ReadKey();
            }
        }

        public void ShowMainMenu()
        {
            var currentUser = _userService.GetCurrentUser();
            Console.Clear();
            Console.WriteLine($"=== Main Menu ({currentUser.Role}) ===");

            switch (currentUser.Role)
            {
                case UserRole.Rider:
                    ShowRiderMenu();
                    break;
                case UserRole.Driver:
                    ShowDriverMenu();
                    break;
                case UserRole.Admin:
                    ShowAdminMenu();
                    break;
            }
        }

        private void ShowRiderMenu()
        {
            Console.Clear();
            Console.WriteLine("1. Book New Ride");
            Console.WriteLine("2. View My Rides");
            Console.WriteLine("3. View Ride Details");
            Console.WriteLine("4. Cancel Ride");
            Console.WriteLine("5. Setup Location Monitoring");
            Console.WriteLine("6. Logout");

            switch (Console.ReadLine())
            {
                case "1":
                    BookRide();
                    break;
                case "2":
                    ViewMyRides();
                    break;
                case "3":
                    ViewRideDetails();
                    break;
                case "4":
                    CancelRide();
                    break;

                case "5":
                    SetupLocationMonitoring();
                    break;
                case "6":
                    _userService.Logout();
                    Console.WriteLine("Logged out successfully!");
                    Console.ReadKey();
                    break;
                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    Console.ReadKey();
                    ShowRiderMenu();
                    break;
            }
        }
        private void ShowDriverMenu()
        {
            Console.Clear();
            Console.WriteLine("1. View and Accept Ride Requests");  
            Console.WriteLine("2. Post a Ride");
            Console.WriteLine("3. View My Assigned Rides");
            Console.WriteLine("4. Logout");

            switch (Console.ReadLine())
            {
                case "1":
                    ViewAndAcceptRideRequests();
                    break;
                case "2":
                    Console.Write("Enter your Driver ID: ");
                    string driverId = Console.ReadLine();
                    PostRide(driverId);
                    break;
                case "3":
                    ViewMyRides();
                    break;
                case "4":
                    _userService.Logout();
                    Console.WriteLine("Logged out successfully!");
                    Console.ReadKey();
                    break;
                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    Console.ReadKey();
                    ShowDriverMenu();
                    break;
            }
        }



        private void ShowAdminMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Admin Menu ===");
                Console.WriteLine("1. View All Users");
                Console.WriteLine("2. View All Rides");
                Console.WriteLine("3. Add New User");
                Console.WriteLine("4. Deactivate User");
                Console.WriteLine("5. View Ride Statistics");
                Console.WriteLine("6. Logout");

                switch (Console.ReadLine())
                {
                    case "1":
                        ViewAllUsers();
                        break;
                    case "2":
                        ViewAllRides();
                        break;
                    case "3":
                        AddNewUser();
                        break;
                    case "4":
                        DeactivateUser();
                        break;
                    case "5":
                        ViewRideStatistics();
                        break;
                    case "6":
                        _userService.Logout();
                        Console.WriteLine("Logged out successfully!");
                        Console.ReadKey();
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void BookRide()
        {
            Console.Clear();
            Console.Write("Enter Pickup Location: ");
            var pickup = Console.ReadLine();
            Console.Write("Enter Drop Location: ");
            var drop = Console.ReadLine();

            try
            {
                var currentUser = _userService.GetCurrentUser();
                var rideId = _rideService.RequestRide(currentUser.UserId, pickup, drop);
                Console.WriteLine($"Ride booked successfully! Ride ID: {rideId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Press any key to return to the menu...");
                Console.ReadKey();
            }
        }

        private void ViewMyRides()
        {
            Console.Clear();
            var currentUser = _userService.GetCurrentUser();
            var rides = _rideService.GetUserRides(currentUser.UserId);

            if (rides.Count == 0)
            {
                Console.WriteLine("No rides found.");
            }
            else
            {
                foreach (var ride in rides)
                {
                    Console.WriteLine($"Ride ID: {ride.Id}, Status: {ride.Status}, Fare: {ride.Fare}");
                }
            }

            Console.WriteLine("Press any key to return to the menu...");
            Console.ReadKey();
        }

      

        private void ViewRideDetails()
        {
            Console.Clear();
            Console.Write("Enter Ride ID: ");
            var rideId = Console.ReadLine();

            var ride = _rideService.GetRideDetails(rideId);
            if (ride != null)
            {
                Console.WriteLine($"Ride ID: {ride.Id}");
                Console.WriteLine($"Pickup: {ride.PickupLocation}");
                Console.WriteLine($"Drop: {ride.DropLocation}");
                Console.WriteLine($"Status: {ride.Status}");
                Console.WriteLine($"Fare: {ride.Fare}");
                Console.WriteLine($"Booking Time: {ride.BookingTime}");
            }
            else
            {
                Console.WriteLine("Ride not found.");
            }

            Console.WriteLine("Press any key to return to the menu...");
            Console.ReadKey();
        }

        private void CancelRide()
        {
            Console.Clear();
            Console.Write("Enter Ride ID to cancel: ");
            var rideId = Console.ReadLine();
            Console.Write("Enter Reason for cancellation: ");
            var reason = Console.ReadLine();

            try
            {
                _rideService.CancelRide(rideId, reason);
                Console.WriteLine("Ride cancelled successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("Press any key to return to the menu...");
            Console.ReadKey();
        }
        public void PostRide(string driverId)
        {
            Console.Clear();
            Console.WriteLine("Post a New Ride");

            Console.Write("Enter Pickup Location: ");
            string pickup = Console.ReadLine();

            Console.Write("Enter Destination: ");
            string destination = Console.ReadLine();

            Console.Write("Enter Available Seats: ");
            int seatsAvailable = int.Parse(Console.ReadLine());

            try
            {
                string rideId = _rideService.RequestRide(driverId, pickup, destination);
                Console.WriteLine($"Ride posted successfully! Ride ID: {rideId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error posting ride: {ex.Message}");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private void ViewAndAcceptRideRequests()
        {
            Console.Clear();
            Console.WriteLine("=== Available Ride Requests ===\n");

            var currentUser = _userService.GetCurrentUser();
            var availableRides = _rideService.GetAvailableRides();

            if (!availableRides.Any())
            {
                Console.WriteLine("No ride requests available at this time.");
                Console.WriteLine("\nPress any key to return to menu...");
                Console.ReadKey();
                return;
            }

            for (int i = 0; i < availableRides.Count; i++)
            {
                var ride = availableRides[i];
                Console.WriteLine($"{i + 1}. Ride ID: {ride.Id}");
                Console.WriteLine($"   From: {ride.PickupLocation}");
                Console.WriteLine($"   To: {ride.DropLocation}");
                Console.WriteLine($"   Fare: ${ride.Fare}");
                Console.WriteLine($"   Booking Time: {ride.BookingTime}\n");
            }

            Console.WriteLine("\nEnter the number of the ride you want to accept (or 0 to return to menu):");

            if (int.TryParse(Console.ReadLine(), out int selection) && selection > 0 && selection <= availableRides.Count)
            {
                var selectedRide = availableRides[selection - 1];
                Console.WriteLine($"\nYou selected the ride from {selectedRide.PickupLocation} to {selectedRide.DropLocation}");
                Console.Write("Do you want to accept this ride? (Y/N): ");

                if (Console.ReadLine()?.ToUpper() == "Y")
                {
                    try
                    {
                        _rideService.AssignRide(selectedRide.Id, currentUser.UserId);
                        _rideService.UpdateRideStatus(selectedRide.Id, RideStatus.InProgress);

                        Console.WriteLine("\nRide successfully assigned to you and is now in progress!");
                        Console.WriteLine("The rider will now be able to use safety monitoring features.");

                        Console.WriteLine("\nRide Details:");
                        Console.WriteLine($"Ride ID: {selectedRide.Id}");
                        Console.WriteLine($"Pickup: {selectedRide.PickupLocation}");
                        Console.WriteLine($"Destination: {selectedRide.DropLocation}");
                        Console.WriteLine($"Status: In Progress");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\nError assigning ride: {ex.Message}");
                    }
                }
            }

            Console.WriteLine("\nPress any key to return to menu...");
            Console.ReadKey();
        }


        public void ViewRideDetails(string rideId)
        {
            var ride = _rideService.GetRideDetails(rideId);
            if (ride != null)
            {
                Console.Clear();
                Console.WriteLine("Ride Details:");
                Console.WriteLine($"Rider ID: {ride.RiderId}");
                Console.WriteLine($"Pickup Location: {ride.PickupLocation}");
                Console.WriteLine($"Drop Location: {ride.DropLocation}");
                Console.WriteLine($"Fare: {ride.Fare:C}");
                Console.WriteLine($"Status: {ride.Status}");

                if (ride.Status == RideStatus.Requested)
                {
                    Console.WriteLine("Approve or Reject ride? (A/R)");
                    string choice = Console.ReadLine();

                    var currentUser = _userService.GetCurrentUser();
                    if (choice?.ToUpper() == "A")
                    {
                        _rideService.AssignRide(rideId, currentUser.UserId);
                        Console.WriteLine("Ride approved and assigned.");
                    }
                }
            }
            else
            {
                Console.WriteLine("Ride not found.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public void ViewTripDetails(string rideId)
        {
            var ride = _rideService.GetRideDetails(rideId);
            if (ride != null && ride.Status == RideStatus.InProgress)
            {
                Console.Clear();
                Console.WriteLine("Trip Details:");
                Console.WriteLine($"Pickup Location: {ride.PickupLocation}");
                Console.WriteLine($"Drop Location: {ride.DropLocation}");
                Console.WriteLine($"Current Location: {ride.PickupLocation} (update based on GPS)");
                Console.WriteLine($"Estimated Time of Arrival: {ride.BookingTime.AddMinutes(15):HH:mm}");
                Console.WriteLine($"Ride Status: {ride.Status}");
            }
            else
            {
                Console.WriteLine("Ride not in progress or invalid ride ID.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private void ViewAllUsers()
        {
            Console.Clear();
            Console.WriteLine("=== All Users ===\n");

            var users = _userService.GetAllUsers();
            if (!users.Any())
            {
                Console.WriteLine("No users found.");
            }
            else
            {
                Console.WriteLine("ID\tName\t\tRole\tStatus");
                Console.WriteLine("----------------------------------------");
                foreach (var user in users)
                {
                    Console.WriteLine($"{user.UserId}\t{user.Name}\t{user.Role}\t{(user.IsActive ? "Active" : "Inactive")}");
                }
            }

           // Console.WriteLine("\nPress any key to return to menu...");
            Console.ReadKey();
        }

        private void ViewAllRides()
        {
            Console.Clear();
            Console.WriteLine("=== All Rides ===\n");

            var rides = _rideService.GetAvailableRides();
            if (!rides.Any())
            {
                Console.WriteLine("No rides found.");
            }
            else
            {
                Console.WriteLine("ID\tRider\tDriver\tStatus\tFare\tPickup\t\tDrop");
                Console.WriteLine("----------------------------------------------------------------------------");
                foreach (var ride in rides)
                {
                    Console.WriteLine($"{ride.Id}\t{ride.RiderId}\t{ride.DriverId ?? "N/A"}\t{ride.Status}\t${ride.Fare}\t{ride.PickupLocation}\t{ride.DropLocation}");
                }
            }

            Console.WriteLine("\nPress any key to return to menu...");
            Console.ReadKey();
        }

        private void AddNewUser()
        {
            Console.Clear();
            Console.WriteLine("=== Add New User ===\n");

            try
            {
                Console.Write("Enter User ID: ");
                string userId = Console.ReadLine();

                Console.Write("Enter Password: ");
                string password = Console.ReadLine();

                Console.Write("Enter Name: ");
                string name = Console.ReadLine();

                Console.WriteLine("\nSelect Role:");
                Console.WriteLine("1. Rider");
                Console.WriteLine("2. Driver");
                Console.WriteLine("3. Admin");
                Console.Write("Enter choice (1-3): ");

                UserRole role = Console.ReadLine() switch
                {
                    "1" => UserRole.Rider,
                    "2" => UserRole.Driver,
                    "3" => UserRole.Admin,
                    _ => throw new Exception("Invalid role selection")
                };

                var newUser = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    Password = password,
                    Name = name,
                    Role = role,
                    IsActive = true
                };

                _userService.AddUser(newUser);
                Console.WriteLine("\nUser added successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError adding user: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to return to menu...");
            Console.ReadKey();
        }

        private void DeactivateUser()
        {
            Console.Clear();
            Console.WriteLine("=== Deactivate User ===\n");

            ViewAllUsers();
            Console.Write("\nEnter User ID to deactivate: ");
            string userId = Console.ReadLine();

            try
            {
                _userService.DeactivateUser(userId);
                Console.WriteLine("User deactivated successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deactivating user: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to return to menu...");
            Console.ReadKey();
        }

        private void ViewRideStatistics()
        {
            Console.Clear();
            Console.WriteLine("=== Ride Statistics ===\n");

            var allRides = _rideService.GetAvailableRides();

            var totalRides = allRides.Count;
            var completedRides = allRides.Count(r => r.Status == RideStatus.Completed);
            var cancelledRides = allRides.Count(r => r.Status == RideStatus.Cancelled);
            var activeRides = allRides.Count(r => r.Status == RideStatus.InProgress);
            var totalRevenue = allRides.Where(r => r.Status == RideStatus.Completed).Sum(r => r.Fare);

            Console.WriteLine($"Total Rides: {totalRides}");
            Console.WriteLine($"Completed Rides: {completedRides}");
            Console.WriteLine($"Cancelled Rides: {cancelledRides}");
            Console.WriteLine($"Active Rides: {activeRides}");
            Console.WriteLine($"Total Revenue: ${totalRevenue:F2}");

            if (totalRides > 0)
            {
                Console.WriteLine($"Completion Rate: {((double)completedRides / totalRides * 100):F1}%");
                Console.WriteLine($"Cancellation Rate: {((double)cancelledRides / totalRides * 100):F1}%");
            }

            Console.WriteLine("\nPress any key to return to menu...");
            Console.ReadKey();
        }

        private void SetupLocationMonitoring()
        {
            Console.Clear();
            Console.WriteLine("=== Setup Safety Location Monitoring ===\n");

            Console.Write("Enter Ride ID: ");
            var rideId = Console.ReadLine();

            var ride = _rideService.GetRideDetails(rideId);
            if (ride == null)
            {
                Console.WriteLine("Ride not found.");
                Console.ReadKey();
                return;
            }

            Console.Write("Enter Emergency Contact: ");
            var emergencyContact = Console.ReadLine();

            Console.WriteLine("\nEnter waypoint locations (one per line, empty line to finish):");
            var waypoints = new List<string>();
            while (true)
            {
                var waypoint = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(waypoint)) break;
                waypoints.Add(waypoint);
            }

            try
            {
                _rideService.StartLocationMonitoring(rideId, waypoints, emergencyContact);
                Task.Run(() => MonitorLocation(rideId));
                Console.WriteLine("Location monitoring started successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("Press any key to return to menu...");
            Console.ReadKey();
        }

        private async Task MonitorLocation(string rideId)
        {
            var ride = _rideService.GetRideDetails(rideId);
            while (ride.Status == RideStatus.InProgress && ride.IsLocationMonitoring)
            {
                var currentLocation = _rideService.GetCurrentLocation(rideId);
                if (IsNearWaypoint(currentLocation, ride.WaypointLocations))
                {
                    bool responded = ShowSafetyAlert();
                    _rideService.HandleSafetyAlert(rideId, responded);
                }
                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }

        private bool IsNearWaypoint(string currentLocation, List<string> waypoints)
        {
            return true;
        }

        private bool ShowSafetyAlert()
        {
            Console.Clear();
            Console.WriteLine("!!! SAFETY CHECK !!!");
            Console.WriteLine("Are you safe? Type 'YES' within 30 seconds:");

            var task = Task.Run(() =>
            {
                var response = Console.ReadLine();
                return response?.Trim().ToUpper() == "YES";
            });

            if (Task.WaitAny(new Task[] { task }, TimeSpan.FromSeconds(30)) == -1)
            {
                return false;
            }

            return task.Result;
        }

    }
}



//8d78bee-247e-4779-97b1-ab2be6603cbf