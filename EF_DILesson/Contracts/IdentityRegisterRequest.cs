using System.ComponentModel.DataAnnotations;

namespace EF_DILesson.Contracts
{
    public class IdentityRegisterRequest
    {
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Phone { get; set; }
        public string PhotoPath { get; set; }
        public int Age { get; set; }
    }
}