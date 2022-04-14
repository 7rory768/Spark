using Newtonsoft.Json;

namespace BusinessLibrary.Models
{
    public class TaskList
    {
        public TaskList(int projectId, string name, DateTime? dateCreated)
        {
            this.projectId = projectId;
            this.name = name;
            this.dateCreated = dateCreated;
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
