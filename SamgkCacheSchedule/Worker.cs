using ClientSamgk;
using Firebase.Database;

namespace SamgkCacheSchedule;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly FirebaseClient _firebaseClient;
    private readonly ClientSamgkApi _clientSamgkApi;

    public Worker(ILogger<Worker> logger, FirebaseClient firebaseClient, ClientSamgkApi clientSamgkApi)
    {
        _logger = logger;
        _firebaseClient = firebaseClient;
        _clientSamgkApi = clientSamgkApi;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            
            await Task.Delay(1000, stoppingToken);
        }
    }
}