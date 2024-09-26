using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UberSytem.Dto.DTOs
{
    public class DriverDto
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public double LocationLatitude { get; set; }
        public double LocationLongitude { get; set; }
        
    }
}
