namespace RestApiLambda
{
    public interface IDateService
    {
        DateTime GetCurrentDate();
        DateTime GetPastDate(int numberOfDays);
        DateTime GetFutureDate(int numberOfDays);
    }

    public class DateService : IDateService
    {
        public DateTime GetCurrentDate()
        {
            return DateTime.UtcNow;
        }

        public DateTime GetPastDate(int numberOfDays)
        {
            return DateTime.UtcNow.AddDays(-numberOfDays);
        }

        public DateTime GetFutureDate(int numberOfDays)
        {
            return DateTime.UtcNow.AddDays(numberOfDays);
        }
    }
}
