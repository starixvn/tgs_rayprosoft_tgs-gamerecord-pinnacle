using Microsoft.Extensions.Logging.Abstractions;

using NSubstitute;
using NSubstitute.ReceivedExtensions;

using Tgs.GameInterface.Pinnacle.Client;
using Tgs.GameRecord.Pinnacle.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Pinnacle.Core.UnitTest.ApplicationServices;

public class SchedulePullRecordCommandServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<SchedulePullRecordCommandService>.Instance;
        var fakeClient = Substitute.For<IGameInterfacePinnacleClient>();
        var fakeGameRecordUtensil = Substitute.For<IGameRecordUtensil>();
        var fakeService = Substitute.For<ICommandService<ScheduleHandleRecordCommand>>();

        var command = new SchedulePullRecordCommand(
            ApiUrl: new("http://localhost/"),
            AgentCode: "agentCode",
            AgentKey: "agentKey",
            SecretKey: "secretKey",
            StartTime: DateTimeOffset.UtcNow.AddMinutes(-10),
            EndTime: DateTimeOffset.UtcNow);

        var service = new SchedulePullRecordCommandService(
            fakeLogger,
            fakeClient,
            fakeGameRecordUtensil,
            fakeService);

        await service
            .ExecuteAsync(
                command: command)
            .ConfigureAwait(false);

        _ = await fakeClient
            .Received(1)
            .GameRecordAsync(
                apiUrl: Arg.Is(command.ApiUrl),
                agentCode: Arg.Is(command.AgentCode),
                agentKey: Arg.Is(command.AgentKey),
                secretKey: Arg.Is(command.SecretKey),
                startTime: Arg.Is(command.StartTime),
                endTime: Arg.Is(command.EndTime),
                cancellationToken: Arg.Any<CancellationToken>())
            .ToArrayAsync(
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }
}