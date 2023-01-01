using System;
using SrsManageCommon;

namespace SRSManageCommon.ManageStructs
{
    [Serializable]
    public class ResponseStruct
    {
        private ErrorNumber code;
        private string message = null!;

        public ResponseStruct(ErrorNumber code, string message)
        {
            Code = code;
            Message = message;
        }

        public ResponseStruct()
        {
        }

        public ErrorNumber Code
        {
            get => code;
            set => code = value;
        }

        public string Message
        {
            get => message;
            set => message = value;
        }
    }
}