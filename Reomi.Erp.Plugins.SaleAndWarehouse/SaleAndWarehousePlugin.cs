using System;
using Newtonsoft.Json;
using WebVella.Erp;
using WebVella.Erp.Api;

namespace Reomi.Erp.Plugins.SaleAndWarehouse
{
	public partial class SaleAndWarehousePlugin : ErpPlugin
	{
		[JsonProperty(PropertyName = "name")] public override string Name { get; protected set; } = "saleandwarehouse";

		public override void Initialize(IServiceProvider serviceProvider)
		{
			using (var ctx = SecurityContext.OpenSystemScope())
			{
			}

		}
	}
}
