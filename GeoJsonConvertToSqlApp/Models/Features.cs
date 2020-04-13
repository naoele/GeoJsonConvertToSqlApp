using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GeoJsonConvertToSqlApp.Models
{
    [DataContract(Name = "features")]
    public class Features
    {

        [DataMember(Name = "type")]
        public string type { get; set; }

        //[DataMember(Name = "properties")]
        //public Properties properties { get; set; }

        [DataMember(Name = "geometry")]
        public Geometry geometry { get; set; }

    }
}
