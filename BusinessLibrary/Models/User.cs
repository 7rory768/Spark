namespace BusinessLibrary.Models
{

    public enum UserType
    {
        EMPLOYEE,
        MANAGER
    }

    public static class UserTypeExtensions
    {
        public static UserType parse(string arg)
        {
            return (UserType)Enum.Parse(typeof(UserType), arg);
        }

    }

    public class User
    {
        public User(string username, string fName, string lName, string password, string email, DateTime dateCreated, UserType userType)
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
        public UserType userType { get; set; }
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
        public UserType userType { get; set; }
    }
}
