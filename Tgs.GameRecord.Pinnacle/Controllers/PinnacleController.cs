using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.Pinnacle.Common;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Pinnacle.Controllers;

[Route("pinnacle")]
[ApiController]
public class PinnacleController : ControllerBase
{
	/// <summary>
	/// Games the record manual asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
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
	public ValueTask GameRecordManualAsync(
		[FromServices] IMessageSender messageSender,
		PinnacleGameRecordMessage source)
		=> messageSender.PublishAsync(
			subject: PinnacleNatsConst.ManualPublish,
			data: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(source)),
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Games the record schedule asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
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
	public ValueTask GameRecordScheduleAsync(
		[FromServices] IMessageSender messageSender,
		PinnacleGameRecordMessage source)
		=> messageSender.PublishAsync(
			subject: PinnacleNatsConst.SchedulePublish,
			data: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(source)),
			cancellationToken: HttpContext.RequestAborted);
}
