using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace TCPServer
{
    class Server
    {
        private Thread _serverThread=null;
       private bool _threadIsRun=false;
        private GlobalVal _globalVal= GlobalVal.Instance();


        private IPAddress _ipAddress = IPAddress.Parse("127.0.0.1");
        private const int port = 55555;
        private TcpListener _server;
        public void Run()
        {
            _server = new TcpListener(new IPEndPoint(_ipAddress, port));
            _server.Start();

            while(_threadIsRun)
            {


                try
                {
                    TcpClient tcpClient;


                        tcpClient = _server.AcceptTcpClient();



                        int clientID = _globalVal.GetClientID();
                        Client client = new Client(clientID, tcpClient);

                        NetworkStream stream = new NetworkStream(tcpClient.Client);

                        client.ThreadStart();
                    


           



                }
                catch (Exception e)
                {


                }

                Thread.Sleep(100);
            }



        }



        public bool ThreadStart()
        {

            try
            {
                _threadIsRun = true;
                _serverThread = new Thread(Run);
                _serverThread.Start();

                return true;
            }
            catch(Exception e)
            {
                _threadIsRun = false;
  
                return false;
            }
        }

        public bool ThreadStop() {

            _threadIsRun = false;
            _server.Stop();
            foreach (var client in _globalVal.GetClient())
            {
                client.ThreadIsRun = false;

            };

            try
            {
                if (_serverThread != null)
                {
                    _serverThread.Join(1000);
                }

                return true;


            }
            catch (Exception e)
            {

                if (_serverThread != null)
                {
                    _serverThread.Abort(1000);
                }


                return false;
            }
            finally {
                _serverThread = null;
                _server = null;
            
            }
        }






    }
}
