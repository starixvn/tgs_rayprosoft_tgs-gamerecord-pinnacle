namespace Tgs.GameRecord.Pinnacle.Core.JobApplicationServices;

public record JobExecuteCommand(
    string GameProviderCode,
    string Name,
    Uri ApiUrl,
    string AgentCode,
    string AgentKey,
    string SecretKey);
