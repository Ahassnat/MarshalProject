using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarshalProject.Model
{
    public class Area
    {
        public int Id { get; set; }
        public string  Name  { get; set; }
        public Point Location { get; set; }
        public double Height { get; set; }
        public string  Status { get; set; }
        public string CamLink { get; set; }
        public int Population { get; set; }
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }
    }
}
