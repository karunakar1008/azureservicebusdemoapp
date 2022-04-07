using Microsoft.Azure.ServiceBus;
using System;
using System.Threading.Tasks;
using System.Transactions;

namespace QueueSenderWithTransaction
{
    class Program
    {
        static string connectionString = "Endpoint=sb://nsdssmydemoservicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=vEolqxkly+SUazqMAriu62qr/rQQKuBB8IpoZeEXp0g=";
        static string queueName = "demo-queue";
        static async Task Main()
        {
            var queueClient = new QueueClient(connectionString, queueName);
            using (TransactionScope scope = new TransactionScope())
            {
                Console.WriteLine("Prepare 10 mesages and send as one transaction!");

                for (int i = 0; i < 10; i++)
                {
                    // Send a message
                    var message = $"Message- {i}";
                    Message msg = new Message(System.Text.Encoding.UTF8.GetBytes(message));
                    msg.PartitionKey = "demopartition";
                    queueClient.SendAsync(msg).Wait();
                    Console.WriteLine(message);
                }

                Console.WriteLine("Done!");
                Console.WriteLine();
                // Should we commit the transaction?
                Console.WriteLine("Commit sent 10 messages? (yes or no)");
                string reply = Console.ReadLine();
                if (reply.ToLower().Equals("yes"))
                    scope.Complete();
            }
            Console.WriteLine();
        }
    }
}
