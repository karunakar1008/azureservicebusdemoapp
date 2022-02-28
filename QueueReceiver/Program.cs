using Azure.Messaging.ServiceBus;
using System;
using System.Threading.Tasks;

namespace QueueReceiver
{
    class Program
    {
        static string connectionString = "Endpoint=sb://csnapps.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=oablfx3KxFgeGIxgrz7o7ehDBCircddQUF0igrWvrxQ=";
        static string queueName = "dssdemoqueue";
        static ServiceBusClient client;
        static ServiceBusProcessor processor;
        static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            Console.WriteLine($"Received: {body}, Count: {args.Message.DeliveryCount}");
            //await args.CompleteMessageAsync(args.Message);
        }
        static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
        static async Task Main()
        {
            client = new ServiceBusClient(connectionString);
            processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions()
            {
                ReceiveMode = ServiceBusReceiveMode.PeekLock,
                AutoCompleteMessages = false
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
