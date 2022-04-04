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

        public string username { get; set; }
        public string fName { get; set; }
        public string? lName { get; set; }
        public string password { get; set; }

        public string? email { get; set; }
        public DateTime dateCreated { get; set; }
        public string userType { get; set; }
    }

    public class UserDBO
    {
        public UserDBO(User user)
        {
            this.username = user.username;
            this.fName = user.fName;
            this.lName = user.lName;
            this.email = user.email;
            this.dateCreated = user.dateCreated;
            this.userType = user.userType;
        }

        public string username { get; set; }
        public string fName { get; set; }
        public string lName { get; set; }

        public string email { get; set; }
        public DateTime dateCreated { get; set; }
        public string userType { get; set; }
    }
}
