namespace Gibbon.Http
{

    public class Context
    {
        #region Properties

        public RoutingContext Route { get; set; }

        public string ResponseString { get; set; }
        public int StatusCode { get; set; }

        #endregion

        #region Constructors and Creators

        internal Context(int status, string response)
        {
            StatusCode = status;
            ResponseString = response;
        }

        internal Context()
        {
            StatusCode = 200;
        }

        public static Context NotFound(string message = "")
        {
            return new Context(404, message);
        }

        #endregion

        #region Conversions

        public static implicit operator Context(string response)
        {
            return new Context(200, response);
        }

        #endregion
    }
}
