using AspireSampleApp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.

// Redis �̏o�̓L���b�V���̍\���� SQL Server �̃f�[�^�x�[�X�̍\��
builder.AddRedisOutputCache("redis");
builder.AddSqlServerDbContext<SampleDbContext>("sampleDb");


var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

// �o�̓L���b�V���̃~�h���E�F�A��ǉ�
app.UseOutputCache();


// 1 ���L���b�V������
app.MapGet("/tweets",
    [OutputCache(Duration = 60)]
    (SampleDbContext sampleDbContext) =>
    {
        return sampleDbContext.Tweets.AsAsyncEnumerable();
    });

// �f�[�^�ǉ�
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
