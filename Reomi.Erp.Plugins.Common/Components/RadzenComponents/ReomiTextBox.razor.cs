using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Radzen;
using Radzen.Blazor;
using Reomi.Erp.Plugins.Common.Hooks;
using WebVella.Erp.Api;
using WebVella.Erp.Api.Models;
using WebVella.Erp.Web;
using WebVella.Erp.Web.Components;
using WebVella.Erp.Web.Models;
using WebVella.Erp.Web.Services;
using WebVella.TagHelpers.Models;

namespace Reomi.Erp.Plugins.Common.Components.RadzenComponents;

public partial class ReomiTextBox : ReomiFieldComponentBase<PcFieldText.PcFieldTextOptions>
{
    
    private bool _isRequired = false;
    // private bool _isVisible = true;

    protected override PcFieldText.PcFieldTextOptions InitializeFieldOptions()
    {
	    var context = BlazorPageComponentContext.PageComponentContext;
	    var pcFieldText = new PcFieldText(BlazorPageComponentContext.ErpRequestContext);
        #region << Init >>
		
		var baseOptions = pcFieldText.InitPcFieldBaseOptions(context);
		var options = PcFieldText.PcFieldTextOptions.CopyFromBaseOptions(baseOptions);
		if (context.Options != null)
		{
			options = JsonConvert.DeserializeObject<PcFieldText.PcFieldTextOptions>(Context.Node.Options);
			if (context.Mode != ComponentMode.Options)
			{
				if (options.MaxLength == null)
					options.MaxLength = baseOptions.MaxLength;
  
				if(String.IsNullOrWhiteSpace(options.LabelHelpText))
					options.LabelHelpText = baseOptions.LabelHelpText;
  
				if (String.IsNullOrWhiteSpace(options.Description))
					options.Description = baseOptions.Description;
  
			}
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
		var model = (PcFieldBase.PcFieldBaseModel)pcFieldText.InitPcFieldBaseModel(context,options, label: out modelFieldLabel);
		if (String.IsNullOrWhiteSpace(options.LabelText) && context.Mode != ComponentMode.Options)
		{
			options.LabelText = modelFieldLabel;
		}
		if (String.IsNullOrWhiteSpace(options.Placeholder) && context.Mode != ComponentMode.Options) {
			options.Placeholder = model.Placeholder;
		}
  
		//Implementing Inherit label mode
		if (options.LabelMode == WvLabelRenderMode.Undefined &&
		    baseOptions.LabelMode != WvLabelRenderMode.Undefined)
			options.LabelMode = baseOptions.LabelMode;
  
		if (options.Mode == WvFieldRenderMode.Undefined && baseOptions.Mode != WvFieldRenderMode.Undefined)
			options.Mode = baseOptions.Mode;
  
  
		// var componentMeta = new PageComponentLibraryService().GetComponentMeta(context.Node.ComponentName);
  
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
  
		#endregion
  
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
			IsVisible = isVisible;
		}

		return options;
    }
}