using AspireSampleApp;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
// sampleDb のための DbContext を追加 
builder.AddSqlServerDbContext<SampleDbContext>("sampleDb");

var host = builder.Build();

// マイグレーションを実行してテスト用のデータを追加
using var scope = host.Services.CreateScope();
await using (var dbContext = scope.ServiceProvider.GetRequiredService<SampleDbContext>())
{
    await dbContext.Database.MigrateAsync();

    if (!await dbContext.Tweets.AnyAsync())
    {
        await dbContext.Tweets.AddAsync(new Tweet { Text = "Hello, World!" });
        await dbContext.Tweets.AddAsync(new Tweet { Text = "こんにちは、世界!" });
        await dbContext.SaveChangesAsync();
    }
}
