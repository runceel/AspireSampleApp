using AspireSampleApp;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
// sampleDb �̂��߂� DbContext ��ǉ� 
builder.AddSqlServerDbContext<SampleDbContext>("sampleDb");

var host = builder.Build();

// �}�C�O���[�V���������s���ăe�X�g�p�̃f�[�^��ǉ�
using var scope = host.Services.CreateScope();
await using (var dbContext = scope.ServiceProvider.GetRequiredService<SampleDbContext>())
{
    await dbContext.Database.MigrateAsync();

    if (!await dbContext.Tweets.AnyAsync())
    {
        await dbContext.Tweets.AddAsync(new Tweet { Text = "Hello, World!" });
        await dbContext.Tweets.AddAsync(new Tweet { Text = "����ɂ��́A���E!" });
        await dbContext.SaveChangesAsync();
    }
}
