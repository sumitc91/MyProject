using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel
{
    public class UserVertexModel
    {
        public string FirstName {set;get;}
        public string LastName {set;get;}
        public string Username {set;get;}
        public string Gender {set;get;}
        public string CreatedTime {set;get;}
        public string ImageUrl {set;get;}
        public string CoverImageUrl { set; get; }
        public string _id { get; set; }
        public string _type { get; set; }
    }
}
