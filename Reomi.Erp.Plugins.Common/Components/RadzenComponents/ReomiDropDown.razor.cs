using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis.Options;
using Radzen;
using WebVella.Erp.Api.Models;
using WebVella.Erp.Web.Components;

namespace Reomi.Erp.Plugins.Common.Components.RadzenComponents;

public partial class ReomiDropDown : ComponentBase
{
    [Parameter]
    public string Value { get; set; } = string.Empty;
    [Parameter]
    public List<SelectOption> Options { get; set; } = new List<SelectOption>();
    [Parameter]
    public PcFieldSelect.PcFieldSelectOptions FieldOptions { get; set; }

    // [Parameter] public bool IsExpanded { get; set; } = true;
    // [Parameter] public bool IsEditable { get; set; } = true;
    // [Parameter] public string? CustomEqlCommand { get; set; }
    IEnumerable<SelectOption> options;

    protected override void OnInitialized()
    {
        base.OnInitialized();
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
}