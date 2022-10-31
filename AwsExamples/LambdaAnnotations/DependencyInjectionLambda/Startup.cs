using Microsoft.Extensions.DependencyInjection;
using Amazon.Lambda.Annotations;
using Amazon.S3;

namespace DependencyInjectionLambda
{
    [LambdaStartup]
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ISampleService, SampleService>();
            services.AddAWSService<IAmazonS3>();
        }
    }
}
