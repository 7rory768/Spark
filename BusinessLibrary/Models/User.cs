using Newtonsoft.Json;

namespace BusinessLibrary.Models
{

    public static class UserType
    {
        public const string EMPLOYEE = "EMPLOYEE";
        public const string MANAGER = "MANAGER";

        public static bool isValid(string? arg)
        {
            return arg != null && (arg.Equals(EMPLOYEE) || arg.Equals(MANAGER));
        }
    }

    public class User
    {
        public User(string username, string fName, string lName, string password, string email, DateTime dateCreated, string userType)
        {
            this.username = username;
            this.fName = fName;
            this.lName = lName;
            this.password = password;
            this.email = email;
            this.dateCreated = dateCreated;
            this.userType = userType;
        }

        [JsonProperty]
        public string username { get; set; }
        [JsonProperty]
        public string fName { get; set; }
        [JsonProperty]
        public string? lName { get; set; }
        public string password { get; set; }

        [JsonProperty]
        public string? email { get; set; }
        [JsonProperty]
        public DateTime dateCreated { get; set; }
        [JsonProperty]
        public string userType { get; set; }
    }
}
