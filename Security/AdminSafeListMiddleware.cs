using EmployeeManagment.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeManagment.Security
{
    public class AdminSafeListMiddleware
    {
        private readonly RequestDelegate _next;
        private IIpDetectedRepository ipDetected;
        private ICustomIpPatternRepository ipPattern;
        private readonly ILogger<AdminSafeListMiddleware> _logger;
        UserManager<ApplicationUser> userManager;
        RoleManager<IdentityRole> roleManager;
        SignInManager<ApplicationUser> signInMeneger;
        private int trackerCounter = 0;

        public AdminSafeListMiddleware(
            RequestDelegate next,
            ILogger<AdminSafeListMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, IIpDetectedRepository ipDetected, AppDbContext appDb, ICustomIpPatternRepository ipPattern, 
            RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInMeneger, IHttpContextAccessor httpContextAccessor)
        {
            this.ipDetected = ipDetected;
            this.ipPattern = ipPattern;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInMeneger = signInMeneger;



            var remoteIp = context.Connection.RemoteIpAddress.ToString();
            _logger.LogError($"\n-------------------------------------------------------\n");
            _logger.LogError($"\nthis first remote ip : {remoteIp}\n");
            if (!string.IsNullOrWhiteSpace(context.Request.Headers["HTTP_X_FORWARDED_FOR"]))
                remoteIp = context.Request.Headers["HTTP_X_FORWARDED_FOR"].ToString().Split(',')[0].Trim();
            else if (!string.IsNullOrWhiteSpace(context.Request.Headers["REMOTE_ADDR"]))
                remoteIp = context.Request.Headers["REMOTE_ADDR"];
            else if (!string.IsNullOrWhiteSpace(context.Request.Headers["AR_REAL_IP"]))
                remoteIp = context.Request.Headers["AR_REAL_IP"];
            _logger.LogError($"\nthis HTTP_X_FORWARDED_FOR : {context.Request.Headers["HTTP_X_FORWARDED_FOR"]}\n");
            _logger.LogError($"\nthis REMOTE_ADDR : {context.Request.Headers["REMOTE_ADDR"]}\n");
            _logger.LogError($"\nthis AR_REAL_IP : {context.Request.Headers["AR_REAL_IP"]}\n");
            _logger.LogError($"\nthis last remote ip : {remoteIp}\n");

            _logger.LogError($"\n-------------------------------------------------------\n");
            if (context.User.IsInRole("Admin"))
            {
                await _next.Invoke(context);
                return;
            }

            var pattern = ipPattern.GetEnabledPattern();

            if(pattern == null)
            {
                await _next.Invoke(context);
                ipDetected.add(remoteIp, -1);
                return;
            }
            if (context.Request.Method.Equals(pattern.VerbsOrMethod,StringComparison.OrdinalIgnoreCase))
            {


                var IpParts = remoteIp.Split('.');

                if (IpParts.Length != 4)
                {
                    await _next.Invoke(context);

                    var model = ipDetected.add(remoteIp, -1);
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

                    var model = ipDetected.add(remoteIp,pattern.Id);
                    trackerCounter++;
                    if(trackerCounter > 5)
                    {
                        _logger.LogWarning($"this pattern detected {pattern.Title} ");
                        return;
                    }
                    else
                        ipDetected.add(remoteIp, -1);

                }
            }
            else
                ipDetected.add(remoteIp, -1);


            await _next.Invoke(context);
        }
    }
}
