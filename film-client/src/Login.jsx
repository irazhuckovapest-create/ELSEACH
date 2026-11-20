import { useState } from "react";

const API = "https://localhost:7150/api";

function Login({ onSuccess, goRegister }) {
    const [login, setLogin] = useState("");
    const [password, setPassword] = useState("");
    const [message, setMessage] = useState("");
    const [isError, setIsError] = useState(false);

    async function handleLogin() {
        try {
            const response = await fetch(API + "/Auth/login", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ login: login, password: password }),
            });

            if (!response.ok) {
                setIsError(true);
                setMessage("Перевірте дані (логін від 3, пароль від 6 символів)");
                return;
            }

            const isValid = await response.json();

            if (isValid === true) {
                setIsError(false);
                setMessage("Вхід виконано успішно");
                setTimeout(onSuccess, 1000);
            } else {
                setIsError(true);
                setMessage("Неправильний логін або пароль");
            }
        } catch (error) {
            setIsError(true);
            setMessage("Сервер недоступний");
        }
    }

    return (
        <div className="box">
            <h2>LOGIN</h2>
            <input
                placeholder="Login"
                value={login}
                onChange={(e) => setLogin(e.target.value)}
            />
            <input
                type="password"
                placeholder="Password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
            />
            <div className="buttons">
                <button onClick={handleLogin}>Login</button>
                <button onClick={goRegister}>Register</button>
            </div>
            {message && <p className={isError ? "error" : "success"}>{message}</p>}
        </div>
    );
}

export default Login;