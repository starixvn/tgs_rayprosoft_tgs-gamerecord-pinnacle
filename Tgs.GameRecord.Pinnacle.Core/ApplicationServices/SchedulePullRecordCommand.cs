namespace Tgs.GameRecord.Pinnacle.Core.ApplicationServices;

public record SchedulePullRecordCommand(
    Uri ApiUrl,
    string AgentCode,
    string AgentKey,
    string SecretKey,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
