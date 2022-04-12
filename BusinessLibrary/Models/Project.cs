namespace BusinessLibrary.Models
{
    public class Project
    {
        public Project(int id, int teamId, string name, int budget, DateTime dateCreated, string mgrUsername)
        {
            this.id = id;
            this.teamId = teamId;
            this.name = name;
            this.budget = budget;
            this.dateCreated = dateCreated;
            this.mgrUsername = mgrUsername;
        }

        public int id { get; set; }
        public int teamId { get; set; }
        public string name { get; set; }
        public int budget { get; set; }
        public DateTime dateCreated { get; set; }
        public string mgrUsername { get; set; }
    }
}
