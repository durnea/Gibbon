using System.Collections.Generic;
using System.Text.RegularExpressions;
using Gibbon.Extensions;

namespace Gibbon.Http.Routing
{
    public class Route
    {
        public bool AcceptsMultipart { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }

        public string Key { get; internal set; }
        public Regex Expression { get; internal set; }
        public List<string> Literals = new List<string>();

        static readonly Regex ExtractLiterals = new Regex("{:(?<literal>([a-z0-9]*))}", RegexOptions.Compiled);

        internal Route(string route, bool acceptsMultipart = false, string controller = null, string action = null) //, RequestDecoder decoder = null)
        {
            AcceptsMultipart = acceptsMultipart;
            Controller = controller;
            Action = action;
            var literals = ExtractLiterals.Matches(route);
            foreach (Match literal in literals)
            {
                string value = literal.Groups["literal"].Value;

                if (!Literals.Contains(value))
                    Literals.Add(value);

                route = value == "params" ? route.Replace("/{:params}", "(/(?<params>(.)*))") : route.Replace("{:" + value + "}", "(?<" + value + ">([^/]+))");
            }

            Expression = new Regex(route, RegexOptions.Compiled);
        }

        public static Route Now(string route, bool acceptsMultipart = false, string controller = null, string action = null)
        {
            return new Route(route, acceptsMultipart, controller, action);
        }

        public RoutingContext GetContext(Match match)
        {
            var context = new RoutingContext();
            foreach (var literal in Literals)
            {
                if (literal == "action")
                    context.Action = match.Groups[literal].Value;
                else if (literal == "controller")
                    context.Controller = match.Groups[literal].Value;
                else if (literal == "params")
                    context.NamelessParameters = match.Groups[literal].Value.Split('/');
                else
                {
                    if (context.Parameters == null)
                        context.Parameters = new Dictionary<string, string>();
                    context.Parameters.AddOrReplace(literal, match.Groups[literal].Value);
                }
            }

            return context;
        }
    }
}
