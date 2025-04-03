using WebVella.Erp.Api.Models;

namespace Reomi.Erp.Plugins.Common.Helper;

public static class EntityHelperExtension
{
    public static EntityRecord ToEntityObject(this IDictionary<string,object> data)
    {
        var entity = new EntityRecord();

        foreach (var d in data)
        {
            entity[d.Key] = d.Value;
        }
        return entity;
    }
}