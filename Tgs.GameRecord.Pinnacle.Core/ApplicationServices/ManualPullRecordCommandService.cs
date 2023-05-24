using Microsoft.Extensions.Logging;

using Tgs.GameInterface.Pinnacle.Client;
using Tgs.GameInterface.Pinnacle.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Pinnacle.Core.ApplicationServices;

internal class ManualPullRecordCommandService : ICommandService<ManualPullRecordCommand>
{
    private readonly IGameInterfacePinnacleClient _client;
    private readonly IGameRecordUtensil _gameRecordUtensil;
    private readonly ILogger<ManualPullRecordCommandService> _logger;
    private readonly ICommandService<ManualHandleRecordCommand> _service;

    public ManualPullRecordCommandService(
        ILogger<ManualPullRecordCommandService> logger,
        IGameInterfacePinnacleClient client,
        IGameRecordUtensil gameRecordUtensil,
        ICommandService<ManualHandleRecordCommand> service)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _gameRecordUtensil = gameRecordUtensil ?? throw new ArgumentNullException(nameof(gameRecordUtensil));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask ExecuteAsync(ManualPullRecordCommand command, CancellationToken cancellationToken = default)
    {
        var gameRecords = await _client
            .GameRecordAsync(
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

        _logger.LogDebug("LogOn:{logOn} Message: GameRecord. GameRecords:{gameRecords}", DateTime.Now.ToString("o"), gameRecords);

        if (!gameRecords.Any())
        {
            return;
        }

        foreach (var gameRecord in gameRecords)
        {
            _logger.LogDebug("LogOn:{logOn} Message: GameRecord. GameRecords:{gameRecord}", DateTime.Now.ToString("o"), gameRecord);

            try
            {
                var record = await _gameRecordUtensil
                    .SettlementConvertRecordAsync(
                        gameRecord,
                        cancellationToken)
                    .ConfigureAwait(false);

                _logger.LogDebug("LogOn:{logOn} Message: GameRecord. Record:{record}", DateTime.Now.ToString("o"), record);

                await _service
                    .ExecuteAsync(
                        command: new(record),
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LogOn:{logOn} Message: GameRecord Exception. GameRecord:{gameRecord}", DateTime.Now.ToString("o"), gameRecord);
            }
        }
    }
}