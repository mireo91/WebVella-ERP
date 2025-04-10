using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WebVella.Erp.Api.Models;
using WebVella.Erp.Web;
using WebVella.Erp.Web.Components;
using WebVella.Erp.Web.Models;
using WebVella.Erp.Web.Services;
using WebVella.TagHelpers.Models;

namespace Reomi.Erp.Plugins.Common.Components.RadzenComponents;

public partial class ReomiTemplateForm : ReomiCollectionComponentBase<WebVella.Erp.Web.Components.PcForm.PcFormOptions>
{
    [Parameter]
    public string AntiforgeryToken { get; set; }
    private string Action { get; set; } = string.Empty;
    protected override PcForm.PcFormOptions InitializeFieldOptions()
    {
        #region << Init >>
        
        var instanceOptions = new PcForm.PcFormOptions();
        if (Context.Node.Options != null)
        {
            instanceOptions = JsonConvert.DeserializeObject<PcForm.PcFormOptions>(Context.Node.Options.ToString());
            if (instanceOptions.LabelMode == WvLabelRenderMode.Undefined)
                instanceOptions.LabelMode = WvLabelRenderMode.Stacked;
            if (instanceOptions.Mode == WvFieldRenderMode.Undefined)
                instanceOptions.Mode = WvFieldRenderMode.Form;
        }

        if (String.IsNullOrWhiteSpace(instanceOptions.Id))
        {
            instanceOptions.Id = "wv-" + Context.Node.Id.ToString();
        }

        // var componentMeta = new PageComponentLibraryService().GetComponentMeta(Context.Node.ComponentName);
        #endregion



        // ViewBag.Options = instanceOptions;
        // ViewBag.Node = context.Node;
        // ViewBag.ComponentMeta = componentMeta;
        // ViewBag.RequestContext = ErpRequestContext;
        // ViewBag.AppContext = ErpAppContext.Current;
        // ViewBag.ComponentContext = context;
        // ViewBag.GeneralHelpSection = HelpJsApiGeneralSection;

        // ViewBag.LabelRenderModeOptions = WebVella.TagHelpers.Utilities.ModelExtensions.GetEnumAsSelectOptions<WvLabelRenderMode>();

        // ViewBag.FieldRenderModeOptions = WebVella.TagHelpers.Utilities.ModelExtensions.GetEnumAsSelectOptions<WvFieldRenderMode>();

        // context.Items[typeof(WvLabelRenderMode)] = instanceOptions.LabelMode;
        // context.Items[typeof(WvFieldRenderMode)] = instanceOptions.Mode;

        // ViewBag.MethodOptions = new List<SelectOption>() {
        //     new SelectOption("get","get"),
        //     new SelectOption("post","post")
        //     };
        var context = BlazorPageComponentContext.PageComponentContext;
        if (context.Mode != ComponentMode.Options && context.Mode != ComponentMode.Help)
        {
            var isVisible = true;
            var isVisibleDS = context.DataModel.GetPropertyValueByDataSource(instanceOptions.IsVisible);
            if (isVisibleDS is string && !String.IsNullOrWhiteSpace(isVisibleDS.ToString()))
            {
                if (Boolean.TryParse(isVisibleDS.ToString(), out bool outBool))
                {
                    isVisible = outBool;
                }
            }
            else if (isVisibleDS is Boolean)
            {
                isVisible = (bool)isVisibleDS;
            }
            IsVisible = isVisible;

            // var validation = context.DataModel.GetProperty("Validation") as ValidationException ?? new ValidationException();
            //
            // context.Items[typeof(ValidationException)] = validation;
            // ViewBag.Validation = validation;
            
            if (!String.IsNullOrWhiteSpace(instanceOptions.HookKey))
            {
                // Console.WriteLine(BlazorPageComponentContext.ErpRequestContext);
                // Console.WriteLine(BlazorPageComponentContext.ErpRequestContext.PageContext);
                // Console.WriteLine(BlazorPageComponentContext.ErpRequestContext.PageContext.HttpContext);
                // Console.WriteLine(BlazorPageComponentContext.ErpRequestContext.PageContext.HttpContext.Request);
                // Console.WriteLine(BlazorPageComponentContext.ErpRequestContext.PageContext.HttpContext.Request.Query);
                // Console.WriteLine(BlazorPageComponentContext.ErpRequestContext.PageContext.HttpContext.Request.Query.Keys.Count);
                // var httpContext = BlazorPageComponentContext.ErpRequestContext.PageContext.HttpContext;
                // var queryList = new List<SelectOption>();
                // foreach (var key in httpContext.Request.Query.Keys)
                // {
                //     if (key != "hookKey")
                //     {
                //         queryList.Add(new SelectOption(key, httpContext.Request.Query[key].ToString()));
                //     }
                // }
                // queryList.Add(new SelectOption("hookKey", instanceOptions.HookKey)); //override even if already present
                //
                // Action = string.Format(httpContext.Request.Path + "?{0}", string.Join("&", queryList.Select(kvp => string.Format("{0}={1}", kvp.Value, kvp.Label))));
            }
        }

        return instanceOptions;
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