using Amazon.Lambda.Annotations;
using Amazon.Lambda.Core;
using Amazon.S3;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DependencyInjectionLambda;

public class Function
{
    private readonly IAmazonS3 _s3Client;

    /// <summary>
    /// Constructs an instance with a S3 client.
    /// </summary>
    /// <param name="s3Client"></param>
    public Function(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
    }

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    [LambdaFunction(MemorySize = 128)]
    public string FunctionHandler([FromServices] ISampleService sampleService, string input, ILambdaContext context)
    {
        if(_s3Client != null)
        {
            sampleService.DummyMethod();

            return $"Successful executon of DI! {input.ToUpper()}";
        }

        return $"Unsuccessful executon of DI! {input.ToUpper()}";
    }
}
