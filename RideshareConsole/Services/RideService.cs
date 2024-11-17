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
    public class RideService : IRideService
    {
        private readonly List<Ride> _rides;
        private readonly IUserService _userService;
        private const int MAX_MISSED_ALERTS = 5;
        private readonly Dictionary<string, DateTime> _lastAlertTimes;

        public RideService(IUserService userService)
        {
            _rides = new List<Ride>();
            _userService = userService;
            _lastAlertTimes = new Dictionary<string, DateTime>();
        }

        public string RequestRide(string riderId, string pickup, string drop)
        {
            var ride = new Ride
            {
                Id = Guid.NewGuid().ToString(),
                RiderId = riderId,
                PickupLocation = pickup,
                DropLocation = drop,
                BookingTime = DateTime.Now,
                Status = RideStatus.Requested,
                Fare = CalculateFare(pickup, drop)
            };

            _rides.Add(ride);
            return ride.Id;
        }

        public void AssignRide(string rideId, string driverId)
        {
            var ride = GetRideDetails(rideId);
            if (ride != null)
            {
                ride.DriverId = driverId;
                ride.Status = RideStatus.Assigned;
            }
        }

        public void StartRide(string rideId)
        {
            var ride = GetRideDetails(rideId);
            if (ride != null)
            {
                ride.Status = RideStatus.InProgress;
            }
        }

        public void CompleteRide(string rideId)
        {
            var ride = GetRideDetails(rideId);
            if (ride != null)
            {
                ride.Status = RideStatus.Completed;
            }
        }

        public void CancelRide(string rideId, string reason)
        {
            var ride = GetRideDetails(rideId);
            if (ride != null)
            {
                ride.Status = RideStatus.Cancelled;
                ride.CancellationReason = reason;
            }
            else
            {
                throw new Exception("Ride not found.");
            }
        }

        public List<Ride> GetAvailableRides()
        {
            return _rides.Where(r => r.Status == RideStatus.Requested && r.DriverId == null).ToList();
        }

        public List<Ride> GetUserRides(string userId)
        {
            var currentUser = _userService.GetCurrentUser();
            return currentUser.Role == UserRole.Driver
                ? _rides.Where(r => r.DriverId == userId).ToList()
                : _rides.Where(r => r.RiderId == userId).ToList();
        }

        public Ride GetRideDetails(string rideId)
        {
            return _rides.FirstOrDefault(r => r.Id == rideId);
        }

        private decimal CalculateFare(string pickup, string drop)
        {
            return 200.00m;
        }

        public List<Ride> GetDriverRides(string driverId)
        {
            return _rides.Where(r => r.DriverId == driverId).ToList();
        }

        public void StartLocationMonitoring(string rideId, List<string> waypoints, string emergencyContact)
        {
            var ride = GetRideDetails(rideId);
            if (ride != null)
            {
                ride.WaypointLocations = waypoints;
                ride.EmergencyContact = emergencyContact;
                ride.IsLocationMonitoring = true;
                ride.AlertsMissed = 0;

                // Add or update the last alert time
                if (_lastAlertTimes.ContainsKey(rideId))
                {
                    _lastAlertTimes[rideId] = DateTime.Now;
                }
                else
                {
                    _lastAlertTimes.Add(rideId, DateTime.Now);
                }

                CheckSafetyStatus(ride);
            }
        }

        public void HandleSafetyAlert(string rideId, bool responded)
        {
            var ride = GetRideDetails(rideId);
            if (ride != null && ride.IsLocationMonitoring)
            {
                if (!responded)
                {
                    ride.AlertsMissed++;
                    LogSafetyEvent(ride, $"Missed safety check #{ride.AlertsMissed}");

                    if (ride.AlertsMissed >= MAX_MISSED_ALERTS)
                    {
                        SendEmergencyAlert(ride);
                        ride.Status = RideStatus.SafetyAlert;
                    }
                }
                else
                {
                    ride.AlertsMissed = 0;
                    LogSafetyEvent(ride, "Safety check acknowledged");
                }
                _lastAlertTimes[rideId] = DateTime.Now;
            }
        }

        private void CheckSafetyStatus(Ride ride)
        {
            if (ride == null || !_lastAlertTimes.ContainsKey(ride.Id))
                return;

            var timeSinceLastAlert = DateTime.Now - _lastAlertTimes[ride.Id];
            if (timeSinceLastAlert.TotalMinutes >= 5) // Check every 5 minutes
            {
                // If no response received, handle as missed alert
                HandleSafetyAlert(ride.Id, false);
            }
        }

        private void SendEmergencyAlert(Ride ride)
        {
            var currentLocation = GetCurrentLocation(ride.Id);
            var alertTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            var message = new StringBuilder();
            message.AppendLine($"⚠️ EMERGENCY SAFETY ALERT ⚠️");
            message.AppendLine($"Time: {alertTime}");
            message.AppendLine($"Rider ID: {ride.RiderId}");
            message.AppendLine($"Driver ID: {ride.DriverId}");
            message.AppendLine($"Current Location: {currentLocation}");
            message.AppendLine($"Trip: {ride.PickupLocation} → {ride.DropLocation}");
            message.AppendLine($"Missed Checks: {ride.AlertsMissed}");
            message.AppendLine($"Emergency Contact: {ride.EmergencyContact}");

            LogSafetyEvent(ride, message.ToString());
            NotifyEmergencyContact(ride.EmergencyContact, message.ToString());
            UpdateRideStatus(ride.Id, RideStatus.SafetyAlert);
        }

        private void LogSafetyEvent(Ride ride, string message)
        {
            if (ride.SafetyLog == null)
                ride.SafetyLog = new List<string>();

            ride.SafetyLog.Add($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}");
            Console.WriteLine($"Safety Event - Ride {ride.Id}: {message}");
        }

        private void NotifyEmergencyContact(string contact, string message)
        {
            Console.WriteLine($"\nEmergency Alert to {contact}:");
            Console.WriteLine(message);
        }

        public List<string> GetSafetyLog(string rideId)
        {
            var ride = GetRideDetails(rideId);
            return ride?.SafetyLog ?? new List<string>();
        }

        public bool IsSafetyAlertActive(string rideId)
        {
            var ride = GetRideDetails(rideId);
            return ride?.Status == RideStatus.SafetyAlert;
        }

        public string GetCurrentLocation(string rideId)
        {
            var ride = GetRideDetails(rideId);
            if (ride != null)
            {
                return $"Simulated location between {ride.PickupLocation} and {ride.DropLocation}";
            }
            return null;
        }

        public void UpdateRideStatus(string rideId, RideStatus newStatus)
        {
            var ride = GetRideDetails(rideId);
            if (ride == null)
            {
                throw new Exception("Ride not found");
            }

            ride.Status = newStatus;
        }
    }
}
