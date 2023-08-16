using Microsoft.Extensions.Options;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Pinnacle.Core.ApplicationServices;

internal class HandleRecordCommandService : ICommandService<HandleRecordCommand>
{
    private readonly IGameRecordHandleClient _client;
    private readonly GameRecordInfoOptions _gameRecordInfo;

    public HandleRecordCommandService(
        IOptions<GameRecordInfoOptions> options,
        IGameRecordHandleClient client)
    {
        _gameRecordInfo = options.Value;
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask ExecuteAsync(HandleRecordCommand command, CancellationToken cancellationToken = default)
    {
        var externalCreatedOn = command.Info.ExternalCreatedOn.ToDateTimeOffset();
        var externalClosedOn = (!command.Info.IsClosed) ? (DateTimeOffset?)null : command.Info.ExternalClosedOn.ToDateTimeOffset();
        var bet = command.Info.Bet.ToDecimal().GetValueOrDefault(0);
        var validBet = (!command.Info.IsClosed) ? command.Info.Bet.ToDecimal().GetValueOrDefault(0) : command.Info.Turnover.ToDecimal().GetValueOrDefault(0);
        var settle = 0.0m;
        var commission = 0.0m;
        var turnover = 0.0m;
        var winLoss = 0.0m;
        var status = GameRecordStatus.Open;
        var clientStatus = GameRecordClientStatus.Open;
        if (command.Info.IsClosed)
        {
            status = GameRecordStatus.Closed;
            clientStatus = (command.Info.IsVoid) ? GameRecordClientStatus.Void : command.Info.WinLoss <= 0 ? GameRecordClientStatus.Loss : GameRecordClientStatus.Win;
            settle = command.Info.Settle.ToDecimal().GetValueOrDefault(0);
            turnover = command.Info.Turnover.ToDecimal().GetValueOrDefault(0);
            winLoss = command.Info.WinLoss.ToDecimal().GetValueOrDefault(0);
        }

        await _client
            .MessageAsync(
            gameProviderCode: _gameRecordInfo.GameProviderCode,
            externalRoundId: command.Info.ExternalRoundId,
            externalGameId: _gameRecordInfo.GameExternalId,
            playerCode: command.Info.PlayerCode,
            status: status,
            clientStatus: clientStatus,
            externalCreatedOn: externalCreatedOn,
            externalClosedOn: externalClosedOn,
            bet: bet,
            validBet: validBet,
            settle: settle,
            commission: commission,
            turnover: turnover,
            playerWinLoss: winLoss,
            additionalData: command.Info.AdditionalData,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
