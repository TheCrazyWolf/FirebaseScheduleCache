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
            await WaitForAllowedTime(stoppingToken);
            
            var currentDate = DateTime.Now;

            while (currentDate.DayOfWeek != DayOfWeek.Saturday && currentDate.DayOfWeek != DayOfWeek.Sunday)
                currentDate = currentDate.AddDays(1);
            

            for (int i = 0; i < 2; i++)
            {
                _logger.LogInformation($"Begin caching at: {currentDate}");
                
                try
                {
                    var query = new ScheduleQuery()
                        .WithAllForSearchType(ScheduleSearchType.Employee)
                        .WithDelay(2500)
                        .WithDate(DateOnly.FromDateTime(DateTime.Now));

                    var schedule = await _clientSamgkApi.Schedule.GetScheduleAsync(query, stoppingToken);
                    var json = JsonConvert.SerializeObject(schedule);
                    await _firebaseClient
                        .Child(DateOnly.FromDateTime(DateTime.Now).ToString("yy-MM-dd"))
                        .PutAsync(json);
                }
                catch (Exception e)
                {
                    _logger.LogInformation($"Failed cache at: {currentDate}");
                }
                _logger.LogInformation($"End caching at: {currentDate}");
                currentDate = currentDate.AddDays(1);
            }
            
            await Task.Delay(12600000, stoppingToken);
        }
    }

    protected async Task WaitForAllowedTime(CancellationToken cancellationToken)
    {
        while (!CanWorkSerivce(DateTime.Now))
        {
            await Task.Delay(1000, cancellationToken);
        }
    }

    protected virtual bool CanWorkSerivce(DateTime nowTime)
    {
#if DEBUG
        return true;
#else
        return nowTime.Hour switch
        {
            >= 19 or <= 7 => false,
            _ => true
        };
#endif
    }
}