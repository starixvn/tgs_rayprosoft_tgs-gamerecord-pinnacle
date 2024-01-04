using Microsoft.Extensions.Logging;

using Tgs.GameInterface.Pinnacle.Client;
using Tgs.GameInterface.Pinnacle.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Pinnacle.Core.ApplicationServices;

internal class PullRecordCommandService(
    ILogger<PullRecordCommandService> logger,
    IGameInterfacePinnacleClient client,
    ICommandService<HandleRecordCommand> service) : ICommandService<PullRecordCommand>
{
    public async ValueTask ExecuteAsync(PullRecordCommand command, CancellationToken cancellationToken = default)
    {
        var gameRecords = await client.GameRecordAsync(
            apiUrl: command.ApiUrl,
            agentCode: command.AgentCode,
            agentKey: command.AgentKey,
            secretKey: command.SecretKey,
            startTime: command.StartTime,
            endTime: command.EndTime,
            cancellationToken: cancellationToken)
            .ToArrayAsync(
            cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? Enumerable.Empty<GameRecordInfo>();

        logger.LogDebug("LogOn:{logOn} Message: GameRecord. GameRecords:{gameRecords}", DateTime.Now.ToString("o"), gameRecords);

        if (!gameRecords.Any())
        {
            return;
        }

        foreach (var gameRecord in gameRecords)
        {
            logger.LogDebug("LogOn:{logOn} Message: GameRecord. GameRecords:{gameRecord}", DateTime.Now.ToString("o"), gameRecord);

            try
            {
                await service
                    .ExecuteAsync(
                        command: new(gameRecord),
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "LogOn:{logOn} Message: GameRecord Exception. GameRecord:{gameRecord}", DateTime.Now.ToString("o"), gameRecord);
            }
        }
    }
}
