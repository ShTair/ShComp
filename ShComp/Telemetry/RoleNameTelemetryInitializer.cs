using Microsoft.ApplicationInsights;

namespace ShComp.Telemetry;

public class RoleNameTelemetryInitializer
{
    private readonly string _roleName;

    public RoleNameTelemetryInitializer(string roleName)
    {
        _roleName = roleName;
    }

    public void Initialize(TelemetryClient client)
    {
        if (string.IsNullOrEmpty(client.Context.Cloud.RoleName))
        {
            client.Context.Cloud.RoleName = _roleName;
        }
    }
}
