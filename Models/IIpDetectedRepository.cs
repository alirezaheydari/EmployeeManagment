using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagment.Models
{
    public interface IIpDetectedRepository
    {
        IpDetected add(string ip, int idPattern);
        void DeleteLastOne();
        List<IpDetected> GetAll();
        IpDetected Delete(int id);
        void DeleteAll();
    }
}
