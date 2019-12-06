using System;
using System.Threading.Tasks;
using GrpcServer;
using Grpc.Net.Client;

namespace GrpcServices
{
    class Program
    {
        static async Task Main()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var input = new HelloRequest { Name = "Anh" };
            var reply = await client.SayHelloAsync(input);

            Console.WriteLine("Greeting: " + reply.Message);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
