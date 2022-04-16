using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BusinessLibrary.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Spark.ContextHelpers;
using Spark.ControllerHelpers;
using Newtonsoft.Json.Linq;

namespace Spark.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RewardsController : SparkControllerBase
    {
        public RewardsController(IWebHostEnvironment hostingEnvironment, AppSettingsHelper appSettings, DatabaseContextHelper database) : base(hostingEnvironment, appSettings, database)
        {
            // Initalize values in SparkControllerBase
        }

        // Adds Rewards to a user
        [HttpPost]
        [Route("rewardUser")]
        public ResponseMessage Create([FromBody] JObject data)
        {
            if (!isAuthenticated()) return getNotAuthenticatedResponse();

            var response = RewardHelper.AddReward(getUser(), data,
                context: Database.DbContext,
                statusCode: out HttpStatusCode statusCode,
                includeDetailedErrors: HostingEnvironment.IsDevelopment());
            HttpContext.Response.StatusCode = (int)statusCode;
            return response;
        }

        //// Gets a user's total points for a team
        //[HttpGet]
        //[Route("team/{id}")]
        //public ResponseMessage GetUserTeamRewards(int id)
        //{
        //    if (!isAuthenticated()) return getNotAuthenticatedResponse();

        //    var response = RewardHelper.GetUserTeamRewards(getUser(), id,
        //        context: Database.DbContext,
        //        statusCode: out HttpStatusCode statusCode,
        //        includeDetailedErrors: HostingEnvironment.IsDevelopment());
        //    HttpContext.Response.StatusCode = (int)statusCode;
        //    return response;
        //}

    }
}
