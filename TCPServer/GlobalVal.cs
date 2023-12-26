using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace TCPServer
{

    enum ServerMessageType
    {
        ClientList,
        Chat,
        Heartbeat,
        IsClosed,
        NewClient,
        LeftClient,
    }
    enum ClientMessageType
    {
        SelectedClients,
        Chat,
        Heartbeat,
        IsClosed,
    }


    enum MessageLogType
    {
        Connected,
        Left,
        Received,
        Sent,
    }

    enum MessageFormat
    {
        STX,
        Type,
        SenderID,
        Message,
        ETX,
    }


    enum ClientStatus
    {
        Connected,
        Left,

    }

    class GlobalVal
    {
        private static GlobalVal _globarVal;

        private List<(int clientID, TcpClient tcpClient, Client client)> _clients = new List<(int, TcpClient, Client)>();
        private List<(int, ClientStatus)> updatedClients = new List<(int, ClientStatus)>();
        private Queue<(MessageLogType type, string message, int? clientID, int? receiveCount)> messageLog = new Queue<(MessageLogType, string, int?, int?)>();
        private List<(ServerMessageType type, int clientID, int senderID, string message)> messageToSend = new List<(ServerMessageType type, int clientID, int senderID, string message)>();

        private object lockMTS = new object();
        private object lockClients = new object();


        private const int serverID = 10000000;

        public static GlobalVal Instance()
        {
            if (_globarVal == null)
            {
                _globarVal = new GlobalVal();
            }
            return _globarVal;


        }



        public int ServerID
        {
            get
            {
                return serverID;

            }


        }


        public void AddMessageLog(MessageLogType type, string message, int? clientID = null, int? receiveCount = null)
        {
            messageLog.Enqueue((type, message, clientID, receiveCount));

        }

        public (MessageLogType type, string message, int? clientID, int? receiveCount)? GetMessageLog()
        {
            if (messageLog.Count > 0)
            {
                return messageLog.Dequeue();
            }
            else
                return null;


        }

        public List<(ServerMessageType type, int senderID, string message)> GetMessageToSend(int clientID)
        {

            lock (lockMTS)
            {
                List<(ServerMessageType type, int senderID, string message)> rtnMsg = messageToSend.FindAll(item => item.clientID == clientID)
                                                                                .Select(item => (item.type, item.senderID, item.message))
                                                                                .ToList();
                messageToSend.RemoveAll(message => message.clientID == clientID);
                return rtnMsg;
            }


        }
        public void AddMessageToSend(ServerMessageType type, int clientID, int senderID, string message)
        {
            lock (lockMTS)
            {
                messageToSend.Add((type, clientID, senderID, message));
            }
        }




        public void AddClient(int clientID, TcpClient tcpClient, Client client)
        {
            lock (lockClients)
            {
                updatedClients.Add((clientID, ClientStatus.Connected));
                AddMessageLog(MessageLogType.Connected, clientID.ToString());

                foreach (var clients in _clients)
                {
                    AddMessageToSend(ServerMessageType.NewClient, clients.clientID, ServerID, clientID.ToString());


                }



                _clients.Add((clientID, tcpClient, client));
            }

        }


        public List<(int, ClientStatus)> GetUpdatedClients()
        {
            lock (lockClients)
            {
                List<(int, ClientStatus)> rtnClients = updatedClients.ToList();

                updatedClients.Clear();

                return rtnClients;
            }

        }


        public List<int> GetClientsIDList(int clientID)
        {

            lock (lockClients)
            {
                List<int> rtnIDList = _clients.FindAll(client => (client.clientID != clientID)).Select(client => client.clientID).ToList();



                return rtnIDList;
            }
        }







        public int GetClientID()
        {
            lock (lockClients)
            {
                int clientID;

                Random rand = new Random();

                do
                {
                    clientID = rand.Next(10000001, 99999999);


                    break;

                } while (_clients.FirstOrDefault(client => client.clientID == clientID).tcpClient != null);

                return clientID;
            }
        }

        public List<Client> GetClient() 
        {
            return _clients.Select(e => e.client).ToList();
        }

        public void leftClient(int clientID)
        {

            lock (lockClients)
            {
                _clients.RemoveAll(client => client.clientID == clientID);


                updatedClients.Add((clientID, ClientStatus.Left));

                foreach (var clients in _clients)
                {
                    AddMessageToSend(ServerMessageType.LeftClient, clients.clientID, ServerID, clientID.ToString());
                }


                //todo 사용자가 떠났음을 알리는 로그를 추가하고 싶다면
                AddMessageLog(MessageLogType.Left, clientID.ToString());




            }
        }

    }
}
