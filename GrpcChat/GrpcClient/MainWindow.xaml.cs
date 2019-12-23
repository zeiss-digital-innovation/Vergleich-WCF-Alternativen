using Grpc.Net.Client;
using GrpcServer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using Core;
using System.Threading;
using Grpc.Core;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace GrpcClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static GrpcChannel channel = GrpcChannel.ForAddress("https://localhost:5001");
        static Chat.ChatClient client = new Chat.ChatClient(channel);
        static UserControl1 input = new UserControl1();
        static string name = "";

        CancellationTokenSource tokenSource = new CancellationTokenSource();
        public MainWindow()
        {
            InitializeComponent();
            input = inputUserControl;
        }

        private async void OnCustomNameClickAsync(object sender, RoutedEventArgs e)
        {
            name = input.Username;
            var request = new User { Name = name };
            var replies = client.Join(request);
            var list = client.GetUserList(new Google.Protobuf.WellKnownTypes.Empty());

            var _ = ListenUserListAsync(list.ResponseStream, tokenSource.Token);
            var _2 = ListenAsync(replies.ResponseStream, tokenSource.Token);
        }

        static async Task ListenAsync(IAsyncStreamReader<MessageModel> stream, CancellationToken token)
        {
            await foreach (var message in stream.AsAsyncEnumerable(token))
            {
                input.ChatPanel += message.Time.ToDateTime().ToShortTimeString() + " - " + message.User + ": " + message.Text + Environment.NewLine;
            }
        }
        static async Task ListenUserListAsync(IAsyncStreamReader<UserNumber> stream, CancellationToken token)
        {
            while (await stream.MoveNext(token))
            {
                input.Userlist = stream.Current.Count.ToString() + " Teilnehmer";
            }
        }
        private void OnCustomSendClick(object sender, RoutedEventArgs e)
        {
            var message = new MessageModel { User = name, Text = input.ChatText };
            _ = client.Send(message);
            input.ChatText = "";
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            client.LogOut(new User { Name = name });
            tokenSource.Cancel();
        }
    }
    internal static class StreamExtensions
    {
        public static async IAsyncEnumerable<T> AsAsyncEnumerable<T>(
            this IAsyncStreamReader<T> stream,
            [EnumeratorCancellation] CancellationToken token)
        {
            while (await stream.MoveNext(token))
            {
                yield return stream.Current;
            }
        }
    }
}
