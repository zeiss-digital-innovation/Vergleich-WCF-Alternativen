using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GrpcServer
{
    public class ChatService : Chat.ChatBase
    {

        private readonly ILogger<ChatService> _logger;
        private static List<MessageModel> history = new List<MessageModel>();
        private static UserNumber users = new UserNumber() { Count = 0 };

        public ChatService(ILogger<ChatService> logger)
        {
            _logger = logger;
        }

        public override async Task Join(User request, IServerStreamWriter<MessageModel> responseStream, ServerCallContext context)
        {
            var token = context.CancellationToken;
            users.Count++;
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
            history.Add(message);
            return Task.FromResult(new Empty());
        }
        public override Task<Empty> LogOut(User request, ServerCallContext context)
        {
            users.Count--;
            return Task.FromResult(new Empty());
        }
        public override async Task GetUserList(Empty _, IServerStreamWriter<UserNumber> responseStream, ServerCallContext context)
        {
            var token = context.CancellationToken;
            int i = -1;
            while (!token.IsCancellationRequested)
            {
                if (i != users.Count)
                    await responseStream.WriteAsync(users);
                i = users.Count;
            }
        }
    }
}
