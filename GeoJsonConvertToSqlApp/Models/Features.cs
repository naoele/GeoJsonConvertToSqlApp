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
        public string Type { get; set; }

        [DataMember(Name = "geometry")]
        public Geometry Geometry { get; set; }

    }
}
