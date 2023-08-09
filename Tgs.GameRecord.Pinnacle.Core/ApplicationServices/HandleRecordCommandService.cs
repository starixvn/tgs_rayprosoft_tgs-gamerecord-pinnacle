using Tgs.GameRounds.Work.Client;
using Tgs.GameRounds.Work.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Pinnacle.Core.ApplicationServices;

internal class HandleRecordCommandService : ICommandService<HandleRecordCommand>
{
    private readonly IGameRoundsWorkClient _client;
    private readonly IUtensil _utensil;

    public HandleRecordCommandService(
        IUtensil utensil,
        IGameRoundsWorkClient client)
    {
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask ExecuteAsync(HandleRecordCommand command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _utensil
            .GetGameProviderIdAsync(
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderCode = _utensil
            .GetGameProviderCode();

        var player = await _utensil
            .GetPlayerAsync(
            playerCode: command.Info.PlayerCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var externalGameId = _utensil.GetExternalGameId();

        var gameId = await _utensil
            .GetGameIdAsync(
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderConfigurationId = await _utensil
            .GetBrandGameProviderConfigurationIdAsync(
            brandId: player.BrandId,
            gameProviderId: gameProviderId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var externalCreatedOn = command.Info.ExternalCreatedOn.ToDateTimeOffset();
        var externalClosedOn = (!command.Info.IsClosed) ? (DateTimeOffset?)null : command.Info.ExternalClosedOn.ToDateTimeOffset();
        var bet = command.Info.Bet.ToDecimal().GetValueOrDefault(0);
        var validBet = (!command.Info.IsClosed) ? command.Info.Bet.ToDecimal().GetValueOrDefault(0) : command.Info.Turnover.ToDecimal().GetValueOrDefault(0);
        var settle = 0.0m;
        var commission = 0.0m;
        var turnover = 0.0m;
        var winLoss = 0.0m;
        var status = RoundStatus.Open;
        var clientStatus = RoundClientStatus.Open;
        if (command.Info.IsClosed)
        {
            status = RoundStatus.Closed;
            clientStatus = (command.Info.IsVoid) ? RoundClientStatus.Void : command.Info.WinLoss <= 0 ? RoundClientStatus.Loss : RoundClientStatus.Win;
            settle = command.Info.Settle.ToDecimal().GetValueOrDefault(0);
            turnover = command.Info.Turnover.ToDecimal().GetValueOrDefault(0);
            winLoss = command.Info.WinLoss.ToDecimal().GetValueOrDefault(0);
        }

        await _client
        .RoundBidding(
            gameProviderId: gameProviderId,
            gameProviderCode: gameProviderCode,
            externalRoundId: command.Info.ExternalRoundId,
            gameId: gameId,
            externalGameId: externalGameId,
            playerCode: player.PlayerCode,
            walletId: player.DefaultWalletId,
            currencyCode: player.DefaultWalletCurrencyCode,
            brandId: player.BrandId,
            gameProviderConfigurationId: gameProviderConfigurationId,
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
