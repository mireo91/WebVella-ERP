using Newtonsoft.Json;

namespace Reomi.Erp.Plugins.WholesaleFeedIntegrator.Model;

public class PluginSettings
{
    [JsonProperty(PropertyName = "version")]
    public int Version { get; set; }
}