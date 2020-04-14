using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoJsonConvertToSqlApp.Models
{
    /// <summary>
    /// m_course_line_pointテーブルINSERT用SQL作成モデル
    /// </summary>
    public class Course
    {
        public Course(CourseCsv courseCsv)
        {
            this.Id = courseCsv.Id;
            this.Cd_kikan1 = courseCsv.Cd_kikan1;
            this.Cd_kikan2 = courseCsv.Cd_kikan2;
            this.Cd_kikan3 = courseCsv.Cd_kikan3;
            this.Junkai_course_name = courseCsv.Junkai_course_name;
            this.Disp_order = courseCsv.Disp_order;
        }

        public Course(CoursePoint coursePoint)
        {
            this.Junkai_course_name = coursePoint.Name;
            this.Coordinates = coursePoint.Coordinates;
        }

        public Course(CourseCsv courseCsv, CoursePoint coursePoint)
        {
            this.Id = courseCsv.Id;
            this.Cd_kikan1 = courseCsv.Cd_kikan1;
            this.Cd_kikan2 = courseCsv.Cd_kikan2;
            this.Cd_kikan3 = courseCsv.Cd_kikan3;
            this.Disp_order = courseCsv.Disp_order;
            this.Junkai_course_name = coursePoint.Name;
            this.Coordinates = coursePoint.Coordinates;
        }

        public int Id { get; set; }

        public int Cd_kikan1 { get; set; }

        public int Cd_kikan2 { get; set; }

        public int Cd_kikan3 { get; set; }

        public string Junkai_course_name { get; set; }

        public int Disp_order { get; set; }

        public List<GeoCoordinate> Coordinates { get; set; }

    }
}
