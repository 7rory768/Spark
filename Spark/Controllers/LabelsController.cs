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
    public class LabelsController : SparkControllerBase
    {
        public LabelsController(IWebHostEnvironment hostingEnvironment, AppSettingsHelper appSettings, DatabaseContextHelper database) : base(hostingEnvironment, appSettings, database)
        {
            // Initalize values in SparkControllerBase
        }

        // Gets all labels for a project
        [HttpGet]
        [Route("{projectId}")]
        public ResponseMessage Get(int projectId)
        {
            if (!isAuthenticated()) return getNotAuthenticatedResponse();

            var response = LabelHelper.GetLabels(getUser(), projectId,
                context: Database.DbContext,
                statusCode: out HttpStatusCode statusCode,
                includeDetailedErrors: HostingEnvironment.IsDevelopment());
            HttpContext.Response.StatusCode = (int)statusCode;
            return response;
        }

        // Creates a new label
        [HttpPost]
        [Route("create")]
        public ResponseMessage Create([FromBody] JObject data)
        {
            if (!isAuthenticated()) return getNotAuthenticatedResponse();

            var response = LabelHelper.Add(getUser(), data,
                context: Database.DbContext,
                statusCode: out HttpStatusCode statusCode,
                includeDetailedErrors: HostingEnvironment.IsDevelopment());
            HttpContext.Response.StatusCode = (int)statusCode;
            return response;
        }

        // Delets a label
        [HttpDelete]
        [Route("{id}")]
        public ResponseMessage Delete(int id)
        {
            if (!isAuthenticated()) return getNotAuthenticatedResponse();

            var response = LabelHelper.DeleteLabel(getUser(), id,
                context: Database.DbContext,
                statusCode: out HttpStatusCode statusCode,
                includeDetailedErrors: HostingEnvironment.IsDevelopment());
            HttpContext.Response.StatusCode = (int)statusCode;
            return response;
        }

    }
}
