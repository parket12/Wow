namespace Yakshin.Models
{
    public record class Users
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Passwords { get; set; }
        public int ID_Roles { get; set; }
    }
}
