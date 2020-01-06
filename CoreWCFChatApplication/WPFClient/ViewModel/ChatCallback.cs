using System;
using System.Collections.Generic;
using System.Windows;
using CoreWCFChatApplication.Shared.Contract;
using GalaSoft.MvvmLight.Ioc;

namespace WPFClient.ViewModel
{
    class ChatCallback : IChatCallback
    {
        public void ReceiveChat(Message message)
        {
            var main = SimpleIoc.Default.GetInstance<MainViewModel>();
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                new Action(() => main.Messages.Add(message)));
        }

        public void RefreshUserList(List<string> userList)
        {
            var main = SimpleIoc.Default.GetInstance<MainViewModel>();
            string users = userList.Count + " Teilnehmer:";
            foreach (string user in userList)
                users += " " + user;
            main.Userlist = users;
        }
    }
}
