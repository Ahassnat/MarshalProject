using MarshalProject.Model;
using MarshalProject.ViewModel.CivilDefense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarshalProject.Repository
{
    public interface IAdminRepo
    {
        #region CivilDefense
        Task<IEnumerable<ApplicationUser>> GetCivilDefenses();
        Task<ApplicationUser> AddCivilDefenseAsync(AddCivilDefenseModel model);
        Task<ApplicationUser> GetCivilDefense(string id);
        Task<ApplicationUser> EditCivilDefenseAsync(EditCivilDefenseModel model);
        Task<bool> DeleteCivilDefenseAsync(List<string> ids);
       // Task<IEnumerable<UserRolesModel>> GetUserRoleAsync();
        Task<IEnumerable<ApplicationRole>> GetRolesAsync();
        //   Task<bool> EditUserRoleAsync(EditUserRoleModel model);
        #endregion

        #region Citizen
         Task<IEnumerable<ApplicationUser>> GetCitizens();
        #endregion
    }
}
