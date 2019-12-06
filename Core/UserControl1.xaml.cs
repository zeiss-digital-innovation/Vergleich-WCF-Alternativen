using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Core
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public string Username
        {
            get { return NameTextBox.Text; }
            set { NameTextBox.Text = value; }
        }
        public string ChatText
        {
            get { return ChatTextBox.Text; }
            set { ChatTextBox.Text = value; }
        }
        public string ChatPanel
        {
            get { return ChatPanelTextBox.Text; }
            set { ChatPanelTextBox.Text = value; }
        }
        public string Userlist
        {
            get { return (string)UserlistLabel.Content; }
            set { UserlistLabel.Content = value; }
        }

        public event RoutedEventHandler CustomNameClick;
        public event RoutedEventHandler CustomSendClick;
        public UserControl1()
        {
            InitializeComponent();
            ChatView.Visibility = Visibility.Hidden;
        }

        private void Name_Button_Click(object sender, RoutedEventArgs e)
        {
            if (CustomNameClick != null)
            {
                CustomNameClick(this, new RoutedEventArgs());
                if(ChatView.Visibility != Visibility.Visible)
                    ChatView.Visibility = Visibility.Visible;
            }
        }

        private void Send_Button_Click(object sender, RoutedEventArgs e)
        {
            if (CustomSendClick != null)
            {
                CustomSendClick(this, new RoutedEventArgs());
            }
        }

        private void OnKeyDownHandler(Object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Send_Button_Click(this, new RoutedEventArgs());
        }
    }
}