using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using WebVella.Erp.Api.Models;
using WebVella.Erp.Eql;
using WebVella.Erp.Web.Models;
using WebVella.Erp.Web.Utils;

namespace Reomi.Erp.Plugins.Common.DataSource
{
	public class HierarchicalOptionList : CodeDataSource
	{
		public HierarchicalOptionList() : base()
		{
			Id = new Guid("48C2D7D6-1ACB-4AB8-B518-8052184528C9");
			Name = "HierarchicalListOption";
			Description = "Provides hierarchical list with dynamic depth";
			ResultModel = "List<HierarchicalListOption>";

			//define custom meta
			//DataSourceModelFieldMeta dsMeta = new DataSourceModelFieldMeta();
			//dsMeta.EntityName = string.Empty;
			//dsMeta.Name = "CurrentDate";
			//dsMeta.Type = FieldType.DateField;
			//Fields.Add(dsMeta);
			
			Parameters.Add(new DataSourceParameter { Name = "depth", Type = "int", Value = "0" });
			Parameters.Add(new DataSourceParameter { Name = "propertyParentKey", Type = "text", Value = "0" });
			Parameters.Add(new DataSourceParameter { Name = "optionName", Type = "text", Value = "0" });
			Parameters.Add(new DataSourceParameter { Name = "optionValue", Type = "text", Value = "0" });
			Parameters.Add(new DataSourceParameter { Name = "entityName", Type = "text", Value = "cdm_common_category" });

		}

		public class HierarchicalListOption
		{
			[JsonProperty(PropertyName = "name")] public string Name { get; set; } = string.Empty;
			[JsonProperty(PropertyName = "value")] public string Value { get; set; } = string.Empty;
			[JsonIgnore] public string? ParentId { get; set; }
			[JsonProperty(PropertyName = "children")] public List<HierarchicalListOption> Children { get; set; } = new();

			public static HierarchicalListOption FromEntity(EntityRecord record) => new()
			{
				Name = (string)(record.Properties["title"]), Value = ((Guid)record.Properties["id"]).ToString(),
				ParentId = ((Guid?)record["parentcategoryid"])?.ToString()
			};
		}

		public override object Execute(Dictionary<string, object> arguments)
		{
			try{

				int depth = 0;

				if (arguments.ContainsKey("depth"))
					depth = (int)arguments["depth"];
				
				PageDataModel pageModel = arguments["PageModel"] as PageDataModel;
				if (pageModel == null)
					return JsonConvert.SerializeObject(new List<HierarchicalListOption>());
				
				var eqlResult = (new EqlCommand("SELECT id, title, parentcategoryid FROM cdm_common_category").Execute()).Select(HierarchicalListOption.FromEntity).ToList();
				var groups = eqlResult.GroupBy(x => x.ParentId);
				var roots = groups.FirstOrDefault(g => g.Key == null).ToList();
				
				if (roots.Count > 0)
				{
					var dict = groups.Where(g => g.Key != null).ToDictionary(g => g.Key, g => g.ToList());
					for (int i = 0; i < roots.Count; i++)
						AddChildren(roots[i], dict);
				}
				
				return roots;
			}
			catch(Exception ex){
				return "Error: " + ex.Message;
			}
		}
		
		private static void AddChildren(HierarchicalListOption node, IDictionary<string, List<HierarchicalListOption>> source)
		{
			if (source.ContainsKey(node.Value))
			{
				node.Children = source[node.Value];
				for (int i = 0; i < node.Children.Count; i++)
					AddChildren(node.Children[i], source);
			}
			else
			{
				node.Children = new List<HierarchicalListOption>();
			}
		}
		
	}
}