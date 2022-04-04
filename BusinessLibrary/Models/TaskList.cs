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

        public int projectId { get; set; }
        public string name { get; set; }
        public DateTime? dateCreated { get; set; }
    }
}
