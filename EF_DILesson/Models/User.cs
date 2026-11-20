using Microsoft.AspNetCore.Identity;

namespace EF_DILesson.Models
{
    public class User : IdentityUser
    {
        public string DisplayName { get; set; }
        public string PhotoPath { get; set; }
        // public List<Film> OldFilm { get; set; }
    }
}