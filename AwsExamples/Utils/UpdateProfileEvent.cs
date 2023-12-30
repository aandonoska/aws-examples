namespace Utils
{
    public class UpdateProfileEvent
    {
        public string Id { get; set; }

        public DateTime Timestamp { get; set; }

        public string ProfileId { get; set; }

        public string UpdateType { get; set; }

        public string NewValue { get; set; }
    }
}