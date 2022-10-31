using Amazon.Lambda.Core;
using Amazon.Lambda.Annotations;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace RestApiLambda
{
    /// <summary>
    /// A collection of sample Lambda functions that provide a REST api. 
    /// </summary>
    public class Functions
    {
        private readonly IDateService _dateService;

        public Functions(IDateService dateService)
        {
            _dateService = dateService;
        }

        /// <summary>
        /// Root route that provides information about the other requests that can be made.
        /// </summary>
        /// <returns>API descriptions.</returns>
        [LambdaFunction(MemorySize = 128)]
        [HttpApi(LambdaHttpMethod.Get, "/")]
        public DateTime Default()
        {
            return _dateService.GetCurrentDate();
        }

        /// <summary>
        /// Perform date in the past calculation
        /// </summary>
        /// <param name="numberOfDays"></param>
        /// <returns>Date in the past.</returns>
        [LambdaFunction(MemorySize = 256)]
        [HttpApi(LambdaHttpMethod.Get, "/getpastdate/{numberOfDays}")]
        public DateTime GetPastDate(int numberOfDays)
        {
            
            return _dateService.GetPastDate(numberOfDays);
        }

        /// <summary>
        /// Perform date in the future calculation
        /// </summary>
        /// <param name="numberOfDays"></param>
        /// <returns>Date in the future.</returns>
        [LambdaFunction(MemorySize = 256)]
        [HttpApi(LambdaHttpMethod.Get, "/getfuturedate/{numberOfDays}")]
        public DateTime GetFutureDate(int numberOfDays)
        {

            return _dateService.GetFutureDate(numberOfDays);
        }
    }
}
