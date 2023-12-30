﻿using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using System.Text;
using Utils;

namespace KinesisProducerLambda
{
    public interface IKinesisProducer
    {
         Task SendEvents(int numberOfEvents);
    }

    public class KinesisProducer: IKinesisProducer
    {
        private readonly IAmazonKinesis _kinesisClient;
        private readonly string _streamName;

        private const int maxBatchSizeBytes = 5 * 1024 * 1024;
        private const int maxRecordsPerBatch = 500; 

        public KinesisProducer(IAmazonKinesis amazonKinesis)
        {
            _kinesisClient = amazonKinesis;
            _streamName = Environment.GetEnvironmentVariable("StreamName");
        }

        public async Task SendEvents(int numberOfEvents)
        {
            PutRecordsResponse[] putRecordsResponses = Array.Empty<PutRecordsResponse>();
            try
            {
                var events = GenerateEvents(numberOfEvents);
                var putRecordsRequestsBatches = SplitEventsIntoBatches(events);
                    
                putRecordsResponses = await Task.WhenAll(putRecordsRequestsBatches.Select(x => SendBatchToKinesis(x)));
            }
            catch (Exception ex)
            {
                LambdaLogger.Log($"An unexpected error occured on SendEvents {ex.Message}");
            }
            
            CheckForFailedRecords(putRecordsResponses);
        }

        private static List<UpdateProfileEvent> GenerateEvents(int numberOfEvents)
        {
            Random random = new();
            var events = new List<UpdateProfileEvent>();
            for (int i = 0; i < numberOfEvents; i++)
            {
                events.Add(new UpdateProfileEvent
                {
                    Id = Guid.NewGuid().ToString(),
                    Timestamp = DateTime.UtcNow,
                    ProfileId = Guid.NewGuid().ToString(),
                    UpdateType = "Username",
                    NewValue = $"user{random.Next()}"
                });
            }

            return events;
        }

        private static List<List<PutRecordsRequestEntry>> SplitEventsIntoBatches(List<UpdateProfileEvent> events)
        {
            int currentBatchSizeBytes = 0;
            var recordsBatch = new List<List<PutRecordsRequestEntry>>();
            var recordBatch = new List<PutRecordsRequestEntry>();
            foreach (var eventData in events)
            {
                try
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
                catch(Exception ex)
                {
                    LambdaLogger.Log($"An unexpected error occured on SplitEventsIntoBatches for event {eventData.Id} {ex.Message}");
                }  
            }

            if (recordBatch.Any())
            {
                recordsBatch.Add(recordBatch);
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
                        LambdaLogger.Log($"Record {record.SequenceNumber} failed to be sent: {record.ErrorCode} {record.ErrorMessage}");
                    }
                }
            }
        }
    }
}
