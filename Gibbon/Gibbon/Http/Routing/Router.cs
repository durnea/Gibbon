using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Gibbon.Http.Routing
{
    public class Router
    {
        private readonly Dictionary<string, KeyValuePair<bool, RoutingDelegate>> _simpleRoutes =
            new Dictionary<string, KeyValuePair<bool, RoutingDelegate>>();

        private readonly Dictionary<string, KeyValuePair<Route, RoutingDelegate>> _routes =
            new Dictionary<string, KeyValuePair<Route, RoutingDelegate>>();

        private readonly string _method = "get";

        public Router(string method)
        {
            _method = method;
        }

        public RoutingDelegate this[string key]
        {
            set
            {
                if (key.Contains('{'))
                {
                    if (_routes.ContainsKey(key))
                    {
                        if (_routes[key].Value != value)
                            _routes[key] = new KeyValuePair<Route, RoutingDelegate>(new Route(key), value);
                    }
                    else
                        _routes.Add(key, new KeyValuePair<Route, RoutingDelegate>(new Route(key), value));
                }
                else
                {
                    if (_simpleRoutes.ContainsKey(key))
                        _simpleRoutes[key] = new KeyValuePair<bool, RoutingDelegate>(false, value);
                    else
                        _simpleRoutes.Add(key, new KeyValuePair<bool, RoutingDelegate>(false, value));
                }
            }
        }

        public RoutingDelegate this[string key, bool acceptsMultipart]
        {
            set
            {
                if (key.Contains('{'))
                {
                    if (_routes.ContainsKey(key))
                    {
                        if (_routes[key].Value != value)
                            _routes[key] = new KeyValuePair<Route, RoutingDelegate>(new Route(key, acceptsMultipart),
                                                                                   value);
                    }
                    else
                        _routes.Add(key,
                                   new KeyValuePair<Route, RoutingDelegate>(new Route(key, acceptsMultipart), value));
                }
                else
                {
                    if (_simpleRoutes.ContainsKey(key))
                        _simpleRoutes[key] = new KeyValuePair<bool, RoutingDelegate>(acceptsMultipart, value);
                    else
                        _simpleRoutes.Add(key, new KeyValuePair<bool, RoutingDelegate>(acceptsMultipart, value));
                }
            }
        }

        public Context Route(ServerRequest request, string missing = "", Context context = null)
        {
            if (request.HttpMethod.ToLower() == _method)
            {
                if (_simpleRoutes.ContainsKey(request.RawUrl))
                {
                    if (context == null)
                        return _simpleRoutes[request.RawUrl].Value(new Context());
                    return _simpleRoutes[request.RawUrl].Value(context);
                }
                foreach (var route in _routes)
                {
                    Match match = route.Value.Key.Expression.Match(request.RawUrl);
                    if (match.Success && match.Length == request.RawUrl.Length)
                    {
                        var contexter = new Context {Route = route.Value.Key.GetContext(match)};
                        return route.Value.Value(contexter);
                    }
                }

                return Context.NotFound(missing);
            }

            return Context.NotFound(missing);
        }
    }
}
