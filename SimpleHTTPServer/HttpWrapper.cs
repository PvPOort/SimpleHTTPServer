using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;


namespace SimpleHTTPServer
{
    public class HttpWrapper
    {
        private Mutex HttpRequestMutex = new Mutex();
        private int Port = 0;

        Dictionary<string, ServiceBase> Services = new Dictionary<string, ServiceBase>();

        public HttpWrapper(int InPort)
        {
            Port = InPort;
        }

        public void Start()
        {
            InitServices();

            HttpServer httpProcessor = HttpProcessor.InitHTTPServer(GetLocalIPAddress(), Port);

            httpProcessor.OnHandleHttpPOSTRequest = OnHandleHttpPOSTRequestDelegate;
            httpProcessor.OnHandleHttpGETRequest = OnHandleHttpGETRequestDelegate;
        }

        void InitServices()
        {
            Services.Add("service", new ServiceBase());
        }

        CoreTools.ResponseData InvokeServiceMethod(string Method, CoreTools.EResponseType ResponseType)
        {
            if (Method.Length == 0)
            {
                return new CoreTools.ResponseData(ResponseType, CoreTools.EResultCode.INVALID_SERVICE);
            }

            if (Method.IndexOf('/') == 0)
            {
                Method = Method.Remove(0, 1);
            }

            char[] DelimiterChars = { '/' };
            string[] Tokens = Method.Split(DelimiterChars);

            string ServiceName = "";
            string MethodName = "";

            if (Tokens.Length > 0)
            {
                ServiceName = Tokens[0];
            }

            if (Tokens.Length > 1)
            {
                MethodName = Tokens[1];
            }

            ServiceBase Service = Services.ContainsKey(ServiceName) ? Services[ServiceName] : null;

            if (Service == null)
            {
                return new CoreTools.ResponseData(ResponseType, CoreTools.EResultCode.INVALID_SERVICE);
            }

            Type TypeOfService = Service.GetType();
            MethodInfo ServiceMethod = TypeOfService.GetMethod(MethodName);

            if (ServiceMethod == null)
            {
                return new CoreTools.ResponseData(ResponseType, CoreTools.EResultCode.INVALID_METHOD);
            }

            return (CoreTools.ResponseData)ServiceMethod.Invoke(Service, null);
        }

        // Http Wrapper
        void ReturnWithStatus(CoreTools.EResponseType responseType, CoreTools.EResultCode status, string method, string body, HttpProcessor p)
        {
            String statusText = status.ToString();

            if (body.Length == 0)
            {
                body = "{}";
            }

            CoreTools.JsonHttpResponse response = new CoreTools.JsonHttpResponse();

            response.status = statusText;
            response.method = method;
            response.body = body;

            string responseStr = "";

            string FullResponse = "HTTP/1.1 200 OK\nContent-type:";

            switch (responseType)
            {
                case CoreTools.EResponseType.ERT_Json:
                    responseStr = response.Serialize();

                    responseStr = responseStr.Replace("body\":\"", "body\":").Replace("}\"", "}");    // Anal fix

                    FullResponse += " application/json\\nContent-Length:" + responseStr.Length + "\n\n";
                    break;
                case CoreTools.EResponseType.ERT_HTML:
                    responseStr = body;

                    FullResponse += " text/html; charset=utf-8\n\n";

                    if (status != CoreTools.EResultCode.OK)
                    {
                        responseStr = statusText;
                    }
                    break;
                default:
                    break;
            }

            FullResponse += responseStr;

            //Console.WriteLine("Sending response: {0}", FullResponse);

            p.outputStream.Write(FullResponse);

            return;
        }

        public void OnHandleHttpGETRequestDelegate(String method, HttpProcessor p)
        {
            HttpRequestMutex.WaitOne();

            Console.WriteLine("GET request: {0}", method);

            CoreTools.ResponseData ResponseData = InvokeServiceMethod(method, CoreTools.EResponseType.ERT_HTML);

            if (ResponseData == null)
            {
                ResponseData = new CoreTools.ResponseData(CoreTools.EResponseType.ERT_HTML);
            }

            ReturnWithStatus(ResponseData.Type, ResponseData.Code, method, ResponseData.Body, p);

            HttpRequestMutex.ReleaseMutex();
        }

        public void OnHandleHttpPOSTRequestDelegate(String method, String data, HttpProcessor p)
        {
            HttpRequestMutex.WaitOne();

            Console.WriteLine("POST request {0} data: {1}", method, data);

            var jObject = JObject.Parse(data);

            CoreTools.JsonHttpRequest requestObject = new CoreTools.JsonHttpRequest();

            ReturnWithStatus(CoreTools.EResponseType.ERT_Json, 0, method, "", p);

            HttpRequestMutex.ReleaseMutex();
        }

        // Helpers
        string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }

        string GetMd5Hash(MD5 md5Hash, string input)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}
