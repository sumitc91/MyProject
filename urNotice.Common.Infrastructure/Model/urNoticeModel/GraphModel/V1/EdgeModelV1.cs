using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel.V1
{
    public class EdgeModelV1
    {
        public string id { get; set; }
        public string label { get; set; }
        public string type { get; set; }
        public string inVLabel { get; set; }
        public string outVLabel { get; set; }
        public long inV { get; set; }
        public long outV { get; set; }
        public Dictionary<string, string> properties { get; set; }
    }
}
