using Azure.Messaging.ServiceBus;
using System;
using System.Threading.Tasks;

namespace QueueReceiver
{
    class Program
    {
        static string connectionString = "Endpoint=sb://nsdssmydemoservicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=vEolqxkly+SUazqMAriu62qr/rQQKuBB8IpoZeEXp0g=";
        static string queueName = "demosession-queue";
        static ServiceBusClient client;
        static ServiceBusSessionProcessor processor;
        static async Task MessageHandler(ProcessSessionMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            Console.WriteLine($"Received: {body}, Count: {args.Message.DeliveryCount}");
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
            processor = client.CreateSessionProcessor(queueName, new ServiceBusSessionProcessorOptions()
            {
                ReceiveMode = ServiceBusReceiveMode.PeekLock,
                AutoCompleteMessages = false,
                SessionIds = { "s1" }
            });
            processor.ProcessMessageAsync += MessageHandler;
            processor.ProcessErrorAsync += ErrorHandler;
            await processor.StartProcessingAsync();
            Console.WriteLine("Wait for a minute and then press any key to end the processing");
            Console.ReadKey();
            // stop processing
            Console.WriteLine("\nStopping the receiver...");
            await processor.StopProcessingAsync();
            Console.WriteLine("Stopped receiving messages");
            await processor.DisposeAsync();
            await client.DisposeAsync();
        }
    }
}
