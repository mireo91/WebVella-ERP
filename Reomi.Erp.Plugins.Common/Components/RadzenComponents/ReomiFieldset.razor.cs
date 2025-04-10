using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Reomi.Erp.Plugins.Common.Hooks;
using WebVella.Erp.Web;
using WebVella.Erp.Web.Components;
using WebVella.Erp.Web.Models;
using WebVella.Erp.Web.Services;
using WebVella.TagHelpers.Models;
using static System.Net.Http.HttpMethod;

namespace Reomi.Erp.Plugins.Common.Components.RadzenComponents;

public partial class ReomiFieldset : ReomiCollectionComponentBase<PcSection.PcSectionOptions>
{
   void Change(string text)
    {
        // Console.WriteLine($"{text}");
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

    protected override PcSection.PcSectionOptions InitializeFieldOptions()
    {
        var context = BlazorPageComponentContext.PageComponentContext;
        var httpContext = BlazorPageComponentContext.ErpRequestContext.PageContext.HttpContext;
        // try
        // {
        
        #region << Init >>
        
        var options = new PcSection.PcSectionOptions();
        if (context.Options != null)
        {
            options = JsonConvert.DeserializeObject<PcSection.PcSectionOptions>(Context!.Node.Options.ToString());
        }
        
        //Check if it is defined in form group
        if (options.LabelMode == WvLabelRenderMode.Undefined)
        {
            if (context.Items.ContainsKey(typeof(WvLabelRenderMode)))
            {
                options.LabelMode = (WvLabelRenderMode)context.Items[typeof(WvLabelRenderMode)];
            }
            else
            {
                options.LabelMode = WvLabelRenderMode.Stacked;
            }
        }
        
        //Check if it is defined in form group
        if (options.FieldMode == WvFieldRenderMode.Undefined)
        {
            if (context.Items.ContainsKey(typeof(WvFieldRenderMode)))
            {
                options.FieldMode = (WvFieldRenderMode)context.Items[typeof(WvFieldRenderMode)];
            }
            else
            {
                options.FieldMode = WvFieldRenderMode.Form;
            }
        }
        
        // var componentMeta = new PageComponentLibraryService().GetComponentMeta(context.Node.ComponentName);
        
        //Init IsCollapsed from userPreferences
        // if (httpContext.User != null)
        // {
        //     var currentUser = AuthService.GetUser(httpContext.User);
        //     if (currentUser != null)
        //     {
        //         var componentData =
        //             new UserPreferencies().GetComponentData(currentUser.Id,
        //                 "WebVella.Erp.Web.Components.PcSection");
        //         if (componentData != null)
        //         {
        //             var collapsedNodeIds = new List<Guid>();
        //             var uncollapsedNodeIds = new List<Guid>();
        //             if (componentData.Properties.ContainsKey("collapsed_node_ids") &&
        //                 componentData["collapsed_node_ids"] != null)
        //             {
        //                 if (componentData["collapsed_node_ids"] is string)
        //                 {
        //                     try
        //                     {
        //                         collapsedNodeIds =
        //                             JsonConvert.DeserializeObject<List<Guid>>(
        //                                 (string)componentData["collapsed_node_ids"]);
        //                     }
        //                     catch
        //                     {
        //                         throw new Exception(
        //                             "WebVella.Erp.Web.Components.PcSection component data object in user preferences not in the correct format. collapsed_node_ids should be List<Guid>");
        //                     }
        //                 }
        //                 else if (componentData["collapsed_node_ids"] is List<Guid>)
        //                 {
        //                     collapsedNodeIds = (List<Guid>)componentData["collapsed_node_ids"];
        //                 }
        //                 else if (componentData["collapsed_node_ids"] is JArray)
        //                 {
        //                     collapsedNodeIds = ((JArray)componentData["collapsed_node_ids"]).ToObject<List<Guid>>();
        //                 }
        //                 else
        //                 {
        //                     throw new Exception("Unknown format of collapsed_node_ids");
        //                 }
        //             }
        //
        //             if (componentData.Properties.ContainsKey("uncollapsed_node_ids") &&
        //                 componentData["uncollapsed_node_ids"] != null)
        //             {
        //                 if (componentData["uncollapsed_node_ids"] is string)
        //                 {
        //                     try
        //                     {
        //                         uncollapsedNodeIds =
        //                             JsonConvert.DeserializeObject<List<Guid>>(
        //                                 (string)componentData["uncollapsed_node_ids"]);
        //                     }
        //                     catch
        //                     {
        //                         throw new Exception(
        //                             "WebVella.Erp.Web.Components.PcSection component data object in user preferences not in the correct format. uncollapsed_node_ids should be List<Guid>");
        //                     }
        //                 }
        //                 else if (componentData["uncollapsed_node_ids"] is List<Guid>)
        //                 {
        //                     uncollapsedNodeIds = (List<Guid>)componentData["uncollapsed_node_ids"];
        //                 }
        //                 else if (componentData["uncollapsed_node_ids"] is JArray)
        //                 {
        //                     uncollapsedNodeIds =
        //                         ((JArray)componentData["uncollapsed_node_ids"]).ToObject<List<Guid>>();
        //                 }
        //                 else
        //                 {
        //                     throw new Exception("Unknown format of uncollapsed_node_ids");
        //                 }
        //             }
        //
        //             if (collapsedNodeIds.Contains(context.Node.Id))
        //             {
        //                 options.IsCollapsed = true;
        //             }
        //             else if (uncollapsedNodeIds.Contains(context.Node.Id))
        //             {
        //                 options.IsCollapsed = false;
        //             }
        //         }
        //
        //     }
        // }
        
        #endregion
        
        var isCollapsed = context.DataModel.GetPropertyValueByDataSource(options.IsCollapsedDs) as bool?;
        if (isCollapsed != null)
        {
            options.IsCollapsed = isCollapsed.Value;
        }
        else if (options.IsCollapsedDs.ToLowerInvariant() == "true")
        {
            options.IsCollapsed = true;
        }
        // Context.InitializeBlazorForm(new BlazorForm("test"));
        // context.Items[typeof(WvLabelRenderMode)] = FieldOptions.LabelMode;
        // context.Items[typeof(WvFieldRenderMode)] = FieldOptions.FieldMode;
        // }
        // catch
        // {
        //     
        // }
        return options;
    }
}