using Newtonsoft.Json;
using System.Collections.Generic;

namespace WebVella.Erp.Api.Models
{
	public class EntityRecordList : List<EntityRecord>
	{
		[JsonProperty(PropertyName = "total_count")]
		public int TotalCount { get; set; } = 0;
		
		[JsonProperty(PropertyName = "entity_record_name")]
		public string EntityRecordName { get; set; }
	}
}
