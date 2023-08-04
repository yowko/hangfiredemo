using Hangfire;
using HangfireDemo;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<UpdateCacheService>();
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseInMemoryStorage()); //使用 memory 來儲存資料

builder.Services.AddHangfireServer();


var app = builder.Build();

const string updateCacheId= "UpdateCache";
app.Services.GetService<IRecurringJobManager>()
    .AddOrUpdate(updateCacheId, 
        ()=> app.Services.GetService<UpdateCacheService>()!.GetAsync(new CancellationToken()),
        "0 0 * * *",
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Shanghai")
        }
    );

app.Services.GetService<IRecurringJobManager>()?.Trigger(updateCacheId);

app.MapGet("/", () => "Hello World!");

app.Run();