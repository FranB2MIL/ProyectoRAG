import { useState } from "react"

function ChatInput({ onSend, isLoading }) {
  const [text, setText] = useState("")

  const handleSubmit = (e) => {
    e.preventDefault()
    if (!text.trim() || isLoading) return
    onSend(text.trim())
    setText("")
  }

  const handleKeyDown = (e) => {
    if (e.key === "Enter" && !e.shiftKey) {
      handleSubmit(e)
    }
  }

  return (
    <form className="chat-input" onSubmit={handleSubmit}>
      <input
        type="text"
        className="chat-input-field"
        placeholder="Ask about the lore..."
        value={text}
        onChange={(e) => setText(e.target.value)}
        onKeyDown={handleKeyDown}
        disabled={isLoading}
      />
      <button
        type="submit"
        className="chat-input-button"
        disabled={isLoading || !text.trim()}
      >
        {isLoading ? "..." : "Ask"}
      </button>
    </form>
  )
}

export default ChatInput