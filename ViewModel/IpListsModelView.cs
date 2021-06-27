using EmployeeManagment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagment.ViewModel
{
    public class IpDetectedModelView : IpDetected
    {
        private readonly ICustomIpPatternRepository patternRepository;
        public IpDetectedModelView(ICustomIpPatternRepository patternRepository,IpDetected ip)
        {
            this.Id = ip.Id;
            this.Ip = ip.Ip;
            this.Pattern = ip.Pattern;
            this.requestTime = ip.requestTime;
            this.patternRepository = patternRepository;
            this.PatternName = patternRepository.Get(this.Pattern) != null ? patternRepository.Get(this.Pattern).Title : "NOT HAVE";
        }
        public string PatternName { get; set; }
    }
}
