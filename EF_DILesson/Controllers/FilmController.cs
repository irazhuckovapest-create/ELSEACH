using AutoMapper;
using EF_DILesson.Contracts;
using EF_DILesson.Dto;
using EF_DILesson.Interfaces;
using EF_DILesson.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace EF_DILesson.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilmController : ControllerBase
    {
        private readonly IFilmService _filmService;
        private readonly IValidator<CreateMovieRequest> _createValidator;
        private readonly IValidator<UpdateMovieRequest> _updateValidator;
        private readonly IValidator<FilmFilterRequest> _filterValidator;
        private readonly IMapper _mapper;

        public FilmController(
            IFilmService filmService,
            IValidator<CreateMovieRequest> createValidator,
            IValidator<UpdateMovieRequest> updateValidator,
            IValidator<FilmFilterRequest> filterValidator,
            IMapper mapper)
        {
            _filmService = filmService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _filterValidator = filterValidator;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<List<MovieResponse>> GetAll()
        {
            var films = _filmService.GetAllFilms();
            return Ok(_mapper.Map<List<MovieResponse>>(films));
        }

        [HttpGet("search")]
        public ActionResult<List<MovieResponse>> Search([FromQuery] FilmFilterRequest request)
        {
            var res = _filterValidator.Validate(request);
            if (!res.IsValid) return BadRequest(res.Errors);

            var filter = _mapper.Map<FilmFilterDto>(request);
            var films = _filmService.SearchFilms(filter);

            return Ok(_mapper.Map<List<MovieResponse>>(films));
        }

        [HttpGet("{id}")]
        public ActionResult<MovieResponse> GetById(int id)
        {
            Film film = _filmService.GetFilmById(id);
            if (film == null) return NotFound("Фільм з id " + id + " не знайдено");
            return Ok(_mapper.Map<MovieResponse>(film));
        }

        [HttpPost]
        public ActionResult<MovieResponse> Create(CreateMovieRequest request)
        {
            var res = _createValidator.Validate(request);
            if (!res.IsValid) return BadRequest(res.Errors);

            Film film = _mapper.Map<Film>(request);

            _filmService.AddFilm(film);
            return CreatedAtAction(nameof(GetById), new { id = film.Id }, _mapper.Map<MovieResponse>(film));
        }

        [HttpPut("{id}")]
        public ActionResult<MovieResponse> Update(int id, UpdateMovieRequest request)
        {
            var res = _updateValidator.Validate(request);
            if (!res.IsValid) return BadRequest(res.Errors);

            Film newData = _mapper.Map<Film>(request);

            Film film = _filmService.UpdateFilm(id, newData);
            if (film == null) return NotFound("Фільм з id " + id + " не знайдено");
            return Ok(_mapper.Map<MovieResponse>(film));
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            bool deleted = _filmService.DeleteFilm(id);
            if (!deleted) return NotFound("Фільм з id " + id + " не знайдено");
            return NoContent();
        }
    }
}