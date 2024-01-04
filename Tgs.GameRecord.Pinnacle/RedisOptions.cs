using Tgs.GameRecord.Pinnacle.Core;
namespace Tgs.GameRecord.Pinnacle;

public record RedisOptions
{
    public string ConnectionString { get; set; } = "localhost";
}
