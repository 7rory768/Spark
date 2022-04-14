using Newtonsoft.Json;

namespace BusinessLibrary.Models
{
    public class Task
    {
        public Task(int projectId, string listName, string name, string description, DateTime? dateCreated, int priority, DateOnly? deadline, bool completed)
        {
            this.projectId = projectId;
            this.listName = listName;
            this.name = name;
            this.description = description;
            this.dateCreated = dateCreated;
            this.priority = priority;
            this.deadline = deadline;
            this.completed = completed;
        }

        [JsonProperty]
        public int projectId { get; set; }
        [JsonProperty]
        public string listName { get; set; }
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
    }
}
