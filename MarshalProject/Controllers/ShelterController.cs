using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarshalProject.Repository.Shelter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarshalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class ShelterController : ControllerBase
    {
        private readonly IShelterRepo _repo;
        public ShelterController(IShelterRepo repo)
        {
  _repo = repo;
        }
       


        [Route("AddShelter")]
        [HttpPost]
        public async Task<IActionResult> AddShelter(ViewModel.ShelterVM model)
        {
            if (model == null) return BadRequest();
            var shelter = await _repo.AddShelterAsync(model);

            if (shelter != null)
                return Ok(shelter);

            return BadRequest();
        }

        [Route("GetShelters")]
        [HttpGet]
        public async Task<IEnumerable<Model.Shelter>> GetShelters()
        {
            return await _repo.GetShelters();
        }

        [Route("GetShelter/{id}")]
        [HttpGet]
        public async Task<ActionResult<Model.Shelter>> GetShelter(int id)
        {
            if (id < 1) return NotFound();
            var shelter = await _repo.GetShelter(id);
            if (shelter != null) return shelter;
            return BadRequest();

        }

        [Route("EditShelter")]
        [HttpPut]
        public async Task<IActionResult> EditShelter(Model.Shelter model)
        {
            if (model == null) return BadRequest();
            var shelter = await _repo.EditShelterAsync(model);
            if (shelter != null) return Ok();
            return BadRequest();
        }

        #region Single Delete

        [HttpDelete("DeleteShelter/{id}")]
        public async Task<ActionResult> DeleteShelter(int id)
        {

            if (id < 1) return NotFound();
            var shelter = await _repo.GetShelter(id);
            _repo.DeleteShelterAsync(shelter);
            if (await _repo.SaveAllAsync()) return Ok();

            return BadRequest("Problem deleting the message");

        }
        #endregion
        #region Multi Delete
        [Route("DeleteShelters")]
        [HttpPost]
        public async Task<IActionResult> DeleteShelters(List<string> ids)
        {
            if (ids.Count() < 1) return BadRequest();

            var result = await _repo.DeleteSheltersAsync(ids);

            if (result) return Ok();

            return BadRequest();

        }
        #endregion


        [HttpGet("SearchShelters/{search}")]
        public async Task<IEnumerable<Model.Shelter>> SearchShelter(string search)
        {
            if (search == null || string.IsNullOrEmpty(search))
            {
                return null;
            }

            return await _repo.SearchSheltersAsync(search);
        }


    }
}
