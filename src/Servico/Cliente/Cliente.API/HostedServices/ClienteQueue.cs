﻿using Catalogo.API.Models;
using Microsoft.Azure.ServiceBus;
using System.Text;
using System.Text.Json;

public class ProductQueueConsumer : IHostedService
{
    static IQueueClient queueClient;
    private readonly IConfiguration _config;

    public ProductQueueConsumer(IConfiguration config)
    {
        _config = config;
        var serviceBusConnection = _config.GetValue<string>("AzureServiceBus");
        queueClient = new QueueClient(serviceBusConnection, "product");
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("############## Starting Consumer - Queue ####################");
        ProcessMessageHandler();
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("############## Stopping Consumer - Queue ####################");
        await queueClient.CloseAsync();
        await Task.CompletedTask;
    }

    private void ProcessMessageHandler()
    {
        var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
        {
            MaxConcurrentCalls = 1,
            AutoComplete = false
        };

        queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
    }

    private async Task ProcessMessagesAsync(Message message, CancellationToken token)
    {
        Console.WriteLine("### Processing Message - Queue ###");
        Console.WriteLine($"{DateTime.Now}");
        Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");
        Produto _produto = JsonSerializer.Deserialize<Produto>(message.Body);

        await queueClient.CompleteAsync(message.SystemProperties.LockToken);
    }

    private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
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