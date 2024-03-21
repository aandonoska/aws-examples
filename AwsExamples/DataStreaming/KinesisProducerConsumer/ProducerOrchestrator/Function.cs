using Amazon.Lambda;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ProducerOrchestratorLambda;

public class Function
{
    private readonly IOrchestrationService _orchestrationService;

    public Function()
    {
        _orchestrationService = new OrchestrationService(new AmazonLambdaClient());
    }

    
    /// <summary>
    /// A function that orchestrates Kinesis producers
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task FunctionHandler(ProducerOrchestratorRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation($"Invoked with number of producers {request.NumberOfProducers} number of events {request.NumberOfEvents}");
        long startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        int i = 0;
        while (true)
        {
            long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            if (currentTime - startTime >= request.MilisecondsToRun)
            {
                break;
            }
            try
            {
                await _orchestrationService.OrchestrateKineisProducers(request.NumberOfProducers, request.NumberOfEvents);
                i++;
                await Task.Delay(4000);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"An unexpected error occured {ex}");
            }

            context.Logger.LogInformation($"Number of iterations {i}");
        }
    }
}
