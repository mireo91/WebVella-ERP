using System;
using Newtonsoft.Json;
using WebVella.Erp;
using WebVella.Erp.Api;

namespace Reomi.Erp.Plugins.Inventory;

public class InventoryPlugin : ErpPlugin
{
    [JsonProperty(PropertyName = "name")] public override string Name { get; protected set; } = "inventory";

    public override void Initialize(IServiceProvider serviceProvider)
    {
        using (var ctx = SecurityContext.OpenSystemScope())
        {
        }

    }
}