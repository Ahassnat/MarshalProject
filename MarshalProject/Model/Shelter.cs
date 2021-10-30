using NetTopologySuite.Geometries;

namespace MarshalProject.Model
{
    public class Shelter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Point Location { get; set; }
    }
}
