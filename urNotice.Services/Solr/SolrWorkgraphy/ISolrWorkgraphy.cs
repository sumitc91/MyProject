using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;

namespace urNotice.Services.Solr.SolrWorkgraphy
{
    public interface ISolrWorkgraphy
    {
        Dictionary<String,String> InsertNewWorkgraphy(OrbitPageWorkgraphy orbitPageWorkgraphy, bool optimize);
    }
}
