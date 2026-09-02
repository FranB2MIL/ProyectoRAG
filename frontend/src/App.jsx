import { BrowserRouter, Routes, Route } from "react-router-dom"
import LandingPage from "./pages/LandingPage"
import ChatPage from "./pages/ChatPage"
import OrbitalBackground from "./components/OrbitalBackground"
import "./App.css"

function App() {
  return (
    <BrowserRouter>
      <OrbitalBackground />
      <Routes>
        <Route path="/" element={<LandingPage />} />
        <Route path="/chat" element={<ChatPage />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App