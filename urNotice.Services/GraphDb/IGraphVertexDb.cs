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
        Dictionary<String, String> AddVertex(string email, string graphName, Dictionary<string, string> properties, HashSet<string> canEdit, HashSet<string> canDelete, HashSet<string> sendNotificationToUsers);

        Dictionary<string, string> UpdateVertex(string vertexId, string email, string graphName, Dictionary<string, string> properties);
        String GetVertexDetail(string gremlinQuery, string vertexId, string graphName, Dictionary<string, string> properties);
        Dictionary<string, string> DeleteVertex(string vertexId, string userVertexId, string label);
    }
}
