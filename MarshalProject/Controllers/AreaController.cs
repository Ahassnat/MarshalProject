using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarshalProject.Repository.Area;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarshalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AreaController : ControllerBase
    {
        private readonly IAreaRepo _repo;

        public AreaController(IAreaRepo repo)
        {
            _repo = repo;
        }


        [Route("AddArea")]
        [HttpPost]
        public async Task<IActionResult> AddArea(Model.Area model)
        {
            if (model == null) return BadRequest();
            var area = await _repo.AddAreaAsync(model);

            if (area != null)
                return Ok(area);

            return BadRequest();
        }

        [Route("GetAreas")]
        [HttpGet]
        public async Task<IEnumerable<Model.Area>> GetAreas()
        {
            return await _repo.GetAreas();
        }

        [Route("GetArea/{id}")]
        [HttpGet]
        public async Task<ActionResult<Model.Area>> GetArea(int id)
        {
            if (id < 1) return NotFound();
            var area = await _repo.GetArea(id);
            if (area != null) return area;
            return BadRequest();

        }

        [Route("EditArea")]
        [HttpPut]
        public async Task<IActionResult> EditArea(Model.Area model)
        {
            if (model == null) return BadRequest();
            var area = await _repo.EditAreaAsync(model);
            if (area != null) return Ok();
            return BadRequest();
        }

        #region Single Delete
       
        [HttpDelete("DeleteArea/{id}")]
        public async Task<ActionResult> DeleteArea(int id)
        {

            if (id < 1) return NotFound();
            var area = await _repo.GetArea(id);
            _repo.DeleteAreaAsync(area);
            if (await _repo.SaveAllAsync()) return Ok();

            return BadRequest("Problem deleting the message");

        }
        #endregion
        #region Multi Delete
        [Route("DeleteAreas")]
        [HttpPost]
        public async Task<IActionResult> DeleteAreas(List<string> ids)
        {
            if (ids.Count() < 1) return BadRequest();

            var result = await _repo.DeleteAreasAsync(ids);

            if (result) return Ok();

            return BadRequest();

        }
        #endregion

       
        [HttpGet("SearchAreas/{search}")]
        public async Task<IEnumerable<Model.Area>> SearchArea(string search)
        {
            if (search == null || string.IsNullOrEmpty(search))
            {
                return null;
            }

            return await _repo.SearchAreasAsync(search);
        }


    }
}
