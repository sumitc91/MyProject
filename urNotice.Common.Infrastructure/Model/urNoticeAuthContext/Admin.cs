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
    
    public partial class Admin
    {
        public int Id { get; set; }
        public string userId { get; set; }
        public System.DateTime createdDate { get; set; }
        public short priviledgeLevel { get; set; }
        public Nullable<bool> isDBSynced { get; set; }
    }
}