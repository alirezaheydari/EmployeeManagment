using EmployeeManagment.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace EmployeeManagment.Security
{
    public class AdminSafeListMiddleware
    {
        private readonly RequestDelegate _next;
        private IIpDetectedRepository ipDetected;
        private ICustomIpPatternRepository ipPattern;
        private readonly ILogger<AdminSafeListMiddleware> _logger;
        private int trackerCounter = 0;

        public AdminSafeListMiddleware(
            RequestDelegate next,
            ILogger<AdminSafeListMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, IIpDetectedRepository ipDetected, AppDbContext appDb, ICustomIpPatternRepository ipPattern)
        {
            this.ipDetected = ipDetected;
            this.ipPattern = ipPattern;

            var pattern = ipPattern.GetEnabledPattern();
            var remoteIp = context.Connection.RemoteIpAddress;


            if (context.Request.Method.Equals(pattern.VerbsOrMethod,StringComparison.OrdinalIgnoreCase))
            {


                var IpParts = remoteIp.ToString().Split('.');

                if (IpParts.Length != 4)
                {
                    await _next.Invoke(context);

                    var model = ipDetected.add(remoteIp.ToString(), -1);
                    return;
                }


                bool first = false, second = false, third = false, forth = false, agent = false;

                var firstPart = IpParts[0];

                if (!string.IsNullOrWhiteSpace(pattern.FirstIpPart))
                {
                    first = firstPart.Equals(pattern.FirstIpPart);
                }



                var secondPart = IpParts[1];

                if (!string.IsNullOrWhiteSpace(pattern.SeconIpdPart))
                {
                    second = secondPart.Equals(pattern.SeconIpdPart);
                }

                
                var thirdPart = IpParts[2];

                if (!string.IsNullOrWhiteSpace(pattern.ThirdIpPart))
                {
                    third = thirdPart.Equals(pattern.ThirdIpPart);
                }

                
                var forthPart = IpParts[3];

                if (!string.IsNullOrWhiteSpace(pattern.ForthIpPart))
                {
                    forth = forthPart.Equals(pattern.ForthIpPart);
                }



                
                if((!string.IsNullOrWhiteSpace(pattern.FirstIpPart)) && firstPart.Equals(pattern.FirstIpPart))

                if (!string.IsNullOrWhiteSpace(pattern.UserAgent))
                {
                        agent = context.Request.Headers["User-Agent"].ToString().ToLower().Contains(pattern.UserAgent.ToLower());
                }

                if (agent || first || second || third || forth)
                {

                    var model = ipDetected.add(remoteIp.ToString(),pattern.Id);
                    trackerCounter++;
                    if(trackerCounter > 5)
                    {
                        _logger.LogWarning($"this pattern detected {pattern.Title} ");
                        return;
                    }
                    else
                        ipDetected.add(remoteIp.ToString(), -1);

                }
            }
            else
                ipDetected.add(remoteIp.ToString(), -1);


            await _next.Invoke(context);
        }
    }
}
