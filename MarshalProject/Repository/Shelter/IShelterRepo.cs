using MarshalProject.Data;
using MarshalProject.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarshalProject.Repository.Shelter
{
   public interface IShelterRepo
    {
        Task<IEnumerable<Model.Shelter>> GetShelters();
        Task<Model.Shelter> AddShelterAsync(ViewModel.ShelterVM model);
        Task<Model.Shelter> GetShelter(int id);
        Task<Model.Shelter> EditShelterAsync(MarshalProject.Model.Shelter model);
        Task<bool> DeleteSheltersAsync(List<string> ids);
        void DeleteShelterAsync(Model.Shelter Shelter);
        Task<IEnumerable<Model.Shelter>> SearchSheltersAsync(string search);
        Task<bool> SaveAllAsync();
    }
    public class ShelterRepo : IShelterRepo
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public ShelterRepo(DataContext context,
                        UserManager<ApplicationUser> userManager,
                        RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<Model.Shelter> AddShelterAsync(ViewModel.ShelterVM model)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var shelter = new Model.Shelter
            {
                Location= geometryFactory.CreatePoint(new Coordinate(model.Longitude, model.Latitude)),
                Name =model.Name

            };
            _context.Add(shelter);
            await _context.SaveChangesAsync();
            return shelter;
        }

        public async Task<bool> DeleteSheltersAsync(List<string> ids)
        {
            if (ids.Count < 1) return false;

            var i = 0;

            foreach (string id in ids)
            {
                var shelter = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
                if (shelter == null) return false;
                _context.Users.Remove(shelter);
                i++;
            }

            if (i > 0) await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Model.Shelter> EditShelterAsync(Model.Shelter model)
        {
            if (model == null || model.Id < 1) return null;

            var shelter = await _context.Shelters.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (shelter == null) return null;

            _context.Shelters.Attach(shelter);
            shelter.Name = model.Name;
            shelter.Location = model.Location;
           
            _context.Entry(shelter).Property(x => x.Name).IsModified = true;
         
            _context.Entry(shelter).Property(x => x.Location).IsModified = true;
           
            await _context.SaveChangesAsync();
            return shelter;
        }

        public async Task<IEnumerable<Model.Shelter>> GetShelters()
        {
            return await _context.Shelters.ToListAsync();
        }

        public async Task<Model.Shelter> GetShelter(int id)
        {
            var shelter = await _context.Shelters.FirstOrDefaultAsync(x => x.Id == id);
            if (shelter == null) return null;
            return shelter;
        }

        public void DeleteShelterAsync(Model.Shelter shelter)
        {

            _context.Shelters.Remove(shelter);
        }


        public async Task<IEnumerable<Model.Shelter>> SearchSheltersAsync(string search)
        {
            return await _context.Shelters.OrderByDescending(x => x.Id)
                .Where(x => x.Name.ToLower().Contains(search.ToLower()))
                .ToListAsync();

        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
