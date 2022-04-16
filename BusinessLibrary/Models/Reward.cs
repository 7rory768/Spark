using System;
using Newtonsoft.Json;

namespace BusinessLibrary.Models
{
    public class Reward
    {
        public Reward(string username, int numPoints, int teamId, int projectId, DateTime dateGiven)
        {
            this.username = username;
            this.numPoints = numPoints;
            this.teamId = teamId;
            this.projectId = projectId;
            this.dateGiven = dateGiven;
        }

        [JsonProperty]
        public string username { get; set; }
        [JsonProperty]
        public int numPoints { get; set; }
        [JsonProperty]
        public DateTime dateGiven { get; set; }
        [JsonProperty]
        public int teamId { get; set; }
        [JsonProperty]
        public int projectId { get; set; }
    }
}
