using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tgs.GameRecord.Pinnacle.Core;
using Tgs.GameRounds.Schedule.Client;
using Tgs.GameRounds.Schedule.Protos;
using Tgs.GameRecord.Pinnacle.ViewModels;

namespace Tgs.GameRecord.Pinnacle.Controllers;

[Route("pinnacle")]
[ApiController]
public class PinnacleController : ControllerBase
{
    private readonly GameRecordInfoOptions _gameRecordInfo;

    public PinnacleController(
        IOptions<GameRecordInfoOptions> options)
    {
        _gameRecordInfo = options.Value; ;
    }

    /// <summary>
    /// Manuals the game record asynchronous.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///         {
    ///             "apiUrl": "https://paapistg.oreo88.com",
    ///             "agentCode": "V0201",
    ///             "agentKey": "8d0e9ac0-99d8-48c7-8307-bc328c4181f6",
    ///             "secretKey": "zenDIdY7Knuzmeen",
    ///             "startTime": "2023-05-23T19:25:00+08:00",
    ///             "endTime": "2023-05-23T19:30:00+08:00"
    ///         }
    /// </remarks>
    [HttpPost("manual")]
    public ValueTask ManualGameRecordAsync(
        [FromServices] IGameRoundsScheduleClient client,
        PinnacleViewModel source)
        => client.Pinnacle_ScheduleFormQueuedAsync(
             gameProviderCode: _gameRecordInfo.GameProviderCode,
             name: source.AgentCode,
             subject: "gp.gamerecord.scheduleform.pinnacle.queued",
             source: ScheduleFormSource.Manual,
             apiUrl: source.ApiUrl,
             agentCode: source.AgentCode,
             agentKey: source.AgentKey,
             secretKey: source.SecretKey,
             startTime: source.StartTime,
             endTime: source.EndTime,
             cancellationToken: HttpContext.RequestAborted);

    /// <summary>
    /// Schedules the game record asynchronous.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///         {
    ///             "apiUrl": "https://paapistg.oreo88.com",
    ///             "agentCode": "V0201",
    ///             "agentKey": "8d0e9ac0-99d8-48c7-8307-bc328c4181f6",
    ///             "secretKey": "zenDIdY7Knuzmeen",
    ///             "startTime": "2023-05-23T19:25:00+08:00",
    ///             "endTime": "2023-05-23T19:30:00+08:00"
    ///         }
    /// </remarks>
    [HttpPost("schedule")]
    public ValueTask ScheduleGameRecordAsync(
        [FromServices] IGameRoundsScheduleClient client,
        PinnacleViewModel source)
        => client.Pinnacle_ScheduleFormQueuedAsync(
             gameProviderCode: _gameRecordInfo.GameProviderCode,
             name: source.AgentCode,
             subject: "gp.gamerecord.scheduleform.pinnacle.queued",
             source: ScheduleFormSource.Schedule,
             apiUrl: source.ApiUrl,
             agentCode: source.AgentCode,
             agentKey: source.AgentKey,
             secretKey: source.SecretKey,
             startTime: source.StartTime,
             endTime: source.EndTime,
             cancellationToken: HttpContext.RequestAborted);
}
