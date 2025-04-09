
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Reomi.Erp.Plugins.Common.Hooks;
using WebVella.Erp.Exceptions;
using WebVella.Erp.Web;
using WebVella.Erp.Web.Components;
using WebVella.Erp.Web.Models;
using WebVella.Erp.Web.Services;
using WebVella.Erp.Web.Utils;
using WebVella.TagHelpers.Models;


namespace Reomi.Erp.Plugins.Common.Components;

[PageComponent(Label = "Fieldset", Library = "Reomi", Description = "A foldable section", Version = "0.0.1", IconClass = "far fa-object-group")]
public class Fieldset : PageComponent
{
	protected ErpRequestContext ErpRequestContext { get; set; }

	public Fieldset([FromServices]ErpRequestContext coreReqCtx)
	{
		ErpRequestContext = coreReqCtx;
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
				if (pageFromModel is ErpPage)
				{
					currentPage = (ErpPage)pageFromModel;
				}
				else
				{
					return await Task.FromResult<IViewComponentResult>(Content("Error: PageModel does not have Page property or it is not from ErpPage Type"));
				}

				if (currentPage == null)
				{
					return await Task.FromResult<IViewComponentResult>(Content("Error: The page Id is required to be set as query parameter 'pid', when requesting this component"));
				}

				var options = new PcSection.PcSectionOptions();
				if (context.Options != null)
				{
					options = JsonConvert.DeserializeObject<PcSection.PcSectionOptions>(context.Options.ToString());
				}

				//Check if it is defined in form group
				if (options.LabelMode == WvLabelRenderMode.Undefined)
				{
					if (context.Items.ContainsKey(typeof(WvLabelRenderMode)))
					{
						options.LabelMode = (WvLabelRenderMode)context.Items[typeof(WvLabelRenderMode)];
					}
					else
					{
						options.LabelMode = WvLabelRenderMode.Stacked;
					}
				}

				//Check if it is defined in form group
				if (options.FieldMode == WvFieldRenderMode.Undefined)
				{
					if (context.Items.ContainsKey(typeof(WvFieldRenderMode)))
					{
						options.FieldMode = (WvFieldRenderMode)context.Items[typeof(WvFieldRenderMode)];
					}
					else
					{
						options.FieldMode = WvFieldRenderMode.Form;
					}
				}
				var componentMeta = new PageComponentLibraryService().GetComponentMeta(context.Node.ComponentName);

                //Init IsCollapsed from userPreferences
                if (HttpContext.User != null) {
                    var currentUser = AuthService.GetUser(HttpContext.User);
                    if (currentUser != null) {
                        var componentData = new UserPreferencies().GetComponentData(currentUser.Id, "WebVella.Erp.Web.Components.PcSection");
                        if (componentData != null)
                        {
                            var collapsedNodeIds = new List<Guid>();
                            var uncollapsedNodeIds = new List<Guid>();
                            if (componentData.Properties.ContainsKey("collapsed_node_ids") && componentData["collapsed_node_ids"] != null)
                            {
                                if (componentData["collapsed_node_ids"] is string)
                                {
                                    try
                                    {
                                        collapsedNodeIds = JsonConvert.DeserializeObject<List<Guid>>((string)componentData["collapsed_node_ids"]);
                                    }
                                    catch
                                    {
                                        throw new Exception("WebVella.Erp.Web.Components.PcSection component data object in user preferences not in the correct format. collapsed_node_ids should be List<Guid>");
                                    }
                                }
                                else if (componentData["collapsed_node_ids"] is List<Guid>)
                                {
                                    collapsedNodeIds = (List<Guid>)componentData["collapsed_node_ids"];
                                }
                                else if (componentData["collapsed_node_ids"] is JArray)
                                {
                                    collapsedNodeIds = ((JArray)componentData["collapsed_node_ids"]).ToObject<List<Guid>>();
                                }
                                else
                                {
                                    throw new Exception("Unknown format of collapsed_node_ids");
                                }
                            }
                            if (componentData.Properties.ContainsKey("uncollapsed_node_ids") && componentData["uncollapsed_node_ids"] != null)
                            {
                                if (componentData["uncollapsed_node_ids"] is string)
                                {
                                    try
                                    {
                                        uncollapsedNodeIds = JsonConvert.DeserializeObject<List<Guid>>((string)componentData["uncollapsed_node_ids"]);
                                    }
                                    catch
                                    {
                                        throw new Exception("WebVella.Erp.Web.Components.PcSection component data object in user preferences not in the correct format. uncollapsed_node_ids should be List<Guid>");
                                    }
                                }
                                else if (componentData["uncollapsed_node_ids"] is List<Guid>)
                                {
                                    uncollapsedNodeIds = (List<Guid>)componentData["uncollapsed_node_ids"];
                                }
                                else if (componentData["uncollapsed_node_ids"] is JArray)
                                {
                                    uncollapsedNodeIds = ((JArray)componentData["uncollapsed_node_ids"]).ToObject<List<Guid>>();
                                }
                                else
                                {
                                    throw new Exception("Unknown format of uncollapsed_node_ids");
                                }
                            }
                            if (collapsedNodeIds.Contains(context.Node.Id)) {
                                options.IsCollapsed = true;
                            }
                            else if (uncollapsedNodeIds.Contains(context.Node.Id))
                            {
                                options.IsCollapsed = false;
                            }
                        }
                        
                    }
                }

  
                #endregion


				ViewBag.Options = options;
				ViewBag.Node = context.Node;
				ViewBag.ComponentMeta = componentMeta;
				ViewBag.RequestContext = ErpRequestContext;
				ViewBag.AppContext = ErpAppContext.Current;
				ViewBag.ComponentContext = context;
				ViewBag.GeneralHelpSection = HelpJsApiGeneralSection;

				if (context.Mode == ComponentMode.Display || context.Mode == ComponentMode.Design)
				{
                    var isVisible = true;
                    var isVisibleDS = context.DataModel.GetPropertyValueByDataSource(options.IsVisible);
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
                    ViewBag.IsVisible = isVisible;

                    ViewBag.ProcessedTitle = context.DataModel.GetPropertyValueByDataSource(options.Title);

                    var isCollapsed = context.DataModel.GetPropertyValueByDataSource(options.IsCollapsedDs) as bool?;
                    if (isCollapsed != null)
                    {
                        options.IsCollapsed = isCollapsed.Value;
                        ViewBag.Options = options;
                    }
                    else if (options.IsCollapsedDs.ToLowerInvariant() == "true") {
                        options.IsCollapsed = true;
                        ViewBag.Options = options;
                    }

                }

				context.Items[typeof(WvLabelRenderMode)] = options.LabelMode;
				context.Items[typeof(WvFieldRenderMode)] = options.FieldMode;
				// BlazorPageComponentContext.Initialize(ref context, ErpRequestContext);
				switch (context.Mode)
				{
					case ComponentMode.Display:
						return await Task.FromResult<IViewComponentResult>(View("Display"));
					case ComponentMode.Design:
						return await Task.FromResult<IViewComponentResult>(View("Design"));
					case ComponentMode.Options:
						ViewBag.LabelRenderModeOptions = WebVella.TagHelpers.Utilities.ModelExtensions.GetEnumAsSelectOptions<WvLabelRenderMode>();
						ViewBag.FieldRenderModeOptions = WebVella.TagHelpers.Utilities.ModelExtensions.GetEnumAsSelectOptions<WvFieldRenderMode>();
						return await Task.FromResult<IViewComponentResult>(View("Options"));
					case ComponentMode.Help:
						return await Task.FromResult<IViewComponentResult>(View("Help"));
					default:
						ViewBag.Error = new ValidationException()
						{
							Message = "Unknown component mode"
						};
						return await Task.FromResult<IViewComponentResult>(View("Error"));
				}

			}
			catch (ValidationException ex)
			{
				ViewBag.Error = ex;
				return await Task.FromResult<IViewComponentResult>(View("Error"));
			}
			catch (Exception ex)
			{
				ViewBag.Error = new ValidationException()
				{
					Message = ex.Message
				};
				return await Task.FromResult<IViewComponentResult>(View("Error"));
			}
		}
}