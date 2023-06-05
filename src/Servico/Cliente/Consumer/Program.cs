using System.Text.Json;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Azure.ServiceBus;
using Catalogo.API.Models;
using Microsoft.Extensions.Configuration;

namespace Consumer
{
    class Program
    {
        const string ServiceBusConnectionString = "Endpoint=sb://msgproduct-bus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=EkhhYngOqDQRXtcQlXNXK/EqnpYK5+vM6+ASbGbSpHk=";
        const string QueueName = "product";
        static IQueueClient queueClient;


        static void Main(string[] args)
        {
            queueClient = new QueueClient(ServiceBusConnectionString, QueueName);

            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);

            Console.WriteLine($"Consumer is ready");
            Console.ReadKey();

            queueClient.CloseAsync().Wait();
        }

        static async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

            var messageBody = Encoding.UTF8.GetString(message.Body);

            var produto = JsonConvert.DeserializeObject<Produto>(messageBody);

            await queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}