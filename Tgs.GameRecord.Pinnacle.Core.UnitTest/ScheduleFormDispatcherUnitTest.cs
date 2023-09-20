using System.Text.Json;

using Google.Protobuf.WellKnownTypes;

using Microsoft.Extensions.Logging.Abstractions;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using Tgs.GameRecord.Pinnacle.Core.ApplicationServices;
using Tgs.GameRounds.Schedule.Client;
using Tgs.GameRounds.Schedule.Common.Protos;
using Tgs.GameRounds.Schedule.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Pinnacle.Core.UnitTest;

public class ScheduleFormDispatcherUnitTest
{
    [Fact]
    public async Task Exception_Occurs()
    {
        var fakeLogger = NullLogger<ScheduleFormDispatcher>.Instance;
        var fakeClient = Substitute.For<IGameRoundsScheduleClient>();
        var fakeScheduleFormLock = Substitute.For<IScheduleFormLock>();
        var fakeSchedulePullRecord = Substitute.For<ICommandService<PullRecordCommand>>();

        var gameProviderCode = "PINNACLE";
        var id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var parameter = new PinnacleParameter
        {
            ApiUrl = "http://localhost/",
            AgentCode = "agentCode",
            AgentKey = "agentKey",
            SecretKey = "secretKey",
            StartTime = DateTimeOffset.UtcNow.AddMinutes(-10).ToTimestamp(),
            EndTime = DateTimeOffset.UtcNow.ToTimestamp(),
        };
        var scheduleFormReply = new ScheduleFormReply
        {
            Id = id,
            GameProviderId = Guid.NewGuid(),
            GameProviderCode = gameProviderCode,
            Name = "test",
            Subject = "gp.gamerecord.scheduleform.pinnacle.queued",
            Source = ScheduleFormSource.Schedule,
            Parameters = JsonSerializer.Serialize(parameter),
            State = ScheduleFormState.Queued,
            Status = $"{ScheduleFormState.Queued}..."
        };
        _ = fakeClient
            .ScheduleFormGetAsync(
                gameProviderCode: Arg.Is(gameProviderCode),
                id: Arg.Is(id),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(scheduleFormReply);

        _ = fakeScheduleFormLock
            .LockingAsync(
                id: Arg.Is(id))
            .Returns(true);

        var scheduleCommand = new PullRecordCommand(
            ApiUrl: new Uri(parameter.ApiUrl),
            AgentCode: parameter.AgentCode,
            AgentKey: parameter.AgentKey,
            SecretKey: parameter.SecretKey,
            StartTime: parameter.StartTime.ToDateTimeOffset(),
            EndTime: parameter.EndTime.ToDateTimeOffset());

        _ = fakeSchedulePullRecord
           .ExecuteAsync(
               command: Arg.Is(scheduleCommand),
               cancellationToken: Arg.Any<CancellationToken>())
           .Throws<Exception>();

        var dispatcher = new ScheduleFormDispatcher(
            fakeLogger,
            fakeClient,
            fakeScheduleFormLock,
            fakeSchedulePullRecord);

        await dispatcher
            .ExecuteAsync(
            gameProviderCode: gameProviderCode,
            id: id);

        await fakeClient
            .Received(1)
            .ScheduleFormProcessingAsync(
            gameProviderCode: Arg.Is(gameProviderCode),
            id: Arg.Is(id),
            cancellationToken: Arg.Any<CancellationToken>());

        await fakeClient
            .Received(0)
            .ScheduleFormCompleteAsync(
            gameProviderCode: Arg.Is(gameProviderCode),
            id: Arg.Is(id),
            cancellationToken: Arg.Any<CancellationToken>());

        await fakeClient
            .Received(1)
            .ScheduleFormFaileAsync(
            gameProviderCode: Arg.Is(gameProviderCode),
            id: Arg.Is(id),
            status: Arg.Any<string>(),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = await fakeScheduleFormLock
            .Received(1)
            .UnlockAsync(
            id: Arg.Is(id));
    }

    [Fact]
    public async Task Normal_Process_Schedule()
    {
        var fakeLogger = NullLogger<ScheduleFormDispatcher>.Instance;
        var fakeClient = Substitute.For<IGameRoundsScheduleClient>();
        var fakeScheduleFormLock = Substitute.For<IScheduleFormLock>();
        var fakeSchedulePullRecord = Substitute.For<ICommandService<PullRecordCommand>>();

        var gameProviderCode = "PINNACLE";
        var id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var parameter = new PinnacleParameter
        {
            ApiUrl = "http://localhost/",
            AgentCode = "agentCode",
            AgentKey = "agentKey",
            SecretKey = "secretKey",
            StartTime = DateTimeOffset.UtcNow.AddMinutes(-10).ToTimestamp(),
            EndTime = DateTimeOffset.UtcNow.ToTimestamp(),
        };
        var scheduleFormReply = new ScheduleFormReply
        {
            Id = id,
            GameProviderId = Guid.NewGuid(),
            GameProviderCode = gameProviderCode,
            Name = "test",
            Subject = "gp.gamerecord.scheduleform.pinnacle.queued",
            Source = ScheduleFormSource.Schedule,
            Parameters = JsonSerializer.Serialize(parameter),
            State = ScheduleFormState.Queued,
            Status = $"{ScheduleFormState.Queued}..."
        };
        _ = fakeClient
            .ScheduleFormGetAsync(
                gameProviderCode: Arg.Is(gameProviderCode),
                id: Arg.Is(id),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(scheduleFormReply);

        _ = fakeScheduleFormLock
            .LockingAsync(
                id: Arg.Is(id))
            .Returns(true);

        var dispatcher = new ScheduleFormDispatcher(
            fakeLogger,
            fakeClient,
            fakeScheduleFormLock,
            fakeSchedulePullRecord);

        await dispatcher.ExecuteAsync(
            gameProviderCode: gameProviderCode,
            id: id);

        var scheduleCommand = new PullRecordCommand(
            ApiUrl: new Uri(parameter.ApiUrl),
            AgentCode: parameter.AgentCode,
            AgentKey: parameter.AgentKey,
            SecretKey: parameter.SecretKey,
            StartTime: parameter.StartTime.ToDateTimeOffset(),
            EndTime: parameter.EndTime.ToDateTimeOffset());

        await fakeSchedulePullRecord
            .Received(1)
            .ExecuteAsync(
            command: Arg.Is(scheduleCommand),
            cancellationToken: Arg.Any<CancellationToken>());

        await fakeClient
            .Received(1)
            .ScheduleFormProcessingAsync(
            gameProviderCode: Arg.Is(gameProviderCode),
            id: Arg.Is(id),
            cancellationToken: Arg.Any<CancellationToken>());

        await fakeClient
            .Received(1)
            .ScheduleFormCompleteAsync(
            gameProviderCode: Arg.Is(gameProviderCode),
            id: Arg.Is(id),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = await fakeScheduleFormLock
            .Received(1)
            .UnlockAsync(
            id: Arg.Is(id));
    }

    [Fact]
    public async Task Schedule_Form_State_Is_Not_Queued()
    {
        var fakeLogger = NullLogger<ScheduleFormDispatcher>.Instance;
        var fakeClient = Substitute.For<IGameRoundsScheduleClient>();
        var fakeScheduleFormLock = Substitute.For<IScheduleFormLock>();
        var fakeSchedulePullRecord = Substitute.For<ICommandService<PullRecordCommand>>();

        var gameProviderCode = "PINNACLE";
        var id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var parameter = new PinnacleParameter
        {
            ApiUrl = "http://localhost/",
            AgentCode = "agentCode",
            AgentKey = "agentKey",
            SecretKey = "secretKey",
            StartTime = DateTimeOffset.UtcNow.AddMinutes(-10).ToTimestamp(),
            EndTime = DateTimeOffset.UtcNow.ToTimestamp(),
        };
        var scheduleFormReply = new ScheduleFormReply
        {
            Id = id,
            GameProviderId = Guid.NewGuid(),
            GameProviderCode = gameProviderCode,
            Name = "test",
            Subject = "gp.gamerecord.scheduleform.pinnacle.queued",
            Source = ScheduleFormSource.Schedule,
            Parameters = JsonSerializer.Serialize(parameter),
            State = ScheduleFormState.Processing,
            Status = $"{ScheduleFormState.Processing}..."
        };
        _ = fakeClient
            .ScheduleFormGetAsync(
                gameProviderCode: Arg.Is(gameProviderCode),
                id: Arg.Is(id),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(scheduleFormReply);

        var dispatcher = new ScheduleFormDispatcher(
            fakeLogger,
            fakeClient,
            fakeScheduleFormLock,
            fakeSchedulePullRecord);

        await dispatcher
            .ExecuteAsync(
            gameProviderCode: gameProviderCode,
            id: id);

        _ = await fakeScheduleFormLock
            .Received(0)
            .LockingAsync(
            id: Arg.Is(id));

        var scheduleCommand = new PullRecordCommand(
            ApiUrl: new Uri(parameter.ApiUrl),
            AgentCode: parameter.AgentCode,
            AgentKey: parameter.AgentKey,
            SecretKey: parameter.SecretKey,
            StartTime: parameter.StartTime.ToDateTimeOffset(),
            EndTime: parameter.EndTime.ToDateTimeOffset());

        await fakeSchedulePullRecord
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(scheduleCommand),
            cancellationToken: Arg.Any<CancellationToken>());

        await fakeClient
            .Received(0)
            .ScheduleFormProcessingAsync(
            gameProviderCode: Arg.Is(gameProviderCode),
            id: Arg.Is(id),
            cancellationToken: Arg.Any<CancellationToken>());

        await fakeClient
            .Received(0)
            .ScheduleFormCompleteAsync(
            gameProviderCode: Arg.Is(gameProviderCode),
            id: Arg.Is(id),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = await fakeScheduleFormLock
            .Received(0)
            .UnlockAsync(
            id: Arg.Is(id));
    }
}
