import { useNavigate } from "react-router-dom"
import GhostLoader from "./GhostLoader"

function Header() {
    const navigate = useNavigate()

    return (

        <header className="header">
            <div className="header-content">
                <GhostLoader size={72} />
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