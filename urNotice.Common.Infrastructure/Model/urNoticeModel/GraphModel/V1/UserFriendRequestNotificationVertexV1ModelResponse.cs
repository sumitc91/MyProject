﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel.V1
{
    public class UserFriendRequestNotificationVertexV1ModelResponse
    {
        public string requestId { get; set; }
        public UserFriendRequestNotificationVertexModelV1ResultResponse result { get; set; }
    }
}