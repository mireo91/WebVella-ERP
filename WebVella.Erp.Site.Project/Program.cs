﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace WebVella.Erp.Site.Project
{
	public class Program
	{
		public static void Main(string[] args)
		{
			BuildWebHost(args).Run();
		}

		public static IWebHost BuildWebHost(string[] args) =>
		   WebHost.CreateDefaultBuilder(args)
			   .UseStaticWebAssets()
			   .UseStartup<Startup>()
			   .Build();
	}
}
