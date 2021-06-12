using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagment.Models
{
    public interface ICustomIpPatternRepository
    {
        CustomIpPattern GetEnabledPattern();
        CustomIpPattern Add(CustomIpPattern model);
        CustomIpPattern Update(CustomIpPattern model);
        IEnumerable<CustomIpPattern> GetAllPattern();
    }
}
