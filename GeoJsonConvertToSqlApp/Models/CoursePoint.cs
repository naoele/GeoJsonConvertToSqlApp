using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoJsonConvertToSqlApp.Models
{
    class CoursePoint
    {
        public CoursePoint(string name, Geometry geometry)
        {
            this.name = name;
            coordinates = new List<GeoCoordinate>();
            foreach (double[] coor in geometry.CoordinatesList)
            {
                coordinates.Add(new GeoCoordinate(coor[1], coor[0]));
            }
        }

        public string name { get; set; }

        public IList<GeoCoordinate> coordinates { get; set; }

        public int cd_kikan2 { get; set; }

        public int cd_kikan3 { get; set; }

        public string junkai_course_name { get; set; }

        public int disp_order { get; set; }

    }
}
