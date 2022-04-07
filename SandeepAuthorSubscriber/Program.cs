using Azure.Messaging.ServiceBus;
using System;
using System.Threading.Tasks;

namespace SandeepAuthorSubscriber
{
    class Program
    {
        static string connectionString = "Endpoint=sb://nsdssmydemoservicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=vEolqxkly+SUazqMAriu62qr/rQQKuBB8IpoZeEXp0g=";
        static string topicName = "books";
        static string subscriptionName = "SandeepAuthor"; //AllSubscription or SandeepSubscription
        static ServiceBusClient client;
        static ServiceBusProcessor processor;
        static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            Console.WriteLine($"Delivery count:{args.Message.DeliveryCount}");
           Console.WriteLine($"Received: {body} from subscription: {subscriptionName}");
            await args.CompleteMessageAsync(args.Message);
        }
        static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
        static async Task Main()
        {
            client = new ServiceBusClient(connectionString);
            var options = new ServiceBusProcessorOptions();
            options.ReceiveMode = ServiceBusReceiveMode.PeekLock;
            options.AutoCompleteMessages = true;
            processor = client.CreateProcessor(topicName, subscriptionName, options);
            try
            {
                processor.ProcessMessageAsync += MessageHandler;
                processor.ProcessErrorAsync += ErrorHandler;
                await processor.StartProcessingAsync();
                Console.WriteLine("Wait for a minute and then press any key to end the processing");
                Console.ReadKey();
                Console.WriteLine("\nStopping the receiver...");
                await processor.StopProcessingAsync();
                Console.WriteLine("Stopped receiving messages");
            }
            finally
            {
                await processor.DisposeAsync();

                await client.DisposeAsync();
            }
        }
    }
}
