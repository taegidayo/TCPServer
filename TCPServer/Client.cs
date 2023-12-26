using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace TCPServer
{
    class Client
    {
        private TcpClient _tcpClient = null;
        private int _clientID;
        private GlobalVal _globalVal = GlobalVal.Instance();
        private NetworkStream _stream = null;

        private Thread _heartbeatThread;
        private Thread _writerThread;
        private Thread _readerThread;
        private Thread _checkWriteMessageThread;
        private Thread _checkReadMessageThread;

        private bool _threadIsRun = false;
        private Queue<(ServerMessageType type, int senderID, string message)> _sendQueue = new Queue<(ServerMessageType, int, string)>();
        private Queue<(byte[] encodedMsg, int length)> _receiveQueue = new Queue<(byte[] s, int length)>();


        private object _lockSendQuene = new object();
        private object _lockReceiveQueue = new object();
        private object _lockSelectedClient = new object();


        private List<int> _selectedClients = new List<int>();

        bool _isConnected = false;


        const string stx = "start";
        const string etx = "end";

        const string separator = "\r\n";



        public Client(int clientID, TcpClient tcpClient)
        {
            _clientID = clientID;
            this._tcpClient = tcpClient;
            _stream = new NetworkStream(tcpClient.Client);
        }

        public bool ThreadIsRun {

            set {
                _threadIsRun = value;
            }
        }


        public void RunHeartbeat()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            AddSendQueue(ServerMessageType.Heartbeat, _globalVal.ServerID, _clientID.ToString());
            int interval = 2000;

            while (_threadIsRun)
            {


                if (_tcpClient.Client.Poll(0, SelectMode.SelectRead)) {
                    byte[] buff = new byte[1];

                    // 연결이 종료 된 경우 SocketFlags.Peek의 값이 0이 나옴.
                    if (_tcpClient.Client.Receive(buff, SocketFlags.Peek) == 0)
                    {
                        _threadIsRun = false;
                        ThreadStop();

                    }
                }
                

                if (_isConnected)
                {
           
                    interval = 5000;
                }

                if (stopwatch.ElapsedMilliseconds >= interval)
                {

                    AddSendQueue(ServerMessageType.Heartbeat, _globalVal.ServerID, _clientID.ToString());
                    stopwatch.Restart();
                }
                Thread.Sleep(1);
            }

        }


        public void RunCheckMessage()
        {

            while (_threadIsRun)
            {
                var msgList = _globalVal.GetMessageToSend(_clientID);



                foreach (var msg in msgList)
                {

                    AddSendQueue(msg.type, msg.senderID, msg.message);

                }

                Thread.Sleep(1);
            }


        }


        public void AddSendQueue(ServerMessageType type, int senderID, string message)
        {
       
                _sendQueue.Enqueue((type, senderID, message));
            
        }

        public (ServerMessageType type, int senderID, string message) GetSendQueue()
        {
        lock(_lockSendQuene)
            {
                return _sendQueue.Dequeue();
            }
        }




        public void RunWriter()
        {
            while (_threadIsRun)
            {
                String message = stx + separator;

                if (_sendQueue.Count > 0)
                {
                    try
                    {
                 
                        var queueData=GetSendQueue();
                         message = message + (int)queueData.type + separator + queueData.senderID + separator + queueData.message + separator + etx;
                            byte[] encodedMsg = Encoding.UTF8.GetBytes(message);
                            _stream.Write(encodedMsg, 0, encodedMsg.Length);
                            _stream.Flush();


               
                    }
                    catch (Exception e)
                    {
                 

                    }
                }
                Thread.Sleep(40);

            }
        }

        public void AddReceiveQueue(byte[] encodedMsg, int length)
        {
            lock (_lockReceiveQueue)
            {
                _receiveQueue.Enqueue((encodedMsg, length));
            }
        }

        public (byte[] encodedMsg, int length) GetReceiveQueue()
        {
            lock (_lockReceiveQueue)
            {
                return _receiveQueue.Dequeue();
            }
        }

        public void RunReader()
        {
            byte[] encodedMsg = new byte[1024];
            int msgLength;

            while (_threadIsRun)
            {
                try
                {

                    if (_stream.DataAvailable)
                    {
                        msgLength = _stream.Read(encodedMsg, 0, encodedMsg.Length);
                        AddReceiveQueue(encodedMsg, msgLength);

                    }

                    Thread.Sleep(1);
                }
                catch (Exception e)
                {

  
                }

                //Thread.Sleep(1);

            }

        }


        public void RunReadMessage()
        {
            string[] slice;
            while (_threadIsRun)
            {
                if (_receiveQueue.Count > 0)
                {
        
                    var data = GetReceiveQueue();
                  
                    string receivedMsg = Encoding.UTF8.GetString(data.encodedMsg, 0, data.length);
                    slice = receivedMsg.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                    Console.WriteLine(string.Join(" ",slice));

                    if (slice[(int)MessageFormat.STX] != stx)
                    {
                        continue;
                    }
                    if (slice[(int)MessageFormat.ETX] != etx)
                    {
                        continue;
                    }
                    int type = int.Parse(slice[(int)MessageFormat.Type]);
                    if (type == (int)ClientMessageType.SelectedClients)
                    {
                        string[] datus = slice[(int)MessageFormat.Message].Split(new string[] { "///" }, StringSplitOptions.RemoveEmptyEntries);

                        lock (_lockSelectedClient)
                        {
                            foreach (string a in datus)
                            {
                                _selectedClients.Add(int.Parse(a));


                            }

                        }



                    }
                    else if (type == (int)ClientMessageType.Chat)
                    {
                        lock (_lockSelectedClient)
                        {
                            foreach (int client in _selectedClients)
                            {

                                if (client == 10000000)
                                {
                                    _globalVal.AddMessageLog(MessageLogType.Received, slice[(int)MessageFormat.Message], _clientID);

                                }
                                else
                                {
                                    _globalVal.AddMessageToSend(ServerMessageType.Chat, client, _clientID, slice[(int)MessageFormat.Message]);
                                }



                            }

                            _selectedClients.Clear();

                        }
                    }
                    else if (type == (int)ClientMessageType.Heartbeat)
                    {

                        if (!_isConnected)
                        {

                            _isConnected = true;

                            List<int> clientList = _globalVal.GetClientsIDList(_clientID);
                            string msg = "";

                            foreach (int clientID in clientList)
                            {
                                msg += clientID + "///";


                            }


                            AddSendQueue(ServerMessageType.ClientList, _globalVal.ServerID, msg);

                            _globalVal.AddClient(_clientID, _tcpClient, this);

                            //todo 사용자의 접속로그를 추가하고 싶다면...
                            //_globalVal.AddMessageLog(MessageLogType.Connected, _clientID.ToString());

                        }
                        else
                        {
                        }
                    }
                    else if (type == (int)ClientMessageType.IsClosed)
                    {

                        ThreadStop();

                    }

                }
                Thread.Sleep(1);
            }


        }







        public bool ThreadStart()
        {

            try
            {
                _threadIsRun = true;
                _heartbeatThread = new Thread(new ThreadStart(RunHeartbeat));
                _writerThread = new Thread(new ThreadStart(RunWriter));
                _checkWriteMessageThread = new Thread(new ThreadStart((RunCheckMessage)));
                _readerThread = new Thread(new ThreadStart(RunReader));
                _checkReadMessageThread = new Thread(new ThreadStart(RunReadMessage));





                _heartbeatThread.Start();
                _writerThread.Start();
                _readerThread.Start();
                _checkWriteMessageThread.Start();
                _checkReadMessageThread.Start();

                return true;
            }
            catch (Exception e)
            {
                _threadIsRun = false;
                return false;
            }



        }
        public bool ThreadStop()
        {
                

            _threadIsRun = false;
            string byeMessage = stx + separator + (int)ServerMessageType.IsClosed + separator + _globalVal.ServerID + separator + "Bye" + separator + etx;
            byte[] encodedMsg = Encoding.UTF8.GetBytes(byeMessage);



            try
            {

                if (_tcpClient != null)
                {
                    _stream.Write(encodedMsg, 0, encodedMsg.Length);



                }
                if (_heartbeatThread != null)
                {
                    _heartbeatThread.Join(1000);
                }
                if (_writerThread != null)
                {
                    _writerThread.Join(1000);
                }
                if (_readerThread != null)
                {
                    _readerThread.Join(1000);
                }
                if (_checkWriteMessageThread != null)
                {
                    _checkWriteMessageThread.Join(1000);
                }
                if (_checkReadMessageThread != null)
                {
                    _checkReadMessageThread.Join(1000);
                }


                return true;
            }
            catch (Exception e)
            {
                if (_heartbeatThread != null)
                {
                    _heartbeatThread.Abort();
                }
                if (_writerThread != null)
                {
                    _writerThread.Abort();
                }
                if (_readerThread != null)
                {
                    _readerThread.Abort();
                }

                if (_checkWriteMessageThread != null)
                {
                    _checkWriteMessageThread.Abort();
                }
                if (_checkReadMessageThread != null)
                {
                    _checkReadMessageThread.Abort();
                }
                return false;
            }
            finally
            {
                if (_stream != null)
                {
                    _globalVal.leftClient(_clientID);
                }

                _stream.Flush();
                _stream.Close();

                _tcpClient.Close();
                _heartbeatThread = null;
                _writerThread = null;
                _readerThread = null;
                _checkWriteMessageThread = null;
                _checkReadMessageThread = null;
            }



        }











    }
}
