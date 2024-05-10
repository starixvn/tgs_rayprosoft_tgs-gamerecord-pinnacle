using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameInterface.Pinnacle.Protos;
using Tgs.GameRecord.Pinnacle.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Pinnacle.Core.UnitTest.ApplicationServices;

public class HandleRecordCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process_Settlement_Loss()
	{
		var faceLogger = NullLogger<HandleRecordCommandService>.Instance;
		var gameRecordInfo = Options.Create(new GameRecordInfoOptions
		{
			GameProviderCode = "PINNACLE",
			GameProviderConfiguration = "PINNACLEConfiguration",
			GameExternalId = "sports_book",
			HangfirePrefix = "gamerecord-pinnacle-hangfire:",
			HangfireLocking = "gamerecord-pinnacle-locking:",
			IntervalMinutes = 3,
			IntervalMinutesMax = 15,
			IntervalMinutesMinimum = 1
		});
		var fakeMessageSender = Substitute.For<IMessageSender>();

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

		var service = new HandleRecordCommandService(
			faceLogger,
			gameRecordInfo,
			fakeMessageSender);

		await service.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest),
			data: Arg.Is(new Handle.Common.Protos.GameRecordHandleRequest
			{
				GameProviderCode = gameRecordInfo.Value.GameProviderCode,
				PlayerCode = command.Info.PlayerCode,
				ExternalRoundId = command.Info.ExternalRoundId,
				ExternalBetId = command.Info.ExternalRoundId,
				ExternalGameId = gameRecordInfo.Value.GameExternalId,
				ExternalCreatedOn = command.Info.ExternalCreatedOn,
				ExternalClosedOn = command.Info.ExternalClosedOn,
				ExternalPayoffOn = command.Info.ExternalClosedOn,
				ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
				BetState = Handle.Common.Protos.BetState.Loss,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 1.0m,
				ValidBet = 1.0m,
				Settle = 0.0M,
				Commission = 0.0M,
				Turnover = 1.0m,
				WinLoss = -1.0m,
				Jackpot = 0.0M,
				AdditionalData = command.Info.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Normal_Process_Settlement_Void()
	{
		var faceLogger = NullLogger<HandleRecordCommandService>.Instance;
		var gameRecordInfo = Options.Create(new GameRecordInfoOptions
		{
			GameProviderCode = "PINNACLE",
			GameProviderConfiguration = "PINNACLEConfiguration",
			GameExternalId = "sports_book",
			HangfirePrefix = "gamerecord-pinnacle-hangfire:",
			HangfireLocking = "gamerecord-pinnacle-locking:",
			IntervalMinutes = 3,
			IntervalMinutesMax = 15,
			IntervalMinutesMinimum = 1
		});
		var fakeMessageSender = Substitute.For<IMessageSender>();

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

		var service = new HandleRecordCommandService(
			faceLogger,
			gameRecordInfo,
			fakeMessageSender);

		await service.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest),
			data: Arg.Is(new Handle.Common.Protos.GameRecordHandleRequest
			{
				GameProviderCode = gameRecordInfo.Value.GameProviderCode,
				PlayerCode = command.Info.PlayerCode,
				ExternalRoundId = command.Info.ExternalRoundId,
				ExternalBetId = command.Info.ExternalRoundId,
				ExternalGameId = gameRecordInfo.Value.GameExternalId,
				ExternalCreatedOn = command.Info.ExternalCreatedOn,
				ExternalClosedOn = command.Info.ExternalClosedOn,
				ExternalPayoffOn = command.Info.ExternalClosedOn,
				ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
				BetState = Handle.Common.Protos.BetState.Void,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 1.0m,
				ValidBet = 0.0M,
				Settle = 0.0M,
				Commission = 0.0M,
				Turnover = 0.0M,
				WinLoss = 0.0m,
				Jackpot = 0.0M,
				AdditionalData = command.Info.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Normal_Process_Settlement_Win()
	{
		var faceLogger = NullLogger<HandleRecordCommandService>.Instance;
		var gameRecordInfo = Options.Create(new GameRecordInfoOptions
		{
			GameProviderCode = "PINNACLE",
			GameProviderConfiguration = "PINNACLEConfiguration",
			GameExternalId = "sports_book",
			HangfirePrefix = "gamerecord-pinnacle-hangfire:",
			HangfireLocking = "gamerecord-pinnacle-locking:",
			IntervalMinutes = 3,
			IntervalMinutesMax = 15,
			IntervalMinutesMinimum = 1
		});
		var fakeMessageSender = Substitute.For<IMessageSender>();

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

		var service = new HandleRecordCommandService(
			faceLogger,
			gameRecordInfo,
			fakeMessageSender);

		await service.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest),
			data: Arg.Is(new Handle.Common.Protos.GameRecordHandleRequest
			{
				GameProviderCode = gameRecordInfo.Value.GameProviderCode,
				PlayerCode = command.Info.PlayerCode,
				ExternalRoundId = command.Info.ExternalRoundId,
				ExternalBetId = command.Info.ExternalRoundId,
				ExternalGameId = gameRecordInfo.Value.GameExternalId,
				ExternalCreatedOn = command.Info.ExternalCreatedOn,
				ExternalClosedOn = command.Info.ExternalClosedOn,
				ExternalPayoffOn = command.Info.ExternalClosedOn,
				ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
				BetState = Handle.Common.Protos.BetState.Win,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 1.0m,
				ValidBet = 1.0m,
				Settle = 1.91m,
				Commission = 0.0M,
				Turnover = 1.0m,
				WinLoss = 0.91m,
				Jackpot = 0.0M,
				AdditionalData = command.Info.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Normal_Process_Unsettlement()
	{
		var faceLogger = NullLogger<HandleRecordCommandService>.Instance;
		var gameRecordInfo = Options.Create(new GameRecordInfoOptions
		{
			GameProviderCode = "PINNACLE",
			GameProviderConfiguration = "PINNACLEConfiguration",
			GameExternalId = "sports_book",
			HangfirePrefix = "gamerecord-pinnacle-hangfire:",
			HangfireLocking = "gamerecord-pinnacle-locking:",
			IntervalMinutes = 3,
			IntervalMinutesMax = 15,
			IntervalMinutesMinimum = 1
		});
		var fakeMessageSender = Substitute.For<IMessageSender>();

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

		var service = new HandleRecordCommandService(
			faceLogger,
			gameRecordInfo,
			fakeMessageSender);

		await service.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest),
			data: Arg.Is(new Handle.Common.Protos.GameRecordHandleRequest
			{
				GameProviderCode = gameRecordInfo.Value.GameProviderCode,
				PlayerCode = command.Info.PlayerCode,
				ExternalRoundId = command.Info.ExternalRoundId,
				ExternalBetId = command.Info.ExternalRoundId,
				ExternalGameId = gameRecordInfo.Value.GameExternalId,
				ExternalCreatedOn = command.Info.ExternalCreatedOn,
				ExternalClosedOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
				ExternalPayoffOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
				ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
				BetState = Handle.Common.Protos.BetState.Running,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 1.0m,
				ValidBet = 1.0m,
				Settle = 0.0m,
				Commission = 0.0M,
				Turnover = 0.0m,
				WinLoss = 0.0m,
				Jackpot = 0.0M,
				AdditionalData = command.Info.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
