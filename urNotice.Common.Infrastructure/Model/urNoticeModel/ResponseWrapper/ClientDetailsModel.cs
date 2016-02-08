namespace urNotice.Common.Infrastructure.Model.urNoticeModel.ResponseWrapper
{
    public class ClientDetailsModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public string RequestUrlAuthority { get; set; }
        public string imageUrl { get; set; }
        public string gender { get; set; }
        public string isLocked { get; set; }

        public string vertexId { get; set; }
    }
}