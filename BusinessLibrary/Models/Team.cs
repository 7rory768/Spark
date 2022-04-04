namespace BusinessLibrary.Models
{
    public class Team
    {
        public Team(int id, string name, string mgrUsername)
        {
            this.id = id;
            this.name = name;
            this.mgrUsername = mgrUsername;
        }

        public int id { get; set; }
        public string name { get; set; }
        public string mgrUsername { get; set; }
    }
}
