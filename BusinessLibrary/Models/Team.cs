using Newtonsoft.Json;

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

        [JsonProperty]
        public int id { get; set; }
        [JsonProperty]
        public string name { get; set; }
        [JsonProperty]
        public string mgrUsername { get; set; }
    }
}
