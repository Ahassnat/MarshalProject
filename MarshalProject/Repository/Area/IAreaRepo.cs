using MarshalProject.Data;
using MarshalProject.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarshalProject.Repository.Area
{
    public interface IAreaRepo
    {
        #region Area
        Task<IEnumerable<Model.Area>> GetAreas();
        Task<Model.Area> AddAreaAsync(MarshalProject.Model.Area model);
        Task<Model.Area> GetArea(int id);
        Task<Model.Area> EditAreaAsync(MarshalProject.Model.Area model);
        Task<bool> DeleteAreasAsync(List<string> ids);
        void DeleteAreaAsync(Model.Area area);
        Task<IEnumerable<Model.Area>> SearchAreasAsync(string search);
        Task<bool> SaveAllAsync();

        #endregion
    }
    public class AreaRepo : IAreaRepo
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AreaRepo(DataContext context,
                        UserManager<ApplicationUser> userManager,
                        RoleManager<ApplicationRole> roleManager)
        {
           _context = context;
            _userManager = userManager;
           _roleManager = roleManager;
        }
        public async Task<Model.Area> AddAreaAsync(Model.Area model)
        {

            var area = new Model.Area
            {
              
                CamLink = model.CamLink,
                Height = model.Height,
                Location = model.Location,
                Name = model.Name,
                Population=model.Population,
                Status = model.Status,
                
            };
            _context.Add(area);
            await _context.SaveChangesAsync();
            return area;
        }

        public async Task<bool> DeleteAreasAsync(List<string> ids)
        {
            if (ids.Count < 1) return false;

            var i = 0;

            foreach (string id in ids)
            {
                var area = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
                if (area == null) return false;
                _context.Users.Remove(area);
                i++;
            }

            if (i > 0) await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Model.Area> EditAreaAsync(Model.Area model)
        {
            if (model == null || model.Id < 1) return null;

            var area = await _context.Areas.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (area == null) return null;

            _context.Areas.Attach(area);
            area.Name = model.Name;
            area.CamLink = model.CamLink;
            area.Height = model.Height;
            area.Location = model.Location;
            area.Population = model.Population;
            area.Status = model.Status;
            _context.Entry(area).Property(x => x.Name).IsModified = true;
            _context.Entry(area).Property(x => x.CamLink).IsModified = true;
            _context.Entry(area).Property(x => x.Height).IsModified = true;
            _context.Entry(area).Property(x => x.Location).IsModified = true;
            _context.Entry(area).Property(x => x.Population).IsModified = true;
            _context.Entry(area).Property(x => x.Status).IsModified = true;

            await _context.SaveChangesAsync();
            return area;
        }

        public async Task<IEnumerable<Model.Area>> GetAreas()
        {
            return await _context.Areas.ToListAsync();
        }

        public async Task<Model.Area> GetArea(int id)
        {
            var area = await _context.Areas.Include(x=>x.User).FirstOrDefaultAsync(x => x.Id == id);
            if (area == null) return null;
            return area;
        }

        public void DeleteAreaAsync(Model.Area area)
        {

            _context.Areas.Remove(area);            
        }


        public async Task<IEnumerable<Model.Area>> SearchAreasAsync(string search)
        {
            return await _context.Areas.OrderByDescending(x => x.Id).Include(x => x.User)
                .Where(x => x.Name.ToLower().Contains(search.ToLower()))
                .ToListAsync();

        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
