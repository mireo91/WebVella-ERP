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

public partial class ReomiDropDown : ComponentBase
{
    [Parameter]
    public string Value { get; set; } = string.Empty;
    [Parameter]
    public List<SelectOption> Options { get; set; } = new List<SelectOption>();
    [Parameter]
    public PcFieldSelect.PcFieldSelectOptions FieldOptions { get; set; }

    [Parameter] public PageBodyNode Node { get; set; }
    
    private bool _isRequired = false;
    
    private bool _isVisible = true;
    
    RadzenTemplateForm<string> _form;
    // [Parameter] public bool IsExpanded { get; set; } = true;
    // [Parameter] public bool IsEditable { get; set; } = true;
    // [Parameter] public string? CustomEqlCommand { get; set; }
    IEnumerable<SelectOption> options;
    string fieldValue = String.Empty;

    private Entity? _entity = null;
    private EntityRecord? _record = null;
    protected override void OnInitialized()
    {
        base.OnInitialized();
        InitializeFieldOptions();
        fieldValue = FieldOptions.Value;
        if (BlazorPageComponentContext.ErpRequestContext.Entity != null)
        {
	        if (FieldOptions.Name != null) _isRequired = BlazorPageComponentContext.ErpRequestContext.Entity.Fields.Any(c => c.Name == FieldOptions.Name && c.Required);
        }
        if (FieldOptions.ConnectedEntityId != null)
        {
	        
            var response = (new EntityManager()).ReadEntity((Guid)FieldOptions.ConnectedEntityId!);
            if( response.Success )
            {
                _entity = response.Object;
                var field = _entity.Fields.Find(f => f.Name == FieldOptions.Name);
                
                //@todo dynamicznie w zależności od strony trzeba przypisać tą wartość
                var recordId = "f4d87b41-1fe1-48fe-b091-b2c7e566a8ee";
                //@endtodo
                
                _record = new EqlCommand($"SELECT id, {FieldOptions.Name} FROM {_entity.Name} WHERE id = @id", new EqlParameter("id", recordId)).Execute().FirstOrDefault();
                if(_record!=null)
                    fieldValue = _record![FieldOptions.Name]!=null?_record[FieldOptions.Name].ToString()!:"";
                if (FieldOptions.Name != null) _isRequired = _entity.Fields.Any(c => c.Name == FieldOptions.Name && c.Required);
            }
        }
        // FieldOptions.
        // FieldOptions.ConnectedEntityId
        // Console.WriteLine(Model.AjaxDatasourceApi);
        // Console.WriteLine(Model.AjaxDatasourceApi);
        // Console.WriteLine(Model.AjaxApiUrlDs);
    }
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
    object? _inlineEditablePreviousValue = null;

    private bool _inlineEditable = false;
    async Task EditRow()
    {
        _inlineEditablePreviousValue = fieldValue;
        _inlineEditable = true;
    }
    async Task SaveRow()
    {
        _inlineEditable = false;
        await _form.Submit.InvokeAsync(fieldValue);
    }

    void CancelEdit()
    {
        _inlineEditable = false;
        fieldValue = _inlineEditablePreviousValue.ToString();
    }
    
    private void InitializeFieldOptions()
    {
	    var context = BlazorPageComponentContext.CurrentPageContext;
	    var pcFieldSelect = new PcFieldSelect(BlazorPageComponentContext.ErpRequestContext);
	    // try
        // {
	        #region << Init >>
	        // new PcField
	        var baseOptions = pcFieldSelect.InitPcFieldBaseOptions(context);
	        var options = PcFieldSelect.PcFieldSelectOptions.CopyFromBaseOptions(baseOptions);
	        if (Node.Options != null)
	        {
		        options = JsonConvert.DeserializeObject<PcFieldSelect.PcFieldSelectOptions>(Node.Options.ToString());
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
		        _isVisible = isVisible;
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
	        FieldOptions = options;
	        // Options = model.Options;
        // }
        // catch
        // {
	       //  // ignored
        // }
    }
}