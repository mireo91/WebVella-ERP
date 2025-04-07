using Microsoft.AspNetCore.Http;
using WebVella.Erp.Web;
using WebVella.Erp.Web.Models;

namespace Reomi.Erp.Plugins.Common.Hooks;

public static class BlazorPageComponentContext
{
    public static PageComponentContext CurrentPageContext { get; set; }
    
    public static ErpRequestContext ErpRequestContext { get; set; }

    public static void Initialize(ref PageComponentContext context, ErpRequestContext erpRequestContext)
    {
        CurrentPageContext = context;
        ErpRequestContext = erpRequestContext;
    }
}