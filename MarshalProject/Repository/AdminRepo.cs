using MarshalProject.Data;
using MarshalProject.Model;
using MarshalProject.ViewModel.CivilDefense;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarshalProject.Repository
{
    public class AdminRepo:IAdminRepo
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AdminRepo(DataContext context,
                        UserManager<ApplicationUser> userManager,
                        RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        #region CivilDefense

        public async Task<ApplicationUser> AddCivilDefenseAsync(AddCivilDefenseModel model)
        {
            if (model == null) return null;
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

                return user;
            }
            return null;
        }

        public async Task<bool> DeleteCivilDefenseAsync(List<string> ids)
        {
            if (ids.Count < 1) return false;

            var i = 0;

            foreach (string id in ids)
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
                if (user == null) return false;
                _context.Users.Remove(user);
                i++;
            }

            if (i > 0) await _context.SaveChangesAsync();

            return true;
        }

        public async Task<ApplicationUser> EditCivilDefenseAsync(EditCivilDefenseModel model)
        {
            if (model.Id == null) return null;

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == model.Id);

            if (user == null) return null;

            if (model.Password != user.PasswordHash)
            {
                var result = await _userManager.RemovePasswordAsync(user);
                if (result.Succeeded)
                {
                    await _userManager.AddPasswordAsync(user, model.Password);
                }
            }

            _context.Users.Attach(user);

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.EmailConfirmed = model.EmailConfirmed;
            user.PhoneNumber = model.PhoneNumber;

            _context.Entry(user).Property(x => x.UserName).IsModified = true;
            _context.Entry(user).Property(x => x.Email).IsModified = true;
            _context.Entry(user).Property(x => x.EmailConfirmed).IsModified = true;
            _context.Entry(user).Property(x => x.PhoneNumber).IsModified = true;

            await _context.SaveChangesAsync();



            return user;
        }

      
        public async Task<IEnumerable<ApplicationRole>> GetRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<ApplicationUser> GetCivilDefense(string id)
        {
            if (id == null) return null;
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null) return null;
            return user;
        }


      
        public async Task<IEnumerable<ApplicationUser>> GetCivilDefenses()
        {
            return await _context.Users.ToListAsync();
        }


        /** Get User Role 
         public async Task<IEnumerable<UserRolesModel>> GetUserRoleAsync()
      {
          var query = await (
               from userRole in _context.UserRoles

               join users in _context.Users
               on userRole.UserId equals users.Id

               join roles in _context.Roles
               on userRole.RoleId equals roles.Id

               select new
               {
                   userRole.UserId,
                   users.UserName,
                   userRole.RoleId,
                   roles.Name
               }).ToListAsync();
          List<UserRolesModel> userRolesModels = new List<UserRolesModel>();
          if (query.Count <= 0)
          {
              return userRolesModels;

          }
          for (var i = 0; i < query.Count; i++)
          {
              var model = new UserRolesModel
              {
                  UserId = query[i].UserId,
                  UserName = query[i].UserName,
                  RoleId = query[i].RoleId,
                  RoleName = query[i].Name
              };
              userRolesModels.Add(model);
          }

          return userRolesModels;
      }

        **/
        /** Edit User Role
        public async Task<bool> EditUserRoleAsync(EditUserRoleModel model)
      {
          if (model.UserId == null || model.RoleId == null) return false;

          var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == model.UserId);
          if (user == null) return false;

          var currentRoleId = await _context.UserRoles.Where(x => x.UserId == model.UserId)
                                                      .Select(x => x.RoleId).FirstOrDefaultAsync();

          var currentRoleName = await _context.Roles.Where(x => x.Id == currentRoleId)
                                                     .Select(x => x.Name).FirstOrDefaultAsync();

          var newRoleName = await _context.Roles.Where(x => x.Id == model.RoleId)
              .Select(x => x.Name).FirstOrDefaultAsync();


          if (await _userManager.IsInRoleAsync(user, currentRoleName))
          {
              var x = await _userManager.RemoveFromRoleAsync(user, currentRoleName);
              if (x.Succeeded)
              {
                  var s = await _userManager.AddToRoleAsync(user, newRoleName);
                  if (s.Succeeded) return true;
              }
          }

          return false;
      }

        **/

        #endregion

        #region Citizen
        public async Task<IEnumerable<ApplicationUser>> GetCitizens()
        {
            return await _context.Users.ToListAsync();
        }
        #endregion

    }
}
