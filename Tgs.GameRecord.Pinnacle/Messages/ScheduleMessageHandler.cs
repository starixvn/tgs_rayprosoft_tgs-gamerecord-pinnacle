using System.Text;
using System.Text.Json;
using Tgs.GameRecord.Pinnacle.Common;
using Tgs.GameRecord.Pinnacle.Core;
using Tgs.GameRecord.Pinnacle.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Pinnacle.Messages;

internal class ScheduleMessageHandler(
	ILogger<ScheduleMessageHandler> logger,
	IPullRecordDispatcher dispatcher) : IMessageHandler<ReadOnlyMemory<byte>>
{
	public ValueTask HandleAsync(string subject, ReadOnlyMemory<byte> data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		var json = Encoding.UTF8.GetString(data.Span);
		var message = JsonSerializer.Deserialize<PinnacleGameRecordMessage>(json)
			?? throw new NullReferenceException("The request data resolves to a null value");

		logger.LogInformation("{logInformation}",
			JsonSerializer.Serialize(new
			{
				LogOn = DateTime.Now.ToString("u"),
				Subject = subject,
				Data = new
				{
					message.ApiUrl,
					message.AgentCode,
					message.AgentKey,
					message.SecretKey,
					StartTime = message.StartTime.ToString("u"),
					EndTime = message.EndTime.ToString("u")
				},
				Message = "This message receiving handler."
			}));

		return dispatcher.PushCommandAsync(
			command: new PullRecordCommand(
				ApiUrl: message.ApiUrl,
				AgentCode: message.AgentCode,
				AgentKey: message.AgentKey,
				SecretKey: message.SecretKey,
				StartTime: message.StartTime,
				EndTime: message.EndTime),
			cancellationToken: cancellationToken);
	}
}
