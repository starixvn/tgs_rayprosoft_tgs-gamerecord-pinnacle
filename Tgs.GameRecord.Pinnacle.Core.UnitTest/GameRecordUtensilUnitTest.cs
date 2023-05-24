using Google.Protobuf.WellKnownTypes;

using NSubstitute;

using Tgs.GameCommand.Common;
using Tgs.GameInterface.Pinnacle.Protos;

namespace Tgs.GameRecord.Pinnacle.Core.UnitTest;

public class GameRecordUtensilUnitTest
{
    [Fact]
    public async Task Settlement_Brand_Game_Provider_Setting_Does_Not_Exist()
    {
        var fakeUtensil = Substitute.For<IUtensil>();

        var source = new GameRecordInfo
        {
            ExternalRoundId = "50257869",
            ExternalGame = "Soccer",
            ExternalGameId = "29",
            PlayerCode = "4ucmqplxb029",
            IsClosed = true,
            IsVoid = false,
            ExternalCreatedOn = DateTimeOffset.Parse("2023-05-23T05:36:17-04:00").ToTimestamp(),
            ExternalClosedOn = DateTimeOffset.Parse("2023-05-23T07:26:20").ToTimestamp(),
            Bet = 1,
            Turnover = 1,
            Settle = 0,
            WinLoss = -1,
            AdditionalData = "[{\"wagerId\":50257869,\"eventId\":1573701098,\"eventName\":\"Brimbank Stallions-vs-Geelong Rangers SC\",\"parentEventName\":null,\"headToHead\":null,\"wagerDateFm\":\"2023-05-23 05:36:17\",\"eventDateFm\":\"2023-05-23 05:30:00\",\"settleDateFm\":\"2023-05-23 07:26:20\",\"status\":\"SETTLED\",\"homeTeam\":\"Brimbank Stallions\",\"awayTeam\":\"Geelong Rangers SC\",\"selection\":\"Brimbank Stallions\",\"handicap\":-0.25,\"odds\":0.952,\"oddsFormat\":2,\"betType\":2,\"league\":\"Australia - Cup Qualifiers\",\"leagueId\":10984,\"stake\":1.0,\"sportId\":29,\"sport\":\"Soccer\",\"currencyCode\":\"CNY\",\"inplayScore\":\"0-0\",\"inPlay\":true,\"homePitcher\":null,\"awayPitcher\":null,\"homePitcherName\":null,\"awayPitcherName\":null,\"period\":0,\"cancellationStatus\":null,\"parlaySelections\":[],\"category\":null,\"toWin\":0.95,\"toRisk\":1.0,\"product\":\"SB\",\"isResettle\":false,\"parlayMixOdds\":0.952,\"wagerType\":\"single\",\"competitors\":[],\"userCode\":\"V020100001\",\"loginId\":\"4ucmqplxb029\",\"winLoss\":-1.0,\"turnover\":1.0,\"scores\":[{\"period\":0,\"score\":\"1-3\"},{\"period\":1,\"score\":\"0-1\"}],\"result\":\"LOSE\",\"volume\":0.95,\"view\":\"D-Compact\"}]"
        };

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
            BrandCode: "PB2",
            PlayerCode: source.PlayerCode,
            Username: "test",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "PHP"
            );
        _ = fakeUtensil
            .GetPlayerAsync(
                playerCode: Arg.Is(source.PlayerCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(player);

        var gameId = 1;
        _ = fakeUtensil
            .GetGameIdAsync(
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameId);

        _ = fakeUtensil
            .GetBrandGameProviderConfigurationIdAsync(
                brandId: Arg.Is(player.BrandId),
                gameProviderId: Arg.Is(gameProviderId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<Guid?>(null));

        var gameRecordUtensil = new GameRecordUtensil(fakeUtensil);

        var actual = await gameRecordUtensil
             .SettlementConvertRecordAsync(
                 source: source)
             .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.False(actual.Success);
        Assert.Equal(gameProviderId, actual.GameProviderId);
        Assert.Equal(gameProviderCode, actual.GameProviderCode);
        Assert.Equal(source.ExternalRoundId, actual.ExternalRoundId);
        Assert.Equal(gameId, actual.GameId);
        Assert.Equal(source.ExternalGameId.ToString(), actual.ExternalGameId);
        Assert.Equal(source.PlayerCode, actual.PlayerCode);
        Assert.Equal(player.DefaultWalletId, actual.WalletId);
        Assert.Equal(player.DefaultWalletCurrencyCode, actual.CurrencyCode);
        Assert.Equal(player.BrandId, actual.BrandId);
        Assert.Equal(Guid.Empty, actual.GameProviderConfigurationId);
        Assert.True(actual.Closed);
        Assert.Equal(source.ExternalCreatedOn.ToDateTimeOffset(), actual.ExternalCreatedOn);
        Assert.Equal(source.ExternalClosedOn.ToDateTimeOffset(), actual.ExternalClosedOn);
        Assert.Equal(source.Bet.ToDecimal().GetValueOrDefault(0), actual.Bet);
        Assert.Equal(source.Bet.ToDecimal().GetValueOrDefault(0), actual.ValidBet);
        Assert.Equal(source.Bet.ToDecimal().GetValueOrDefault(0), actual.Turnover);
        Assert.Equal(source.WinLoss.ToDecimal().GetValueOrDefault(0), actual.WinLoss);
        Assert.Equal(source.Settle.ToDecimal().GetValueOrDefault(0), actual.Settle);
        Assert.Equal(source.AdditionalData, actual.AdditionalData);
    }

    [Fact]
    public async Task Settlement_Normal_Process()
    {
        var fakeUtensil = Substitute.For<IUtensil>();

        var source = new GameRecordInfo
        {
            ExternalRoundId = "50257869",
            ExternalGame = "Soccer",
            ExternalGameId = "29",
            PlayerCode = "4ucmqplxb029",
            IsClosed = true,
            IsVoid = false,
            ExternalCreatedOn = DateTimeOffset.Parse("2023-05-23T05:36:17-04:00").ToTimestamp(),
            ExternalClosedOn = DateTimeOffset.Parse("2023-05-23T07:26:20").ToTimestamp(),
            Bet = 1,
            Turnover = 1,
            Settle = 0,
            WinLoss = -1,
            AdditionalData = "[{\"wagerId\":50257869,\"eventId\":1573701098,\"eventName\":\"Brimbank Stallions-vs-Geelong Rangers SC\",\"parentEventName\":null,\"headToHead\":null,\"wagerDateFm\":\"2023-05-23 05:36:17\",\"eventDateFm\":\"2023-05-23 05:30:00\",\"settleDateFm\":\"2023-05-23 07:26:20\",\"status\":\"SETTLED\",\"homeTeam\":\"Brimbank Stallions\",\"awayTeam\":\"Geelong Rangers SC\",\"selection\":\"Brimbank Stallions\",\"handicap\":-0.25,\"odds\":0.952,\"oddsFormat\":2,\"betType\":2,\"league\":\"Australia - Cup Qualifiers\",\"leagueId\":10984,\"stake\":1.0,\"sportId\":29,\"sport\":\"Soccer\",\"currencyCode\":\"CNY\",\"inplayScore\":\"0-0\",\"inPlay\":true,\"homePitcher\":null,\"awayPitcher\":null,\"homePitcherName\":null,\"awayPitcherName\":null,\"period\":0,\"cancellationStatus\":null,\"parlaySelections\":[],\"category\":null,\"toWin\":0.95,\"toRisk\":1.0,\"product\":\"SB\",\"isResettle\":false,\"parlayMixOdds\":0.952,\"wagerType\":\"single\",\"competitors\":[],\"userCode\":\"V020100001\",\"loginId\":\"4ucmqplxb029\",\"winLoss\":-1.0,\"turnover\":1.0,\"scores\":[{\"period\":0,\"score\":\"1-3\"},{\"period\":1,\"score\":\"0-1\"}],\"result\":\"LOSE\",\"volume\":0.95,\"view\":\"D-Compact\"}]"
        };

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
            BrandCode: "PB2",
            PlayerCode: source.PlayerCode,
            Username: "test",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "PHP"
            );
        _ = fakeUtensil
            .GetPlayerAsync(
                playerCode: Arg.Is(source.PlayerCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(player);

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

        var gameRecordUtensil = new GameRecordUtensil(fakeUtensil);

        var actual = await gameRecordUtensil
             .SettlementConvertRecordAsync(
                 source: source)
             .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.True(actual.Success);
        Assert.Equal(gameProviderId, actual.GameProviderId);
        Assert.Equal(gameProviderCode, actual.GameProviderCode);
        Assert.Equal(source.ExternalRoundId, actual.ExternalRoundId);
        Assert.Equal(gameId, actual.GameId);
        Assert.Equal(source.ExternalGameId.ToString(), actual.ExternalGameId);
        Assert.Equal(source.PlayerCode, actual.PlayerCode);
        Assert.Equal(player.DefaultWalletId, actual.WalletId);
        Assert.Equal(player.DefaultWalletCurrencyCode, actual.CurrencyCode);
        Assert.Equal(player.BrandId, actual.BrandId);
        Assert.Equal(gameProviderConfigurationId, actual.GameProviderConfigurationId);
        Assert.True(actual.Closed);
        Assert.Equal(source.ExternalCreatedOn.ToDateTimeOffset(), actual.ExternalCreatedOn);
        Assert.Equal(source.ExternalClosedOn.ToDateTimeOffset(), actual.ExternalClosedOn);
        Assert.Equal(source.Bet.ToDecimal().GetValueOrDefault(0), actual.Bet);
        Assert.Equal(source.Bet.ToDecimal().GetValueOrDefault(0), actual.ValidBet);
        Assert.Equal(source.Bet.ToDecimal().GetValueOrDefault(0), actual.Turnover);
        Assert.Equal(source.WinLoss.ToDecimal().GetValueOrDefault(0), actual.WinLoss);
        Assert.Equal(source.Settle.ToDecimal().GetValueOrDefault(0), actual.Settle);
        Assert.Equal(source.AdditionalData, actual.AdditionalData);
    }

    [Fact]
    public async Task Settlement_Player_Does_Not_Exist()
    {
        var fakeUtensil = Substitute.For<IUtensil>();

        var source = new GameRecordInfo
        {
            ExternalRoundId = "50257869",
            ExternalGame = "Soccer",
            ExternalGameId = "29",
            PlayerCode = "4ucmqplxb029",
            IsClosed = true,
            IsVoid = false,
            ExternalCreatedOn = DateTimeOffset.Parse("2023-05-23T05:36:17-04:00").ToTimestamp(),
            ExternalClosedOn = DateTimeOffset.Parse("2023-05-23T07:26:20").ToTimestamp(),
            Bet = 1,
            Turnover = 1,
            Settle = 0,
            WinLoss = -1,
            AdditionalData = "[{\"wagerId\":50257869,\"eventId\":1573701098,\"eventName\":\"Brimbank Stallions-vs-Geelong Rangers SC\",\"parentEventName\":null,\"headToHead\":null,\"wagerDateFm\":\"2023-05-23 05:36:17\",\"eventDateFm\":\"2023-05-23 05:30:00\",\"settleDateFm\":\"2023-05-23 07:26:20\",\"status\":\"SETTLED\",\"homeTeam\":\"Brimbank Stallions\",\"awayTeam\":\"Geelong Rangers SC\",\"selection\":\"Brimbank Stallions\",\"handicap\":-0.25,\"odds\":0.952,\"oddsFormat\":2,\"betType\":2,\"league\":\"Australia - Cup Qualifiers\",\"leagueId\":10984,\"stake\":1.0,\"sportId\":29,\"sport\":\"Soccer\",\"currencyCode\":\"CNY\",\"inplayScore\":\"0-0\",\"inPlay\":true,\"homePitcher\":null,\"awayPitcher\":null,\"homePitcherName\":null,\"awayPitcherName\":null,\"period\":0,\"cancellationStatus\":null,\"parlaySelections\":[],\"category\":null,\"toWin\":0.95,\"toRisk\":1.0,\"product\":\"SB\",\"isResettle\":false,\"parlayMixOdds\":0.952,\"wagerType\":\"single\",\"competitors\":[],\"userCode\":\"V020100001\",\"loginId\":\"4ucmqplxb029\",\"winLoss\":-1.0,\"turnover\":1.0,\"scores\":[{\"period\":0,\"score\":\"1-3\"},{\"period\":1,\"score\":\"0-1\"}],\"result\":\"LOSE\",\"volume\":0.95,\"view\":\"D-Compact\"}]"
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeUtensil
            .GetGameProviderIdAsync(
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        var gameProviderCode = "PINNACLE";
        _ = fakeUtensil
            .GetGameProviderCode()
            .Returns(gameProviderCode);

        _ = fakeUtensil
            .GetPlayerAsync(
                playerCode: Arg.Is(source.PlayerCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<PlayerInfo?>(null));

        var gameId = 1;
        _ = fakeUtensil
            .GetGameIdAsync(
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameId);

        var gameRecordUtensil = new GameRecordUtensil(fakeUtensil);

        var actual = await gameRecordUtensil
             .SettlementConvertRecordAsync(
                 source: source)
             .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.False(actual.Success);
        Assert.Equal(gameProviderId, actual.GameProviderId);
        Assert.Equal(gameProviderCode, actual.GameProviderCode);
        Assert.Equal(source.ExternalRoundId, actual.ExternalRoundId);
        Assert.Equal(gameId, actual.GameId);
        Assert.Equal(source.ExternalGameId.ToString(), actual.ExternalGameId);
        Assert.Equal(string.Empty, actual.PlayerCode);
        Assert.Equal(Guid.Empty, actual.WalletId);
        Assert.Equal(string.Empty, actual.CurrencyCode);
        Assert.Equal(Guid.Empty, actual.BrandId);
        Assert.Equal(Guid.Empty, actual.GameProviderConfigurationId);
        Assert.True(actual.Closed);
        Assert.Equal(source.ExternalCreatedOn.ToDateTimeOffset(), actual.ExternalCreatedOn);
        Assert.Equal(source.ExternalClosedOn.ToDateTimeOffset(), actual.ExternalClosedOn);
        Assert.Equal(source.Bet.ToDecimal().GetValueOrDefault(0), actual.Bet);
        Assert.Equal(source.Bet.ToDecimal().GetValueOrDefault(0), actual.ValidBet);
        Assert.Equal(source.Bet.ToDecimal().GetValueOrDefault(0), actual.Turnover);
        Assert.Equal(source.WinLoss.ToDecimal().GetValueOrDefault(0), actual.WinLoss);
        Assert.Equal(source.Settle.ToDecimal().GetValueOrDefault(0), actual.Settle);
        Assert.Equal(source.AdditionalData, actual.AdditionalData);
    }
}