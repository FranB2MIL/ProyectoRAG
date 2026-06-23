import { useNavigate } from "react-router-dom"
import { useState } from "react"

function LandingPage() {
  const navigate = useNavigate()
  const [open, setOpen] = useState(false)

  return (
    <div className="landing">
      <div className="corner corner-tl"></div>
      <div className="corner corner-tr"></div>
      <div className="corner corner-bl"></div>
      <div className="corner corner-br"></div>

      <img src="/ghost2.png" alt="Ghost" className="landing-ghost" />

      <h1 className="landing-title">Grimoires of Sol</h1>
      <div className="landing-divider"></div>
      <p className="landing-subtitle">A Destiny Lore Archive</p>
      <div className="landing-divider"></div>

      <p className="landing-desc">
        The history of this system spans billions of years — the Collapse, the Darkness,
        the Hive, the Vex, and everything in between. Ask what you've always wanted to know.
      </p>

      <div className="landing-divider"></div>

      <button className="landing-btn" onClick={() => navigate("/chat")}>
        Enter the Archives
      </button>

      <div className="landing-divider"></div>

      <div className="landing-faq">
        <button className="faq-toggle" onClick={() => setOpen(!open)}>
          <span>{open ? "▲" : "▼"}</span> How does this work?
        </button>
        {open && (
          <div className="faq-content">
            <p>
              Grimoires of Sol uses a RAG (Retrieval-Augmented Generation) system — it searches
              a database of Destiny lore entries and uses AI to formulate the answer in character.
              It does not browse the internet nor does it have real-time information.
            </p>
            <p className="faq-subtitle">Expected limitations:</p>
            <ul>
              <li>For very broad questions, try being more specific — "What is the Hive's relationship with the Darkness?" works better than "Tell me everything about the Hive"</li>
              <li>It won't answer questions unrelated to Destiny lore</li>
              <li>Sources are the Complete Destiny Timeline documents — not every grimoire card or lore tab may be indexed</li>
              <li>Answers are generated, not copied — they may occasionally miss nuance on very obscure topics</li>
            </ul>
          </div>
        )}
      </div>
    </div>
  )
}

export default LandingPage