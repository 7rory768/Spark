﻿using System;
using Newtonsoft.Json;

namespace BusinessLibrary.Models
{
    public class TaskList
    {
        public TaskList(int projectId, string name, DateTime? dateCreated, int position)
        {
            this.projectId = projectId;
            this.name = name;
            this.dateCreated = dateCreated;
            this.position = position;
        }

        [JsonProperty]
        public int projectId { get; set; }
        [JsonProperty]
        public string name { get; set; }
        [JsonProperty]
        public int position { get; set; }

        [JsonProperty]
        public DateTime? dateCreated { get; set; }
    }
}
