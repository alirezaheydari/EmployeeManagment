using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagment.Models
{
    public class CustomIpPattern
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string VerbsOrMethod { get; set; }
        public string UserAgent { get; set; }
        public string FirstIpPart { get; set; }
        public string SeconIpdPart { get; set; }
        public string ThirdIpPart { get; set; }
        public string ForthIpPart { get; set; }

        [Required]
        public bool Enabled { get; set; }
    }
}
