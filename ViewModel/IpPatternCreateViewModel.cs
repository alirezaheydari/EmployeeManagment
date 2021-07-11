﻿using EmployeeManagment.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EmployeeManagment.ViewModel
{
    public class IpPatternCreateViewModel
    {
        
        [Required]
        [MaxLength(50, ErrorMessage = "این نمیتونه بیشتر از 50 کاراکتر باشه")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Method*")]
        public HttpMethodEnum VerbsOrMethod { get; set; }
        public string UserAgent { get; set; }
        [MaxLength(3)]
        [Display(Name = "FirstIPPart")]
        public string FirstIpPart { get; set; }
        [MaxLength(3)]
        [Display(Name = "secondIPPart")]
        public string SeconIpdPart { get; set; }
        [MaxLength(3)]
        [Display(Name = "ThirdIPPart")]
        public string ThirdIpPart { get; set; }
        [MaxLength(3)]
        [Display(Name = "ForthIPPart")]
        public string ForthIpPart { get; set; }

        public bool Enabled { get; set; }
    }
}
