namespace urNotice.Common.Infrastructure.Model.urNoticeModel.RequestWrapper
{
    public class ContactUsRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public string SendMeACopy { get; set; }
    }
}