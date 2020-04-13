using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GeoJsonConvertToSqlApp.Models
{
    [DataContract]
    public class CourseJson
    {

        [DataMember(Name = "type")]
        public string Name { get; set; }

        // ハッシュマップ型
        [DataMember(Name = "features")]
        public List<Features> features { get; private set; } = new List<Features>();

    }
}
