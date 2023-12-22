using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using Amazon.Lambda.Core;
using Utils;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace KinesisProducerLambda;

public class Function
{
    private readonly IAmazonKinesis _amazonKinesis;

    public Function()
    {
        _amazonKinesis = new AmazonKinesisClient();
    }
    
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task FunctionHandler(int numberOfEvents, ILambdaContext context)
    {
        List<Task<PutRecordResponse>> responseTasks = new List<Task<PutRecordResponse>>();
        var eventsToSend = new List<Event>();
        for(int i = 0; i < numberOfEvents; i++)
        {
            eventsToSend.Add(new Event { Name = Guid.NewGuid().ToString() });
        }

        var batchSize = eventsToSend.Count >500 ? 500 : numberOfEvents;
        var loops = numberOfEvents / batchSize;
        loops = Math.Max(1, loops);

        var putRecords = new List<PutRecordRequest>();
        var partitionKeyMin = 0;
        for(int i=0; i < loops; i++)
        {
            var putRecordBatch = new List<PutRecordsRequestEntry>();
            for (int j = 0; j < batchSize; j++)
            {
                var index = *
            }

        }

    }
}
