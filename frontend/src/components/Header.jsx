function Header() {
  return (
    <header className="header">
      <div className="header-content">
        <img src="/ghost2.png" alt="Ghost" className="header-ghost" />
        <div>
          <h1 className="header-title">Grimoires of Sol</h1>
          <p className="header-subtitle">Ask anything about the lore</p>
        </div>
      </div>
       <img src="/grimoire.png" alt="Grimoire" className="header-grimoire" />
    </header>
  )
}

export default Header