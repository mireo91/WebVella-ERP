using Newtonsoft.Json;
using WebVella.Erp;
using WebVella.Erp.Api;

namespace Reomi.Erp.Plugin.Sale;

public partial class SalePlugin: ErpPlugin
{
    [JsonProperty(PropertyName = "name")]
    public override string Name { get; protected set; } = "sale";

    public override void Initialize(IServiceProvider serviceProvider)
    {
        using (var ctx = SecurityContext.OpenSystemScope())
        {
            // ProcessPatches();
        }
    }
}