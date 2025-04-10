using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebVella.Erp.Web;
using WebVella.Erp.Web.Components;
using WebVella.Erp.Web.Models;
using WebVella.Erp.Web.Services;
using WebVella.TagHelpers.Models;

namespace Reomi.Erp.Plugins.Common.Components.RadzenComponents;

public partial class ReomiHiddenField : ReomiFieldComponentBase<PcFieldHidden.PcFieldHiddenOptions>
{
    protected override PcFieldHidden.PcFieldHiddenOptions InitializeFieldOptions()
    {
	    #region << Init >>
	    var context = BlazorPageComponentContext.PageComponentContext;
	    var pcField = new PcFieldHidden(BlazorPageComponentContext.ErpRequestContext);
        var baseOptions = pcField.InitPcFieldBaseOptions(context);
		var options = PcFieldHidden.PcFieldHiddenOptions.CopyFromBaseOptions(baseOptions);
		if (Context.Node.Options != null)
		{
			options = JsonConvert.DeserializeObject<PcFieldHidden.PcFieldHiddenOptions>(Context.Node.Options.ToString());
		}
		var modelFieldLabel = "";
		var model = (PcFieldBase.PcFieldBaseModel)pcField.InitPcFieldBaseModel(context, options, label: out modelFieldLabel);
		if (String.IsNullOrWhiteSpace(options.LabelText))
		{
			options.LabelText = modelFieldLabel;
		}

		if (options.LabelMode == WvLabelRenderMode.Undefined && baseOptions.LabelMode != WvLabelRenderMode.Undefined)
			options.LabelMode = baseOptions.LabelMode;

		if (options.Mode == WvFieldRenderMode.Undefined && baseOptions.Mode != WvFieldRenderMode.Undefined)
			options.Mode = baseOptions.Mode;


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

			// options.Value = context.DataModel.GetPropertyValueByDataSource(options.Value);
		}

		return options;
    }
}