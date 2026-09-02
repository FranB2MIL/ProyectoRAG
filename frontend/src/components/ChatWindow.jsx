import { useEffect, useRef } from "react"
import Message from "./Message"
import GhostLoader from "./GhostLoader"

function ChatWindow({ messages, isLoading }) {
    const bottomRef = useRef(null)

    useEffect(() => {
        bottomRef.current?.scrollIntoView({ behavior: "smooth" })
    }, [messages, isLoading])

    return (
        <div className="chat-window">
            {messages.length === 0 && (
                <p className="chat-empty">
                    The archives await, Guardian. Ask your question.
                </p>
            )}
            {messages.map((message, index) => (
                <Message key={index} message={message} />
            ))}
            {isLoading && (
                <div className="message message-loremaster">
                    <span className="message-role">Loremaster</span>
                    <div className="ghost-loader-container">
                        <GhostLoader size={96} animated />
                    </div>
                </div>
            )}
            <div ref={bottomRef} />
        </div>
    )
}

export default ChatWindow