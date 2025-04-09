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
    public PcFieldText.PcFieldTextOptions FieldOptions { get; set; }
    // [Parameter] public PageBodyNode Node { get; set; }
    
    private bool _isRequired = false;
    private bool _isVisible = true;
    string _fieldValue = String.Empty;
    RadzenTemplateForm<string> _form;
    private Entity? _entity = null;
    private EntityRecord? _record = null;
    object? _inlineEditablePreviousValue = null;
    private bool _inlineEditable = false;

    protected override void OnInitialized()
    {
	    base.OnInitialized();
	    // InitializeFieldOptions();
	    // _fieldValue = FieldOptions.Value;
	    // if (!BlazorPageComponentContext.ModelForm.ContainsKey("FirstName"))
	    // {
		   //  BlazorPageComponentContext.ModelForm.Add("FirstName", _fieldValue);
		   //  BlazorPageComponentContext.ModelForm.Add("LastName", _fieldValue);
	    // }

	    // FieldOptions.ConnectedEntityId ??= BlazorPageComponentContext.ErpRequestContext.RecordId;
	    // if (_entity != null)
	    // {
		   //  if (FieldOptions.Name != null) _isRequired = _entity.Fields.Any(c => c.Name == FieldOptions.Name && c.Required);
	    // }
	    // if (FieldOptions.ConnectedEntityId != null)
	    // {
	    //     
		   //  var response = (new EntityManager()).ReadEntity((Guid)FieldOptions.ConnectedEntityId!);
		   //  if( response.Success )
		   //  {
			  //   _entity = response.Object;
			  //   var field = _entity.Fields.Find(f => f.Name == FieldOptions.Name);
     //            
			  //   //@todo dynamicznie w zależności od strony trzeba przypisać tą wartość
			  //   var recordId = "f4d87b41-1fe1-48fe-b091-b2c7e566a8ee";
			  //   //@endtodo
     //            
			  //   _record = new EqlCommand($"SELECT id, {FieldOptions.Name} FROM {_entity.Name} WHERE id = @id", new EqlParameter("id", recordId)).Execute().FirstOrDefault();
			  //   if(_record!=null)
				 //    fieldValue = _record![FieldOptions.Name]!=null?_record[FieldOptions.Name].ToString()!:"";
			  //   if (FieldOptions.Name != null) _isRequired = _entity.Fields.Any(c => c.Name == FieldOptions.Name && c.Required);
		   //  }
	    // }
    }

    async Task EditRow()
    {
        _inlineEditablePreviousValue = _fieldValue;
        _inlineEditable = true;
    }
    async Task SaveRow()
    {
        _inlineEditable = false;
        await _form.Submit.InvokeAsync(_fieldValue);
    }

    void CancelEdit()
    {
        _inlineEditable = false;
        _fieldValue = _inlineEditablePreviousValue.ToString();
    }
    
    void OnSubmit(object value)
    {
        if (_entity == null) return;
        if (_record == null) return;
        _record[FieldOptions.Name] = value;
        var response = (new RecordManager()).UpdateRecord(_entity?.Name, _record);
        var message = new NotificationMessage
        {
            Severity = NotificationSeverity.Success, Summary = "Success Summary", Detail = response.Message,
            Duration = 4000
        };
        if (!response.Success)
        {
            message = new NotificationMessage
            {
                Severity = NotificationSeverity.Error, Summary = "Error Summary", Detail = response.Message,
                Duration = 4000
            };
        }
        
        NotificationService.Notify(message);
    }
    protected override void InitializeFieldOptions()
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
  
		FieldOptions = options;
  
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
			_isVisible = isVisible;
		}
  
		_entity = BlazorPageComponentContext.ErpRequestContext.Entity;
    }
}