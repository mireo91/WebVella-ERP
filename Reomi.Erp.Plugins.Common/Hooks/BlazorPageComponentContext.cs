using Microsoft.AspNetCore.Http;
using WebVella.Erp.Api;
using WebVella.Erp.Web;
using WebVella.Erp.Web.Models;
using WebVella.Erp.Web.Services;

namespace Reomi.Erp.Plugins.Common.Hooks;

public static class BlazorPageComponentContext
{
    public static PageComponentContext CurrentPageContext { get; set; }
    
    public static ErpRequestContext ErpRequestContext { get; set; }

    public static void Initialize(ref PageComponentContext context, ErpRequestContext erpRequestContext)
    {
        ErpRequestContext = erpRequestContext;
        CurrentPageContext = context;
    }
}