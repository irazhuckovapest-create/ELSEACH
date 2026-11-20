using EF_DILesson;
using EF_DILesson.Interfaces;
using EF_DILesson.MapperProfile;
using EF_DILesson.Models;
using EF_DILesson.Repository;
using EF_DILesson.Service;
using EF_DILesson.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;
    options.AddSchemaTransformer((schema, context, cancellationToken) =>
    {
        if (context.JsonTypeInfo.Type == typeof(int) || context.JsonTypeInfo.Type == typeof(long))
        {
            schema.Type = Microsoft.OpenApi.JsonSchemaType.Integer;
            schema.Format = "int32";
            schema.AnyOf?.Clear();
            schema.OneOf?.Clear();
        }
        return Task.CompletedTask;
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentityCore<User>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddTransient<IdentityAuthService>();

builder.Services.AddScoped<IFilmRepository, FilmRepository>();
builder.Services.AddScoped<IFilmService, FilmService>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ICommentService, CommentService>();

builder.Services.AddValidatorsFromAssemblyContaining<CreateMovieRequestValidator>();

builder.Services.AddAutoMapper(opt =>
{
    opt.AddProfile<RequestResponseProfile>();
    opt.AddProfile<DtoMapperProfile>();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:5174")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.MapOpenApi();
app.UseSwaggerUI(o => o.SwaggerEndpoint("/openapi/v1.json", "MovieApi"));

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();

    if (!context.Films.Any())
    {
        context.Films.Add(new Film
        {
            Title = "Бабадук",
            Description = "Мати-одиначка та її син стикаються з моторошною істотою після появи загадкової книжки.",
            PosterUrl = "https://media.themoviedb.org/t/p/w220_and_h330_face/fF8wVHBiOhCS5LE48GlNnct63f2.jpg",
            MovieUrl = "https://example.com/babadook",
            ReleaseYear = 2014,
            Rating = 5.8
        });

        context.Films.Add(new Film
        {
            Title = "Аннабель",
            Description = "Подружя, що чекає на дитину, опиняється в центрі жахів через стару колекційну ляльку.",
            PosterUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRau7zkJ7YTJ3M3i5RTBkQnkeI4CfxZMwfWKQ_y8oMT2g&s",
            MovieUrl = "https://example.com/annabelle",
            ReleaseYear = 2014,
            Rating = 6.4
        });

        context.Films.Add(new Film
        {
            Title = "Пила",
            Description = "Двоє незнайомців прокидаються в пастці та мають зіграти в жорстоку гру, щоб вижити.",
            PosterUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSdmOl0Mldcy8rqQFnh8QDcq2EC9hYNmHUHKuWXAFNB0A&s=10",
            MovieUrl = "https://example.com/saw",
            ReleaseYear = 2004,
            Rating = 7.6
        });

        context.Films.Add(new Film
        {
            Title = "Обсесія",
            Description = "Хорор про одержимість, яка поступово руйнує життя героїв.",
            PosterUrl = "https://cdnrrviubnyuntopbez5.svc.cdn.yandex.net/get-kinopoisk-image/10900341/3e447310-fffb-4a7d-b6cf-1a003b924601/576x",
            MovieUrl = "https://example.com/obsession",
            ReleaseYear = 2026,
            Rating = 2.5
        });

        context.Films.Add(new Film
        {
            Title = "3 метра над рівнем неба",
            Description = "Бунтівний хлопець і вихована дівчина з багатої родини закохуються, попри різні світи.",
            PosterUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTGah_FGoOeru2_Sq1evwn3r_QZyTzo9JLCo7kvnAhNMnrgFLihi6z0gHhQ&s=10",
            MovieUrl = "https://example.com/three-meters",
            ReleaseYear = 2010,
            Rating = 9.2
        });

        context.SaveChanges();
    }

    if (!context.Films.Any(f => f.Title == "Інша мама"))
    {
        context.Films.Add(new Film
        {
            Title = "Інша мама",
            Description = "Напружений психологічний трилер про таємниці минулого.",
            PosterUrl = "https://m.media-amazon.com/images/M/MV5BOGUyODYyYTctMmY0Mi00ZWIwLThjYWItZDJmODY4NGU1YjY1XkEyXkFqcGc@._V1_FMjpg_UX1000_.jpg",
            MovieUrl = "https://example.com/insha-mama",
            ReleaseYear = 2026,
            Rating = 7.4
        });

        context.SaveChanges();
    }

    var extraFilms = new List<Film>
    {
        new Film
        {
            Title = "Пасажири",
            Description = "Космічний корабель перевозить тисячі людей на нову планету, але через збій двоє пасажирів прокидаються на 90 років раніше.",
            PosterUrl = "https://upload.wikimedia.org/wikipedia/ru/d/d0/%D0%9F%D0%B0%D1%81%D1%81%D0%B0%D0%B6%D0%B8%D1%80%D1%8B_%28%D0%BF%D0%BE%D1%81%D1%82%D0%B5%D1%80_%D1%84%D0%B8%D0%BB%D1%8C%D0%BC%D0%B0%2C_2016%29.jpg?utm_source=ru.wikipedia.org&utm_campaign=index&utm_content=original",
            MovieUrl = "https://example.com/passengers",
            ReleaseYear = 2016,
            Rating = 7.0
        },
        new Film
        {
            Title = "Інтерстеллар",
            Description = "Команда дослідників вирушає крізь червоточину в космосі, щоб знайти новий дім для людства.",
            PosterUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS58xqOmddVJT6oT94e65UHgibI_ZHyFzNgOvTPQ7r53j8akZSdekuIqAc&s=10",
            MovieUrl = "https://example.com/interstellar",
            ReleaseYear = 2014,
            Rating = 8.7
        },
        new Film
        {
            Title = "Острів проклятих",
            Description = "Двоє судових приставів вирушають на віддалений острів у Массачусетсі, щоб розслідувати зникнення пацієнтки клініки для душевнохворих.",
            PosterUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQSn9tmMYi5gtMkEw9qlAE66i-ba8o1mo37WUgSJf-LUw&s=10",
            MovieUrl = "https://example.com/shutter-island",
            ReleaseYear = 2010,
            Rating = 8.2
        },
        new Film
        {
            Title = "Зникла",
            Description = "На п'яту річницю весілля дружина безслідно зникає, а всі докази починають вказувати на її чоловіка.",
            PosterUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRd9lAqMykhtyY2NR09UrBkvD30CJ2Flt51d6Ms5nzrC3XvUpPsD_WcmSHK&s=10",
            MovieUrl = "https://example.com/gone-girl",
            ReleaseYear = 2014,
            Rating = 8.1
        }
    };

    foreach (var film in extraFilms)
    {
        if (!context.Films.Any(f => f.Title == film.Title))
        {
            context.Films.Add(film);
        }
    }

    context.SaveChanges();

    var genreData = new (string Title, string Genre, string Country)[]
    {
        ("Бабадук", "Жахи", "Австралія"),
        ("Аннабель", "Жахи", "США"),
        ("Пила", "Жахи", "США"),
        ("Обсесія", "Жахи", "США"),
        ("3 метра над рівнем неба", "Мелодрама", "Іспанія"),
        ("Інша мама", "Трилер", "США"),
        ("Пасажири", "Фантастика", "США"),
        ("Інтерстеллар", "Фантастика", "США"),
        ("Острів проклятих", "Трилер", "США"),
        ("Зникла", "Трилер", "США"),
    };

    foreach (var g in genreData)
    {
        var film = context.Films.FirstOrDefault(f => f.Title == g.Title && string.IsNullOrEmpty(f.Genre));
        if (film != null)
        {
            film.Genre = g.Genre;
            film.Country = g.Country;
        }
    }

    context.SaveChanges();
}

app.UseHttpsRedirection();
app.UseCors("AllowReact");
app.UseAuthorization();
app.MapControllers();
app.Run();