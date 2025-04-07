using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Reomi.Erp.Plugins.Common.Hooks;
using WebVella.Erp.Web;
using WebVella.Erp.Web.Components;
using WebVella.Erp.Web.Models;
using WebVella.Erp.Web.Services;
using WebVella.TagHelpers.Models;
using static System.Net.Http.HttpMethod;

namespace Reomi.Erp.Plugins.Common.Components.RadzenComponents;

public partial class ReomiFieldset : ComponentBase
{
    public PcSection.PcSectionOptions FieldOptions { get; set; }
    [Parameter] public PageBodyNode Node { get; set; }

    [Parameter] public RenderFragment? ChildContent { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        // await GetView();
        InitializeFieldOptions();
        // orders = dbContext.Orders.Include("Customer").Include("Employee").ToList();
    }

    private void InitializeFieldOptions()
    {
        ErpPage currentPage = null;
        var context = BlazorPageComponentContext.CurrentPageContext;
        var httpContext = BlazorPageComponentContext.ErpRequestContext.PageContext.HttpContext;
        // try
        // {
            #region << Init >>

            var pageFromModel = context.DataModel.GetProperty("Page");
            if (pageFromModel is ErpPage)
            {
                currentPage = (ErpPage)pageFromModel;
            }

            var options = new PcSection.PcSectionOptions();
            if (context.Options != null)
            {
                options = JsonConvert.DeserializeObject<PcSection.PcSectionOptions>(context.Options.ToString());
            }

            //Check if it is defined in form group
            if (options.LabelMode == WvLabelRenderMode.Undefined)
            {
                if (context.Items.ContainsKey(typeof(WvLabelRenderMode)))
                {
                    options.LabelMode = (WvLabelRenderMode)context.Items[typeof(WvLabelRenderMode)];
                }
                else
                {
                    options.LabelMode = WvLabelRenderMode.Stacked;
                }
            }

            //Check if it is defined in form group
            if (options.FieldMode == WvFieldRenderMode.Undefined)
            {
                if (context.Items.ContainsKey(typeof(WvFieldRenderMode)))
                {
                    options.FieldMode = (WvFieldRenderMode)context.Items[typeof(WvFieldRenderMode)];
                }
                else
                {
                    options.FieldMode = WvFieldRenderMode.Form;
                }
            }

            // var componentMeta = new PageComponentLibraryService().GetComponentMeta(context.Node.ComponentName);

            //Init IsCollapsed from userPreferences
            // if (httpContext.User != null)
            // {
            //     var currentUser = AuthService.GetUser(httpContext.User);
            //     if (currentUser != null)
            //     {
            //         var componentData =
            //             new UserPreferencies().GetComponentData(currentUser.Id,
            //                 "WebVella.Erp.Web.Components.PcSection");
            //         if (componentData != null)
            //         {
            //             var collapsedNodeIds = new List<Guid>();
            //             var uncollapsedNodeIds = new List<Guid>();
            //             if (componentData.Properties.ContainsKey("collapsed_node_ids") &&
            //                 componentData["collapsed_node_ids"] != null)
            //             {
            //                 if (componentData["collapsed_node_ids"] is string)
            //                 {
            //                     try
            //                     {
            //                         collapsedNodeIds =
            //                             JsonConvert.DeserializeObject<List<Guid>>(
            //                                 (string)componentData["collapsed_node_ids"]);
            //                     }
            //                     catch
            //                     {
            //                         throw new Exception(
            //                             "WebVella.Erp.Web.Components.PcSection component data object in user preferences not in the correct format. collapsed_node_ids should be List<Guid>");
            //                     }
            //                 }
            //                 else if (componentData["collapsed_node_ids"] is List<Guid>)
            //                 {
            //                     collapsedNodeIds = (List<Guid>)componentData["collapsed_node_ids"];
            //                 }
            //                 else if (componentData["collapsed_node_ids"] is JArray)
            //                 {
            //                     collapsedNodeIds = ((JArray)componentData["collapsed_node_ids"]).ToObject<List<Guid>>();
            //                 }
            //                 else
            //                 {
            //                     throw new Exception("Unknown format of collapsed_node_ids");
            //                 }
            //             }
            //
            //             if (componentData.Properties.ContainsKey("uncollapsed_node_ids") &&
            //                 componentData["uncollapsed_node_ids"] != null)
            //             {
            //                 if (componentData["uncollapsed_node_ids"] is string)
            //                 {
            //                     try
            //                     {
            //                         uncollapsedNodeIds =
            //                             JsonConvert.DeserializeObject<List<Guid>>(
            //                                 (string)componentData["uncollapsed_node_ids"]);
            //                     }
            //                     catch
            //                     {
            //                         throw new Exception(
            //                             "WebVella.Erp.Web.Components.PcSection component data object in user preferences not in the correct format. uncollapsed_node_ids should be List<Guid>");
            //                     }
            //                 }
            //                 else if (componentData["uncollapsed_node_ids"] is List<Guid>)
            //                 {
            //                     uncollapsedNodeIds = (List<Guid>)componentData["uncollapsed_node_ids"];
            //                 }
            //                 else if (componentData["uncollapsed_node_ids"] is JArray)
            //                 {
            //                     uncollapsedNodeIds =
            //                         ((JArray)componentData["uncollapsed_node_ids"]).ToObject<List<Guid>>();
            //                 }
            //                 else
            //                 {
            //                     throw new Exception("Unknown format of uncollapsed_node_ids");
            //                 }
            //             }
            //
            //             if (collapsedNodeIds.Contains(context.Node.Id))
            //             {
            //                 options.IsCollapsed = true;
            //             }
            //             else if (uncollapsedNodeIds.Contains(context.Node.Id))
            //             {
            //                 options.IsCollapsed = false;
            //             }
            //         }
            //
            //     }
            // }


            #endregion


            FieldOptions = options;


            var isCollapsed = context.DataModel.GetPropertyValueByDataSource(options.IsCollapsedDs) as bool?;
            if (isCollapsed != null)
            {
                options.IsCollapsed = isCollapsed.Value;
                FieldOptions = options;
            }
            else if (options.IsCollapsedDs.ToLowerInvariant() == "true")
            {
                options.IsCollapsed = true;
                FieldOptions = options;
            }

            // context.Items[typeof(WvLabelRenderMode)] = FieldOptions.LabelMode;
            // context.Items[typeof(WvFieldRenderMode)] = FieldOptions.FieldMode;
        // }
        // catch
        // {
        //     
        // }
    }


    protected async Task GetView()
    {
        var request = new HttpRequestMessage(HttpMethod.Post,
            "http://localhost:5000/api/v3.0/pc/Reomi.Erp.Plugins.Common.Components.DropDown/view/display?v=0.0.1&nid=318709a7-06da-4f18-9f57-20d4839d02a6&pid=2eeea984-d5be-4241-a987-e12daec04422");
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        request.Headers.Add("Cookie", "umb_installId=279c0132-c2b2-4744-85d5-03e0b097ecb5; UMB-XSRF-V=CfDJ8KUTWWY5wDtBlVW65cv5LShY6pTzArLbEEo5BQcrD-mr9wvzlfD6VRnoGXh_pbmRAFOYRyicQJyoapMutSPYAHQ_89G0oQbn177KmAJ9jChFoSwtiC6g41n3uFAUM-_XJTP6uHpfneEw2FTfPBYzmIA; .AspNetCore.Antiforgery.HESgUHZ6l1E=CfDJ8KUTWWY5wDtBlVW65cv5LShy2_flmuWURllJjeLcJMotibHzsf-efXjQSpcf0O0D-ZA0uHNVZM8QkZucZhpyAIAWhvD2JByzcNE9M4Miu0RkV69z0ggiaHUKVpPH-QA-Q4fOKaHfls7akZNFpF95_j8; umbracoCommerce-b1e61994-b83b-420a-903e-63a7a15942dc=ZHBjPTEzNWRjOTYyLTQ5NmMtNGIwNi04MzUxLTAxOTU3YzljZjBlZSZkc2M9MTM1ZGM5NjItNDk2Yy00YjA2LTgzNTEtMDE5NTdjOWNmMGVlJmR0Yz0xN2EyZWNhMC1kMjFmLTQ2MmEtODkxNS04YjI2MDY2NjFlZmQmZGM9NTc3OTFmNGItN2FlNy00NWU4LWFiOTktMDE5NTdjOWM2MzFj; UMB-XSRF-TOKEN=CfDJ8KUTWWY5wDtBlVW65cv5LSh7-g8zKz8few1zFsb_DJWy3iGBXDFyNtzwgi46hpcJqOD-N2Dnh61yT1qW4rnRBMdP-vLWHcL4c4FiXR8lGy6TzoWpcdDHdUrhqXTrLzZ8rCaEVs8Iu-A5XzeezNYxWf0MmB4YWuDdNG-c2C7fhO-AolirA89mX2ZzFG_wwpky-w; UMB_UCONTEXT_C=2025-03-10T22%3A48%3A01.9814235%2B00%3A00; .AspNetCore.Antiforgery.9XuVCnU1ImE=CfDJ8KUTWWY5wDtBlVW65cv5LSg7eM18g9I-T81zIp0XyUDS_A0Gj8jM71b0vDxef8ETCLTVd9-nPJWxvaT6LrTM0IoZgmvZwvtluxI3cs5olJcPmb1C8bdUJtvrwZvgZ8v4yNDa19hgFRarZX8AqZ94E94; .AspNetCore.Antiforgery.zt4A-q2KyLg=CfDJ8KUTWWY5wDtBlVW65cv5LSgiL_v2XA0a7m8YbMRo1GFE6jVvItmmdghc_6xsKCgB82wjAY00ds60CGL1ClrqCjrKbYau5Iiioq57RaF_GFQjTW9gKftxsCSBq9EjImx5EqLgr0jQyCLF28tF5d315Qc; erp_auth_project=CfDJ8KUTWWY5wDtBlVW65cv5LSgpmzEQOaKGq7KXJaWBBh4qGMDXd7tAQMirP94Q0YZ0WRfAVUHtZV1uhz6WRqJAqvc32XQhcgiz5tsXxascsrrGJ6KXkJWcxDojU-_OlXv-ewOfVGiY_Ycsbmhtb0HqBld94rqXweLItA9pBrE8KGF2QOeaVM3MI_J-uZI40UbY3_bOYfr5Ibm90vypEAcpRXyN72-J5WOjj8qUnH-A5Kh29xxiNFiDSsTboi8Xuf5ud4PbCsQsF6kQnOy7JQSFialRzvc8my5Nyy7-IeSZShYpyoABqAnJFBvU-s-ZNH6gSMPlAe8JO68jLHdrGC7dEHgPbwZPlEKQhw1qP9kI_JifvdzPf0g2dCsP71tklGOVbALnR7lHAJrMUnO8AeE7Y8TIXTb3Uk0GpT92f7PiJm3hN1um2VRArcCnfthl8lZBNkerGnjTv-s-amNppoKdr_ikixO05gUkyCjzj8BGX2SPuJmEyWFGR94adQw89fCWttnbZcAITuWrIijUatFA31gU8iCl0K5-F-PexLMn2WKyHRPaju0iBqJ8W6EtUv9tUoVVxawjixtGunc-2L38Fb_q_Vfb9r7Ete0TK6KkxzbdBYtijgl6L0uS7n73u8EsHqQJArKxfK3ML3IPpGHpS3DWzZ6MM1ImwY1J8o0aQnCVcau6sJXSU9jCh-UD6fRv2XMU2PtNYxM_BqQt-eyEmG7whoZt_LsXD6JzKupRiff6YuUUlwfiVmdox8bFI9zUGNJzVSNwHYDKo7UfRh8rVEE; .AspNetCore.Antiforgery.uV12RJnajOo=CfDJ8KUTWWY5wDtBlVW65cv5LSjAmaLxDxGehLIgUgLuLhi-pzMmAaCchonxinjNMqUinWSYpUsaNr-tYdEYd6AjcGOCnPdM5ffAeARb3cMehg_k8rQGmykTNPHZcoYsWsRGnzy9hlCFOJJG2rnlyM8b2NQ; erp_auth_base=CfDJ8KUTWWY5wDtBlVW65cv5LShATYrchAJz4J0BugOkCjYFeUnwAO71y_h-dOmjASyVChVxZUieg_TXFUWdMOU1inwcSFNxabSgVnak3L7BN25O4Da08cr16NjKr-oVN4LA_YVprD_PAA-TOS9llrR7DED8PyjwRnJ_Lu-0WUtCow468lbVqj-DCt1pWsqqlm25_BSxwMBPbSASydr-hMa8ZiaK9z2-W4fPSlD9Uj7a_dhAaxkmu2iTmtXpVj-7sCjdSLssJcWps2fekTxYvlVbJkVPy_Qovs20NwPnuxxW7wkYoy2m_GR_b7HGXWAk_GZ8LKkayT8R-yx2LoKj1jny4u6Fx5AiOQh0-pDeyNAYWDeY4gBtbc_Kk0INI0LknHzoz7OqVtqsrzomj-i_LpOme6WAaPbvWcftD7P_Ryty-fYyJgW_JJXpNjE4ktxrbbsFsA31-Y2z-bXOB82gN6-0diMOz-TEPk24VOeSEQtvf14L8149qRjGAXROkxvnBOsEd9gxemaKxgIyzTptLPK3v3hOrMMQN_hHnlnYy7IlLOWYb_Z40TLcfWvYwC1a4HBMfWxY_NNBMDanw68PngEzCM2FjHT4OsLicQqPHASqLePC_Z_axvr2HXn3VBRZCz2N2Zr4sXFN1xKEe6Tq5qcAbNRfmnb7-o2cEe774qO3eiy-AdxS8lDosCca3GYYkKrb3rhvHuVzc7bb1T_BiyFRjoOGyEM_pHfhL5_HM5VrnyJjmk8O1YDjn_abI_zmZn5XAMMHtrhHFndxAM09ai9JzUs");
        // request.Headers.Add("X-Requested-With", [ "XMLHttpRequest" ]);
        request.SetBrowserRequestMode(BrowserRequestMode.NoCors);
        request.Headers.Add("Accept", "application/json, text/plain, */*");
        request.Content = new StringContent(
            "{\"is_visible\":\"true\",\"label_mode\":\"0\",\"label_text\":\"Jakiś tam label\",\"link\":\"\",\"mode\":\"3\",\"value\":\"\",\"name\":\"subjectid\",\"class\":\"\",\"show_icon\":\"false\",\"placeholder\":\"\",\"options\":\"{\\\"type\\\":\\\"0\\\",\\\"string\\\":\\\"CategoryListOptions\\\",\\\"default\\\":\\\"\\\"}\",\"connected_entity_id\":\"0a8cebf5-498b-47ea-8c98-ab509124402d\",\"connected_record_id_ds\":\"\",\"access_override_ds\":\"\",\"required_override_ds\":\"\",\"ajax_api_url_ds\":\"\",\"ajax_datasource_api\":\"\",\"description\":\"\",\"label_help_text\":\"\",\"select_match_type\":\"0\"}",
            Encoding.UTF8, 
            MediaTypeHeaderValue.Parse("application/json"));
        var response = await Client.SendAsync(request);
        var stringValue = await response.Content.ReadAsStringAsync();
        Console.WriteLine(stringValue);
        // new RenderFragment(stringValue);
        // ChildContent = new MarkupString(stringValue.ToString());
        Console.WriteLine("GetView4");
        // convert response data to Article object
        // article = await response.Content.ReadFromJsonAsync<Article>();
    }

    void Change(string text)
    {
        // Console.WriteLine($"{text}");
    }
    
    // readonly Dictionary<string, Type> _widgets = new Dictionary<string, Type>
    // {
    //     ["Counter"] = typeof(Test),
    //     ["Weather"] = typeof(Test)
    // };

    private static RenderFragment RenderNode(string componentName, PageBodyNode node) =>
        async builder =>
    {
        var helperType = Type.GetType(componentName);
        var t = Type.GetType($"Reomi.Erp.Plugins.Common.Components.RadzenComponents.Reomi{helperType?.Name}");
        if (t == null)
            return;
        
        builder.OpenComponent(0, t);
        builder.AddComponentParameter(0, "Node", node);
        builder.CloseComponent();
    };
}