import { useNavigate } from "react-router-dom"

function Header() {
    const navigate = useNavigate()

    return (

        <header className="header">
            <div className="header-content">
                <img src="/ghost2.png" alt="Ghost" className="header-ghost" />
                <div>
                    <h1 className="header-title">Grimoires of Sol</h1>
                    <p className="header-subtitle">Ask anything about the lore</p>
                </div>
            </div>
            <button className="header-exit-btn" onClick={() => navigate("/")}>
                ✕ Exit
            </button>
        </header>
    )
}

export default Header