using Dummy;
using Greet;
using Grpc.Core;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        const string target = "127.0.0.1:50051";//connection IP on which the client want to connect
        static async Task Main(string[] args)
        {
            //WITHOUT SSL
            //Channel channel = new Channel(target, ChannelCredentials.Insecure);
            //WITH SSL
            var clientCert = File.ReadAllText("ssl/client.crt");
            var clientKey = File.ReadAllText("ssl/client.key");
            var caCrt = File.ReadAllText("ssl/ca.crt");

            var channelCredentials = new SslCredentials(caCrt, new KeyCertificatePair(clientCert, clientKey));
            Channel channel = new Channel("localhost",50051, channelCredentials);

            //-------------------------------------------------------------


            await channel.ConnectAsync().ContinueWith((task) =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                    Console.WriteLine("The Client Connected Successfully");
            });
            //var client = new DummyService.DummyServiceClient(channel);

            var client = new GreetingService.GreetingServiceClient(channel);

            DoSimpleGreet(client);//UNARY
            //await DoManyGreetings(client);//SERVER STREAMING
            //await DoLongGreet(client);//CLIENT STREAMING
            //await DoGreetEveryone(client);//BI-DIRECTINAL STREAMING

            channel.ShutdownAsync().Wait();
            Console.ReadKey();
        }

        //UNARY
        public static void DoSimpleGreet(GreetingService.GreetingServiceClient client)
        {
            var greeting = new Greeting()
            {
                FirstName = "Program",
                LastName = "Skan"
            };
            var request = new GreetingRequest() { Greeting = greeting };
            var response = client.Greet(request);
            Console.WriteLine(response.Result);
        }

        //SERVER STREAMING
        public static async Task DoManyGreetings(GreetingService.GreetingServiceClient client)
        {
            var greeting = new Greeting()
            {
                FirstName = "Program",
                LastName = "Skan"
            };
            var request = new GreetManyTimesRequest() { Greeting = greeting };
            var response = client.GreetManyTimes(request);
            while (await response.ResponseStream.MoveNext())
            {
                Console.WriteLine(response.ResponseStream.Current.Result);
                await Task.Delay(1000);
            }
        }

        //CLIENT STREAMING
        public static async Task DoLongGreet(GreetingService.GreetingServiceClient client)
        {
            var greeting = new Greeting()
            {
                FirstName = "Program",
                LastName = "Skan"
            };
            var request = new LongGreetRequest() { Greeting = greeting };
            var stream = client.LongGreet();

            foreach (int i in Enumerable.Range(1, 10))
            {
                await stream.RequestStream.WriteAsync(request);
            }
            await stream.RequestStream.CompleteAsync();
            var response = await stream.ResponseAsync;
            Console.WriteLine(response.Result);
        }

        //BI-DIRECTINAL STREAMING
        public static async Task DoGreetEveryone(GreetingService.GreetingServiceClient client)
        {
            var stream = client.GreetEveryone();
            var responseReaderTask = Task.Run(async () =>
            {
                while (await stream.ResponseStream.MoveNext())
                {
                    Console.WriteLine("Received : " + stream.ResponseStream.Current.Result);
                }
            });

            Greeting[] greetings =
            {
                new Greeting(){ FirstName="Program1",LastName="Skan1"},
                new Greeting(){ FirstName="Program2",LastName="Skan2"},
                new Greeting(){ FirstName="Program3",LastName="Skan3"},
                new Greeting(){ FirstName="Program4",LastName="Skan4"},
                new Greeting(){ FirstName="Program5",LastName="Skan5"}
            };

            foreach(var greeting in greetings)
            {
                Console.WriteLine("Sending : " + greeting.ToString());
                await stream.RequestStream.WriteAsync(new GreetEveryoneRequest()
                {
                    Greeting = greeting
                });
                //await Task.Delay(1000);
            }
            await stream.RequestStream.CompleteAsync();
            await responseReaderTask;

        }
    }
}
