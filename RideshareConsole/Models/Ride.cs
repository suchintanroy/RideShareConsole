using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RideshareConsole.Enum;

namespace RideshareConsole.Models
{
    public class Ride
    {
        public string Id { get; set; }
        public string DriverId { get; set; }
        public string RiderId { get; set; }
        public string PickupLocation { get; set; }
        public string DropLocation { get; set; }
        public DateTime BookingTime { get; set; }
        public decimal Fare { get; set; }
        public RideStatus Status { get; set; }
        public string CancellationReason { get; set; }

        public List<string> WaypointLocations { get; set; } = new List<string>();
        public string EmergencyContact { get; set; }
        public int AlertsMissed { get; set; } = 0;
        public bool IsLocationMonitoring { get; set; } = false;
        public List<string> SafetyLog { get; set; }
    }

}
