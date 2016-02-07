﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb
{
    public class OrbitPageVertexDetail
    {
        public string url{get;set;}
        public string vertexId { get; set; }
        public string graphName { get; set; }
        public Dictionary<string, string> properties { get; set; }
    }
}
