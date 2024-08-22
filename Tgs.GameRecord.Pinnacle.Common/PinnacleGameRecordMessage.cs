using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRecord.Pinnacle.Common;

public record PinnacleGameRecordMessage
{
	[Required]
	[DataType(DataType.Url)]
	[DefaultValue(typeof(Uri), "https://paapistg.oreo88.com")]
	public Uri ApiUrl { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "V0201")]
	public string AgentCode { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "8d0e9ac0-99d8-48c7-8307-bc328c4181f6")]
	public string AgentKey { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "zenDIdY7Knuzmeen")]
	public string SecretKey { get; init; } = default!;
	[Required]
	[DataType(DataType.DateTime)]
	[DefaultValue(typeof(DateTimeOffset), "2023-05-23T19:25:00+08:00")]
	public DateTimeOffset StartTime { get; init; } = default!;
	[Required]
	[DataType(DataType.DateTime)]
	[DefaultValue(typeof(DateTimeOffset), "2023-05-23T19:30:00+08:00")]
	public DateTimeOffset EndTime { get; init; } = default!;
}
