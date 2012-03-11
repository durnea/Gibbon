using System;
using System.Net;

namespace Gibbon.Http
{
    public delegate void ServerRequestHandler(ServerRequest request, ServerResponse response);

    public class Server
    {
        private HttpListener _listener;
        private bool _active;
        private bool _async;
        private string _prefix;
        private int _threads;
        private int _currentThreads;
        private readonly ServerRequestHandler _handler;

        Server(ServerRequestHandler handler)
        {
            _handler = handler;
        }

        public static Server CreateServer(ServerRequestHandler handler, int threads = 1000, bool restarted = false)
        {
            var server = new Server(handler);

            if (!server._active)
            {
                server._listener = new HttpListener();
                server._active = true;
                server._async = threads > 1;
                server._threads = threads;
            }

            return server;
        }

        public void Listen(int port, string domain = "*")
        {
            _prefix = "http://" + domain + ":" + port + "/";
            _listener.Prefixes.Add(_prefix);
            _listener.Start();

            if (_async)
                Receive();
            else
                ReceiveSync();
        }

        private void ReceiveSync()
        {
            try
            {
                HttpListenerContext context = _listener.GetContext();
                var response = new ServerResponse(ref context);
                var request = new ServerRequest(ref context);
                _handler(request, response);
                ReceiveSync();
            }
            catch (Exception e)
            {
                Logging.Console.Error("Exception thrown -> ReceivedSync : {0}", e.Message);
            }
        }

        private void Receive()
        {
            if (_listener.IsListening)
            {
                try
                {
                    IAsyncResult result = _listener.BeginGetContext(ListenerCallback, _handler);
                    _currentThreads++;
                    if (_currentThreads > _threads)
                        result.AsyncWaitHandle.WaitOne();
                    Receive();
                }
                catch (Exception e)
                {
                    Logging.Console.Error("Exception thrown -> Received : {0}", e.Message);
                }
            }
        }

        private void ListenerCallback(IAsyncResult result)
        {
            try
            {
                if (_listener.IsListening)
                {
                    HttpListenerContext context = _listener.EndGetContext(result);
                    ((ServerRequestHandler)result.AsyncState)(new ServerRequest(ref context), new ServerResponse(ref context));
                    _currentThreads--;
                }
            }
            catch (Exception e)
            {
                Logging.Console.Error("Exception thrown -> ListenerCallback : {0}", e.Message);
            }

            _currentThreads--;
        }

        public void Close()
        {
            _listener.Stop();
            _listener.Close();
        }

    }
}
