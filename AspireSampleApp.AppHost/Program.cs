var builder = DistributedApplication.CreateBuilder(args);

// Redis と SQL Server を追加
var redis = builder.AddRedis("redis");
var sqlServer = builder.AddSqlServer("sqlserver");
var sampleDb = sqlServer.AddDatabase("sampleDb");

// Web アプリに Redis と SQL Server を参照として追加
builder.AddProject<Projects.AspireSampleApp>("aspiresampleapp")
    .WithReference(redis)
    .WithReference(sampleDb);

// デプロイ用のマニフェストを作る時には実行しないように RunMode のときだけ DB 初期化のワーカーを追加
if (builder.ExecutionContext.IsRunMode)
{
    // DB 初期化のワーカー
    builder.AddProject<Projects.AspireSampleApp_DbInitializer>("aspiresampleapp-dbinitializer")
        // DB への参照を追加
        .WithReference(sampleDb);
}

builder.Build().Run();
