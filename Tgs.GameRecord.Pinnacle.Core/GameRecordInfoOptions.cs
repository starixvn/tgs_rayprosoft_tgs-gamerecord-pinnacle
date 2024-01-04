namespace Tgs.GameRecord.Pinnacle.Core;

public record GameRecordInfoOptions
{
    public string GameProviderCode { get; set; } = default!;
    public string GameExternalId { get; set; } = default!;
    public string GameProviderConfiguration { get; init; } = default!;
    public string HangfirePrefix { get; set; } = "gamerecord-pinnacle-hangfire:";
    public string HangfireLocking { get; set; } = "gamerecord-pinnacle-locking:";
    public double IntervalMinutes { get; init; } = 3;
    public double IntervalMinutesMax { get; init; } = 1440;
    public double IntervalMinutesMinimum { get; init; } = 1;
}
