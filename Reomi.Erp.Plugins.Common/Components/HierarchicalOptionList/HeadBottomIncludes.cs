using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebVella.Erp.Web.Hooks;
using WebVella.Erp.Web.Models;
using WebVella.Erp.Web.Services;

namespace Reomi.Erp.Plugins.Common.Components
{

	[RenderHookAttachment("head-bottom", 10)]
	public class HeadBottomIncludes : ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync(BaseErpPageModel pageModel)
		{
			ViewBag.ScriptTags = new List<ScriptTagInclude>();
			ViewBag.LinkTags = new List<LinkTagInclude>();

			var cacheKey = new RenderService().GetCacheKey();
			#region === <script> ===
			{
				var includedScriptTags = pageModel.HttpContext.Items.ContainsKey(typeof(List<ScriptTagInclude>)) ? (List<ScriptTagInclude>)pageModel.HttpContext.Items[typeof(List<ScriptTagInclude>)]! : new List<ScriptTagInclude>();
				var scriptTagsToInclude = new List<ScriptTagInclude>();

				//Your includes below >>>>
				scriptTagsToInclude.Add(new ScriptTagInclude()
				{
					Src = $"/_content/Reomi.Erp.Plugins.Common/js/treeselect/treeselectjs.umd.js?cb=" + cacheKey,
					// IsNomodule = true
				});
				//<<<< Your includes up

				includedScriptTags?.AddRange(scriptTagsToInclude);
				pageModel.HttpContext.Items[typeof(List<ScriptTagInclude>)] = includedScriptTags;
				ViewBag.ScriptTags = scriptTagsToInclude;
			}
			#endregion
			#region === <link> ===
			{
				var includedLinkTags = pageModel.HttpContext.Items.ContainsKey(typeof(List<LinkTagInclude>)) ? (List<LinkTagInclude>)pageModel.HttpContext.Items[typeof(List<LinkTagInclude>)] : new List<LinkTagInclude>();
				var linkTagsToInclude = new List<LinkTagInclude>();
			
				//Your includes below >>>>
			
				#region << core plugin >>
				{
					//Always include
					if(pageModel != null && pageModel.ErpAppContext != null && !String.IsNullOrEmpty(pageModel.ErpAppContext.StylesHash))
					{
						linkTagsToInclude.Add(new LinkTagInclude()
						{
							Href = "/_content/Reomi.Erp.Plugins.Common/js/treeselect/treeselectjs.css?cb=" + cacheKey,
							CacheBreaker = pageModel.ErpAppContext.StylesHash,
							//CrossOrigin = CrossOriginType.Anonymous,
							//Integrity = $"sha256-{pageModel.ErpAppContext.StylesHash}"
						});
					}
					else{
						linkTagsToInclude.Add(new LinkTagInclude()
						{
							Href = "/_content/Reomi.Erp.Plugins.Common/js/treeselect/treeselectjs.css?cb=" + cacheKey
						});	
					}
				}
				#endregion
			
				//<<<< Your includes up
			
				includedLinkTags.AddRange(linkTagsToInclude);
				pageModel.HttpContext.Items[typeof(List<LinkTagInclude>)] = includedLinkTags;
				ViewBag.LinkTags = linkTagsToInclude;
			}
			#endregion

			return await Task.FromResult<IViewComponentResult>(View("Default"));
		}
	}
}
