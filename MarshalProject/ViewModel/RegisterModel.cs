using MarshalProject.Model;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MarshalProject.ViewModel
{
    public class RegisterModel
    {
        [StringLength(256), Required]
        [EmailAddress]
        public string Email { get; set; }
        [StringLength(256), Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }

        public int IdentityNumberCard { get; set; }
        public string PhoneNumber { get; set; }
        public string AreaName { get; set; }
        public double Longitude { get; set; }//x
        public double Latitude { get; set; }//y
        //public Point Location { get; set; }


    }
}
