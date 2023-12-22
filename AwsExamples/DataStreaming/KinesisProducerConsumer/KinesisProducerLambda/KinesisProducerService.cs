using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using Newtonsoft.Json;
using System.Text;
using Utils;

namespace KinesisProducerLambda
{
    public class KinesisProducerService
    {
        private readonly IAmazonKinesis _kinesisClient;
        private readonly string _streamName;

        private const int maxBatchSizeBytes = 1024 * 1024;
        private const int maxRecordsPerBatch = 500; 

        public KinesisProducerService(IAmazonKinesis amazonKinesis, string streamName)
        {
            _kinesisClient = amazonKinesis;
            _streamName = streamName;
        }

        public async Task SendEvents(int numberOfEvents)
        {
            var events = GenerateEvents(numberOfEvents);
            var putRecordsRequestsBatches = SplitEventsIntoBatches(events);
            var putRecordsResponses = await Task.WhenAll(putRecordsRequestsBatches.Select(x => SendBatchToKinesis(x)));
            CheckForFailedRecords(putRecordsResponses);
        }

        private static List<Event> GenerateEvents(int numberOfEvents)
        {
            var events = new List<Event>();
            for (int i = 0; i < numberOfEvents; i++)
            {
                events.Add(new Event { Name = Guid.NewGuid().ToString() });
            }

            return events;
        }

        private static List<List<PutRecordsRequestEntry>> SplitEventsIntoBatches(List<Event> events)
        {
            var recordsBatch = new List<List<PutRecordsRequestEntry>>();
            int currentBatchSizeBytes = 0;
            var recordBatch = new List<PutRecordsRequestEntry>();
            foreach (var eventData in events)
            {
                byte[] eventDataBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventData));

                int eventDataSizeBytes = eventDataBytes.Length;

                // Check if adding the current event to the batch would exceed the maximum size or record limit
                if (currentBatchSizeBytes + eventDataSizeBytes > maxBatchSizeBytes || recordsBatch.Count >= maxRecordsPerBatch)
                {
                    recordsBatch.Add(recordBatch);

                    // Reset the batch and size for the next iteration
                    recordBatch.Clear();
                    currentBatchSizeBytes = 0;
                }

                using (var memoryStream = new MemoryStream(eventDataBytes))
                {
                    recordBatch.Add(new PutRecordsRequestEntry
                    {
                        Data = memoryStream,
                        PartitionKey = Guid.NewGuid().ToString()
                    });

                    currentBatchSizeBytes += eventDataSizeBytes;
                }
            }

            return recordsBatch;
        }

        private Task<PutRecordsResponse> SendBatchToKinesis(List<PutRecordsRequestEntry> recordsBatch)
        {
            var putRecordsRequest = new PutRecordsRequest
            {
                Records = recordsBatch,
                StreamName = _streamName
            };

            return _kinesisClient.PutRecordsAsync(putRecordsRequest);
        }

        private static void CheckForFailedRecords(PutRecordsResponse[] putRecordsResponsesTask)
        {
            foreach (var response in putRecordsResponsesTask)
            {
                foreach (var record in response.Records)
                {
                    if (!string.IsNullOrEmpty(record.ErrorCode))
                    {
                        Console.WriteLine($"Record failed to be sent: {record.ErrorCode}");
                    }
                }
            }
        }
    }
}
