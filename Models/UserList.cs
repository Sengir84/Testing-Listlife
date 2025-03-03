namespace ListLife.Models
{
    public class UserList
    {
        public int Id { get; set; }
        //Name of the list
        public string ListName { get; set; }
        //FK to user
        public string UserId { get; set; }
    }
}
