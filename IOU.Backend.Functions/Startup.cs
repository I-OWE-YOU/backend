using System.Text;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

using IOU.Backend.Functions;

[assembly: FunctionsStartup(typeof(Startup))]
namespace IOU.Backend.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
    }
}
