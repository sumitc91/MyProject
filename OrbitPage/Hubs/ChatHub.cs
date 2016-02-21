using System;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace OrbitPage.Hubs
{
    [HubName("ChatHub")]
    public class ChatHub : Hub
    {
        
        private static Dictionary<string, dynamic> connectedClients = new Dictionary<string, dynamic>();

        public void RegisterClient(string userName)
        {
            lock (connectedClients)
            {
                if (connectedClients.ContainsKey(userName))
                {
                    connectedClients[userName] = Clients.Caller;
                }
                else
                {
                    connectedClients.Add(userName, Clients.Caller);
                }
            }
            Clients.Caller.addMessage("'" + userName + "'registered.");
        }

        public void AddNotification(string notificationMessage, string toUser)
        {
            lock (connectedClients)
            {
                if (connectedClients.ContainsKey(toUser))
                {
                    dynamic client = connectedClients[toUser];
                    client.addMessage(notificationMessage);
                }
            }
        }

        public void Send(string name, string message)
        {
            Clients.All.sendMessage(name, message);
        }

        public void LetsChat(string Cl_Name, string Cl_Message)
        {
            Clients.All.NewMessage(Cl_Name, Cl_Message);
        }

        public void BroadCastMessage(String msgFrom, string msg)
        {
            Clients.All.receiveMessage(msgFrom, msg);
        }
    }
}
