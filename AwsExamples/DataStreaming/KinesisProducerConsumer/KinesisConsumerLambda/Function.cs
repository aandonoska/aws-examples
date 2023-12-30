using System.Text;
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

        List<UpdateProfileEvent> events = new List<UpdateProfileEvent>();
        foreach (var record in kinesisEvent.Records)
        {
            try
            {
                context.Logger.LogInformation($"Event ID: {record.EventId}");
                context.Logger.LogInformation($"Event Name: {record.EventName}");

                string recordData = GetRecordContents(record.Kinesis);
                var evnt = JsonConvert.DeserializeObject<UpdateProfileEvent>(recordData);
                if (evnt != null)
                {
                    events.Add(evnt);
                }

                context.Logger.LogInformation($"Record Data:");
                context.Logger.LogInformation(recordData);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"An error occured for event {record.EventId}: {ex}.");
            }  
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