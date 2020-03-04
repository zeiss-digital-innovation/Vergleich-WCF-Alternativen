using CommonServiceLocator;
using WpfClient.ChatServiceReference;

namespace WpfClient.ViewModel
{
    class ChatCallBack : ChatServiceReference.IChatServiceCallback
    {
        public void ReceiveChat(Message message)
        {
            var main = ServiceLocator.Current.GetInstance<MainViewModel>();
            main.Messages.Add(message);
        }

        public void RefreshUserList(string[] userList)
        {
            var main = ServiceLocator.Current.GetInstance<MainViewModel>();
            string users = userList.Length + " Teilnehmer:";
            foreach (string user in userList)
                users += " " + user;
            main.Userlist = users;
        }
    }
}