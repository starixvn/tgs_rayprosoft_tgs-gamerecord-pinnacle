namespace Tgs.GameRecord.Pinnacle.Core;

public record RedisOptions
{
    public string ConnectionString { get; set; } = "localhost";
}