using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoJsonConvertToSqlApp.Models
{
    class CourseCsv
    {
        private int v1;
        private int v2;
        private int v3;
        private int v4;
        private string v5;
        private string v6;

        public CourseCsv(int id, int cd_kikan1, int cd_kikan2, int cd_kikan3, string junkai_course_name, int disp_order)
        {
            this.id = id;
            this.cd_kikan1 = cd_kikan1;
            this.cd_kikan2 = cd_kikan2;
            this.cd_kikan3 = cd_kikan3;
            this.junkai_course_name = junkai_course_name;
            this.disp_order = disp_order;
        }

        public int id { get;  set; }

        public int cd_kikan1 { get;  set; }

        public int cd_kikan2 { get;  set; }

        public int cd_kikan3 { get;  set; }

        public string junkai_course_name { get;  set; }

        public int disp_order { get;  set; }

    }
}
