using Hangfire.Dashboard;

namespace Tgs.GameRecord.Pinnacle;

public class IgnoreHangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context) => true;
}
