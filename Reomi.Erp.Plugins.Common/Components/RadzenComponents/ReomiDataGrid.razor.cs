using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Components;
using Wangkanai.Extensions;
using WebVella.Erp.Eql;

namespace Reomi.Erp.Plugins.Common.Components.RadzenComponents;

public partial class ReomiDataGrid : ComponentBase
{

    [Parameter]
    public string? HierarchicalPropertyKey { get; set; }

    [Parameter] public bool IsExpanded { get; set; } = true;
    [Parameter] public bool IsEditable { get; set; } = true;
    [Parameter] public string? CustomEqlCommand { get; set; }
    
    
    
    protected ObservableCollection<IDictionary<string, object>> Data { get; set; }

    protected IDictionary<string, Type> Columns { get; set; }
    protected string EntityRecordName { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        Columns = new Dictionary<string, Type>()
        {
            // { "parentcategoryid", typeof(Guid?) },
            // { "title", typeof(string) },
            // { "FirstName", typeof(string) },
            // { "LastName", typeof(string) },
            // { "HireDate", typeof(DateTime?) },
            // { "DateOnly", typeof(DateOnly?) },
            // { "TimeOnly", typeof(TimeOnly?) },
            // { "UID", typeof(Guid?) },
        };
        var entityRecordList = new EqlCommand(CustomEqlCommand).Execute();
        EntityRecordName = entityRecordList.EntityRecordName;
        Data = new ObservableCollection<IDictionary<string, object>>(entityRecordList.Select(d => d.Properties).ToList());
        if(!Data.IsNullOrEmpty())
            foreach (var i in Data.First())
            {
                if(!Columns.ContainsKey(i.Key) && i.Key != HierarchicalPropertyKey)
                    Columns.Add(i.Key, i.Value.GetType());
            }
        gridEntities = Data.Where(e => e[HierarchicalPropertyKey] == null);
        // data = new EqlCommand("SELECT * FROM cdm_common_category").Execute().Select(e => e.Properties).ToList();
    }
}