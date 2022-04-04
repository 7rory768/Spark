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

        public int projectId { get; set; }
        public string listName { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public DateTime? dateCreated { get; set; }
        public int priority { get; set; }
        public DateOnly? deadline { get; set; }
        public bool completed { get; set; }
    }
}
