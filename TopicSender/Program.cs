using Azure.Messaging.ServiceBus;
using System;
using System.Threading.Tasks;

namespace TopicSender
{
    class Program
    {
        static string connectionString = "Endpoint=sb://nsdssmydemoservicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=vEolqxkly+SUazqMAriu62qr/rQQKuBB8IpoZeEXp0g=";
        static string topicName = "books";


        static ServiceBusClient client;
        static ServiceBusSender sender;
        static async Task Main()
        {
            client = new ServiceBusClient(connectionString);
            sender = client.CreateSender(topicName);
            while (true)
            {
                Console.WriteLine("Enter Book name (exit to terminate): ");
                string m = Console.ReadLine();
                if (m == "exit")
                    break;
                var msg = new ServiceBusMessage(m);
                Console.WriteLine("Enter Author of message: ");
                string author = Console.ReadLine();
                msg.ApplicationProperties.Add("Author", author);

                msg.MessageId = msg.GetHashCode().ToString();
                await sender.SendMessageAsync(msg);
                Console.WriteLine("Sent...");
            }
            await sender.DisposeAsync();
            await client.DisposeAsync();
        }
    }
}
