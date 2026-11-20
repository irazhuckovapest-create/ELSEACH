namespace EF_DILesson.Contracts
{
    public class UserResponce
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string PhotoPath { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}