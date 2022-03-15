using Microsoft.AspNetCore.Mvc;

namespace Spark.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet(Name = "GetRandomNumber")]
        public int GetRandomNumber()
        {
            return Random.Shared.Next(0, 1000);
        }
    }
}
