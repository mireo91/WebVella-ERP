using Microsoft.AspNetCore.Components;
using WebVella.Erp.Web.Components;

namespace Reomi.Erp.Plugins.Common.Components.RadzenComponents;

public partial class ReomiTemplateForm : ReomiFieldComponentBase<WebVella.Erp.Web.Components.PcForm.PcFormOptions>
{
    protected override PcForm.PcFormOptions InitializeFieldOptions()
    {
        throw new NotImplementedException();
    }

    private RenderFragment RenderNodes() =>
        async builder =>
        {
            int nodeSequence = 0;
            foreach (var node in Context.Node.Nodes)
            {
                var helperType = Type.GetType(node.ComponentName);
                var t = Type.GetType($"Reomi.Erp.Plugins.Common.Components.RadzenComponents.Reomi{helperType?.Name}");
                if (t == null)
                    return;
                builder.OpenComponent(nodeSequence, t);
                builder.AddComponentParameter(1, "Context", BlazorPageComponentContext.CreateFromParentContext(Context, node));
                builder.CloseComponent();
                nodeSequence++;
            }
        };
}