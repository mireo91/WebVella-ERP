using System;
using System.Collections.Generic;
using System.Diagnostics;
using WebVella.Erp;
using WebVella.Erp.Api.Models;
using WebVella.Erp.Hooks;

namespace Reomi.Erp.Plugins.Common.Hooks
{
    [HookAttachment]
    public class RecordHook : IErpDefaultFields
    {
        public void OnDefaultFieldsInit(List<Field> fields,Dictionary<string, Guid> sysFieldIdDictionary)
        {
            GuidField createdBy = new GuidField();

				if (sysFieldIdDictionary != null && sysFieldIdDictionary.ContainsKey("createby"))
				{
					createdBy.Id = sysFieldIdDictionary["createdby"];
				}
				else
				{
					createdBy.Id = Guid.NewGuid();
				}
				createdBy.Name = "createdby";
				createdBy.Label = "Created By";
				createdBy.PlaceholderText = "";
				createdBy.Description = "";
				createdBy.HelpText = "";
				createdBy.Required = false;
				createdBy.Unique = false;
				createdBy.Searchable = false;
				createdBy.Auditable = false;
				createdBy.System = true;
				createdBy.DefaultValue = null;
				createdBy.GenerateNewId = false;

				fields.Add(createdBy);

				GuidField lastModifiedBy = new GuidField();

				if (sysFieldIdDictionary != null && sysFieldIdDictionary.ContainsKey("modifiedby"))
				{
					lastModifiedBy.Id = sysFieldIdDictionary["modifiedby"];
				}
				else
				{
					lastModifiedBy.Id = Guid.NewGuid();
				}
				lastModifiedBy.Name = "modifiedby";
				lastModifiedBy.Label = "Modified By";
				lastModifiedBy.PlaceholderText = "";
				lastModifiedBy.Description = "";
				lastModifiedBy.HelpText = "";
				lastModifiedBy.Required = false;
				lastModifiedBy.Unique = false;
				lastModifiedBy.Searchable = false;
				lastModifiedBy.Auditable = false;
				lastModifiedBy.System = true;
				lastModifiedBy.DefaultValue = null;
				lastModifiedBy.GenerateNewId = false;

				fields.Add(lastModifiedBy);

				DateTimeField createdOn = new DateTimeField();

				if (sysFieldIdDictionary != null && sysFieldIdDictionary.ContainsKey("createdon"))
				{
					createdOn.Id = sysFieldIdDictionary["createdon"];
				}
				else
				{
					createdOn.Id = Guid.NewGuid();
				}
				createdOn.Name = "createdon";
				createdOn.Label = "Created On";
				createdOn.PlaceholderText = "";
				createdOn.Description = "";
				createdOn.HelpText = "";
				createdOn.Required = true;
				createdOn.Unique = false;
				createdOn.Searchable = false;
				createdOn.Auditable = false;
				createdOn.System = true;
				createdOn.DefaultValue = null;

				createdOn.Format = "dd MMM yyyy HH:mm";
				createdOn.UseCurrentTimeAsDefaultValue = true;

				fields.Add(createdOn);

				DateTimeField modifiedOn = new DateTimeField();

				if (sysFieldIdDictionary != null && sysFieldIdDictionary.ContainsKey("modifiedon"))
				{
					modifiedOn.Id = sysFieldIdDictionary["modifiedon"];
				}
				else
				{
					modifiedOn.Id = Guid.NewGuid();
				}
				modifiedOn.Name = "modifiedon";
				modifiedOn.Label = "Modified On";
				modifiedOn.PlaceholderText = "";
				modifiedOn.Description = "";
				modifiedOn.HelpText = "";
				modifiedOn.Required = true;
				modifiedOn.Unique = false;
				modifiedOn.Searchable = false;
				modifiedOn.Auditable = false;
				modifiedOn.System = true;
				modifiedOn.DefaultValue = null;

				modifiedOn.Format = "dd MMM yyyy HH:mm";
				modifiedOn.UseCurrentTimeAsDefaultValue = true;

				fields.Add(modifiedOn);
            // throw new NotImplementedException();
        }
    }
}