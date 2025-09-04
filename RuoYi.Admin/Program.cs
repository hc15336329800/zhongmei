using AspectCore.Extensions.DependencyInjection;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args).Inject();
        builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(3);
            serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(1);
        });
        builder.Host.UseServiceProviderFactory(new DynamicProxyServiceProviderFactory());
        builder.Build().Run();
    }
}