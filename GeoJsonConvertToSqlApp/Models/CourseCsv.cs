using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoJsonConvertToSqlApp.Models
{
    public class CourseCsv
    {
        public CourseCsv(int id, int cd_kikan1, int cd_kikan2, int cd_kikan3, string junkai_course_name, int disp_order)
        {
            this.Id = id;
            this.Cd_kikan1 = cd_kikan1;
            this.Cd_kikan2 = cd_kikan2;
            this.Cd_kikan3 = cd_kikan3;
            this.Junkai_course_name = TrimDoubleQuotationMarks(junkai_course_name);
            this.Disp_order = disp_order;
        }

        public int Id { get; set; }

        public int Cd_kikan1 { get; set; }

        public int Cd_kikan2 { get; set; }

        public int Cd_kikan3 { get; set; }

        public string Junkai_course_name { get; set; }

        public int Disp_order { get; set; }

        public static string TrimDoubleQuotationMarks(string target)
        {
            return target.Trim(new char[] { '"' });
        }

    }
}
