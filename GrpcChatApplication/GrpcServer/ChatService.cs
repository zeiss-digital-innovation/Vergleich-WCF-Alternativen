using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace GrpcServer
{
    public class ChatService : Chat.ChatBase
    {
        private static List<MessageModel> history = new List<MessageModel>();
        private static Userlist users = new Userlist();
        private static readonly TimeSpan Interval = TimeSpan.FromSeconds(10);
        public ChatService()
        {
        }

        public override async Task Join(User request, IServerStreamWriter<MessageModel> responseStream, ServerCallContext context)
        {
            var token = context.CancellationToken;
            users.User.Add(request);
            Console.WriteLine("User {0} is connected!", request);
            int i = 0;
            while (!token.IsCancellationRequested)
            {
                if (i < history.Count)
                    await responseStream.WriteAsync(history[i++]);
            }
        }
        public override Task<Empty> Send(MessageModel message, ServerCallContext context)
        {
            message.Time = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow);
            Console.WriteLine("Message {0} is sended!", message);
            history.Add(message);
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> LogOut(User request, ServerCallContext context)
        {
            users.User.Remove(request);
            Console.WriteLine("User {0} is disconnected!", request);
            return Task.FromResult(new Empty());
        }
        public override async Task GetUserlist(Empty _, IServerStreamWriter<Userlist> responseStream, ServerCallContext context)
        {
            var token = context.CancellationToken;
            int i = -1;
            while (!token.IsCancellationRequested)
            {
                if (i != users.GetHashCode())
                {
                    await responseStream.WriteAsync(users);
                    i = users.GetHashCode();
                }
                // Userlist refresh every 10 seconds
                await Task.Delay(Interval, token);
            }
        }
    }
}
