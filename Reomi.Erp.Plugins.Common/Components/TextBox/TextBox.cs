using Microsoft.AspNetCore.Mvc;
using WebVella.Erp.Web;
using WebVella.Erp.Web.Components;
using WebVella.Erp.Web.Models;

namespace Reomi.Erp.Plugins.Common.Components;

[PageComponent(Label = "TextBox", Library = "Reomi", Description = "Provides texbox blazor functionality", Version = "0.0.1", IconClass = "fas fa-font")]
public class TextBox : PcFieldText
{
  //   protected ErpRequestContext ErpRequestContext { get; set; }
  //
  public TextBox([FromServices] ErpRequestContext coreReqCtx) : base(coreReqCtx)
  {
  }

}