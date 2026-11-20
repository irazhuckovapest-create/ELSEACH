using System.Linq.Expressions;
using EF_DILesson.Models;

namespace EF_DILesson.Interfaces
{
    public interface IFilmRepository
    {
        List<Film> GetAll();
        Film GetById(int id);
        void Add(Film film);
        void Update(Film film);
        bool Delete(int id);
        List<Film> SearchByCondition(Expression<Func<Film, bool>> predicate);
    }
}