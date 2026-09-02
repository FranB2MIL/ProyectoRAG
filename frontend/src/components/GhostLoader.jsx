import "./GhostLoader.css"

function GhostLoader({ size = 64, animated = false, className = "" }) {
  const classes = ["ghost-loader", animated ? "ghost-loader-animated" : "", className]
    .filter(Boolean)
    .join(" ")

  return (
    <img
      src="/ghost2.png"
      alt="Ghost"
      className={classes}
      style={{ width: size, height: size }}
    />
  )
}

export default GhostLoader
