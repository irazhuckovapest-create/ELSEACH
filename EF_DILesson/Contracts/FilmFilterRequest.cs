namespace EF_DILesson.Contracts
{
    public class FilmFilterRequest
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
        public string? Genre { get; set; }
        public int? Year { get; set; }
        public string? Country { get; set; }
        public string? Keyword { get; set; }
    }
}