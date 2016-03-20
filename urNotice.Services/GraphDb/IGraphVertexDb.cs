using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Model.GraphDb;

namespace urNotice.Services.GraphDb
{
    public interface IGraphVertexDb :  IGraphDb
    {
        Dictionary<String, String> AddVertex(string email, string graphName, Dictionary<string, string> properties);
        String GetVertexDetail(string gremlinQuery, string vertexId, string graphName, Dictionary<string, string> properties);
    }
}
