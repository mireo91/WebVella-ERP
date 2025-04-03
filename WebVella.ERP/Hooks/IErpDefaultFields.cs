using System;
using System.Collections.Generic;
using WebVella.Erp.Api.Models;
using WebVella.Erp.Hooks;

namespace WebVella.Erp.Hooks;

[Hook("Provide hook for point in code before entity create to allow aply default fields.")]
public interface IErpDefaultFields 
{
    void OnDefaultFieldsInit(List<Field> fields, Dictionary<string, Guid> sysFieldIdDictionary);
}
