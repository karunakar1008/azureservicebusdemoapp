using Azure.Messaging.ServiceBus;
using System;
using System.Threading.Tasks;

namespace QueueSender
{
    class Program
    {
        static string connectionString = "Endpoint=sb://csnapps.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=oablfx3KxFgeGIxgrz7o7ehDBCircddQUF0igrWvrxQ=";
        static string queueName = "dssdemoqueue";
        static async Task Main()
        {
            ServiceBusClient client = new ServiceBusClient(connectionString);
            ServiceBusSender sender = client.CreateSender(queueName);
            //using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();
            while (true)
            {
                Console.WriteLine("Enter Message (exit to terminate): ");
                string m = Console.ReadLine();
                if (m == "exit")
                    break;
                var msg = new ServiceBusMessage(m);
                msg.ApplicationProperties.Add("Author", "sandeep");
                msg.ApplicationProperties.Add("CreatedAt", DateTime.Now);
                msg.ApplicationProperties.Add("Source", "DemoApp");

                //msg.TimeToLive = new TimeSpan(0, 0, 5);
                msg.MessageId = msg.GetHashCode().ToString();
                await sender.SendMessageAsync(msg);
                Console.WriteLine("Sent...");
            }
            await sender.DisposeAsync();
            await client.DisposeAsync();
        }
    }
}
