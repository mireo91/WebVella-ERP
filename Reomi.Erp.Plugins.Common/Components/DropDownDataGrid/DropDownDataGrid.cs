using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Reomi.Erp.Plugins.Common.Hooks;
using WebVella.Erp.Exceptions;
using WebVella.Erp.Web;
using WebVella.Erp.Web.Components;
using WebVella.Erp.Web.Models;
using WebVella.Erp.Web.Services;
using WebVella.TagHelpers.Models;

namespace Reomi.Erp.Plugins.Common.Components;

[PageComponent(Label = "DropDown DataGrid", Library = "Reomi",  Description = "Provides dynamic dataGrid", Version = "0.0.1", IconClass = "fas fa-list")]
public class DropDownDataGrid : PcFieldSelect
{
    //   protected ErpRequestContext ErpRequestContext { get; set; }
    //
    public DropDownDataGrid([FromServices] ErpRequestContext coreReqCtx) : base(coreReqCtx)
    {
    }

}