using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagment.Models
{
    public interface IIpDetectedRepository
    {
        IpDetected add(string ip, int idPattern);
        List<IpDetected> GetAll();
    }
}
