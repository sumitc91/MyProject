using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper
{
    public class EditMessageRequest
    {
        public string message { get; set; }
        public string imageUrl { get; set; }
        public string messageVertex { get; set; }
        public string userVertex { get; set; }
        public string userEmail { get; set; }
        public string wallVertex { get; set; }

        public bool deletePreviousImage { get; set; }
    }
}
