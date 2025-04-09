using System.Reflection;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using WebVella.Erp.Web.Components;
using WebVella.Erp.Web.Models;
using WebVella.Erp.Web.Services;
using WebVella.TagHelpers.Models;

namespace Reomi.Erp.Plugins.Common.Components.RadzenComponents;

public abstract class ReomiFieldComponentBase<TOptions> : ComponentBase
{
    [Parameter] public TOptions? FieldOptions { get; set; }
    [Parameter] public required BlazorPageComponentContext Context { get; set; }
    protected bool IsVisible = true;

    protected override Task OnInitializedAsync()
    {
        if(FieldOptions == null && Context != null)
            InitializeFieldOptions();
        return base.OnInitializedAsync();
    }

    // protected override void OnInitialized()
    // {
    //     base.OnInitialized();
    //     Console.WriteLine("ReomiFieldComponentBase.OnInitialized");
    //     if(FieldOptions == null && Context != null)
    //         InitializeFieldOptions();
    // }

    protected abstract void InitializeFieldOptions();
}