//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace urNotice.Common.Infrastructure.Model.urNoticeAuthContext
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserReputationMapping
    {
        public int Id { get; set; }
        public string username { get; set; }
        public string type { get; set; }
        public string subType { get; set; }
        public string description { get; set; }
        public Nullable<System.DateTime> DateTime { get; set; }
        public string reputation { get; set; }
        public int UserId { get; set; }
        public Nullable<bool> isDBSynced { get; set; }
    }
}
