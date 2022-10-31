namespace DependencyInjectionLambda
{
    public interface ISampleService
    {
        bool DummyMethod();
    }

    public class SampleService : ISampleService
    {
        public bool DummyMethod()
        {
            return true;
        }
    }
}
