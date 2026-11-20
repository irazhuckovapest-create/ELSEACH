namespace EF_DILesson.Models
{
    public class RegistrationResult
    {
        public bool Success { get; set; }
        public string? UserId { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}