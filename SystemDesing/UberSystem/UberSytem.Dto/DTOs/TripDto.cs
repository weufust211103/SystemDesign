using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UberSytem.Dto.DTOs
{
    public class TripDto
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public long DriverId { get; set; }
        public double PickupLatitude { get; set; }
        public double PickupLongitude { get; set; }
        public double DropoffLatitude { get; set; }
        public double DropoffLongitude { get; set; }
        public DateTime PickupTime { get; set; }
        public DateTime DropoffTime { get; set; }
        public string Status { get; set; } // "waiting", "in-progress", "completed", etc.
    }
}
