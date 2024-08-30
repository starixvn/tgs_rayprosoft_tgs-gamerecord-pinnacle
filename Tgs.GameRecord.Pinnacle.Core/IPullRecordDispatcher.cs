using Tgs.GameRecord.Pinnacle.Core.ApplicationServices;

namespace Tgs.GameRecord.Pinnacle.Core;

public interface IPullRecordDispatcher
{
	ValueTask PushCommandAsync(PullRecordCommand command, CancellationToken cancellationToken = default);
}
