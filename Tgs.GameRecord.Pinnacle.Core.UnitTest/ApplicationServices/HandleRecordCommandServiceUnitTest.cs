using Google.Protobuf.WellKnownTypes;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.Pinnacle.Protos;
using Tgs.GameRecord.Pinnacle.Core.ApplicationServices;
using Tgs.GameRounds.Work.Client;
using Tgs.GameRounds.Work.Common.Protos;

namespace Tgs.GameRecord.Pinnacle.Core.UnitTest.ApplicationServices;

public class HandleRecordCommandServiceUnitTest
{
    [Fact]
    public async void Normal_Process_Settlement_Loss()
    {
        var fakeUtensil = Substitute.For<IUtensil>();
        var fakeClient = Substitute.For<IGameRoundsWorkClient>();

        var command = new HandleRecordCommand(
            Info: new GameRecordInfo
            {
                ExternalRoundId = "50260945",
                ExternalGame = "Soccer",
                ExternalGameId = "29",
                PlayerCode = "4ucmqplxb029",
                IsClosed = true,
                IsVoid = false,
                ExternalCreatedOn = DateTimeOffset.Parse("2023-05-23T05:36:17-04:00").ToTimestamp(),
                ExternalClosedOn = DateTimeOffset.Parse("2023-05-23T07:26:20-04:00").ToTimestamp(),
                Bet = 1,
                Settle = 0,
                Turnover = 1,
                WinLoss = -1m,
                AdditionalData = "{\"wagerId\": \"50260945\"}"
            });

        var gameProviderId = Guid.NewGuid();
        _ = fakeUtensil
            .GetGameProviderIdAsync(
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        var gameProviderCode = "PINNACLE";
        _ = fakeUtensil
            .GetGameProviderCode()
            .Returns(gameProviderCode);

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "4ucmqplxb029",
            Username: "4ucmqplxb029",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY"
            );
        _ = fakeUtensil
            .GetPlayerAsync(
            playerCode: Arg.Is(command.Info.PlayerCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(player);

        var externalGameId = "sports_book";
        _ = fakeUtensil
            .GetExternalGameId()
            .Returns(externalGameId);

        var gameId = 1;
        _ = fakeUtensil
            .GetGameIdAsync(
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameId);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeUtensil
            .GetBrandGameProviderConfigurationIdAsync(
            brandId: Arg.Is(player.BrandId),
            gameProviderId: Arg.Is(gameProviderId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var service = new HandleRecordCommandService(
            fakeUtensil,
            fakeClient);

        await service
            .ExecuteAsync(
                command)
            .ConfigureAwait(false);

        await fakeClient
            .Received(1)
            .RoundBidding(
                gameProviderId: Arg.Is(gameProviderId),
                gameProviderCode: Arg.Is(gameProviderCode),
                externalRoundId: Arg.Is(command.Info.ExternalRoundId),
                gameId: Arg.Is(gameId),
                externalGameId: Arg.Is(externalGameId),
                playerCode: Arg.Is(player.PlayerCode),
                walletId: Arg.Is(player.DefaultWalletId),
                currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
                brandId: Arg.Is(player.BrandId),
                gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
                status: Arg.Is(RoundStatus.Closed),
                clientStatus: Arg.Is(RoundClientStatus.Loss),
                externalCreatedOn: Arg.Is(command.Info.ExternalCreatedOn.ToDateTimeOffset()),
                externalClosedOn: Arg.Is(command.Info.ExternalClosedOn.ToDateTimeOffset()),
                bet: Arg.Is(command.Info.Bet.ToDecimal().GetValueOrDefault(0)),
                validBet: Arg.Is(command.Info.Turnover.ToDecimal().GetValueOrDefault(0)),
                settle: Arg.Is(command.Info.Settle.ToDecimal().GetValueOrDefault(0)),
                commission: Arg.Is(0.0m),
                turnover: Arg.Is(command.Info.Turnover.ToDecimal().GetValueOrDefault(0)),
                playerWinLoss: Arg.Is(command.Info.WinLoss.ToDecimal().GetValueOrDefault(0)),
                additionalData: Arg.Is(command.Info.AdditionalData),
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }

    [Fact]
    public async void Normal_Process_Settlement_Void()
    {
        var fakeUtensil = Substitute.For<IUtensil>();
        var fakeClient = Substitute.For<IGameRoundsWorkClient>();

        var command = new HandleRecordCommand(
            Info: new GameRecordInfo
            {
                ExternalRoundId = "50260945",
                ExternalGame = "Soccer",
                ExternalGameId = "29",
                PlayerCode = "4ucmqplxb029",
                IsClosed = true,
                IsVoid = true,
                ExternalCreatedOn = DateTimeOffset.Parse("2023-05-23T05:33:17-04:00").ToTimestamp(),
                ExternalClosedOn = DateTimeOffset.Parse("2023-05-23T07:26:20-04:00").ToTimestamp(),
                Bet = 1,
                Settle = 0m,
                Turnover = 0m,
                WinLoss = 0m,
                AdditionalData = "{\"wagerId\": \"50260945\"}"
            });

        var gameProviderId = Guid.NewGuid();
        _ = fakeUtensil
            .GetGameProviderIdAsync(
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        var gameProviderCode = "PINNACLE";
        _ = fakeUtensil
            .GetGameProviderCode()
            .Returns(gameProviderCode);

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "4ucmqplxb029",
            Username: "4ucmqplxb029",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY"
            );
        _ = fakeUtensil
            .GetPlayerAsync(
            playerCode: Arg.Is(command.Info.PlayerCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(player);

        var externalGameId = "sports_book";
        _ = fakeUtensil
            .GetExternalGameId()
            .Returns(externalGameId);

        var gameId = 1;
        _ = fakeUtensil
            .GetGameIdAsync(
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameId);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeUtensil
            .GetBrandGameProviderConfigurationIdAsync(
            brandId: Arg.Is(player.BrandId),
            gameProviderId: Arg.Is(gameProviderId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var service = new HandleRecordCommandService(
            fakeUtensil,
            fakeClient);

        await service
            .ExecuteAsync(
                command)
            .ConfigureAwait(false);

        await fakeClient
            .Received(1)
            .RoundBidding(
                gameProviderId: Arg.Is(gameProviderId),
                gameProviderCode: Arg.Is(gameProviderCode),
                externalRoundId: Arg.Is(command.Info.ExternalRoundId),
                gameId: Arg.Is(gameId),
                externalGameId: Arg.Is(externalGameId),
                playerCode: Arg.Is(player.PlayerCode),
                walletId: Arg.Is(player.DefaultWalletId),
                currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
                brandId: Arg.Is(player.BrandId),
                gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
                status: Arg.Is(RoundStatus.Closed),
                clientStatus: Arg.Is(RoundClientStatus.Void),
                externalCreatedOn: Arg.Is(command.Info.ExternalCreatedOn.ToDateTimeOffset()),
                externalClosedOn: Arg.Is(command.Info.ExternalClosedOn.ToDateTimeOffset()),
                bet: Arg.Is(command.Info.Bet.ToDecimal().GetValueOrDefault(0)),
                validBet: Arg.Is(command.Info.Turnover.ToDecimal().GetValueOrDefault(0)),
                settle: Arg.Is(command.Info.Settle.ToDecimal().GetValueOrDefault(0)),
                commission: Arg.Is(0.0m),
                turnover: Arg.Is(command.Info.Turnover.ToDecimal().GetValueOrDefault(0)),
                playerWinLoss: Arg.Is(command.Info.WinLoss.ToDecimal().GetValueOrDefault(0)),
                additionalData: Arg.Is(command.Info.AdditionalData),
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }

    [Fact]
    public async void Normal_Process_Settlement_Win()
    {
        var fakeUtensil = Substitute.For<IUtensil>();
        var fakeClient = Substitute.For<IGameRoundsWorkClient>();

        var command = new HandleRecordCommand(
            Info: new GameRecordInfo
            {
                ExternalRoundId = "50260945",
                ExternalGame = "Soccer",
                ExternalGameId = "29",
                PlayerCode = "4ucmqplxb029",
                IsClosed = true,
                IsVoid = false,
                ExternalCreatedOn = DateTimeOffset.Parse("2023-05-23T05:33:17-04:00").ToTimestamp(),
                ExternalClosedOn = DateTimeOffset.Parse("2023-05-23T07:26:20-04:00").ToTimestamp(),
                Bet = 1,
                Settle = 1.91m,
                Turnover = 1,
                WinLoss = 0.91m,
                AdditionalData = "{\"wagerId\": \"50260945\"}"
            });

        var gameProviderId = Guid.NewGuid();
        _ = fakeUtensil
            .GetGameProviderIdAsync(
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        var gameProviderCode = "PINNACLE";
        _ = fakeUtensil
            .GetGameProviderCode()
            .Returns(gameProviderCode);

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "4ucmqplxb029",
            Username: "4ucmqplxb029",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY"
            );
        _ = fakeUtensil
            .GetPlayerAsync(
            playerCode: Arg.Is(command.Info.PlayerCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(player);

        var externalGameId = "sports_book";
        _ = fakeUtensil
            .GetExternalGameId()
            .Returns(externalGameId);

        var gameId = 1;
        _ = fakeUtensil
            .GetGameIdAsync(
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameId);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeUtensil
            .GetBrandGameProviderConfigurationIdAsync(
            brandId: Arg.Is(player.BrandId),
            gameProviderId: Arg.Is(gameProviderId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var service = new HandleRecordCommandService(
            fakeUtensil,
            fakeClient);

        await service
            .ExecuteAsync(
                command)
            .ConfigureAwait(false);

        await fakeClient
            .Received(1)
            .RoundBidding(
                gameProviderId: Arg.Is(gameProviderId),
                gameProviderCode: Arg.Is(gameProviderCode),
                externalRoundId: Arg.Is(command.Info.ExternalRoundId),
                gameId: Arg.Is(gameId),
                externalGameId: Arg.Is(externalGameId),
                playerCode: Arg.Is(player.PlayerCode),
                walletId: Arg.Is(player.DefaultWalletId),
                currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
                brandId: Arg.Is(player.BrandId),
                gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
                status: Arg.Is(RoundStatus.Closed),
                clientStatus: Arg.Is(RoundClientStatus.Win),
                externalCreatedOn: Arg.Is(command.Info.ExternalCreatedOn.ToDateTimeOffset()),
                externalClosedOn: Arg.Is(command.Info.ExternalClosedOn.ToDateTimeOffset()),
                bet: Arg.Is(command.Info.Bet.ToDecimal().GetValueOrDefault(0)),
                validBet: Arg.Is(command.Info.Turnover.ToDecimal().GetValueOrDefault(0)),
                settle: Arg.Is(command.Info.Settle.ToDecimal().GetValueOrDefault(0)),
                commission: Arg.Is(0.0m),
                turnover: Arg.Is(command.Info.Turnover.ToDecimal().GetValueOrDefault(0)),
                playerWinLoss: Arg.Is(command.Info.WinLoss.ToDecimal().GetValueOrDefault(0)),
                additionalData: Arg.Is(command.Info.AdditionalData),
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }

    [Fact]
    public async void Normal_Process_Unsettlement()
    {
        var fakeUtensil = Substitute.For<IUtensil>();
        var fakeClient = Substitute.For<IGameRoundsWorkClient>();

        var command = new HandleRecordCommand(
            Info: new GameRecordInfo
            {
                ExternalRoundId = "50257869",
                ExternalGame = "Soccer",
                ExternalGameId = "29",
                PlayerCode = "4ucmqplxb029",
                IsClosed = false,
                IsVoid = false,
                ExternalCreatedOn = DateTimeOffset.Parse("2023-05-23T05:36:17-04:00").ToTimestamp(),
                ExternalClosedOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
                Bet = 1,
                Settle = 0m,
                Turnover = 0m,
                WinLoss = 0m,
                AdditionalData = "{\"wagerId\": \"50257869\"}"
            });

        var gameProviderId = Guid.NewGuid();
        _ = fakeUtensil
            .GetGameProviderIdAsync(
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        var gameProviderCode = "PINNACLE";
        _ = fakeUtensil
            .GetGameProviderCode()
            .Returns(gameProviderCode);

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "4ucmqplxb029",
            Username: "4ucmqplxb029",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY"
            );
        _ = fakeUtensil
            .GetPlayerAsync(
            playerCode: Arg.Is(command.Info.PlayerCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(player);

        var externalGameId = "sports_book";
        _ = fakeUtensil
            .GetExternalGameId()
            .Returns(externalGameId);

        var gameId = 1;
        _ = fakeUtensil
            .GetGameIdAsync(
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameId);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeUtensil
            .GetBrandGameProviderConfigurationIdAsync(
            brandId: Arg.Is(player.BrandId),
            gameProviderId: Arg.Is(gameProviderId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var service = new HandleRecordCommandService(
            fakeUtensil,
            fakeClient);

        await service
            .ExecuteAsync(
                command)
            .ConfigureAwait(false);

        await fakeClient
            .Received(1)
            .RoundBidding(
                gameProviderId: Arg.Is(gameProviderId),
                gameProviderCode: Arg.Is(gameProviderCode),
                externalRoundId: Arg.Is(command.Info.ExternalRoundId),
                gameId: Arg.Is(gameId),
                externalGameId: Arg.Is(externalGameId),
                playerCode: Arg.Is(player.PlayerCode),
                walletId: Arg.Is(player.DefaultWalletId),
                currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
                brandId: Arg.Is(player.BrandId),
                gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId),
                status: Arg.Is(RoundStatus.Open),
                clientStatus: Arg.Is(RoundClientStatus.Open),
                externalCreatedOn: Arg.Is(command.Info.ExternalCreatedOn.ToDateTimeOffset()),
                externalClosedOn: Arg.Is((DateTimeOffset?)null),
                bet: Arg.Is(command.Info.Bet.ToDecimal().GetValueOrDefault(0)),
                validBet: Arg.Is(command.Info.Bet.ToDecimal().GetValueOrDefault(0)),
                settle: Arg.Is(command.Info.Settle.ToDecimal().GetValueOrDefault(0)),
                commission: Arg.Is(0.0m),
                turnover: Arg.Is(command.Info.Turnover.ToDecimal().GetValueOrDefault(0)),
                playerWinLoss: Arg.Is(command.Info.WinLoss.ToDecimal().GetValueOrDefault(0)),
                additionalData: Arg.Is(command.Info.AdditionalData),
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }
}
