using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel.V1
{
    public class VertexModelV1
    {
        public long id { get; set; }
        public string label { get; set; }
        public string type { get; set; }
        public Dictionary<string,List<VertexPropertiesModelV1>> properties { get; set; }
    }
}
