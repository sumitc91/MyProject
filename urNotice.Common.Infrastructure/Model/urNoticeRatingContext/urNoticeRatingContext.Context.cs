﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace urNotice.Common.Infrastructure.Model.urNoticeRatingContext
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class urnoticeRatingEntities : DbContext
    {
        public urnoticeRatingEntities()
            : base("name=urnoticeRatingEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<BlogComment> BlogComments { get; set; }
        public virtual DbSet<Blog> Blogs { get; set; }
        public virtual DbSet<BlogUserActivity> BlogUserActivities { get; set; }
        public virtual DbSet<ChatGroup> ChatGroups { get; set; }
        public virtual DbSet<CompanyComment> CompanyComments { get; set; }
        public virtual DbSet<CompanyDesignationXml> CompanyDesignationXmls { get; set; }
        public virtual DbSet<CompanyFromInsiderXl> CompanyFromInsiderXls { get; set; }
        public virtual DbSet<CompanyGallery> CompanyGalleries { get; set; }
        public virtual DbSet<CompanyRating> CompanyRatings { get; set; }
        public virtual DbSet<Jobgraphy> Jobgraphies { get; set; }
        public virtual DbSet<JobgraphyComment> JobgraphyComments { get; set; }
        public virtual DbSet<JobgraphyUserActivity> JobgraphyUserActivities { get; set; }
        public virtual DbSet<NoticePeriodInfo> NoticePeriodInfoes { get; set; }
        public virtual DbSet<Rating> Ratings { get; set; }
        public virtual DbSet<ReasonsToStay> ReasonsToStays { get; set; }
        public virtual DbSet<ReasonToLeaf> ReasonToLeaves { get; set; }
    }
}
