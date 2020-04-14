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
        public CoursePoint(string name, Geometry geometry)
        {
            this.Name = name;
            Coordinates = new List<GeoCoordinate>();
            foreach (double[] coor in geometry.CoordinatesList)
            {
                Coordinates.Add(new GeoCoordinate(coor[1], coor[0]));
            }
        }

        public string Name { get; set; }

        public IList<GeoCoordinate> Coordinates { get; set; }

        public int Cd_kikan2 { get; set; }

        public int Cd_kikan3 { get; set; }

        public string Junkai_course_name { get; set; }

        public int Disp_order { get; set; }

    }
}
