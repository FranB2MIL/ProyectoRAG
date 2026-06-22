function Message({ message }) {
  const isUser = message.role === "user"

  return (
    <div className={`message ${isUser ? "message-user" : "message-loremaster"}`}>
      <span className="message-role">
        {isUser ? "Guardian" : "Loremaster"}
      </span>
      <p className="message-content">{message.content}</p>
    </div>
  )
}

export default Message