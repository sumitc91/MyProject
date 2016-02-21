using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace OrbitPageSignalR.Hubs
{
    [HubName("ChatHub")]
    public class ChatHub : Hub
    {
        public void LetsChat(string Cl_Name, string Cl_Message)
        {
            Clients.All.NewMessage(Cl_Name, Cl_Message);
        }

        public void BroadCastMessage(String msgFrom, string msg)
        {
            Clients.All.receiveMessage(msgFrom,msg);
        }
    }
}
