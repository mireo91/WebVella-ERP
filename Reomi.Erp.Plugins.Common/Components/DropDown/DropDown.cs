using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebVella.Erp.Api.Models;
using WebVella.Erp.Exceptions;
using WebVella.Erp.Web;
using WebVella.Erp.Web.Components;
using WebVella.Erp.Web.Models;
using WebVella.Erp.Web.Services;
using WebVella.TagHelpers.Models;

namespace Reomi.Erp.Plugins.Common.Components;

[PageComponent(Label = "DropDown", Library = "Reomi", Description = "Provides dynamic dataGrid", Version = "0.0.1", IconClass = "fas fa-list")]
public class DropDown : PcFieldSelect
{
  //   protected ErpRequestContext ErpRequestContext { get; set; }
  //
  public DropDown([FromServices] ErpRequestContext coreReqCtx) : base(coreReqCtx)
  {
  }

}