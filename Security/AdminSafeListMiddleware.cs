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

        private bool tryEqualToPartIp(string itemOne, string itemTwo)
        {
            try
            {
                return Convert.ToInt32(itemOne) == Convert.ToInt32(itemTwo);
            }
            catch (Exception e)
            {

            }
            return false;
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
            if (!string.IsNullOrWhiteSpace(context.Request.Headers["ar-real-ip"]))
                remoteIp = context.Request.Headers["ar-real-ip"];
            

            var patterns = ipPattern.GetEnabledPatterns();

            if (patterns == null || (!patterns.Any()))
            {
                ipDetected.add(remoteIp, -1);
                await _next.Invoke(context);
                return;
            }

            var IpParts = remoteIp.Split('.');

            if (IpParts.Length != 4)
            {

                ipDetected.add(remoteIp, -1);
                await _next.Invoke(context);
                return;
            }

            foreach (var pattern in patterns)
            {

                #region StartDetected

                if (!context.Request.Method.Equals(pattern.VerbsOrMethod, StringComparison.OrdinalIgnoreCase))
                {
                    ipDetected.add(remoteIp, -1);
                    await _next.Invoke(context);
                    return;
                }

                


                bool first = false, second = false, third = false, forth = false, agent = false;

                var firstPart = IpParts[0];

                if (!string.IsNullOrWhiteSpace(pattern.FirstIpPart))
                {

                    first = tryEqualToPartIp(firstPart,pattern.FirstIpPart);
                }



                var secondPart = IpParts[1];

                if (!string.IsNullOrWhiteSpace(pattern.SeconIpdPart))
                {
                    second = tryEqualToPartIp(secondPart,pattern.SeconIpdPart);
                }


                var thirdPart = IpParts[2];

                if (!string.IsNullOrWhiteSpace(pattern.ThirdIpPart))
                {
                    third = tryEqualToPartIp(thirdPart,pattern.ThirdIpPart);
                }


                var forthPart = IpParts[3];

                if (!string.IsNullOrWhiteSpace(pattern.ForthIpPart))
                {
                    forth = tryEqualToPartIp(forthPart,pattern.ForthIpPart);
                }




                if (!string.IsNullOrWhiteSpace(pattern.UserAgent))
                {
                    agent = context.Request.Headers["User-Agent"].ToString().ToLower().Contains(pattern.UserAgent.ToLower());
                }



                if (agent || first || second || third || forth)
                {

                    ipDetected.add(remoteIp, pattern.Id);
                    await _next.Invoke(context);
                    //trackerCounter++;
                    //if (trackerCounter > 5)
                    //{
                    //    return;
                    //}
                    return;
                }


                #endregion

            }

            ipDetected.add(remoteIp, -1);
            await _next.Invoke(context);
        }
    }
}
