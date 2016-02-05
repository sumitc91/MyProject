using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.commonMethods
{
    public class OrbitPageUtil
    {
        public static string ConvertVirtualFriendListToUnVirtualFriendSolrPhoneWithoutFormatting(string id2)
        {
            return id2.Replace("tel:", "").Replace("-", "");            
        }
    }
}
