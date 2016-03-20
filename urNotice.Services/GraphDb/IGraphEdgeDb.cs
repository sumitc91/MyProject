using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Services.GraphDb
{
    public interface IGraphEdgeDb
    {
        Dictionary<String, String> AddEdge(String userName, string graphName,Dictionary<string, string> properties);
    }
}
