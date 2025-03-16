using Newtonsoft.Json;

namespace Reomi.Erp.Plugins.SaleAndWarehouse.Model;

public class PluginSettings
{
    [JsonProperty(PropertyName = "version")]
    public int Version { get; set; }
}