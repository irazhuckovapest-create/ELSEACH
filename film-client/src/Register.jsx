import { useState } from "react";

const API = "https://localhost:7150/api";

function Register({ goLogin }) {
    const [username, setUsername] = useState("");
    const [displayName, setDisplayName] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [message, setMessage] = useState("");
    const [isError, setIsError] = useState(false);

    async function handleRegister() {
        try {
            const response = await fetch(API + "/Auth/register", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ username, displayName, email, password, confirmPassword }),
            });

            if (response.ok) {
                setIsError(false);
                setMessage("Реєстрацію виконано успішно. Тепер увійдіть");
                return;
            }

            const data = await response.json();
            let text = "";
            if (Array.isArray(data)) {
                text = data.map((e) => (typeof e === "string" ? e : e.errorMessage)).join(". ");
            }
            setIsError(true);
            setMessage(text || "Перевірте правильність введених даних");
        } catch (error) {
            setIsError(true);
            setMessage("Сервер недоступний");
        }
    }

    return (
        <div className="box">
            <h2>REGISTER</h2>
            <input placeholder="Username (логін)" value={username}
                onChange={(e) => setUsername(e.target.value)} />
            <input placeholder="Display name" value={displayName}
                onChange={(e) => setDisplayName(e.target.value)} />
            <input placeholder="Email" value={email}
                onChange={(e) => setEmail(e.target.value)} />
            <input type="password" placeholder="Password" value={password}
                onChange={(e) => setPassword(e.target.value)} />
            <input type="password" placeholder="Confirm password" value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)} />
            <div className="buttons">
                <button onClick={handleRegister}>Register</button>
                <button onClick={goLogin}>Back to login</button>
            </div>
            {message && <p className={isError ? "error" : "success"}>{message}</p>}
        </div>
    );
}

export default Register;