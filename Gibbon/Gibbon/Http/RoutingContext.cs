using System.Collections.Generic;

namespace Gibbon.Http
{
    public class RoutingContext
    {
        public bool IsRouted
        {
            get;
            internal set;
        }

        public string Controller
        {
            get;
            internal set;
        }

        public string Action
        {
            get;
            internal set;
        }

        public Dictionary<string, string> Parameters
        {
            get;
            internal set;
        }

        public string[] NamelessParameters
        {
            get;
            internal set;
        }
    }
}
