using Azure.Messaging.ServiceBus;
using System;
using System.Threading.Tasks;

namespace QueueSender
{
    class Program
    {
        static string connectionString = "Endpoint=sb://nsdssmydemoservicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=vEolqxkly+SUazqMAriu62qr/rQQKuBB8IpoZeEXp0g=";
        static string queueName = "demosession-queue";
        static async Task Main()
        {
            ServiceBusClient client = new ServiceBusClient(connectionString);
            ServiceBusSender sender = client.CreateSender(queueName);
            //using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();
            while (true)
            {
                Console.WriteLine("Enter Message (exit to terminate): ");
                string m = Console.ReadLine();
                Console.WriteLine("Enter session id:");
                string sessionId = Console.ReadLine();
                if (m == "exit")
                    break;
                var msg = new ServiceBusMessage(m);
                msg.SessionId = sessionId;
                msg.ApplicationProperties.Add("Author", "sandeep");
                msg.ApplicationProperties.Add("CreatedAt", DateTime.Now);
                msg.ApplicationProperties.Add("Source", "DemoApp");

                //msg.TimeToLive = new TimeSpan(0, 0, 10);
                msg.MessageId = m;
                await sender.SendMessageAsync(msg);
                Console.WriteLine("Sent...");
            }
            await sender.DisposeAsync();
            await client.DisposeAsync();
        }
    }
}
