using EmployeeManagment.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagment.Models
{
    //[AllowAnonymous]
    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IIpDetectedRepository ipDetected;
        private readonly ICustomIpPatternRepository ipPattern;
        private readonly UserManager<ApplicationUser> userManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager,
                                        IIpDetectedRepository ipDetected,
                                        ICustomIpPatternRepository ipPattern,
                                        UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.ipDetected = ipDetected;
            this.ipPattern = ipPattern;
            this.userManager = userManager;
        }

        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id {id} cannot be found";
                return View("NotFound");
            }
            var result = await userManager.DeleteAsync(user);

            if (result.Succeeded)
                return RedirectToAction("ListUsers");


            foreach (var err in result.Errors)
                ModelState.AddModelError(string.Empty, err.Description);

            return View("ListUsers");
        }

        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = userManager.Users;
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {

            var user = await userManager.FindByIdAsync(id);

            if(user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            var userClaims = await userManager.GetClaimsAsync(user);
            var userRoles = await userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                City = user.City,
                UserName = user.UserName,
                Claims = userClaims.Select(a => a.Value).ToList(),
                Roles = userRoles
            };

            return View(model);
        }

        public ActionResult IpsList() 
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {

            var user = await userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            user.Email = model.Email;
            user.UserName = model.UserName;
            user.City = model.City;

            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
                return RedirectToAction("ListUsers");

            foreach (var err in result.Errors)
            {
                ModelState.AddModelError(string.Empty, err.Description);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if(ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };

                IdentityResult result = await roleManager.CreateAsync(identityRole);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Administration");
                }

                foreach(IdentityError err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }


        [HttpGet]
        public async  Task<IActionResult> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if(role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cant be found";
                return View("NotFound");
            }

            var model = new EditRoleViewModel()
            {
                Id = role.Id,
                RoleName = role.Name
            };
            var users = userManager.Users.ToList();

            foreach (var user in users)
            {
                try
                {
                    if (await userManager.IsInRoleAsync(user, role.Name))
                    {
                        model.Users.Add(user.UserName);
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
                
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {model.Id} cant be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(string.Empty , err.Description);
                }
                return View(model);
            }
        }


        [HttpGet]
        public async Task<ViewResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role With Id = {roleId} cannot be found";
                return View("NotFound");
            }
            var model = new List<UserRoleViewModel>();
            var users = userManager.Users.ToList();
            foreach (var user in users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                userRoleViewModel.isSelected = await userManager.IsInRoleAsync(user, role.Name);

                model.Add(userRoleViewModel);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);

            if(role == null)
            {
                ViewBag.ErrorMessage = $"Role With Id {roleId} cannot found";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);
                IdentityResult result = null;
                var isInRole = await userManager.IsInRoleAsync(user, role.Name);
                if (model[i].isSelected && !isInRole)
                    result = await userManager.AddToRoleAsync(user, role.Name);
                else if (!model[i].isSelected && isInRole)
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                else
                    continue;

                if(result.Succeeded)
                {
                    if (i < model.Count)
                        continue;
                    return RedirectToAction("EditRole", new { Id = roleId });
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId });
        }


        [HttpPost]
        public ActionResult IpPattern(CustomIpPattern model)
        {

            ipPattern.Update(model);

            return RedirectToAction("IpPattern");
        }

        [HttpGet]
        public ViewResult IpPattern()
        {
            var model = ipPattern.GetAllPattern();
            return View(model);
        }


        [HttpGet]
        public ViewResult AddIpPattern()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult AddIpPattern(IpPatternCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View();

            ipPattern.Add(new CustomIpPattern
            {
                Enabled = model.Enabled,
                FirstIpPart = model.FirstIpPart,
                SeconIpdPart = model.SeconIpdPart,
                ThirdIpPart = model.ThirdIpPart,
                ForthIpPart = model.ForthIpPart,
                Title = model.Title,
                UserAgent = model.UserAgent,
                VerbsOrMethod = model.VerbsOrMethod.ToString()
            });

            return RedirectToAction("IpPattern");
        }

        [HttpPost]
        public ActionResult DeleteThisIp(int id)
        {   
            if (id <= 0)
            {
                ViewBag.ErrorMessage = $"employee with id {id} cannot be found";
                return View("NotFound");
            }
            ipDetected.Delete(id);
            return RedirectToAction("IpsList");
        }


        [HttpPost]
        public ActionResult DeleteIpPattern(int id)
        {
            var result = ipPattern.Get(id);
            if (result == null)
            {
                ViewBag.ErrorMessage = $"employee with id {id} cannot be found";
                return View("NotFound");
            }
            ipPattern.Delete(id);
            return RedirectToAction("IpPattern");
        }

        [HttpPost]
        public ActionResult DeleteAllIps(int id)
        {
            ipDetected.DeleteAll();
            return RedirectToAction("Index","Home");
        }
    }
}
