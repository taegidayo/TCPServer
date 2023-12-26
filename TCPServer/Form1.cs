using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace TCPServer
{
    public partial class Form1 : Form
    {
        Server server = new Server();
        GlobalVal globalVal = GlobalVal.Instance();





        public Form1()
        {
            InitializeComponent();
            server.ThreadStart();
        }

        private void sendBtn_Click(object sender, EventArgs e)
        {
            SendMsg();
        }


        private void Textbox_KeyUp(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar == (char)Keys.Enter)
            {
                SendMsg();
            }
        }

        private void SendMsg()
        {
            string msgText = MessageTextBox.Text;


            if (msgText.Length != 0)
            {
                if (userCheckList.CheckedItems.Count == 0)
                {
                    userCheckList.SetItemChecked(0, true);
                }

                    if (userCheckList.CheckedItems.Count > 0)
                {
                    int receiveCount = 0;
                    foreach (string clientID in userCheckList.CheckedItems)
                    {
                        if (clientID != "All")
                        {
                            receiveCount++;
                            globalVal.AddMessageToSend(ServerMessageType.Chat, int.Parse(clientID), globalVal.ServerID, msgText);
                        }
                    }

                    if (receiveCount != 0)
                    {

                        globalVal.AddMessageLog(MessageLogType.Sent, MessageTextBox.Text, null, receiveCount);
                        MessageTextBox.Text = "";


                        //for (int checklistIdx = 0; checklistIdx < userCheckList.Items.Count; checklistIdx++)
                        //{

                        //    userCheckList.SetItemChecked(checklistIdx, false);

                        //}
                    }

                }

            }
        }

        private void checkClients_Tick(object sender, EventArgs e)
        {
            List<(int,ClientStatus)> updatedClients = globalVal.GetUpdatedClients();
      
            foreach ((int id, ClientStatus state) client in updatedClients.ToArray()) 
            {

                if (client.state == ClientStatus.Connected)
                {
                    userCheckList.Items.Insert(userCheckList.Items.Count, client.id.ToString());
                    userCheckList.SetItemChecked(0, false);

                }
                else 
                {
                    userCheckList.Items.Remove(client.id.ToString());
                }
            
            
            
            }



        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            server.ThreadStop();

        }


        private void userCheckList_ItemCheck(object sender, ItemCheckEventArgs e)
        {

            object checkedItem = userCheckList.Items[e.Index];
            if (checkedItem.ToString() == "All")
            {
                //bool checkState = e.NewValue == CheckState.Checked ? true : false;

                for (int checklistIdx = 1; checklistIdx < userCheckList.Items.Count; checklistIdx++)
                {
                    userCheckList.SetItemCheckState(checklistIdx, e.NewValue);
                }



            }
            else if (userCheckList.GetItemChecked(0) == true)
            {
                if (userCheckList.GetItemChecked(e.Index) == false)
                {
                    userCheckList.SetItemCheckState(0, CheckState.Unchecked);
                }
            }



        }

        private void checkMessages_Tick(object sender, EventArgs e)
        {
            var messageLog = globalVal.GetMessageLog();

            if (messageLog != null) {
       
                string message= DateTime.Now.ToShortTimeString()+":";
                switch (messageLog.Value.type) {
                    case MessageLogType.Connected:
                        message += $"[안내] {messageLog.Value.message}님이 접속하였습니다.";
                        break;
                    case MessageLogType.Left:
                        message += $"[안내] {messageLog.Value.message}님이 종료하였습니다.";
                        break;

                    case MessageLogType.Received:

                        message += $"[FROM {messageLog.Value.clientID}] {messageLog.Value.message}";
                        break;

                    case MessageLogType.Sent:
                        message += $"[{messageLog.Value.receiveCount}명에게:] {messageLog.Value.message}";
                        break;

                    default:
                        message += $"[오류]";
                        break;

                
                }
                MessageListBox.Items.Add(message);
                MessageListBox.TopIndex = MessageListBox.Items.Count - 1;
            }




        }

 

    }


}
