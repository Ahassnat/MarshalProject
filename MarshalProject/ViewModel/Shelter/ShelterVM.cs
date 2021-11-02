using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarshalProject.ViewModel
{
    public class ShelterVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Longitude { get; set; }//x
        public double Latitude { get; set; }//y
    }
}
