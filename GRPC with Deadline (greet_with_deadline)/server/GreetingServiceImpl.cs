using Greeting;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Greeting.GreetingService;

namespace server
{
    public class GreetingServiceImpl : GreetingServiceBase
    {
        public override async Task<GreetingResponse> greet_with_deadline(GreetingRequest request, ServerCallContext context)
        {
            await Task.Delay(300);
            return new GreetingResponse() { Result = "Hello " + request.Name };
        }
    }
}
