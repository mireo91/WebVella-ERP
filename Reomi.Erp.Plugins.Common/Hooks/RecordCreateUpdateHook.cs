using System;
using System.Collections.Generic;
using System.Diagnostics;
using WebVella.Erp;
using WebVella.Erp.Api;
using WebVella.Erp.Api.Models;
using WebVella.Erp.Hooks;
using WebVella.Erp.Web;

namespace Reomi.Erp.Plugins.Common.Hooks
{
    [HookAttachment]
    public class RecordCreateUpdateHook : IErpPreCreateRecordHook, IErpPreUpdateRecordHook
    {

        public void OnPreCreateRecord(string entityName, EntityRecord record, List<ErrorModel> errors)
        {
            if (record.Properties.ContainsKey("createdby"))
            {
                record["createdby"] = SecurityContext.CurrentUser.Id;
            }
            if (record.Properties.ContainsKey("modifiedby"))
            {
                record["modifiedby"] = SecurityContext.CurrentUser.Id;
            }
        }
        public void OnPreUpdateRecord(string entityName, EntityRecord record, List<ErrorModel> errors)
        {
            if (record.Properties.ContainsKey("modifiedon"))
            {
                record["modifiedon"] = DateTime.Now;
            }
            
            
            if (record.Properties.ContainsKey("modifiedby"))
            {
                record["modifiedby"] = SecurityContext.CurrentUser.Id;
            }
        }
    }
}