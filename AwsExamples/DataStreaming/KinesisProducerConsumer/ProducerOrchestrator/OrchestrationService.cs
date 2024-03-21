using Amazon.Lambda;
using Amazon.Lambda.Core;
using Amazon.Lambda.Model;

namespace ProducerOrchestratorLambda
{
    public interface IOrchestrationService
    {
        Task OrchestrateKineisProducers(int numberOfProducers, int numberOfEvents);
    }

    public class OrchestrationService : IOrchestrationService
    {
        private readonly AmazonLambdaClient _amazonLambdaClient;
        private const int maxConcurrentProducers = 500;

        public OrchestrationService(AmazonLambdaClient amazonLambdaClient)
        {
            _amazonLambdaClient = amazonLambdaClient;
        }

        public async Task OrchestrateKineisProducers(int numberOfProducers, int numberOfEvents)
        {
            var maxNumberOfProducers = numberOfProducers > maxConcurrentProducers ? maxConcurrentProducers : numberOfProducers;

            LambdaLogger.Log($"Starting: {maxNumberOfProducers} producers");

            var InvokeResponseTasks = new List<Task<InvokeResponse>>();
            for (int i = 0; i < maxNumberOfProducers; i++)
            {
                InvokeResponseTasks.Add(_amazonLambdaClient.InvokeAsync(new InvokeRequest
                { 
                    FunctionName = "kinesis-producer-lambda",
                    Payload = numberOfEvents.ToString()
                }));
            }

            InvokeResponse[] invokeResponses =  await Task.WhenAll(InvokeResponseTasks);
            CheckForFailedInvocations(invokeResponses);
        }

        private static void CheckForFailedInvocations(InvokeResponse[] invokeResponses)
        {
            foreach (var response in invokeResponses)
            {

                if (!string.IsNullOrEmpty(response.FunctionError))
                {
                    LambdaLogger.Log($"Function call failed with : {response.FunctionError}");
                }
            }
        }
    }
}
