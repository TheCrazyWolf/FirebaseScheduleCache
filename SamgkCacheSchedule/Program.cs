using ClientSamgk;
using Firebase.Database;
using SamgkCacheSchedule;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<FirebaseClient>(s =>
    new FirebaseClient(builder.Configuration["Firebase:Url"], new FirebaseOptions()
    {
        AuthTokenAsyncFactory = () => Task.FromResult(builder.Configuration["Firebase:AuthToken"])
    })
);
builder.Services.AddSingleton<ClientSamgkApi>();

var host = builder.Build();
host.Run();