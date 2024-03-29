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

        var shouldLogEvents = Environment.GetEnvironmentVariable("LogEventContent") == true.ToString();

        List<DeviceUsage> events = new List<DeviceUsage>();
        foreach (var record in kinesisEvent.Records)
        {
            try
            {
                string recordData = GetRecordContents(record.Kinesis);
                var evnt = JsonConvert.DeserializeObject<DeviceUsage>(recordData);
                if (evnt != null)
                {
                    events.Add(evnt);
                }
                if (shouldLogEvents)
                {
                    context.Logger.LogInformation($"Record Data:");
                    context.Logger.LogInformation(recordData);
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"An error occured for event {record.EventId}: {ex}.");
            }
        }

        context.Logger.Log($"Completed consuming data");
        context.Logger.LogInformation(JsonConvert.SerializeObject(new { KinesisConsumedEvents = events.Count }));
       
    }

    private string GetRecordContents(KinesisEvent.Record streamRecord)
    {
        using (var reader = new StreamReader(streamRecord.Data, Encoding.ASCII))
        {
            return reader.ReadToEnd();
        }
    }
}