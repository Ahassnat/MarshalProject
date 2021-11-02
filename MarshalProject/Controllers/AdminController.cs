using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarshalProject.Data;
using MarshalProject.Model;
using MarshalProject.Repository;
using MarshalProject.ViewModel.CivilDefense;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace MarshalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepo _repo;
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AdminController(IAdminRepo repo, DataContext context,
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

        #region without Reposotery
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(AddCivilDefenseModel model)
        {
            if (model == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                //if (EmailExist(model.Email))
                //{
                //    return BadRequest("Email is used ");
                //}
                //var userNameExist = _context.Users.Any(x => x.UserName == model.UserName);
                //if (userNameExist)
                //{
                //    return BadRequest("user name is used ");
                //}
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    EmailConfirmed = model.EmailConfirmed,
                    PhoneNumber = model.PhoneNumber,
                    AreaName = model.AreaName,
                    IdentityCardNumber = model.IdentityNumberCard,
                    Location = geometryFactory.CreatePoint(new Coordinate(model.Longitude, model.Latitude)),

                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (await _roleManager.RoleExistsAsync("CivilDefense"))// كل مستخدم جديد هنعطيه صلاحية ال  ظابط اسعاف 
                        if (!await _userManager.IsInRoleAsync(user, "CivilDefense") &&
                            !await _userManager.IsInRoleAsync(user, "Admin")) // حتي ما يضيف يوزرات جديدة بنفس الرول تاعت اليوزر في جدول AspNetUserRoles
                        {
                            await _userManager.AddToRoleAsync(user, "CivilDefense");
                        }
                    return Ok(user);
                }
                else
                {
                    return BadRequest(result.Errors);
                } // to show error msg.
            }
            return StatusCode(StatusCodes.Status400BadRequest);
        }
        //private bool EmailExist(string email)
        //{
        //    return _context.Users.Any(x => x.Email == email);
        //}
        #endregion

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
}
