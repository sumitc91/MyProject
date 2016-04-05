using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper;

namespace urNotice.Common.Infrastructure.Model.Workgraphy.Model
{
    public class StoryPostRequest
    {
        public StoryPostData Data { get; set; }
        public List<ImgurImageResponse> ImgurList { get; set; }
        public List<GoogleApiLocationModel> location { get; set; }
        public String formatted_address { get; set; }
        public String vertexId { get; set; }
    }
}
