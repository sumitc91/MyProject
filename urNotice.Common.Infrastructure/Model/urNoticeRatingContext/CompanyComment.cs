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
    
    public partial class CompanyComment
    {
        public int Id { get; set; }
        public string companyId { get; set; }
        public string userId { get; set; }
        public bool isAnonymous { get; set; }
        public bool showAnonymous { get; set; }
        public string comment { get; set; }
        public System.DateTime createdDate { get; set; }
        public bool isVerified { get; set; }
        public string verifiedBy { get; set; }
        public string guid { get; set; }
        public int markSpamCount { get; set; }
        public int upvote { get; set; }
        public int downvote { get; set; }
        public Nullable<bool> isDBSynced { get; set; }
    }
}
