namespace Utils
{
    public class DeviceUsage
    {
        public string Id { get; set; }

        public DateTime Timestamp { get; set; }

        public string DeviceId { get; set; }

        public int CPUUtilization { get; set; }

        public int MemoryUtilization { get; set; }

        public int BatteryLevel { get; set; }

        public List<string> Errors { get; set; }
    }
}