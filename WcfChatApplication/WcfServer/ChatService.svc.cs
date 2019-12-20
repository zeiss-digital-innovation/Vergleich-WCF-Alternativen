using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WcfServer
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    public class ChatService : IChatService
    {
        Dictionary<IChatCallback, string> users = new Dictionary<IChatCallback, string>();
        List<Message> history = new List<Message>();
        List<string> userList = new List<string>();
        public List<Message> GetChats()
        {
            return history;
        }

        public void Join(string name)
        {
            var connection = OperationContext.Current.GetCallbackChannel<IChatCallback>();
            users[connection] = name;
            userList.Add(name);
        }

        public void LogOut()
        {
            var connection = OperationContext.Current.GetCallbackChannel<IChatCallback>();
            if (users.TryGetValue(connection, out string name))
                userList.Remove(name);
            users.Remove(connection);
            Refresh();
        }

        public void Refresh()
        {
            foreach (var user in users.Keys)
            {
                user.RefreshUserList(userList);
            }
        }

        public void SendChat(string name, string text)
        {
            var connection = OperationContext.Current.GetCallbackChannel<IChatCallback>();
            Message newChat = new Message(name, text, DateTime.Now);
            history.Add(newChat);

            if (!users.TryGetValue(connection, out name))
                return;

            foreach (var user in users.Keys)
            {
                user.ReceiveChat(newChat);
            }
        }
    }
}
