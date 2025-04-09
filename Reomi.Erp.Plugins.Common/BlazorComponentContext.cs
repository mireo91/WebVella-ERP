using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;
using WebVella.Erp.Web;
using WebVella.Erp.Web.Models;
using WebVella.Erp.Web.Utils;

namespace Reomi.Erp.Plugins.Common;

// public class BlazorComponentContext
// {
//     // public PageComponentContext PageComponentContext { get; private set; }
//     public BlazorPageComponentContext BlazorPageComponentContext { get; private set; }
//     
// }

public class BlazorPageComponentContext
{
    public BlazorForm? FormData { get; private set; }
    public static PageComponentContext? PageComponentContext { get; set; }
    public static ErpRequestContext? ErpRequestContext { get; set; }
    public PageBodyNode Node { get; protected set; }
    
    public BlazorPageComponentContext(PageComponentContext pageComponentContext, ErpRequestContext erpRequestContext, PageBodyNode node): this(node)
    {
        ErpRequestContext = erpRequestContext;
        PageComponentContext = pageComponentContext;
    }
    
    [JsonConstructor]
    public BlazorPageComponentContext(PageBodyNode node)
    {
        Node = node;
    }

    public void InitializeBlazorForm(BlazorForm form)
    {
        FormData = form;
    }
    
    // private BlazorPageComponentContext(PageComponentContext pageComponentContext)
    // {
    //     PageComponentContext = pageComponentContext;
    // }
    public static BlazorPageComponentContext CreateFromParentContext(BlazorPageComponentContext context, PageBodyNode node)
    {
        return new BlazorPageComponentContext(node)
        {
            FormData = context.FormData
        };
    }
}
public class BlazorForm : Dictionary<string, string>
{
    public string FormId { get; private set; }

    public BlazorForm(string formId) : base()
    {
        FormId = formId;
    }

    BlazorForm(string formId, int capacity) : base(capacity)
    {
        FormId = formId;
    }
}