import { useState } from "react"
import GlassPanel from "./GlassPanel"

function ChatInput({ onSend, isLoading }) {
  const [text, setText] = useState("")
  const canSend = text.trim().length > 0 && !isLoading

  const handleSubmit = (e) => {
    e.preventDefault()
    if (!canSend) return
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
      <GlassPanel
        as="button"
        type="submit"
        accent="solar"
        emphasized={canSend}
        cornerSize={8}
        className="chat-input-button"
        disabled={!canSend}
      >
        {isLoading ? "..." : "Ask"}
      </GlassPanel>
    </form>
  )
}

export default ChatInput
