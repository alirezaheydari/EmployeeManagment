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
    public class AdminRemoveIP
    {
        private readonly RequestDelegate _next;
        private IIpDetectedRepository ipDetected;
        private ICustomIpPatternRepository ipPattern;
        private readonly ILogger<AdminRemoveIP> _logger;
        UserManager<ApplicationUser> userManager;
        RoleManager<IdentityRole> roleManager;
        SignInManager<ApplicationUser> signInMeneger;
        private int trackerCounter = 0;

        public AdminRemoveIP(
            RequestDelegate next,
            ILogger<AdminRemoveIP> logger)
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


            if (context.User.IsInRole("Admin"))
            {
                ipDetected.DeleteLastOne();
            }


            await _next.Invoke(context);
        }
    }
}
