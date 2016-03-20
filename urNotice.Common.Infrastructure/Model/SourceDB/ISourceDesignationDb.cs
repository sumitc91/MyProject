using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.SourceDB
{
    public interface ISourceDesignationDb
    {
        Dictionary<String, String> AddDesignation(string vertexId, string designationName, bool toBeOptimized);
    }
}
