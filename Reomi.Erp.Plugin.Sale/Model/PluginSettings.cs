using Newtonsoft.Json;

namespace Reomi.Erp.Plugin.Sale.Model;

public class PluginSettings
{
    [JsonProperty(PropertyName = "version")]
    public int Version { get; set; }
}