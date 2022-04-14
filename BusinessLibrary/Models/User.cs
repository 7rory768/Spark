using Newtonsoft.Json;

namespace BusinessLibrary.Models
{
    public class User
    {
        public User(string username, string fName, string lName, string password, string email, DateTime dateCreated)
        {
            this.username = username;
            this.fName = fName;
            this.lName = lName;
            this.password = password;
            this.email = email;
            this.dateCreated = dateCreated;
        }

        [JsonProperty]
        public string username { get; set; }
        [JsonProperty]
        public string fName { get; set; }
        [JsonProperty]
        public string? lName { get; set; }
        private string password { get; set; }

        [JsonProperty]
        public string? email { get; set; }
        [JsonProperty]
        public DateTime dateCreated { get; set; }
    }
}
