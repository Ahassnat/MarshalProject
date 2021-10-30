using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MarshalProject.ViewModel.CivilDefense
{
    public class AddCivilDefenseModel
    {
        [StringLength(256), Required]
        [EmailAddress]
        public string Email { get; set; }
        [StringLength(256), Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public bool EmailConfirmed { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        public int IdentityNumberCard { get; set; }
        public string AreaName { get; set; }
        public double Longitude { get; set; }//x
        public double Latitude { get; set; }//y
    }

}
