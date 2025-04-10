using CsvHelper.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Newtonsoft.Json;
using Radzen;
using Radzen.Blazor;
using Reomi.Erp.Plugins.Common.Hooks;
using WebVella.Erp.Api;
using WebVella.Erp.Api.Models;
using WebVella.Erp.Eql;
using WebVella.Erp.Web;
using WebVella.Erp.Web.Components;
using WebVella.Erp.Web.Models;
using WebVella.Erp.Web.Services;
using WebVella.TagHelpers.Models;
using RecordManager = WebVella.Erp.Api.RecordManager;

namespace Reomi.Erp.Plugins.Common.Components.RadzenComponents;

public partial class ReomiGuidRelationDataGrid : ReomiFieldComponentBase<PcFieldSelect.PcFieldSelectOptions>
{
    [Parameter]
    public List<SelectOption> Options { get; set; } = new List<SelectOption>();

    IEnumerable<SelectOption> options;

    // protected override void InitializeFieldOptions()
    // {
	   //  throw new NotImplementedException();
    // }

    void LoadData(LoadDataArgs args)
    {
        var query = Options;
        
        if (!string.IsNullOrEmpty(args.Filter))
        {
            query = query.Where(c => c.Label.ToLower().Contains(args.Filter.ToLower())).ToList();
        }
        
        options = query.ToList();
        InvokeAsync(StateHasChanged);
    }
    
    protected override PcFieldSelect.PcFieldSelectOptions InitializeFieldOptions()
    {
	    var context = BlazorPageComponentContext.PageComponentContext;
	    var pcFieldSelect = new PcFieldSelect(BlazorPageComponentContext.ErpRequestContext);
	    // try
        // {
	        #region << Init >>
	        // new PcField
	        var baseOptions = pcFieldSelect.InitPcFieldBaseOptions(context);
	        var options = PcFieldSelect.PcFieldSelectOptions.CopyFromBaseOptions(baseOptions);
	        if (Context.Node.Options != null)
	        {
		        options = JsonConvert.DeserializeObject<PcFieldSelect.PcFieldSelectOptions>(Context.Node.Options.ToString());
		        if (context.Mode != ComponentMode.Options)
		        {
			        if (String.IsNullOrWhiteSpace(options.LabelHelpText))
				        options.LabelHelpText = baseOptions.LabelHelpText;
    
			        if (String.IsNullOrWhiteSpace(options.Description))
				        options.Description = baseOptions.Description;
    
		        }
    
		        ////Check for connection to entity field
		        //if (instanceOptions.TryConnectToEntity)
		        //{
		        //	var entity = context.DataModel.GetProperty("Entity");
		        //	if (entity != null && entity is Entity)
		        //	{
		        //		var fieldName = instanceOptions.Name;
		        //		var entityField = ((Entity)entity).Fields.FirstOrDefault(x => x.Name == fieldName);
		        //		if (entityField != null && entityField is PhoneField)
		        //		{
		        //			var castedEntityField = ((PhoneField)entityField);
		        //			//No options connected
		        //		}
		        //	}
		        //}
		        /*
		         * If link is present, evaluate the datasource and find the final link and assign to href
		         * Feature: Linkable Text Field
		         *Author: Amarjeet-L
		         */
		        string link = options.Link;
		        if (link != "")
		        {
			        link = context.DataModel.GetPropertyValueByDataSource(options.Link).ToString();
			        options.Href = link;
		        }
	        }
		  
	        var modelFieldLabel = "";
	        var model = (PcFieldBase.PcFieldSelectModel)pcFieldSelect.InitPcFieldBaseModel(context, options, label: out modelFieldLabel,
		        targetModel: "PcFieldSelectModel");
	        if (String.IsNullOrWhiteSpace(options.LabelText) && context.Mode != ComponentMode.Options)
	        {
		        options.LabelText = modelFieldLabel;
	        }
	        //PcFieldSelectModel model = PcFieldSelectModel.CopyFromBaseModel(baseModel);
    
	        //Implementing Inherit label mode
	        // LabelMode = options.LabelMode;
	        // Mode = options.Mode;
    
	        if (options.LabelMode == WvLabelRenderMode.Undefined &&
	            baseOptions.LabelMode != WvLabelRenderMode.Undefined)
		        options.LabelMode = baseOptions.LabelMode;
    
	        if (options.Mode == WvFieldRenderMode.Undefined && baseOptions.Mode != WvFieldRenderMode.Undefined)
		        options.Mode = baseOptions.Mode;
    
	        var accessOverride =
		        context.DataModel.GetPropertyValueByDataSource(options.AccessOverrideDs) as WvFieldAccess?;
	        if (accessOverride != null)
	        {
		        model.Access = accessOverride.Value;
	        }
    
	        var requiredOverride = context.DataModel.GetPropertyValueByDataSource(options.RequiredOverrideDs) as bool?;
	        if (requiredOverride != null)
	        {
		        model.Required = requiredOverride.Value;
	        }
	        else
	        {
		        if (!String.IsNullOrWhiteSpace(options.RequiredOverrideDs))
		        {
			        if (options.RequiredOverrideDs.ToLowerInvariant() == "true")
			        {
				        model.Required = true;
			        }
			        else if (options.RequiredOverrideDs.ToLowerInvariant() == "false")
			        {
				        model.Required = false;
			        }
		        }
	        }
    
	        #endregion
    
	        // FieldOptions = options;
	        // Model = model;
    
	        if (context.Mode != ComponentMode.Options && context.Mode != ComponentMode.Help)
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
		        IsVisible = isVisible;
		        #region << Init DataSources >>
    
		        model.Value = context.DataModel.GetPropertyValueByDataSource(options.Value);
    
		        dynamic optionsResult = context.DataModel.GetPropertyValueByDataSource(options.Options);
		        var dataSourceOptions = new List<SelectOption>();
		        if (optionsResult == null)
		        {
		        }
    
		        if (optionsResult is List<SelectOption>)
		        {
			        dataSourceOptions = (List<SelectOption>)optionsResult;
		        }
    
		        if (optionsResult is List<WvSelectOption>)
		        {
			        foreach (var option in (List<WvSelectOption>)optionsResult)
			        {
				        dataSourceOptions.Add(new SelectOption
				        {
					        Color = option.Color,
					        IconClass = option.IconClass,
					        Label = option.Label,
					        Value = option.Value
				        });
			        }
		        }
		        else if (optionsResult is string)
		        {
			        var stringProcessed = false;
			        if (String.IsNullOrWhiteSpace(optionsResult))
			        {
				        dataSourceOptions = new List<SelectOption>();
				        stringProcessed = true;
			        }
    
			        //AJAX Options
			        if (!stringProcessed && ((string)optionsResult).StartsWith("{"))
			        {
				        try
				        {
					        options.AjaxDatasource = JsonConvert.DeserializeObject<SelectOptionsAjaxDatasource>(
						        optionsResult,
						        new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Error });
					        stringProcessed = true;
					        FieldOptions = options;
				        }
				        catch
				        {
    
				        }
			        }
    
			        if (!stringProcessed && (((string)optionsResult).StartsWith("{") ||
			                                 ((string)optionsResult).StartsWith("[")))
			        {
				        try
				        {
					        dataSourceOptions = JsonConvert.DeserializeObject<List<SelectOption>>(optionsResult);
					        stringProcessed = true;
				        }
				        catch
				        {
					        stringProcessed = false;
					        throw new Exception("Error: Options Json De-serialization failed!");
					        // return await Task.FromResult<IViewComponentResult>(
					        //  Content("Error: Options Json De-serialization failed!"));
				        }
			        }
    
			        if (!stringProcessed && ((string)optionsResult).Contains(",") &&
			            !((string)optionsResult).Contains("{") && !((string)optionsResult).Contains("["))
			        {
				        var optionsArray = ((string)optionsResult).Split(',');
				        var optionsList = new List<SelectOption>();
				        foreach (var optionString in optionsArray)
				        {
					        optionsList.Add(new SelectOption(optionString, optionString));
				        }
    
				        dataSourceOptions = optionsList;
			        }
		        }
    
		        if (dataSourceOptions.Count > 0)
		        {
			        Options = dataSourceOptions;
		        }
    
		        #endregion
    
	        }
	        // FieldOptions = options;
	        // Options = model.Options;
        // }
        // catch
        // {
	       //  // ignored
        // }
        // AfterOnInitialized();
        return options;
    }
}