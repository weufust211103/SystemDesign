using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UberSytem.Dto.DTOs
{
    public class RatingDto
    {
        public long CustomerId { get; set; }
        public long DriverId { get; set; }
        public long TripId { get; set; }
        public int Rating { get; set; }  // Rating score, e.g., 1 to 5
        public string Feedback { get; set; }
    }
}
