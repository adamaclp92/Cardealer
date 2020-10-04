using Cardealer;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Server
{
    class Program
    {
        const int Port = 50051;
        static void Main(string[] args)
        {
            Grpc.Core.Server server = null;
            

            try     
            {
                server = new Grpc.Core.Server()
                {
                   Services = { CarDealing.BindService(new CarDealerService())},
                    Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
                };

                server.Start();

                Console.WriteLine("The server is listening on the port: " + Port);
                Console.ReadKey();
            }
            catch (IOException e)
            {
                Console.WriteLine("The server failed to start:" + e.Message);

            }
            finally
            {
                if (server != null)
                    server.ShutdownAsync().Wait();
            }
        }
    }
}
