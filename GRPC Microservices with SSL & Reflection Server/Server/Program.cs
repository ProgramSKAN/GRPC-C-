using Greet;
using Grpc.Core;
using Grpc.Reflection;
using Grpc.Reflection.V1Alpha;
using System;
using System.Collections.Generic;
using System.IO;

namespace server
{
    class Program
    {
        const int Port = 50051;
        static void Main(string[] args)
        {
            //FOR SSL------------------------------------
            //var serverCert = File.ReadAllText("ssl/server.crt");
            //var serverKey = File.ReadAllText("ssl/server.key");
            //var keypair = new KeyCertificatePair(serverCert, serverKey);
            //var caCert = File.ReadAllText("ssl/ca.crt");

            //var credentials = new SslServerCredentials(new List<KeyCertificatePair>() { keypair }, caCert, true);
            //---------------------------------------------------
            //FOR REFLECTION
            var reflectionServiceImpl = new ReflectionServiceImpl(GreetingService.Descriptor, ServerReflection.Descriptor);
            //-------------------------------------------------------

            Server server = null;
            try
            {
                server = new Server()
                {
                    Services = { 
                        GreetingService.BindService(new GreetingServiceImpl()) ,
                        ServerReflection.BindService(reflectionServiceImpl)//FOR REFLECTION
                    },//it tells server when client calls greet function, it will call the implementaion of it which is GreetingServiceImpl()
                    
                    Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }//WITHOUT SSL
                    //Ports = { new ServerPort("localhost", Port, credentials) }//WITH SSL

                };
                server.Start();
                Console.WriteLine("The Server is listening on Port : " + Port);
                Console.ReadKey();

            }
            catch (IOException ex)
            {
                Console.WriteLine("The Server failed to start : " + ex.Message);
                throw;
            }
            finally
            {
                if (server != null)
                    server.ShutdownAsync().Wait();//running synchronously here.can be asyncronous if server and client is in different system
            }
        }
    }
}
