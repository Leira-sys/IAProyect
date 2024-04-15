using Blazored.Modal;
using BlazorRecognizerWA;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });



//agregamos inyeccion de dependencia para el base address de la minimal api
//var httpClientHandler = new HttpClientHandler();
//httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, error) => true;

builder.Services.AddSingleton(new HttpClient()
{
    BaseAddress = new Uri("https://localhost:7236/")
}); ;

builder.Services.AddBlazoredModal();
await builder.Build().RunAsync();
