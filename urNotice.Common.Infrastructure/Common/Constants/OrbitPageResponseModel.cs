using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;

namespace urNotice.Common.Infrastructure.Common.Constants
{
    public static class OrbitPageResponseModel
    {
        public static ResponseModel<T> SetNotFound<T>(string message,T payload)
        {
            return new ResponseModel<T>
            {
                Status = 404,
                AbortProcess = true,
                Message = message,
                Payload = payload
            };
        }

        public static ResponseModel<T> SetOk<T>(string message, T payload)
        {
            return new ResponseModel<T>
            {
                Status = 200,
                AbortProcess = false,
                Message = message,
                Payload = payload
            };
        }

        public static ResponseModel<T> SetAlreadyTaken<T>(string message, T payload)
        {
            return new ResponseModel<T>
            {
                Status = 409,
                AbortProcess = false,
                Message = message,
                Payload = payload
            };
        }

        public static ResponseModel<T> SetInternalServerError<T>(string message, T payload)
        {
            return new ResponseModel<T>
            {
                Status = 500,
                AbortProcess = false,
                Message = message,
                Payload = payload
            };
        }
    }
}
