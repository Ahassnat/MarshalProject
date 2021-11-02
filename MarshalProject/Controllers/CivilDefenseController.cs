using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarshalProject.Model;
using MarshalProject.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarshalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CivilDefenseController : ControllerBase
    {
        private readonly IAdminRepo _repo;

        public CivilDefenseController(IAdminRepo repo)
        {
            _repo = repo;
        }


        [HttpGet]
        [Route("GetAllCivilDefenseUsers")]
        public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetAllCivilDefenses()
        {
            var users = await _repo.GetCivilDefenses();
            if (users == null)
            {
                return null;
            }
            return Ok(users);
        }
    }
}
