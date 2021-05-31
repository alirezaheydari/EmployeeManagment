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
        private readonly ILogger<AdminSafeListMiddleware> _logger;
        private readonly Dictionary<IpPattern,int> trackerCounter;

        public AdminSafeListMiddleware(
            RequestDelegate next,
            ILogger<AdminSafeListMiddleware> logger)
        {
            trackerCounter = new Dictionary<IpPattern, int>();
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, IIpDetectedRepository ipDetected, AppDbContext appDb)
        {
            this.ipDetected = ipDetected;
            if (context.Request.Method != HttpMethod.Get.Method)
            {
                var remoteIp = context.Connection.RemoteIpAddress;
                var model = ipDetected.add(remoteIp.ToString());
                if (model.Pattern == IpPattern.none)
                {
                    await _next.Invoke(context);
                    return;
                }


                if (trackerCounter.ContainsKey(model.Pattern))
                    trackerCounter.Add(model.Pattern, 1);
                else
                    trackerCounter[model.Pattern]++;

                if(trackerCounter[model.Pattern] >= 5)
                {
                    _logger.LogWarning($"this pattern detected {model.Pattern} ");
                    return;
                }

            }

            await _next.Invoke(context);
        }
    }
}
