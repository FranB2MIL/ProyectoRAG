import ChatWindow from "../components/ChatWindow"
import ChatInput from "../components/ChatInput"
import Header from "../components/Header"
import { useState } from "react"

function ChatPage() {
  const [messages, setMessages] = useState([])
  const [isLoading, setIsLoading] = useState(false)

  const sendMessage = async (text) => {
    const userMessage = { role: "user", content: text }
    setMessages(prev => [...prev, userMessage])
    setIsLoading(true)

    try {
      const response = await fetch("http://localhost:5218/api/documents/ask", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ query: text })
      })
      const data = await response.json()
      const loremasterMessage = { role: "loremaster", content: data.answer }
      setMessages(prev => [...prev, loremasterMessage])
    } catch (error) {
      console.error("Error:", error)
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <div className="app">
      <Header />
      <ChatWindow messages={messages} isLoading={isLoading} />
      <ChatInput onSend={sendMessage} isLoading={isLoading} />
    </div>
  )
}

export default ChatPage