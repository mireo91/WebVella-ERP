using CsvHelper.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis.Options;
using Radzen;
using Radzen.Blazor;
using WebVella.Erp.Api;
using WebVella.Erp.Api.Models;
using WebVella.Erp.Eql;
using WebVella.Erp.Web.Components;
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

    private bool _isRequired = false;
    
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
        fieldValue = FieldOptions.Value;
        if (FieldOptions.ConnectedEntityId != null)
        {
            var response = (new EntityManager()).ReadEntity((Guid)FieldOptions.ConnectedEntityId!);
            if( response.Success )
            {
                _entity = response.Object;
                var recordId = "f4d87b41-1fe1-48fe-b091-b2c7e566a8ee";
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
}