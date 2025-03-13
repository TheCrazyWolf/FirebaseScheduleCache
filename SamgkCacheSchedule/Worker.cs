using ClientSamgk;
using ClientSamgk.Models;
using ClientSamgk.Models.Enums.Schedule;
using Firebase.Database;
using Newtonsoft.Json;

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
            try
            { 
                var query = new ScheduleQuery()
                    .WithAllForSearchType(ScheduleSearchType.Employee)
                    .WithDelay(0)
                    .WithDate(DateOnly.FromDateTime(DateTime.Now));
                
                var schedule = await _clientSamgkApi.Schedule.GetScheduleAsync(query, stoppingToken);
                var json = JsonConvert.SerializeObject(schedule);
                await _firebaseClient
                    .Child(DateOnly.FromDateTime(DateTime.Now).ToString("yy-MM-dd"))
                    .PutAsync(json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}