using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarshalProject.ViewModel.Area
{
    public class AreaVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Longitude { get; set; }//x
        public double Latitude { get; set; }//y
        public double Height { get; set; }
        public string Status { get; set; }
        public string CamLink { get; set; }
        public int Population { get; set; }
       // public ApplicationUser User { get; set; }
        public string UserId { get; set; }
    }
}
