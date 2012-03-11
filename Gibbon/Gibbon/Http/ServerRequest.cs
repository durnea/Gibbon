using System.Collections.Generic;
using System.Net;
using Gibbon.Extensions;

namespace Gibbon.Http
{
    public class ServerRequest
    {
        readonly HttpListenerContext _context;

        public string[] AcceptTypes { get { return _context.Request.AcceptTypes; } }

        public long Length { get { return _context.Request.ContentLength64; } }
        public string ContentType { get { return _context.Request.ContentType; } }
        public bool HasEmptyBody { get { return _context.Request.HasEntityBody; } }

        public Dictionary<string, string> Headers { get { return _context.Request.Headers.ToDictionary(); } }
        public string HttpMethod { get { return _context.Request.HttpMethod; } }

        public bool IsLocal { get { return _context.Request.IsLocal; } }
        public bool IsSecure { get { return _context.Request.IsSecureConnection; } }
        public bool KeepAlive { get { return _context.Request.KeepAlive; } }

        public Dictionary<string, string> Query { get { return _context.Request.QueryString.ToDictionary(); } }
        public string RawUrl { get { return _context.Request.RawUrl; } }
        public string Referrer { get { return _context.Request.UrlReferrer.ToString(); } }

        public string UserAgent { get { return _context.Request.UserAgent; } }
        public string UserHostAddress { get { return _context.Request.UserHostAddress; } }
        public string UserHostName { get { return _context.Request.UserHostName; } }

        public string[] UserLanguages { get { return _context.Request.UserLanguages; } }

        internal ServerRequest(ref HttpListenerContext context)
        {
            _context = context;
        }


    }
}
