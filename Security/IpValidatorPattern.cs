using EmployeeManagment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagment.Security
{
    public class IpValidatorPattern
    {
        public static IpPattern GetPattern(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
                return IpPattern.none;
            if (MatchWithPatternOne(ip))
                return IpPattern.PaternOne;



            return IpPattern.none;
        }
        public static bool MatchWithPatternOne(string ip)
        {
            var ipParts = ip.Split('.');
            var NotCorrectIp = ipParts.Length != 4 || ipParts.Length <= 3;
            if (NotCorrectIp)
            {
                return false;
            }

            int sumPart1 = 0, sumPart2 = 0;
            for (int i = 0; i < ipParts[0].Length; i++)
            {
                var temp = ipParts[0][i];
                if (char.IsDigit(temp))
                    sumPart1 += temp;

            }
            for (int i = 0; i < ipParts[1].Length; i++)
            {
                var temp = ipParts[1][i];
                if (char.IsDigit(temp))
                    sumPart2 += temp;

            }
            return sumPart1 == sumPart2;
        }
    }
}
