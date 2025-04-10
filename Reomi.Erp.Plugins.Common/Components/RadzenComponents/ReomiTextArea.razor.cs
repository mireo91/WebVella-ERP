using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebVella.Erp.Web;
using WebVella.Erp.Web.Components;
using WebVella.Erp.Web.Models;
using WebVella.Erp.Web.Services;
using WebVella.TagHelpers.Models;

namespace Reomi.Erp.Plugins.Common.Components.RadzenComponents;

public partial class ReomiTextArea : ReomiFieldComponentBase<WebVella.Erp.Web.Components.PcFieldTextarea.PcFieldTextareaOptions>
{
    protected override PcFieldTextarea.PcFieldTextareaOptions InitializeFieldOptions()
    {
	    var context = BlazorPageComponentContext.PageComponentContext;
	    var pcField = new PcFieldTextarea(BlazorPageComponentContext.ErpRequestContext);

        #region << Init >>
		
		var baseOptions = pcField.InitPcFieldBaseOptions(context);
		var options = PcFieldTextarea.PcFieldTextareaOptions.CopyFromBaseOptions(baseOptions);
		if (Context.Node.Options != null)
		{
			options = JsonConvert.DeserializeObject<PcFieldTextarea.PcFieldTextareaOptions>(Context.Node.Options.ToString());
			if (context.Mode != ComponentMode.Options)
			{
				if (String.IsNullOrWhiteSpace(options.LabelHelpText))
					options.LabelHelpText = baseOptions.LabelHelpText;

				if (String.IsNullOrWhiteSpace(options.Description))
					options.Description = baseOptions.Description;

			}
		}
		var modelFieldLabel = "";
		var model = (PcFieldBase.PcFieldBaseModel)pcField.InitPcFieldBaseModel(context, options, label: out modelFieldLabel);
		if (String.IsNullOrWhiteSpace(options.LabelText) && context.Mode != ComponentMode.Options)
		{
			options.LabelText = modelFieldLabel;
		}

		// ViewBag.LabelMode = options.LabelMode;
		// ViewBag.Mode = options.Mode;

		if (options.LabelMode == WvLabelRenderMode.Undefined && baseOptions.LabelMode != WvLabelRenderMode.Undefined)
			options.LabelMode = baseOptions.LabelMode;

		if (options.Mode == WvFieldRenderMode.Undefined && baseOptions.Mode != WvFieldRenderMode.Undefined)
			options.Mode = baseOptions.Mode;

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

			// model.Value = context.DataModel.GetPropertyValueByDataSource(options.Value);
		}
		Console.WriteLine();
		return options;
    }
}