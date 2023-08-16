namespace Tgs.GameRecord.Pinnacle.Core.ApplicationServices;

public record PullRecordCommand(
    Uri ApiUrl,
    string AgentCode,
    string AgentKey,
    string SecretKey,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
