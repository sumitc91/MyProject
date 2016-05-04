using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace urNotice.Common.Infrastructure.Model.urNoticeModel.GraphModel
{
    public class EdgeModel
    {
       public string PostedDate {get;set;}
       public long PostedDateLong {get;set;}
       public string _id {get;set;}
       public string _type {get;set;}
       public string _outV {get;set;}
       public string _inV{get;set;}
       public string _label { get; set; }
    }
}
