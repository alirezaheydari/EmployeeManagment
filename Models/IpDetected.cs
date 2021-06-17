using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagment.Models
{
    public class IpDetected
    {
        [Key]
        public int Id { get; set; }
        public string Ip { get; set; }
        public int Pattern { get; set; }
        public DateTime requestTime { get; set; }
    }
}
