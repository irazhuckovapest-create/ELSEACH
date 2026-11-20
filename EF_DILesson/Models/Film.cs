namespace EF_DILesson.Models
{
    public class Film
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Genre { get; set; } = "";
        public string Country { get; set; } = "";
        public string PosterUrl { get; set; } = "";
        public string MovieUrl { get; set; } = "";
        public int ReleaseYear { get; set; }
        public double Rating { get; set; }

        public List<MovieComment> Comments { get; set; } = new List<MovieComment>();
    }
}