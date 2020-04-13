using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GeoJsonConvertToSqlApp.Models
{
    [DataContract(Name = "geometry")]
    public class Geometry
    {

        [DataMember(Name = "type")]
        public string type { get; set; }

        // リスト型
        [DataMember(Name = "coordinates")]
        public List<double[]> CoordinatesList { get; private set; } = new List<double[]>();

    }
}
