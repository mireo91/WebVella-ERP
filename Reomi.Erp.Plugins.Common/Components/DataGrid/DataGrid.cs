using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebVella.Erp.Exceptions;
using WebVella.Erp.Web;
using WebVella.Erp.Web.Models;
using WebVella.Erp.Web.Services;

namespace Reomi.Erp.Plugins.Common.Components;

[PageComponent(Label = "DataGrid", Library = "Reomi", Description = "Provides dynamic dataGrid", Version = "0.0.1", IconClass = "fas fa-table")]
public class DataGrid : PageComponent
{
    protected ErpRequestContext ErpRequestContext { get; set; }

    public DataGrid([FromServices]ErpRequestContext coreReqCtx)
    {
        ErpRequestContext = coreReqCtx;
    }
    
    public class DataGridOptions
    {
        [JsonProperty(PropertyName = "columns")]
        public string columns { get; set; } = null;

        [JsonProperty(PropertyName = "width")]
        public string Width { get; set; } = null;
    }

    public async Task<IViewComponentResult> InvokeAsync(PageComponentContext context)
    {
        ErpPage currentPage = null;
		try
		{
			#region << Init >>
			if (context.Node == null)
			{
				return await Task.FromResult<IViewComponentResult>(Content("Error: The node Id is required to be set as query parameter 'nid', when requesting this component"));
			}

			var pageFromModel = context.DataModel.GetProperty("Page");
			if (pageFromModel == null)
			{
				return await Task.FromResult<IViewComponentResult>(Content("Error: PageModel cannot be null"));
			}
			else if (pageFromModel is ErpPage)
			{
				currentPage = (ErpPage)pageFromModel;
			}
			else
			{
				return await Task.FromResult<IViewComponentResult>(Content("Error: PageModel does not have Page property or it is not from ErpPage Type"));
			}

			var options = new DataGridOptions();
			if (context.Options != null)
			{
				options = JsonConvert.DeserializeObject<DataGridOptions>(context.Options.ToString());
			}

			var componentMeta = new PageComponentLibraryService().GetComponentMeta(context.Node.ComponentName);
			#endregion

			ViewBag.Options = options;
			ViewBag.Node = context.Node;
			ViewBag.ComponentMeta = componentMeta;
			ViewBag.RequestContext = ErpRequestContext;
			ViewBag.AppContext = ErpAppContext.Current;
			ViewBag.ComponentContext = context;
			ViewBag.CurrentUser = AuthService.GetUser(HttpContext.User);
			
			switch (context.Mode)
			{
				case ComponentMode.Display:
					return await Task.FromResult<IViewComponentResult>(View("Display"));
				case ComponentMode.Design:
					return await Task.FromResult<IViewComponentResult>(View("Design"));
				case ComponentMode.Options:
					return await Task.FromResult<IViewComponentResult>(View("Options"));
				case ComponentMode.Help:
					return await Task.FromResult<IViewComponentResult>(View("Help"));
				default:
					ViewBag.ExceptionMessage = "Unknown component mode";
					ViewBag.Errors = new List<ValidationError>();
					return await Task.FromResult<IViewComponentResult>(View("Error"));
			}
		}
		catch (ValidationException ex)
		{
			ViewBag.ExceptionMessage = ex.Message;
			ViewBag.Errors = new List<ValidationError>();
			return await Task.FromResult<IViewComponentResult>(View("Error"));
		}
		catch (Exception ex)
		{
			ViewBag.ExceptionMessage = ex.Message;
			ViewBag.Errors = new List<ValidationError>();
			return await Task.FromResult<IViewComponentResult>(View("Error"));
		}
    }

}