using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.Workgraphy.Model
{
    public class GoogleApiLocationModel
    {
        public String long_name { get; set; }
        public String short_name { get; set; }
        public List<String> types { get; set; }

    }
}
