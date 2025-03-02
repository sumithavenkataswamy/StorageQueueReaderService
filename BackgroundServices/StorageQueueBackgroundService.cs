using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace StorageQueueReaderService.BackgroundServices;
public class StorageQueueBackgroundService : BackgroundService
{
    private readonly QueueServiceClient _queueServiceClient;
    private readonly ILogger<StorageQueueBackgroundService> _logger;
    private readonly IConfiguration _configuration;
    
    public StorageQueueBackgroundService(QueueServiceClient queueServiceClient , ILogger<StorageQueueBackgroundService> logger, IConfiguration configuration)
    {
        _queueServiceClient = queueServiceClient ?? throw new ArgumentNullException(nameof(queueServiceClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("StorageQueueBackgroundService is starting.");
        var queueName = _configuration["QueueName"];

        if (string.IsNullOrWhiteSpace(queueName))
        {
            throw new ArgumentNullException(nameof(queueName));
        }
        
        var  queueClient = _queueServiceClient.GetQueueClient(queueName);

        while(!stoppingToken.IsCancellationRequested)
        {
            // get the message

            QueueMessage queueMessage = await queueClient.ReceiveMessageAsync();

            if(queueMessage != null) 
            {
                // read message body
                var messageBody = queueMessage.Body.ToString();
                _logger.LogInformation($"Message Body: {messageBody}");
                
                // delete message from the queue
                await queueClient.DeleteMessageAsync(queueMessage.MessageId, queueMessage.PopReceipt);
            }
        }
    }
}