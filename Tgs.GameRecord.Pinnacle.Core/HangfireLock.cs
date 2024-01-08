using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Tgs.GameRecord.Pinnacle.Core;

namespace Tgs.GameRecord.Pinnacle.Core;

internal class HangfireLock(
    IOptions<GameRecordInfoOptions> options,
    IDatabase database) : IHangfireLock
{
    private readonly GameRecordInfoOptions _options = options.Value;

    public Task<bool> LockingAsync(string application, CancellationToken cancellationToken = default)
            => database.StringSetAsync(
            key: GetKey(application),
            value: new JsonObject
            {
                ["MachineName"] = Environment.MachineName,
                ["Application"] = application,
                ["LabelTime"] = DateTimeOffset.Now
            }.ToJsonString(),
            expiry: TimeSpan.FromSeconds(59),
            when: When.NotExists);

    private string GetKey(string application)
        => string.Concat(_options.HangfireLocking, application);
}
