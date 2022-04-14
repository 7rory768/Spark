using System;
using Newtonsoft.Json;

namespace BusinessLibrary.Models
{
    public class Project
    {
        public Project(int id, int teamId, string name, int budget, DateTime dateCreated)
        {
            this.id = id;
            this.teamId = teamId;
            this.name = name;
            this.budget = budget;
            this.dateCreated = dateCreated;
        }

        [JsonProperty]
        public int id { get; set; }
        [JsonProperty]
        public int teamId { get; set; }
        [JsonProperty]
        public string name { get; set; }
        [JsonProperty]
        public int budget { get; set; }
        [JsonProperty]
        public DateTime dateCreated { get; set; }
    }
}
