using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.Workgraphy.Model;
using urNotice.Common.Infrastructure.Session;

namespace urNotice.Services.Workgraphy
{
    public interface IWorkgraphyService
    {
        ResponseModel<StoryPostResponse> PublishNewWorkgraphy(urNoticeSession session, StoryPostRequest req);
    }
}
