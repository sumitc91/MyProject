//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class ReasonToLeaf
    {
        public int Id { get; set; }
        public string userId { get; set; }
        public bool noRecognition { get; set; }
        public bool noOpportunity { get; set; }
        public bool underpaid { get; set; }
        public bool workpressure { get; set; }
        public bool lackOfRoleClarity { get; set; }
        public bool organisationalDecline { get; set; }
        public bool outdatedTechnology { get; set; }
        public bool poorTeam { get; set; }
        public bool incompetentManager { get; set; }
        public bool personalReasons { get; set; }
        public bool furtherStudies { get; set; }
        public string others { get; set; }
        public System.DateTime createdDate { get; set; }
        public string companyId { get; set; }
        public string locationId { get; set; }
        public bool isVerified { get; set; }
        public string verifiedBy { get; set; }
        public string guid { get; set; }
        public Nullable<bool> isDBSynced { get; set; }
    }
}