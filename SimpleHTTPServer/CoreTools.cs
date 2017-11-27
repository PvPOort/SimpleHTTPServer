using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHTTPServer
{
    public class CoreTools
    {
        [DataContract]
        public class JsonHttpRequest : Serialization.SerializableObject
        {
            [DataMember]
            internal string body { get; set; }

            [DataMember]
            internal string signature { get; set; }

            public JsonHttpRequest()
            {
                body = "";
                signature = "";
            }
        }

        [DataContract]
        public class JsonHttpResponse : Serialization.SerializableObject
        {
            [DataMember]
            internal string status { get; set; }

            [DataMember]
            internal string method { get; set; }

            [DataMember]
            internal string body { get; set; }
        }

        public enum EResponseType
        {
            ERT_Json,
            ERT_HTML,

            ERT_Count
        }

        public enum EResultCode
        {
            OK = 0,
            INVALID_SERVICE = 1,
            INVALID_METHOD = 2,
            ARG_NOT_FOUND = 3,
            WRONG_SIGNATURE = 4,
            INVALID_JSON_OBJECT = 5,

            Error = 200,

            Count,
        }

        public class ResponseData
        {
            public EResultCode Code = EResultCode.Count;
            public EResponseType Type = EResponseType.ERT_Count;
            public String Body = "";

            public ResponseData()
            {

            }

            public ResponseData(EResponseType InType)
            {
                Code = EResultCode.Error;
                Type = InType;
                Body = "";
            }

            public ResponseData(EResponseType InType, String InBody)
            {
                Code = EResultCode.OK;
                Type = InType;
                Body = InBody;
            }

            public ResponseData(EResponseType InType, EResultCode InCode)
            {
                Code = InCode;
                Type = InType;
                Body = "";
            }

            public ResponseData(EResultCode InCode, EResponseType InType, String InBody)
            {
                Code = InCode;
                Type = InType;
                Body = InBody;
            }
        }
    }
}
