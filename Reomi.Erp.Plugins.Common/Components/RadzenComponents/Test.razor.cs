using Microsoft.AspNetCore.Components;

namespace Reomi.Erp.Plugins.Common.Components.RadzenComponents;

public partial class Test : ComponentBase
{
    [Parameter]
    public string Text { get; set; }
}