namespace BusinessLibrary.Models
{
    public class Reward
    {
        public Reward(string username, int numPoints, DateTime dateGiven)
        {
            this.username = username;
            this.numPoints = numPoints;
            this.dateGiven = dateGiven;
        }

        public string username { get; set; }
        public int numPoints { get; set; }
        public DateTime dateGiven { get; set; }
    }
}
