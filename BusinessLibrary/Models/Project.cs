namespace BusinessLibrary.Models
{
    public class Project
    {
        public Project(int id, int teamId, string name, string mgrUsername)
        {
            this.id = id;
            this.teamId = teamId;
            this.name = name;
            this.mgrUsername = mgrUsername;
        }

        public int id { get; set; }
        public int teamId { get; set; }

        public string name { get; set; }
        public string mgrUsername { get; set; }
    }
}
