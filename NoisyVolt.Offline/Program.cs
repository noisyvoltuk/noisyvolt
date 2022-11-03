// See https://aka.ms/new-console-template for more information

using NoisyVolt.Offline.Steps;
using NoisyVolt.Services;
//using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static System.Net.Mime.MediaTypeNames;


Console.WriteLine("Batch job for directory");

//1. Look for and process new submissions
// check url - status, on a known site etc
// build embed snippet
// update submission table
// get external links for submitted page only
// then - submit to crawl queue if enabled

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();


var builder = new ServiceCollection()
    .AddSingleton<NewSubmissions, NewSubmissions>()
    .AddSingleton<ISqlDataService, SqlDataService>()
    .AddSingleton<INoSqlDataService, CosmosDBDataService>()
    .AddSingleton<IPageService, PageService>()
    .AddSingleton<IConfiguration>(config)
  .BuildServiceProvider();



NewSubmissions subsApp = builder.GetRequiredService<NewSubmissions>();

subsApp.Process();

