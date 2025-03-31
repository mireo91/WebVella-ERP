using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebVella.Erp.Api;
using WebVella.Erp.Api.Models;
using WebVella.Erp.Exceptions;
using WebVella.Erp.Web;
using WebVella.Erp.Web.Components;
using WebVella.Erp.Web.Models;
using WebVella.Erp.Web.Services;
using WebVella.TagHelpers.Models;

namespace Reomi.Erp.Plugins.Sale.Components;

[PageComponent(Label = "Hierarchical Option List", Library = "Reomi", Description = "Provides hierarchical list", Version = "0.0.1", IconClass = "fas fa-list")]
public class HierarchicalOptionList : PcFieldBase
{
    protected ErpRequestContext ErpRequestContext { get; set; }
    
    public HierarchicalOptionList([FromServices]ErpRequestContext coreReqCtx)
    {
        ErpRequestContext = coreReqCtx;
    }
    
    public class HierarchicalOptionListOptions : PcFieldBaseOptions
    {
	    [JsonProperty(PropertyName = "records")]
	    public string Records { get; set; } = "";
	    
	    [JsonProperty(PropertyName = "options")]
	    public string Options { get; set; } = "";

	    [JsonProperty(PropertyName = "show_icon")]
	    public bool ShowIcon { get; set; } = false;

	    [JsonProperty(PropertyName = "ajax_datasource")]
	    public SelectOptionsAjaxDatasource AjaxDatasource { get; set; } = null;

	    [JsonProperty(PropertyName = "ajax_datasource_api")]
	    public string AjaxDatasourceApi { get; set; } = null;

	    [JsonProperty(PropertyName = "select_match_type")]
	    public WvSelectMatchType SelectMatchingType { get; set; } = WvSelectMatchType.Contains;

	    [JsonProperty(PropertyName = "placeholder")]
	    public string Placeholder { get; set; } = "";
	    
	    public static HierarchicalOptionListOptions CopyFromBaseOptions(PcFieldBaseOptions input)
	    {
		    return new HierarchicalOptionListOptions {
			    IsVisible = input.IsVisible,
			    LabelMode = input.LabelMode,
			    LabelText = input.LabelText,
			    Mode = input.Mode,
			    Name = input.Name,
			    Options = "",
			    AjaxDatasource = null,
			    SelectMatchingType = WvSelectMatchType.Contains,
			    Description = input.Description,
			    LabelHelpText = input.LabelHelpText
		    };
	    }
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

				
				var baseOptions = InitPcFieldBaseOptions(context);
				var options = HierarchicalOptionListOptions.CopyFromBaseOptions(baseOptions);
				if (context.Options != null)
				{
					options = JsonConvert.DeserializeObject<HierarchicalOptionListOptions>(context.Options.ToString());
					if (context.Mode != ComponentMode.Options)
					{
						if (options.MaxLength == null)
							options.MaxLength = baseOptions.MaxLength;

						if(String.IsNullOrWhiteSpace(options.LabelHelpText))
							options.LabelHelpText = baseOptions.LabelHelpText;

						if (String.IsNullOrWhiteSpace(options.Description))
							options.Description = baseOptions.Description;

					}
					// /*
					// * If link is present, evaluate the datasource and find the final link and assign to href
					// * Feature: Linkable Text Field
					// *Author: Amarjeet-L
					// */
					// string link = options.Link;
					// if (link != "")
					// {
					// 	link = context.DataModel.GetPropertyValueByDataSource(options.Link).ToString();
					// 	options.Href = link;
					// }
				}
				var modelFieldLabel = "";
				var model = (PcFieldBaseModel)InitPcFieldBaseModel(context,options, label: out modelFieldLabel);
				if (String.IsNullOrWhiteSpace(options.LabelText) && context.Mode != ComponentMode.Options)
				{
					options.LabelText = modelFieldLabel;
				}
				// if (String.IsNullOrWhiteSpace(options.Placeholder) && context.Mode != ComponentMode.Options) {
				// 	options.Placeholder = model.Placeholder;
				// }

				//Implementing Inherit label mode
				ViewBag.LabelMode = options.LabelMode;
				ViewBag.Mode = options.Mode;

				if (options.LabelMode == WvLabelRenderMode.Undefined && baseOptions.LabelMode != WvLabelRenderMode.Undefined)
					ViewBag.LabelMode = baseOptions.LabelMode;

				if (options.Mode == WvFieldRenderMode.Undefined && baseOptions.Mode != WvFieldRenderMode.Undefined)
					ViewBag.Mode = baseOptions.Mode;


				var componentMeta = new PageComponentLibraryService().GetComponentMeta(context.Node.ComponentName);

				var accessOverride = context.DataModel.GetPropertyValueByDataSource(options.AccessOverrideDs) as WvFieldAccess?;
				if(accessOverride != null){
					model.Access = accessOverride.Value;
				}
				var requiredOverride = context.DataModel.GetPropertyValueByDataSource(options.RequiredOverrideDs) as bool?;
				if(requiredOverride != null){
					model.Required = requiredOverride.Value;
				}
				else{
					if(!String.IsNullOrWhiteSpace(options.RequiredOverrideDs)){
						if(options.RequiredOverrideDs.ToLowerInvariant() == "true"){
							model.Required = true;
						}
						else if(options.RequiredOverrideDs.ToLowerInvariant() == "false"){
							model.Required = false;
						}
					}
				}

				if (context.Mode != ComponentMode.Options && context.Mode != ComponentMode.Help)
				{
										#region << Init DataSources >>

					dynamic valueResult = context.DataModel.GetPropertyValueByDataSource(options.Value);
					if (valueResult == null)
					{
						model.Value = new List<string>();
					}
					else if (valueResult is List<string>)
					{
						model.Value = (List<string>)valueResult;
					}
					else if (valueResult is string)
					{
						var stringProcessed = false;
						if (String.IsNullOrWhiteSpace(valueResult))
						{
							model.Value = new List<string>();
							stringProcessed = true;
						}
						if (!stringProcessed && (((string)valueResult).StartsWith("{") || ((string)valueResult).StartsWith("[")))
						{
							try
							{
								model.Value = JsonConvert.DeserializeObject<List<string>>(valueResult.ToString());
								stringProcessed = true;
							}
							catch
							{
								stringProcessed = false;
								ViewBag.ExceptionMessage = "Value Json Deserialization failed!";
								ViewBag.Errors = new List<ValidationError>();
								return await Task.FromResult<IViewComponentResult>(View("Error"));
							}
						}
						if (!stringProcessed && !((string)valueResult).Contains("{") && !((string)valueResult).Contains("["))
						{
							var valueArray = ((string)valueResult).Split(',',StringSplitOptions.RemoveEmptyEntries);
							model.Value = new List<string>(valueArray);
							stringProcessed = true;
						}
						if(!stringProcessed && ((string)valueResult).StartsWith("[") && ((string)valueResult).EndsWith("]")){
							model.Value = JsonConvert.DeserializeObject<List<string>>((string)valueResult);
						}
					}
					else if (valueResult is List<Guid>)
					{
						model.Value = ((List<Guid>)valueResult).Select(x => x.ToString()).ToList();
					}
					else if (valueResult is Guid)
					{
						model.Value = new List<string>(valueResult.ToString());
					}
					else if (valueResult is List<EntityRecord>)
					{
						if (((List<EntityRecord>)valueResult).Count > 0)
						{
							if(!((List<EntityRecord>)valueResult)[0].Properties.ContainsKey("id"))
								throw new Exception("The provided list of entity records does not contain an 'id' property");
						}
						model.Value = ((List<EntityRecord>)valueResult).Select(x => ((Guid)x["id"]).ToString()).ToList();
					}

					var dataSourceOptions = new List<DataSource.HierarchicalOptionList.HierarchicalListOption>();
					dynamic optionsResult = context.DataModel.GetPropertyValueByDataSource(options.Options);
					if (optionsResult == null) { }
					if (optionsResult is List<DataSource.HierarchicalOptionList.HierarchicalListOption>)
					{
						dataSourceOptions = (List<DataSource.HierarchicalOptionList.HierarchicalListOption>)optionsResult;
					}
					else if (optionsResult is string)
					{
						var stringProcessed = false;
						if (String.IsNullOrWhiteSpace(optionsResult))
						{
							dataSourceOptions = new List<DataSource.HierarchicalOptionList.HierarchicalListOption>();
							stringProcessed = true;
						}
						//AJAX Options
						if (!stringProcessed && ((string)optionsResult).StartsWith("{"))
						{
							try
							{
								// options.AjaxDatasource = JsonConvert.DeserializeObject<SelectOptionsAjaxDatasource>(optionsResult, new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Error });
								stringProcessed = true;
								ViewBag.Options = options;
							}
							catch
							{

							}
						}
						if (!stringProcessed && (((string)optionsResult).StartsWith("{") || ((string)optionsResult).StartsWith("[")))
						{
							try
							{
								dataSourceOptions = JsonConvert.DeserializeObject<List<DataSource.HierarchicalOptionList.HierarchicalListOption>>(optionsResult);
								stringProcessed = true;
							}
							catch
							{
								stringProcessed = false;
								ViewBag.ExceptionMessage = "Options Json Deserialization failed!";
								ViewBag.Errors = new List<ValidationError>();
								return await Task.FromResult<IViewComponentResult>(View("Error"));
							}
						}
						if (!stringProcessed && ((string)optionsResult).Contains(",") && !((string)optionsResult).Contains("{") && !((string)optionsResult).Contains("["))
						{
							var optionsArray = ((string)optionsResult).Split(',');
							var optionsList = new List<DataSource.HierarchicalOptionList.HierarchicalListOption>();
							// foreach (var optionString in optionsArray)
							// {
							// 	optionsList.Add(new DataSource.HierarchicalOptionList.HierarchicalListOption(optionString, optionString));
							// }
							dataSourceOptions = optionsList;
						}
					}
					if (dataSourceOptions.Count > 0)
					{
						options.Options = JsonConvert.SerializeObject(dataSourceOptions);
					}
					#endregion

				}

				#endregion

			ViewBag.Options = options;
			ViewBag.Model = model;
			ViewBag.Node = context.Node;
			ViewBag.ComponentMeta = componentMeta;
			ViewBag.RequestContext = ErpRequestContext;
			ViewBag.AppContext = ErpAppContext.Current;

			if (context.Mode != ComponentMode.Options && context.Mode != ComponentMode.Help)
			{
				model.Value = context.DataModel.GetPropertyValueByDataSource(options.Value);

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
			}
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