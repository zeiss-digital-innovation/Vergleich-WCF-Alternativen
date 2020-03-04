using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ServiceModel;
using System.Windows;
using System.Windows.Input;
using WpfClient.ChatServiceReference;

namespace WpfClient.ViewModel
{
    public class MainViewModel : ViewModelBase, IDataErrorInfo
    {
        // Connecting to server
        static InstanceContext context = new InstanceContext(new ChatCallBack());
        ChatServiceClient server = new ChatServiceClient(context);

        // With Nuget Package PropertyChanged.Fody is no need to define RaisePropertyChanged
        public string Username { get; set; }
        public string ChatText { get; set; }
        public string Userlist { get; set; } = "Teilnehmer";
        public ObservableCollection<Message> Messages { get; set; }
        public string WindowTitle { get; set; }
        public Visibility LoginVisibility { get; set; }
        public Visibility ChatViewVisibility { get; set; }
        public ICommand LoginCommand { get; private set; }
        public ICommand SendCommand { get; private set; }
        public ICommand LogOutCommand { get; private set; }

        // Error Handler
        public string Error => string.Empty;

        public string this[string propertyName]
        {
            get
            {
                switch (propertyName)
                {
                    case nameof(Username):
                        if (Username == "")
                            return "Username is required!";
                        break;
                }
                return string.Empty;
            }
        }

        public MainViewModel()
        {
            LoginVisibility = Visibility.Visible;
            ChatViewVisibility = Visibility.Collapsed;

            LoginCommand = new RelayCommand(LoginMethod);
            SendCommand = new RelayCommand(SendMethod);

            if (IsInDesignMode)
            {
                WindowTitle = "Chat Application (Design)";
            }
            else
            {
                WindowTitle = "Chat Application";
            }

            Application.Current.MainWindow.Closing += new CancelEventHandler(MainWindow_Closing);
        }

        public void LoginMethod()
        {
            if (!string.IsNullOrEmpty(Username))
            {
                LoginVisibility = Visibility.Collapsed;
                ChatViewVisibility = Visibility.Visible;

                server.Join(Username);
                var history = server.GetChats();

                Messages = new ObservableCollection<Message>(history);

                server.RefreshAsync();
            }
        }
        public void SendMethod()
        {
            if (!string.IsNullOrEmpty(ChatText))
                server.SendChat(Username, ChatText);
            ChatText = string.Empty;
        }

        public void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to close the application?", "Chat Application", MessageBoxButton.YesNo) == MessageBoxResult.No)
                e.Cancel = true;
            else
            {
                if (!string.IsNullOrEmpty(Username))
                    server.LogOut();
                server.Close();
            }
                
        }
    }
}