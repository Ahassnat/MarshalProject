using Microsoft.AspNetCore.Identity;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarshalProject.Model
{
    public class ApplicationUser:IdentityUser
    {
        public int IdentityCardNumber { get; set; }
        
        public Area Area { get; set; }
        public string AreaName  { get; set; }
        //public double Longitude { get; set; }//x
        //public double Latitude { get; set; }//y
        public Point Location { get; set; }

    }
}
