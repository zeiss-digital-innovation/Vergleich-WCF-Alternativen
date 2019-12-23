using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Grpc.Core;
using GrpcServer;
using System.Threading.Tasks;

namespace WpfClient.ViewModel
{
    public class MainViewModel : ViewModelBase, IDataErrorInfo
    {
        // Connecting to server
        static Channel channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);
        readonly Chat.ChatClient client = new Chat.ChatClient(channel);

        // With Nuget Package PropertyChanged.Fody is no need to define RaisePropertyChanged
        public string Username { get; set; }
        public string ChatText { get; set; }
        public static ObservableCollection<string> _Userlist { get; set; } = new ObservableCollection<string>();
        public string Userlist { get; set; }
        public static ObservableCollection<Message> Messages { get; set; } = new ObservableCollection<Message>();
        public string WindowTitle { get; set; }
        public Visibility LoginVisibility { get; set; }
        public Visibility ChatViewVisibility { get; set; }
        public ICommand LoginCommand { get; private set; }
        public ICommand SendCommand { get; private set; }
        public ICommand LogOutCommand { get; private set; }

        CancellationTokenSource tokenSource = new CancellationTokenSource();
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

                var request = new User { Name = Username };
                Task[] tasks = new Task[2];

                var replies = client.Join(request);
                tasks[0] = ListenAsync(replies.ResponseStream, tokenSource.Token);

                var list = client.GetUserlist(new Google.Protobuf.WellKnownTypes.Empty());
                tasks[1] = ListenUserlistAsync(list.ResponseStream, tokenSource.Token);
            }
        }

        private async Task ListenUserlistAsync(IAsyncStreamReader<Userlist> stream, CancellationToken token)
        {
            try
            {
                await foreach (var e in stream.AsAsyncEnumerable(token))
                {
                    Userlist = e.User.ToUserString();
                }
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.Cancelled)
            {
                return;
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }

        private async Task ListenAsync(IAsyncStreamReader<MessageModel> stream, CancellationToken token)
        {
            try
            {
                await foreach (var m in stream.AsAsyncEnumerable(token))
                {
                    var t = new Message(m.User, m.Text, m.Time.ToDateTime());
                    Messages.Add(t);
                }
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.Cancelled)
            {
                return;
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }

        public void SendMethod()
        {
            if (!string.IsNullOrEmpty(ChatText))
            {
                var message = new MessageModel { User = Username, Text = ChatText };
                _ = client.Send(message);
            }
            ChatText = string.Empty;
        }

        public void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to close the application?", "Chat Application", MessageBoxButton.YesNo) == MessageBoxResult.No)
                e.Cancel = true;
            else
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
                if (!string.IsNullOrEmpty(Username))
                    client.LogOut(new User { Name = Username });
                channel.ShutdownAsync().Wait();
            }
                
        }
    }
}