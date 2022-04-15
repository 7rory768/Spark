﻿using System;
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
    public class TeamsController : SparkControllerBase
    {
        public TeamsController(IWebHostEnvironment hostingEnvironment, AppSettingsHelper appSettings, DatabaseContextHelper database) : base(hostingEnvironment, appSettings, database)
        {
            // Initalize values in SparkControllerBase
        }

        // Gets all Teams the user is participating in
        [HttpGet]
        [Route("viewTeams")]
        public ResponseMessage GetTeams()
        {
            if (!isAuthenticated()) return getNotAuthenticatedResponse();

            var response = TeamHelper.GetParticipatingTeams(getUser(),
                context: Database.DbContext,
                statusCode: out HttpStatusCode statusCode,
                includeDetailedErrors: HostingEnvironment.IsDevelopment());
            HttpContext.Response.StatusCode = (int)statusCode;
            return response;
        }

        // Creates a new team
        [HttpPost]
        [Route("create")]
        public ResponseMessage Create([FromBody] JObject data)
        {
            if (!isAuthenticated()) return getNotAuthenticatedResponse();

            var response = TeamHelper.Add(getUser(), data,
                context: Database.DbContext,
                statusCode: out HttpStatusCode statusCode,
                includeDetailedErrors: HostingEnvironment.IsDevelopment());
            HttpContext.Response.StatusCode = (int)statusCode;
            return response;
        }
    }
}