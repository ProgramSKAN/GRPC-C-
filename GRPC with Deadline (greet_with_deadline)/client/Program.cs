using Greeting;
using Grpc.Core;
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
            var client = new GreetingService.GreetingServiceClient(channel);

            try
            {
                var response = client.greet_with_deadline(
                                        new GreetingRequest() { Name = "ProgramSkan" },
                                        //deadline: DateTime.UtcNow.AddMilliseconds(500)//no error since server takes 300ms to finish task
                                        deadline: DateTime.UtcNow.AddMilliseconds(100)//error because server takes 300ms to finish task
                                        );
                Console.WriteLine(response.Result);
            }
            catch (RpcException e) when (e.StatusCode==StatusCode.DeadlineExceeded)
            {
                Console.WriteLine("Error : "+e.Status.Detail);
            }

            channel.ShutdownAsync().Wait();
            Console.ReadKey();
        }
    }
}
