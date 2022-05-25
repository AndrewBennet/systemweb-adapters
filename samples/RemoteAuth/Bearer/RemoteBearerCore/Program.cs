using Microsoft.AspNetCore.SystemWebAdapters;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSystemWebAdapters()
    .AddRemoteAppAuthentication(options =>
    {
        options.RemoteServiceOptions.RemoteAppUrl = new(builder.Configuration["ReverseProxy:Clusters:fallbackCluster:Destinations:fallbackApp:Address"]);

        // A real application would not hard code this, but load it
        // securely from environment or configuration
        options.RemoteServiceOptions.ApiKey = "TopSecretString";
    });

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers()
   .RequireRemoteAppAuthentication();

app.MapReverseProxy();

app.Run();
