namespace urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass
{
    public class ResponseModel<T>
    {
        public int Status;
        public string Message;
        public T Payload;
    }
}