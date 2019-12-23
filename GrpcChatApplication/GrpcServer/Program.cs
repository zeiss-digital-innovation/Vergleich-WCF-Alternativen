using System;
using Grpc.Core;

namespace GrpcServer
{
    class Program
    {
        const int Port = 50051;
        public static void Main(string[] args)
        {
            Server server = new Server
            {
                Services = { Chat.BindService(new ChatService()) },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("Chat server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...\n");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}
