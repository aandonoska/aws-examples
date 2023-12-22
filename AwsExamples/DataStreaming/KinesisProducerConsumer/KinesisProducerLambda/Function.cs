using Amazon.Kinesis;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace KinesisProducerLambda;

public class Function
{
    private readonly IKinesisProducerService _kinesisProducerService;

    public Function()
    {
        _kinesisProducerService = new KinesisProducerService(new AmazonKinesisClient(), "<ToDO>");
    }
    
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task FunctionHandler(int numberOfEvents, ILambdaContext context)
    {
        try
        {
            await _kinesisProducerService.SendEvents(numberOfEvents);
        }
        catch (Exception ex)
        {
            LambdaLogger.Log($"An unexpected error occured {ex.Message}");
        }

    }
}
