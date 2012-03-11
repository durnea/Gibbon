using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Gibbon.Http.Rendering
{
    public class Renderer
    {
        private Dictionary<string, MethodInfo> files = new Dictionary<string, MethodInfo>();
        public Type[] types;

        public Renderer()
        {
            var assembly = Assembly.GetCallingAssembly();
            types = assembly.GetTypes();

            foreach(Type t in types)
            {
                if (t.Namespace == "Gorilla.Rendered.Ecs")
                {
                    MethodInfo mi = t.GetMethod("Render");
                    files.Add(t.Name, mi);
                }
            }
        }

        public Context Render(Context context, Dictionary<string, object> parameters = null)
        {
            if (string.IsNullOrEmpty(context.Route.Action)) context.Route.Action = "index";
            if (string.IsNullOrEmpty(context.Route.Action)) context.Route.Controller = "home";

            var name = context.Route.Controller+"_"+context.Route.Action;

            return Render(name, parameters);
        }

        public Context Render(string file, Dictionary<string, object> parameters = null)
        {
            if (files.ContainsKey(file))
                return (string)files[file].Invoke(null, new object[] { parameters });
            return new Context { StatusCode = 404 };
        }

        public Context RenderView(string controller, string action, Dictionary<string, object> parameters = null)
        {
            return Render(controller + "_" + action, parameters);
        }

        public Context RenderPartial(string controller, string partial, Dictionary<string, object> parameters = null)
        {
            return Render(controller + "__" + partial, parameters);
        }
    }
}
