using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WebVella.Erp.Api;
using WebVella.Erp.Api.Models;
using WebVella.Erp.Api.Models.AutoMapper;
using WebVella.Erp.Diagnostics;
using WebVella.Erp.Exceptions;
using WebVella.Erp.Hooks;
using WebVella.Erp.Web;
using WebVella.Erp.Web.Hooks;
using WebVella.Erp.Web.Models;
using WebVella.Erp.Web.Pages.Application;
using WebVella.Erp.Web.Services;

namespace Reomi.Erp.Plugins.SaleAndWarehouse.Pages;

public class OrderCalculation : RecordCreatePageModel
{
    public OrderCalculation(ErpRequestContext reqCtx) : base(reqCtx)
    {
    }
}