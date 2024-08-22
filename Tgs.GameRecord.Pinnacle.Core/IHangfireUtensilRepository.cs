using Tgs.GameRecord.Pinnacle.Core.JobApplicationServices;

namespace Tgs.GameRecord.Pinnacle.Core;

public interface IHangfireUtensilRepository
{
	public Task JobCheckAsync();

	public Task JobExecuteAsync(JobExecuteCommand source);

	public Task JobInitAsync();
}
