using System.Linq.Expressions;
using EF_DILesson.Dto;
using EF_DILesson.Interfaces;
using EF_DILesson.Models;

namespace EF_DILesson.Service
{
    public class FilmService : IFilmService
    {
        private readonly IFilmRepository _filmRepository;

        public FilmService(IFilmRepository filmRepository)
        {
            _filmRepository = filmRepository;
        }

        public List<Film> GetAllFilms()
        {
            return _filmRepository.GetAll();
        }

        public Film GetFilmById(int id)
        {
            return _filmRepository.GetById(id);
        }

        public void AddFilm(Film film)
        {
            _filmRepository.Add(film);
        }

        public Film UpdateFilm(int id, Film newData)
        {
            Film film = _filmRepository.GetById(id);
            if (film == null) return null;

            film.Title = newData.Title;
            film.Description = newData.Description;
            film.PosterUrl = newData.PosterUrl;
            film.Genre = newData.Genre;
            film.Country = newData.Country;
            film.MovieUrl = newData.MovieUrl;
            film.ReleaseYear = newData.ReleaseYear;
            film.Rating = newData.Rating;

            _filmRepository.Update(film);
            return film;
        }

        public bool DeleteFilm(int id)
        {
            return _filmRepository.Delete(id);
        }

        public List<Film> SearchFilms(FilmFilterDto filter)
        {
            int? id = filter.Id;
            int? year = filter.Year;
            string title = filter.Title?.Trim() ?? "";
            string genre = filter.Genre?.Trim() ?? "";
            string country = filter.Country?.Trim() ?? "";
            string keyword = filter.Keyword?.Trim() ?? "";
            Expression<Func<Film, bool>> predicate = f =>
                (id == null || f.Id == id) &&
                (title == "" || f.Title.Contains(title)) &&
                (genre == "" || f.Genre == genre) &&
                (year == null || f.ReleaseYear == year) &&
                (country == "" || f.Country == country) &&
                (keyword == "" || f.Title.Contains(keyword) || f.Description.Contains(keyword));

            return _filmRepository.SearchByCondition(predicate);
        }
    }
}