using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagment.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50,ErrorMessage = "این نمیتونه بیشتر از 50 کاراکتر باشه")]
        public string Name { get; set; }
        //[RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$",ErrorMessage = "لطفا ایمیل معتبر وارد نمایید")]
        [Required]
        [EmailAddress(ErrorMessage = "لطفا ایمیل معتبر وارد نمایید")]
        [Display(Name="office Email")]
        public string Email { get; set; }
        [Required]
        public Dept? Department { get; set; }
        public string PhotoPath { get; set; }
    }
}
