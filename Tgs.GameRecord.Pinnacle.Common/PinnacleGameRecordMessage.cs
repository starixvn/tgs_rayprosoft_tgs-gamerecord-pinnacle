using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRecord.Pinnacle.Common;

public record PinnacleGameRecordMessage
{
    [Required]
    [DataType(DataType.Url)]
    public Uri ApiUrl { get; init; } = default!;
    [Required]
    public string AgentCode { get; init; } = default!;
    [Required]
    public string AgentKey { get; init; } = default!;
    [Required]
    public string SecretKey { get; init; } = default!;
    [Required]
    [DataType(DataType.DateTime)]
    public DateTimeOffset StartTime { get; init; } = default!;
    [Required]
    [DataType(DataType.DateTime)]
    public DateTimeOffset EndTime { get; init; } = default!;
}
