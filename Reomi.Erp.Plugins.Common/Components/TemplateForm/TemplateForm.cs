using Microsoft.AspNetCore.Mvc;

using WebVella.Erp.Web;
using WebVella.Erp.Web.Components;
using WebVella.Erp.Web.Models;


namespace Reomi.Erp.Plugins.Common.Components;

[PageComponent(Label = "TemplateForm", Library = "Reomi", Description = "A foldable section", Version = "0.0.1", IconClass = "fas fa-poll-h")]
public class TemplateFrom : PcForm
{
	// protected ErpRequestContext ErpRequestContext { get; set; }

	public TemplateFrom([FromServices]ErpRequestContext coreReqCtx):base(coreReqCtx)
	{
		// ErpRequestContext = coreReqCtx;
	}
}