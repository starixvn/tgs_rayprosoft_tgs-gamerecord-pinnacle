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

    public ValueTask ExecuteAsync(HandleRecordCommand command, CancellationToken cancellationToken = default)
    {
        var externalCreatedOn = command.Info.ExternalCreatedOn.ToDateTimeOffset();
        var externalClosedOn = DateTimeOffset.UnixEpoch;
        var bet = command.Info.Bet.ToDecimal().GetValueOrDefault(0);
        var validBet = (!command.Info.IsClosed) ? command.Info.Bet.ToDecimal().GetValueOrDefault(0) : command.Info.Turnover.ToDecimal().GetValueOrDefault(0);
        var settle = 0.0m;
        var commission = 0.0m;
        var turnover = 0.0m;
        var winLoss = 0.0m;
        var wagerState = WagerStates.Running;
        if (command.Info.IsClosed)
        {
            externalClosedOn = command.Info.ExternalClosedOn.ToDateTimeOffset();
            wagerState = (command.Info.IsVoid) ? WagerStates.Void : command.Info.WinLoss <= 0 ? WagerStates.Lose : WagerStates.Win;
            settle = command.Info.Settle.ToDecimal().GetValueOrDefault(0);
            turnover = command.Info.Turnover.ToDecimal().GetValueOrDefault(0);
            winLoss = command.Info.WinLoss.ToDecimal().GetValueOrDefault(0);
        }

        return _client
              .MessageAsync(
              gameProviderCode: _gameRecordInfo.GameProviderCode,
              playerCode: command.Info.PlayerCode,
              wagerState: wagerState,
              wagerType: WagerTypes.Normal,
              freeGameState: FreeGameStates.None,
              gambleState: GambleStates.None,
              cashoutState: CashoutStates.None,
              resettleState: ResettleStates.None,
              externalRoundId: command.Info.ExternalRoundId,
              externalWagerId: command.Info.ExternalRoundId,
              externalGameId: _gameRecordInfo.GameExternalId,
              externalCreatedOn: externalCreatedOn,
              externalClosedOn: externalClosedOn,
              externalPayoffOn: externalClosedOn,
              externalResettleOn: DateTimeOffset.UnixEpoch,
              bet: bet,
              validBet: validBet,
              settle: settle,
              commission: commission,
              turnover: turnover,
              winLoss: winLoss,
              additionalData: command.Info.AdditionalData,
              cancellationToken: cancellationToken);
    }
}
