using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RideshareConsole.Enum;
using RideshareConsole.Models;

namespace RideshareConsole.IServices
{
    public interface IRideService
    {
        string RequestRide(string riderId, string pickup, string drop);
        void AssignRide(string rideId, string driverId);
        void StartRide(string rideId);
        void CompleteRide(string rideId);
        void CancelRide(string rideId, string reason);
        List<Ride> GetAvailableRides();
        List<Ride> GetUserRides(string userId);
        Ride GetRideDetails(string rideId);

        List<Ride> GetDriverRides(string driverId);

        void StartLocationMonitoring(string rideId, List<string> waypoints, string emergencyContact);
        void HandleSafetyAlert(string rideId, bool responded);
        string GetCurrentLocation(string rideId);
        void UpdateRideStatus(string rideId, RideStatus newStatus);


    }
}
