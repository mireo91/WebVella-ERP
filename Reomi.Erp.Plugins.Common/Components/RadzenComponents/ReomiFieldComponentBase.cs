using System.Reflection;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Radzen;
using Radzen.Blazor;
using WebVella.Erp.Api;
using WebVella.Erp.Api.Models;
using WebVella.Erp.Eql;
using WebVella.Erp.Utilities.Dynamic;
using WebVella.Erp.Web.Components;
using WebVella.Erp.Web.Models;
using WebVella.Erp.Web.Services;
using WebVella.TagHelpers.Models;

namespace Reomi.Erp.Plugins.Common.Components.RadzenComponents;

public abstract class ReomiFieldComponentBase<TOptions> : ComponentBase
{
    [Inject] NotificationService NotificationService { get; set; }
    [Parameter] public TOptions? FieldOptions { get; set; }
    [Parameter] public required BlazorPageComponentContext Context { get; set; }
    
    protected bool IsVisible = true;
    protected bool IsRequired = false;
    protected RadzenTemplateForm<BlazorForm>? Form;
    protected Entity? _entity = null;
    protected EntityRecord? _record = null;

    protected string? FieldName;
    protected string? FieldValue;
    private bool _isFirstTime = true;
    
    protected override Task OnInitializedAsync()
    {
        if(!_isFirstTime) return base.OnInitializedAsync();
        _isFirstTime = false;
        if(FieldOptions == null)
            FieldOptions = InitializeFieldOptions();

        // FieldName = Context.Node.Id.ToString();
        
        if (Context?.FormData == null)
        {
            Context.InitializeBlazorForm(new BlazorForm(Context.Node.Id.ToString()));
        }

        var type = typeof(TOptions);
        var prop = type.GetProperty("Name");
        if (prop != null)
        {
            FieldName = (string)prop.GetValue(FieldOptions)!;
            FieldValue = (string)type.GetProperty("Value")?.GetValue(FieldOptions)!;
        }

        if (this is not ReomiCollectionComponentBase<TOptions>)
        {
            AfterOnInitialized();
            Context.FormData!.Add(FieldName, FieldValue);
        }

        return base.OnInitializedAsync();
    }
    
    protected virtual void AfterOnInitialized()
    {
        _entity = BlazorPageComponentContext.ErpRequestContext.Entity;
        if (_entity != null)
        {
            IsRequired = BlazorPageComponentContext.ErpRequestContext.Entity.Fields.Any(c => c.Name == FieldName && c.Required);
        }
        
        var recordId = BlazorPageComponentContext.ErpRequestContext.RecordId;
        var type = typeof(TOptions);
        var prop = type.GetProperty("ConnectedEntityId");
        if (prop != null && recordId != null)
        {
            var connectedEntityId = (Guid?)prop.GetValue(FieldOptions);
            if (connectedEntityId != null)
            {
                var response = (new EntityManager()).ReadEntity((Guid)connectedEntityId!);
                if( response.Success )
                {
                    _entity = response.Object;
                }
            }
        }

        if (_entity != null && recordId != null)
        {
            _record = new EqlCommand($"SELECT id, {FieldName} FROM {_entity.Name} WHERE id = @id", new EqlParameter("id", recordId.ToString())).Execute().FirstOrDefault();
            if (_record != null)
            {
                FieldValue = _record![FieldName] != null
                    ? _record[FieldName].ToString()!
                    : "";
            }

            IsRequired = _entity.Fields.Any(c => c.Name == FieldName && c.Required);
        }

        if (IsRequired)
        {
            prop = type.GetProperty("LabelText");
            if (prop != null)
            {
                prop.SetValue(FieldOptions, $"{prop.GetValue(FieldOptions)}{"*"}");
            }
        }
    }

    // protected override void OnInitialized()
    // {
    //     base.OnInitialized();
    //     Console.WriteLine("ReomiFieldComponentBase.OnInitialized");
    //     if(FieldOptions == null && Context != null)
    //         InitializeFieldOptions();
    // }

    protected abstract TOptions InitializeFieldOptions();
    
    protected void OnSubmit(BlazorForm formData)
    {
        if (_entity == null) return;
        if (_record == null) return;
        
        _record[FieldName] = formData[FieldName];
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
    protected bool IsInlineEditable = false;
    protected Task EditRow()
    {
        IsInlineEditable = true;
        return Task.FromResult(Task.CompletedTask);
    }
    protected async Task SaveRow()
    {
        IsInlineEditable = false;
        await Form.Submit.InvokeAsync(Context.FormData);
    }

    protected void CancelEdit()
    {
        IsInlineEditable = false;
        Context.FormData![FieldName] = FieldValue;
    }
}