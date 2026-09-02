import GlassPanel from "./GlassPanel"

function Message({ message }) {
  const isUser = message.role === "user"

  if (isUser) {
    return (
      <div className="message message-user">
        <span className="message-role">Guardian</span>
        <p className="message-content">{message.content}</p>
      </div>
    )
  }

  return (
    <GlassPanel accent="gold" cornerSize={16} className="message message-loremaster">
      <span className="message-role">Loremaster</span>
      <p className="message-content">{message.content}</p>
    </GlassPanel>
  )
}

export default Message
