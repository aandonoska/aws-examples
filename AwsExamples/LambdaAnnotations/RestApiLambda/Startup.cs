namespace RestApiLambda
{
    [Amazon.Lambda.Annotations.LambdaStartup]
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDateService, DateService>();
        }
    }
}
