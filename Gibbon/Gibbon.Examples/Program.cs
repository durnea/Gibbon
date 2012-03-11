using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gibbon.Http.Rendering;
using Gibbon.Http.Routing;
using Gibbon.Http;

namespace Gibbon.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            Renderer just = new Renderer();
            Router GET = new Router("get");

            GET["/{:name}"] = context => just.RenderView("hello", "person", new Dictionary<string, object> 
            { 
                {"name", context.Route.Parameters["name"]} 
            });

            GET["/{:controller}/{:action}"] = context => just.Render(context);

            Server.CreateServer((req, res) =>
            {
                var context = GET.Route(req);

                if (context.StatusCode == 404)
                    res.End("Well, we don't have that!");
                else
                    res.End(GET.Route(req));

            }).Listen(8080);
        }
    }
}
