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
	[HttpPost("schedule")]
	public ValueTask GameRecordScheduleAsync(
		[FromServices] IMessageSender messageSender,
		PinnacleGameRecordMessage source)
		=> messageSender.PublishAsync(
			subject: PinnacleNatsConst.SchedulePublish,
			data: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(source)),
			cancellationToken: HttpContext.RequestAborted);
}
