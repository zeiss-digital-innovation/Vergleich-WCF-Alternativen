using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WCFChat;

namespace WPFChatView
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string Name = "";
        static InstanceContext context = new InstanceContext(new MyCallBack());
        ChatServiceReference.ChatClient server = new ChatServiceReference.ChatClient(context);
        public MainWindow()
        {
            InitializeComponent();
            ChatView.Visibility = Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Name == name.Text)
                return;
            else if (Name == "")
            {
                Name = name.Text;
                server.Join(Name);

                ChatView.Visibility = Visibility.Visible;
                var history = server.GetChats();
                foreach (var chat in history)
                    chatPanel.Text += DisplayChat(chat) + Environment.NewLine;
            }
            else
            {
                server.LogOut();
                Name = name.Text;
                server.Join(Name);
            }

            server.RefreshAsync();
        }

        public string DisplayChat(ChatInfos chat)
        {
            return chat.Time.ToShortTimeString() + " - " + chat.Name + ": " + chat.Chat;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (chatText.Text != "")
                server.SendChat(Name, chatText.Text);
            chatText.Clear();
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (Name != "")
                server.LogOut();
            server.Close();
        }

        private void OnKeyDownHandler(Object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Button_Click_1(this, new RoutedEventArgs());
        }
    }
    public class MyCallBack : ChatServiceReference.IChatCallback
    {
        public void ReceiveChat(ChatInfos chatInfos)
        {
            Window window = Application.Current.MainWindow;
            (window as MainWindow).chatPanel.Text += (window as MainWindow).DisplayChat(chatInfos) + Environment.NewLine;
        }

        public void RefreshUserList(string[] userList)
        {
            Window window = Application.Current.MainWindow;
            (window as MainWindow).userCount.Text = userList.Length + " Teilnehmer:";
            foreach (string user in userList)
                (window as MainWindow).userCount.Text += " " + user;
        }
    }
}
