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
        public static ResponseModel<String> SetNotFound(String message)
        {
            return new ResponseModel<String>
            {
                Status = 404,
                AbortProcess = true,
                Message=message                
            };
        }

        public static ResponseModel<String> SetOk(String message)
        {
            return new ResponseModel<String>
            {
                Status = 200,
                AbortProcess = false,
                Message = message
            };
        }

        public static ResponseModel<String> SetAlreadyTaken(String message)
        {
            return new ResponseModel<String>
            {
                Status = 409,
                AbortProcess = false,
                Message = message
            };
        }

        public static ResponseModel<String> SetInternalServerError(String message)
        {
            return new ResponseModel<String>
            {
                Status = 500,
                AbortProcess = false,
                Message = message
            };
        }
    }
}
