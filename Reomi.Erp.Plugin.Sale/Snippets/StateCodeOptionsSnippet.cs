using System;
using System.Collections.Generic;
using System.Text;
using WebVella.Erp.Api.Models;
using WebVella.Erp.Web.Models;


public class StateCodeOptionsSnippet : ICodeVariable
{
	public object? Evaluate(BaseErpPageModel pageModel)
	{
		var list = ((SelectField)pageModel.ErpRequestContext.Entity.Fields.Find(x => x is SelectField && x.Name == "statecode")!).Options;
		List<Entity> stateCodes = new List<Entity>();
		foreach(var test in list){
			stateCodes.Add( new Entity(){Label = test.Label, Name = test.Value});
		}
		try{
			return ((SelectField)pageModel.ErpRequestContext.Entity.Fields.Find(x => x is SelectField && x.Name == "statecode")!)?.Options;
		}
		catch(Exception ex){
			return "Error: " +  ex.Message;
		}
	}
}

