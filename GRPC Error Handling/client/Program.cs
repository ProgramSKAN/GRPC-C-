using Grpc.Core;
using Sqrt;
using System;
using System.Threading.Tasks;

namespace client
{
    class Program
    {
        const string target = "127.0.0.1:50052";
        static async Task Main(string[] args)
        {
            Channel channel = new Channel(target, ChannelCredentials.Insecure);
            await channel.ConnectAsync().ContinueWith((task) =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                    Console.WriteLine("The Client Connected Successfully");
            });

            var client = new SqrtService.SqrtServiceClient(channel);

            //int number = 16;//4
            int number = -1;//throws exception
            try
            {
                var response = client.sqrt(new SqrtRequest() { Number = number });
                Console.WriteLine(response.SquareRoot);
            }
            catch (RpcException e)
            {
                Console.WriteLine("Error : " + e.Status.Detail);
            }


            channel.ShutdownAsync().Wait();
            Console.ReadKey();
        }
    }
}
