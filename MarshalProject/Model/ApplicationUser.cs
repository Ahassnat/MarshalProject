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
        public Point Location { get; set; }
    }
}
