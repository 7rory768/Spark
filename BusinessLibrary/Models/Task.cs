using System;
using Newtonsoft.Json;

namespace BusinessLibrary.Models
{
    public class Task
    {
        public Task(int id, int projectId, int listId, string name, string description, DateTime? dateCreated, int priority, DateOnly? deadline, bool completed, int completionPoints)
        {
            this.id = id;
            this.projectId = projectId;
            this.listId = listId;
            this.name = name;
            this.description = description;
            this.dateCreated = dateCreated;
            this.priority = priority;
            this.deadline = deadline;
            this.completed = completed;
            this.completionPoints = completionPoints;
        }

        [JsonProperty]
        public int id { get; set; }
        [JsonProperty]
        public int projectId { get; set; }
        [JsonProperty]
        public int listId { get; set; }
        [JsonProperty]
        public string name { get; set; }
        [JsonProperty]
        public string description { get; set; }
        [JsonProperty]
        public DateTime? dateCreated { get; set; }
        [JsonProperty]
        public int priority { get; set; }
        [JsonProperty]
        public DateOnly? deadline { get; set; }
        [JsonProperty]
        public bool completed { get; set; }
        [JsonProperty]
        public int completionPoints { get; set; }
        [JsonProperty]
        public List<User>? assignedUsers { get; set; }
    }
}
