using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Gibbon.Extensions;

namespace Gibbon.Http
{
    public class ServerResponse
    {
        readonly HttpListenerContext _context;

        public int StatusCode { get { return _context.Response.StatusCode; } set { _context.Response.StatusCode = value; } }
        public Dictionary<string, string> Headers { get { return _context.Response.Headers.ToDictionary(); } }

        internal ServerResponse(ref HttpListenerContext context)
        {
            _context = context;
        }

        #region Headers

        public void End(Context context)
        {
            StatusCode = context.StatusCode;

            if (!string.IsNullOrEmpty(context.ResponseString))
            {
                var encoding = Encoding.ASCII;
                byte[] data = encoding.GetBytes(context.ResponseString);

                _context.Response.OutputStream.Write(data, 0, data.Length);
            }

            _context.Response.Close();
        }

        public void WriteHead(int status, Dictionary<string, string> headers = null)
        {
            StatusCode = status;
            _context.Response.ProtocolVersion = new Version("1.1");
            _context.Response.ContentType = "text/plain";

            if (headers != null)
                foreach (var header in headers)
                    Headers.Add(header.Key, header.Value);

            _context.Response.KeepAlive = true;
            _context.Response.Headers.Set(HttpResponseHeader.Server, "gibbon-httpd");
        }

        public void SetHeader(string key, string value)
        {
            Headers.AddOrReplace(key, value);
        }

        public string GetHeader(string key)
        {
            if (Headers.ContainsKey(key))
                return Headers[key];

            return string.Empty;
        }

        public void RemoveHeader(string key)
        {
            Headers.Remove(key);
        }

        #endregion

        public void Write(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                var encoding = Encoding.ASCII;
                byte[] data = encoding.GetBytes(content);

                _context.Response.OutputStream.Write(data, 0, data.Length);
            }
        }

        public void End(string content = "")
        {
            if (!string.IsNullOrEmpty(content))
            {
                var encoding = Encoding.ASCII;
                byte[] data = encoding.GetBytes(content);

                _context.Response.OutputStream.Write(data, 0, data.Length);
                _context.Response.OutputStream.Close();
            }
            _context.Response.Close();
        }

        public void Stream(Stream stream)
        {
            //stream data
        }

        public void Abort()
        {
            _context.Response.Abort();
        }
    }
}
