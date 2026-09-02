import "./GlassPanel.css"

function GlassPanel({
  children,
  accent = "arc",
  emphasized = false,
  cornerSize = 20,
  as: Tag = "div",
  className = "",
  ...rest
}) {
  const classes = [
    "glass-panel",
    `glass-panel--${accent}`,
    emphasized ? "glass-panel--emphasized" : "",
    className,
  ]
    .filter(Boolean)
    .join(" ")

  return (
    <Tag className={classes} style={{ "--corner-size": `${cornerSize}px` }} {...rest}>
      {children}
    </Tag>
  )
}

export default GlassPanel
