import { useState, useEffect, useRef } from "react";

const API = "https://localhost:7150/api";

const DEFAULT_GENRES = [
    "Жахи",
    "Мелодрама",
    "Трилер",
    "Фантастика",
    "Комедія",
    "Драма",
    "Бойовик",
];

const noImage =
    "data:image/svg+xml;utf8,<svg xmlns='http://www.w3.org/2000/svg' width='300' height='450'><rect width='100%' height='100%' fill='%23141416'/><text x='50%' y='50%' fill='%238d8a83' font-family='sans-serif' font-size='16' text-anchor='middle'>No Poster</text></svg>";

function Films({ onLogout }) {
    const [films, setFilms] = useState([]);
    const [genres, setGenres] = useState(DEFAULT_GENRES);
    const [listError, setListError] = useState("");
    const [searched, setSearched] = useState(false);

    // поля поиска
    const [fId, setFId] = useState("");
    const [fTitle, setFTitle] = useState("");
    const [fGenre, setFGenre] = useState("");
    const [fYear, setFYear] = useState("");
    const [fCountry, setFCountry] = useState("");
    const [fKeyword, setFKeyword] = useState("");

    // форма добавления
    const [showForm, setShowForm] = useState(false);
    const [title, setTitle] = useState("");
    const [description, setDescription] = useState("");
    const [genre, setGenre] = useState("");
    const [country, setCountry] = useState("");
    const [posterUrl, setPosterUrl] = useState("");
    const [movieUrl, setMovieUrl] = useState("");
    const [releaseYear, setReleaseYear] = useState("");
    const [rating, setRating] = useState("");
    const [message, setMessage] = useState("");

    const cardsGridRef = useRef(null);
    const pointerRef = useRef({ x: null, y: null, active: false });
    const cardStatesRef = useRef(new Map());

    // отправляет на бэкенд только заполненные поля
    async function searchFilms(params) {
        const query = new URLSearchParams();
        for (const key in params) {
            const value = String(params[key]).trim();
            if (value !== "") query.append(key, value);
        }

        try {
            const response = await fetch(API + "/Film/search?" + query.toString());

            if (!response.ok) {
                const data = await response.json().catch(() => null);
                let text = "Не вдалося виконати пошук";
                if (Array.isArray(data)) {
                    text = data.map((e) => e.errorMessage).join(". ");
                }
                setListError(text);
                return;
            }

            const data = await response.json();
            setFilms(data);
            setListError("");
            setSearched(query.toString() !== "");
        } catch (error) {
            setListError("Сервер недоступний. Не вдалося завантажити фільми");
        }
    }

    function currentFilter() {
        return {
            id: fId,
            title: fTitle,
            genre: fGenre,
            year: fYear,
            country: fCountry,
            keyword: fKeyword,
        };
    }

    function handleSearch() {
        searchFilms(currentFilter());
    }

    function handleReset() {
        setFId("");
        setFTitle("");
        setFGenre("");
        setFYear("");
        setFCountry("");
        setFKeyword("");
        searchFilms({});
    }

    // при открытии страницы: загружаем все фильмы и объединяем готовые жанры с жанрами из БД
    useEffect(() => {
        async function init() {
            try {
                const response = await fetch(API + "/Film/search");
                if (!response.ok) {
                    setListError("Не вдалося завантажити фільми");
                    return;
                }
                const data = await response.json();
                setFilms(data);
                const dbGenres = data.map((f) => f.genre).filter((g) => g);
                const unique = [...new Set([...DEFAULT_GENRES, ...dbGenres])];
                setGenres(unique);
            } catch (error) {
                setListError("Сервер недоступний. Не вдалося завантажити фільми");
            }
        }
        init();
    }, []);

    // плавная анимация наклона и подсветки карточек через requestAnimationFrame
    useEffect(() => {
        let rafId;
        // чем меньше коэффициент (0.075), тем медленнее, мягче и спокойнее движение
        const ease = 0.075;

        function animate() {
            const grid = cardsGridRef.current;
            const { x: px, y: py, active } = pointerRef.current;

            if (grid) {
                const cardEls = grid.querySelectorAll(".card");

                cardEls.forEach((el, idx) => {
                    if (!cardStatesRef.current.has(idx)) {
                        cardStatesRef.current.set(idx, {
                            rx: 0,
                            ry: 0,
                            press: 0,
                            spotX: 50,
                            spotY: 50,
                        });
                    }
                    const st = cardStatesRef.current.get(idx);

                    let targetRx = 0;
                    let targetRy = 0;
                    let targetPress = 0;
                    let targetSpotX = 50;
                    let targetSpotY = 50;

                    if (active && px !== null && py !== null) {
                        const r = el.getBoundingClientRect();
                        const cx = r.left + r.width / 2;
                        const cy = r.top + r.height / 2;

                        // расстояние от курсора до центра карточки
                        const dx = px - cx;
                        const dy = py - cy;

                        // радиус мягкого влияния (охватывает текущую карточку и плавно задевает соседнюю)
                        const maxDist = Math.max(r.width, r.height) * 1.15;
                        const dist = Math.hypot(dx, dy);

                        if (dist < maxDist) {
                            // плавная кривая затухания (smoothstep) от 1 в центре до 0 на краю радиуса
                            const t = 1 - dist / maxDist;
                            const influence = t * t * (3 - 2 * t);

                            const nx = Math.max(-1.2, Math.min(1.2, dx / (r.width * 0.5)));
                            const ny = Math.max(-1.2, Math.min(1.2, dy / (r.height * 0.5)));

                            // очень мягкий наклон на продавливание (максимум ~3.2 град)
                            targetRy = nx * 3.2 * influence;
                            targetRx = -ny * 3.2 * influence;
                            targetPress = influence;

                            targetSpotX = Math.max(0, Math.min(100, ((px - r.left) / r.width) * 100));
                            targetSpotY = Math.max(0, Math.min(100, ((py - r.top) / r.height) * 100));
                        }
                    }

                    // плавная интерполяция (lerp) — убирает любые рывки при переходе между фильмами
                    st.rx += (targetRx - st.rx) * ease;
                    st.ry += (targetRy - st.ry) * ease;
                    st.press += (targetPress - st.press) * ease;
                    st.spotX += (targetSpotX - st.spotX) * (ease * 1.1);
                    st.spotY += (targetSpotY - st.spotY) * (ease * 1.1);

                    const scale = 1 - st.press * 0.012; // едва уловимое утапливание до 0.988

                    el.style.setProperty("--rx", st.rx.toFixed(3) + "deg");
                    el.style.setProperty("--ry", st.ry.toFixed(3) + "deg");
                    el.style.setProperty("--scale", scale.toFixed(4));
                    el.style.setProperty("--press", st.press.toFixed(3));
                    el.style.setProperty("--px", st.spotX.toFixed(1) + "%");
                    el.style.setProperty("--py", st.spotY.toFixed(1) + "%");
                });
            }

            rafId = requestAnimationFrame(animate);
        }

        rafId = requestAnimationFrame(animate);
        return () => cancelAnimationFrame(rafId);
    }, []);

    function handleGridMove(e) {
        pointerRef.current.x = e.clientX;
        pointerRef.current.y = e.clientY;
        pointerRef.current.active = true;
    }

    function handleGridLeave() {
        pointerRef.current.active = false;
    }

    async function addFilm() {
        try {
            const response = await fetch(API + "/Film", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    title: title,
                    description: description,
                    genre: genre,
                    country: country,
                    posterUrl: posterUrl,
                    movieUrl: movieUrl,
                    releaseYear: Number(releaseYear),
                    rating: Number(rating),
                }),
            });

            if (response.ok) {
                if (genre.trim()) {
                    setGenres((prev) => [...new Set([...prev, genre.trim()])]);
                }
                setTitle("");
                setDescription("");
                setGenre("");
                setCountry("");
                setPosterUrl("");
                setMovieUrl("");
                setReleaseYear("");
                setRating("");
                setShowForm(false);
                setMessage("");
                handleSearch();
            } else {
                const data = await response.json();
                let text = "";
                if (Array.isArray(data)) {
                    text = data.map((e) => e.errorMessage).join(" ");
                }
                setMessage(text || "Не вдалося додати фільм. Перевірте дані");
            }
        } catch (error) {
            setMessage("Сервер недоступний");
        }
    }

    async function removeFilm(id) {
        if (!window.confirm("Видалити фільм?")) return;
        try {
            const response = await fetch(API + "/Film/" + id, { method: "DELETE" });
            if (!response.ok) {
                setListError("Не вдалося видалити фільм");
                return;
            }
            handleSearch();
        } catch (error) {
            setListError("Не вдалося видалити фільм");
        }
    }

    return (
        <div className="films-page">
            <header className="top">
                <div className="brand">
                    <span className="brand-subtitle">PRIVATE CINEMA</span>
                    <h1>My Films</h1>
                </div>
                <div className="top-buttons">
                    <button onClick={() => setShowForm(!showForm)}>
                        {showForm ? "Close Form" : "+ Add Film"}
                    </button>
                    <button className="logout-btn" onClick={onLogout}>
                        Logout
                    </button>
                </div>
            </header>

            {/* ---------- ПОИСК ---------- */}
            <section className="search-panel">
                <h3>Пошук фільмів</h3>
                <div className="search-grid">
                    <input
                        type="number"
                        placeholder="ID"
                        value={fId}
                        onChange={(e) => setFId(e.target.value)}
                    />
                    <input
                        placeholder="Назва"
                        value={fTitle}
                        onChange={(e) => setFTitle(e.target.value)}
                    />
                    <select value={fGenre} onChange={(e) => setFGenre(e.target.value)}>
                        <option value="">Всі жанри</option>
                        {genres.map((g) => (
                            <option key={g} value={g}>{g}</option>
                        ))}
                    </select>
                    <input
                        type="number"
                        placeholder="Рік випуску"
                        value={fYear}
                        onChange={(e) => setFYear(e.target.value)}
                    />
                    <input
                        placeholder="Країна"
                        value={fCountry}
                        onChange={(e) => setFCountry(e.target.value)}
                    />
                    <input
                        placeholder="Ключове слово (назва або опис)"
                        value={fKeyword}
                        onChange={(e) => setFKeyword(e.target.value)}
                        onKeyDown={(e) => e.key === "Enter" && handleSearch()}
                    />
                </div>
                <div className="search-actions">
                    <button onClick={handleSearch}>Шукати</button>
                    <button className="logout-btn" onClick={handleReset}>Скинути</button>
                </div>
                <p className="search-count">Знайдено фільмів: {films.length}</p>
            </section>

            {/* ---------- ФОРМА ДОБАВЛЕНИЯ ---------- */}
            {showForm && (
                <div className="add-form">
                    <h3>Новий фільм</h3>
                    <input placeholder="Назва фільму (Title)" value={title}
                        onChange={(e) => setTitle(e.target.value)} />
                    <input placeholder="Опис (Description)" value={description}
                        onChange={(e) => setDescription(e.target.value)} />
                    <div className="form-row">
                        <input placeholder="Жанр" value={genre}
                            onChange={(e) => setGenre(e.target.value)} />
                        <input placeholder="Країна" value={country}
                            onChange={(e) => setCountry(e.target.value)} />
                    </div>
                    <input placeholder="Посилання на постер (https://...)" value={posterUrl}
                        onChange={(e) => setPosterUrl(e.target.value)} />
                    <input placeholder="Посилання на фільм (https://...)" value={movieUrl}
                        onChange={(e) => setMovieUrl(e.target.value)} />
                    <div className="form-row">
                        <input type="number" placeholder="Рік випуску" value={releaseYear}
                            onChange={(e) => setReleaseYear(e.target.value)} />
                        <input type="number" step="0.1" placeholder="Рейтинг (0 - 10)" value={rating}
                            onChange={(e) => setRating(e.target.value)} />
                    </div>
                    <button onClick={addFilm}>Зберегти фільм</button>
                    {message && <p className="error">{message}</p>}
                </div>
            )}

            {listError && <p className="error">{listError}</p>}

            {/* ---------- КАРТОЧКИ ---------- */}
            <div
                className="cards"
                ref={cardsGridRef}
                onMouseMove={handleGridMove}
                onMouseLeave={handleGridLeave}
            >
                {films.length === 0 && !listError && (
                    <p className="empty">
                        {searched
                            ? "Нічого не знайдено. Змініть параметри пошуку"
                            : "Фільмів поки немає. Натисни + Add Film"}
                    </p>
                )}

                {films.map((film) => (
                    <article className="card" key={film.id}>
                        <div className="card-image">
                            <img
                                src={film.posterUrl}
                                alt={film.title}
                                referrerPolicy="no-referrer"
                                onError={(e) => {
                                    e.target.onerror = null;
                                    e.target.src = noImage;
                                }}
                            />
                            <div className="card-overlay"></div>
                            {film.rating > 0 && (
                                <span className="rating-badge">★ {film.rating}</span>
                            )}
                            {film.releaseYear > 0 && (
                                <span className="year-badge">{film.releaseYear}</span>
                            )}
                        </div>
                        <div className="card-body">
                            <h3>{film.title}</h3>
                            <p className="meta">
                                #{film.id}
                                {film.genre ? " · " + film.genre : ""}
                                {film.country ? " · " + film.country : ""}
                            </p>
                            <p>{film.description}</p>
                            <div className="card-actions">
                                <a className="watch-link" href={film.movieUrl}
                                    target="_blank" rel="noreferrer">
                                    Дивитися
                                </a>
                                <button className="remove-btn"
                                    onClick={() => removeFilm(film.id)}
                                    title="Видалити фільм">
                                    Видалити
                                </button>
                            </div>
                        </div>
                    </article>
                ))}
            </div>
        </div>
    );
}

export default Films;