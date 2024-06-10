var builder = DistributedApplication.CreateBuilder(args);

// Redis と SQL Server を追加
IResourceBuilder<IResourceWithConnectionString> createDatabase()
{
    // ローカル実行時には SQL Server のコンテナを使って発行時には接続文字列を使う
    if (builder.ExecutionContext.IsRunMode)
    {
        var sqlServer = builder.AddSqlServer("sqlserver");
        return sqlServer.AddDatabase("sampleDb");
    }
    else
    {
        return builder.AddConnectionString("sampleDb");
    }
}

var redis = builder.ExecutionContext.IsRunMode ? 
    builder.AddRedis("redis") :
    builder.AddConnectionString("redis");
var sampleDb = createDatabase();

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

