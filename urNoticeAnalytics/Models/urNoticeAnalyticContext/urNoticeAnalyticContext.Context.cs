﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace urNoticeAnalytics.Models.urNoticeAnalyticContext
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class urnoticeAnalyticsEntities : DbContext
    {
        public urnoticeAnalyticsEntities()
            : base("name=urnoticeAnalyticsEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<GoogleApiContact> GoogleApiContacts { get; set; }
        public virtual DbSet<GoogleApiCheckLastSynced> GoogleApiCheckLastSynceds { get; set; }
        public virtual DbSet<VirtualFriendList> VirtualFriendLists { get; set; }
    }
}