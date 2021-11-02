using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MarshalProject.Data;
using MarshalProject.Model;
using MarshalProject.Repository;
using MarshalProject.ViewModel;
using MarshalProject.ViewModel.CivilDefense;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace MarshalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAdminRepo _repo;
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AccountController(IAdminRepo repo, DataContext context,
                                UserManager<ApplicationUser> userManager,
                                SignInManager<ApplicationUser> signInManager,
                                RoleManager<ApplicationRole> roleManager)
        {
            _repo = repo;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        #region Account Area

        #region Registration Functions 
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (model == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                if (EmailExist(model.Email))
                {
                    return BadRequest("Email is used ");
                }
                var userNameExist = _context.Users.Any(x => x.UserName == model.UserName);
                if (userNameExist)
                {
                    return BadRequest("user name is used ");
                }
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                var user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.UserName,
                    PhoneNumber =model.PhoneNumber,
                    AreaName = model.AreaName,
                    IdentityCardNumber = model.IdentityNumberCard,
                    Location = geometryFactory.CreatePoint(new Coordinate(model.Longitude, model.Latitude)),

            };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return Ok(user);
                }
                else
                {
                    return BadRequest(result.Errors);
                } // to show error msg.
            }
            return StatusCode(StatusCodes.Status400BadRequest);
        }

        private bool EmailExist(string email)
        {
            return _context.Users.Any(x => x.Email == email);
        }

        #endregion


        #region Login Function
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            await CreateRoles();
            await CreateAdmin();
            if (model == null)
                return NotFound();

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return NotFound();

            //if (!user.EmailConfirmed) 
            //    return Unauthorized("Email Not Confirm yet");




            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);
            if (result.Succeeded)
            {
                if (await _roleManager.RoleExistsAsync("Citizen"))// كل مستخدم جديد هنعطيه صلاحية ال  مواطن 
                    if (!await _userManager.IsInRoleAsync(user, "Citizen") &&
                        !await _userManager.IsInRoleAsync(user, "Admin")) // حتي ما يضيف يوزرات جديدة بنفس الرول تاعت اليوزر في جدول AspNetUserRoles
                    {
                        await _userManager.AddToRoleAsync(user, "Citizen");
                    }
                //if (await _roleManager.RoleExistsAsync("CivilDefense"))// كل مستخدم جديد هنعطيه صلاحية ال  ظابط اسعاف 
                //    if (!await _userManager.IsInRoleAsync(user, "CivilDefense") &&
                //        !await _userManager.IsInRoleAsync(user, "Admin")) // حتي ما يضيف يوزرات جديدة بنفس الرول تاعت اليوزر في جدول AspNetUserRoles
                //    {
                //        await _userManager.AddToRoleAsync(user, "CivilDefense");
                //    }

                //var roleName = await GetRoleNameByUserId(user.Id);
                //if (roleName != null)
                //{
                //    AddCookies(user.UserName, roleName, user.Id, model.RememberMe, user.Email);
                //}

                return Ok();
            }
            else if (result.IsLockedOut)
            {
                return Unauthorized("result: Account is LockOut");
            }
            else
            {
                return BadRequest(result);
            }
            // return StatusCode(StatusCodes.Status204NoContent);



        }
        #endregion

        #region sign out
        [HttpGet]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            //var authProperties = new AuthenticationProperties
            //{
            //    AllowRefresh = true,
            //    IsPersistent = true,
            //    ExpiresUtc = DateTime.UtcNow.AddDays(10)
            //};
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }
        #endregion

        #region Create Admin
        private async Task CreateAdmin()
        {
            var admin = await _userManager.FindByNameAsync("Admin");
            if (admin == null)
            {
                var user = new ApplicationUser
                {
                    Email = "Admin@admin.com",
                    UserName = "Admin",
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(user, "Pass123$");
                if (result.Succeeded)
                {
                    if (await _roleManager.RoleExistsAsync("Admin"))
                        await _userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
        #endregion



        #region Create Admin role + Citizen Role + CivilDefense
        private async Task CreateRoles()
        {
            if (_roleManager.Roles.Count() < 1)
            {
                var role = new ApplicationRole
                {
                    Name = "Admin"
                };
                await _roleManager.CreateAsync(role);

                role = new ApplicationRole
                {
                    Name = "CivilDefense"
                };
                await _roleManager.CreateAsync(role);

                role = new ApplicationRole
                {
                    Name = "Citizen"
                };
                await _roleManager.CreateAsync(role);
            }
        }
        #endregion

        #region get Role-name 
        [HttpGet]
        [Route("GetRoleName/{email}")]
        public async Task<string> GetRoleName(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var userRole = await _context.UserRoles.FirstOrDefaultAsync(x => x.UserId == user.Id);
                if (userRole != null)
                {
                    return await _context.Roles.Where(x => x.Id == userRole.RoleId).Select(x => x.Name).FirstOrDefaultAsync();
                }
            }

            return null;
        }
        #endregion

        #region get Role-name by User id
        private async Task<string> GetRoleNameByUserId(string userId)
        {
            var userRole = await _context.UserRoles.FirstOrDefaultAsync(x => x.UserId == userId);
            if (userRole != null)
            {
                return await _context.Roles.Where(x => 
                                                  x.Id == userRole.RoleId).Select(x => x.Name)
                                                  .FirstOrDefaultAsync();
            }
            return null;
        }
        #endregion

     

        #region Creat Claims Authentication 
        private async void AddCookies(string username, string roleName, string userId, bool remember, string email)
        {
            var claim = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, roleName),
            };

            var claimIdentity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);

            if (remember)
            {
                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(10)
                };

                await HttpContext.SignInAsync
                (
                   CookieAuthenticationDefaults.AuthenticationScheme,
                   new ClaimsPrincipal(claimIdentity),
                   authProperties
                );
            }
            else
            {
                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IsPersistent = false,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                };

                await HttpContext.SignInAsync
                (
                   CookieAuthenticationDefaults.AuthenticationScheme,
                   new ClaimsPrincipal(claimIdentity),
                   authProperties
                );
            }
        }

        #endregion


        #region Check User Clame 
        [Authorize]
        [HttpGet]
        [Route("CheckUserClaims/{email}&{role}")]
        public IActionResult CheckUserClaims(string email, string role)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userEmail != null && userRole != null && id != null)
            {
                if (email == userEmail && role == userRole)
                {
                    return StatusCode(StatusCodes.Status200OK);
                }
            }
            return StatusCode(StatusCodes.Status203NonAuthoritative);
        }
        #endregion
        #endregion





        #region  Admin Controller 
        //[Authorize(Policy = "AdminRole")]
        [HttpGet("GetAllCivilDefenseUsers")]
       
        public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetAllCivilDefenses()
        {
            var users = await _repo.GetCivilDefenses();
            if (users == null)
            {
                return null;
            }
            return Ok(users);
        }

        //#region without Reposotery
        //[HttpPost]
        //[Route("register")]
        //public async Task<IActionResult> Register(AddCivilDefenseModel model)
        //{
        //    if (model == null)
        //    {
        //        return NotFound();
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        //if (EmailExist(model.Email))
        //        //{
        //        //    return BadRequest("Email is used ");
        //        //}
        //        //var userNameExist = _context.Users.Any(x => x.UserName == model.UserName);
        //        //if (userNameExist)
        //        //{
        //        //    return BadRequest("user name is used ");
        //        //}
        //        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        //        var user = new ApplicationUser
        //        {
        //            UserName = model.UserName,
        //            Email = model.Email,
        //            EmailConfirmed = model.EmailConfirmed,
        //            PhoneNumber = model.PhoneNumber,
        //            AreaName = model.AreaName,
        //            IdentityCardNumber = model.IdentityNumberCard,
        //            Location = geometryFactory.CreatePoint(new Coordinate(model.Longitude, model.Latitude)),

        //        };
        //        var result = await _userManager.CreateAsync(user, model.Password);
        //        if (result.Succeeded)
        //        {
        //            if (await _roleManager.RoleExistsAsync("CivilDefense"))// كل مستخدم جديد هنعطيه صلاحية ال  ظابط اسعاف 
        //                if (!await _userManager.IsInRoleAsync(user, "CivilDefense") &&
        //                    !await _userManager.IsInRoleAsync(user, "Admin")) // حتي ما يضيف يوزرات جديدة بنفس الرول تاعت اليوزر في جدول AspNetUserRoles
        //                {
        //                    await _userManager.AddToRoleAsync(user, "CivilDefense");
        //                }
        //            return Ok(user);
        //        }
        //        else
        //        {
        //            return BadRequest(result.Errors);
        //        } // to show error msg.
        //    }
        //    return StatusCode(StatusCodes.Status400BadRequest);
        //}
        ////private bool EmailExist(string email)
        ////{
        ////    return _context.Users.Any(x => x.Email == email);
        ////}
        //#endregion

        [HttpPost("AddCivilDefenseUser")]
        public async Task<IActionResult> AddCivilDefense(AddCivilDefenseModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _repo.AddCivilDefenseAsync(model);
                return Ok(user);
            }
            return BadRequest();
        }


        [HttpGet("GetCivilDefenseUser/{id}")]
        public async Task<ActionResult<ApplicationUser>> GetCivilDefense(string id)
        {
            if (id == null) return NotFound();
            var user = await _repo.GetCivilDefense(id);
            if (user != null)
                return user;
            return BadRequest();

        }


        [HttpPut("EditCivilDefenseUser")]
        public async Task<ActionResult<ApplicationUser>> EditCivilDefense(EditCivilDefenseModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (model.Id == null) return NotFound();

            var user = await _repo.EditCivilDefenseAsync(model);
            if (user != null)
                return user;

            return BadRequest();

        }
    }
    #endregion
}

