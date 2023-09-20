using Tgs.GameRecord.Pinnacle.Core.JobApplicationServices;

namespace Tgs.GameRecord.Pinnacle.Core;

public interface IHangfireUtensil
{
    public Task JobCheckAsync();

    public Task JobExecuteAsync(JobExecuteCommand source);

    public Task JobInitAsync();
}
