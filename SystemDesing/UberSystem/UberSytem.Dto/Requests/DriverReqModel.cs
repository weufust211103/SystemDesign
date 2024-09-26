using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UberSytem.Dto.Requests
{
    public class DriverReqModel
    {
        [Required]
        public long? CabId { get; set; }

        [Required]
        public DateTime? Dob { get; set; }

        [Required]
        public double? LocationLatitude { get; set; }

        [Required]
        public double? LocationLongitude { get; set; }

        [Required]
        public long UserId { get; set; }  // Foreign key to associate with the User table
    }
}
