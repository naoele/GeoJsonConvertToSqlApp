using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoJsonConvertToSqlApp.Models
{
    public class CoursePoint
    {
        public CoursePoint(string name, JsonGeometry geometry)
        {
            this.Name = name;
            Coordinates = new List<GeoCoordinate>();
            foreach (double[] coor in geometry.CoordinatesList)
            {
                Coordinates.Add(new GeoCoordinate(coor[1], coor[0]));
            }
        }

        public string Name { get; set; }

        public List<GeoCoordinate> Coordinates { get; set; }
    }
}
