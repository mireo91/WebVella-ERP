using Newtonsoft.Json;

namespace Reomi.Erp.Plugins.Inventory.Model;

public class PluginSettings
{
    [JsonProperty(PropertyName = "version")]
    public int Version { get; set; }
}