﻿namespace ProducerOrchestratorLambda
{
    public class ProducerOrchestratorRequest
    {
        public int NumberOfProducers { get; set; }
        public int NumberOfEvents { get; set; }

        public int MilisecondsToRun { get; set; }
    }
}
