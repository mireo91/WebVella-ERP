
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using WebVella.Erp.Web;
using WebVella.Erp.Web.Components;
using WebVella.Erp.Web.Models;

using WebVella.Erp.Web.Utils;


namespace Reomi.Erp.Plugins.Common.Components;

[PageComponent(Label = "Fieldset", Library = "Reomi", Description = "A foldable section", Version = "0.0.1", IconClass = "far fa-object-group")]
public class Fieldset : PcSection
{
  //   protected ErpRequestContext ErpRequestContext { get; set; }
  //
  public Fieldset([FromServices] ErpRequestContext coreReqCtx) : base(coreReqCtx)
  {
    // ViewBag.MVC = RenderViewComponent().Result;
  }
}