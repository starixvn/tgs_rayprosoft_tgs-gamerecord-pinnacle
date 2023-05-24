using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRecord.Pinnacle.ViewModels;

public record PinnacleViewModel
{
    [Required]
    public Uri ApiUrl { get; init; } = default!;
    [Required]
    public string AgentCode { get; init; } = default!;
    [Required]
    public string AgentKey { get; init; } = default!;
    [Required]
    public string SecretKey { get; init; } = default!;
    [Required]
    [DataType(DataType.DateTime)]
    public DateTime StartTime { get; init; }
    [Required]
    [DataType(DataType.DateTime)]
    public DateTime EndTime { get; init; }
}
