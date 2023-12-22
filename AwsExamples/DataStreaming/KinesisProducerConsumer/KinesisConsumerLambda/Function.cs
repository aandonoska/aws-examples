using System.Text;
using System.Text.Json.Serialization;
using Amazon.Lambda.Core;
using Amazon.Lambda.KinesisEvents;
using Newtonsoft.Json;
using Utils;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace KinesisConsumerLambda;

public class Function
{

    public void FunctionHandler(KinesisEvent kinesisEvent, ILambdaContext context)
    {
        context.Logger.LogInformation($"Beginning to process {kinesisEvent.Records.Count} records...");

        List<Event> events = new List<Event>();
        foreach (var record in kinesisEvent.Records)
        {
            context.Logger.LogInformation($"Event ID: {record.EventId}");
            context.Logger.LogInformation($"Event Name: {record.EventName}");

            string recordData = GetRecordContents(record.Kinesis);
            var evnt = JsonConvert.DeserializeObject<Event>(recordData);
            events.Add(evnt);
            //push to metrics
            context.Logger.LogInformation($"Record Data:");
            context.Logger.LogInformation(recordData);
        }

        context.Logger.LogInformation("Stream processing complete.");
    }

    private string GetRecordContents(KinesisEvent.Record streamRecord)
    {
        using (var reader = new StreamReader(streamRecord.Data, Encoding.ASCII))
        {
            return reader.ReadToEnd();
        }
    }
}