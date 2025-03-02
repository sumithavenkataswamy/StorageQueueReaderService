using Microsoft.Extensions.DependencyInjection;
using StorageQueueReaderService.BackgroundServices;

namespace StorageQueueReaderService;

public static class Configuration
{
    public static IServiceCollection AddStorageQueueReaderService(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<StorageQueueBackgroundService>();
        return serviceCollection;
    }
}