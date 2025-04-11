using Newtonsoft.Json;
using WebVella.Erp;
using WebVella.Erp.Api;

namespace Reomi.Erp.Plugins.WholesaleFeedIntegrator;

public class WholesaleFeedIntegratorPlugin : ErpPlugin
{
    [JsonProperty(PropertyName = "name")]
    public override string Name { get; protected set; } = "wholesale_feed_integrator";

    public override void Initialize(IServiceProvider serviceProvider)
    {
        using (var ctx = SecurityContext.OpenSystemScope())
        {
            // ProcessPatches();
        }
    }
}