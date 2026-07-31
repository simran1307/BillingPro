using BillingFunctions.Services;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.Services.AddSingleton<SqlService>();
builder.Services.AddSingleton<KeyVaultService>();
builder.Services.AddSingleton<JwtService>();

builder.Build().Run();