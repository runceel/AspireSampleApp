using AspireSampleApp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.

// Redis の出力キャッシュの構成と SQL Server のデータベースの構成
builder.AddRedisOutputCache("redis");
builder.AddSqlServerDbContext<SampleDbContext>("sampleDb");


var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

// 出力キャッシュのミドルウェアを追加
app.UseOutputCache();


// 1 分キャッシュする
app.MapGet("/tweets",
    [OutputCache(Duration = 60)]
    (SampleDbContext sampleDbContext) =>
    {
        return sampleDbContext.Tweets.AsAsyncEnumerable();
    });

// データ追加
app.MapPost("/tweets", async (
    [FromBody]Tweet tweet, 
    SampleDbContext sampleDbContext,
    ILogger<Program> logger) =>
{
    sampleDbContext.Tweets.Add(tweet);
    var updated = await sampleDbContext.SaveChangesAsync();
    logger.LogInformation(
        "Tweet {text} was added. Updated {updated} rows.", 
        tweet.Text, 
        updated);
});

app.Run();
