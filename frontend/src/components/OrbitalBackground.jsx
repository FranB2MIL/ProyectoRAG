import { useMemo } from "react"
import "./OrbitalBackground.css"

const CENTER_X = 960
const CENTER_Y = 540
const STAR_COUNT = 220

function buildStars() {
  return Array.from({ length: STAR_COUNT }, (_, i) => ({
    id: i,
    cx: Math.random() * 1920,
    cy: Math.random() * 1080,
    r: Math.random() * 1.3 + 0.3,
    delay: Math.random() * 4,
  }))
}

function buildSpokes(count, rInner, rOuter) {
  return Array.from({ length: count }, (_, i) => {
    const angle = (i / count) * Math.PI * 2
    return {
      id: i,
      x1: CENTER_X + Math.cos(angle) * rInner,
      y1: CENTER_Y + Math.sin(angle) * rInner,
      x2: CENTER_X + Math.cos(angle) * rOuter,
      y2: CENTER_Y + Math.sin(angle) * rOuter,
    }
  })
}

function OrbitalBackground() {
  const stars = useMemo(() => buildStars(), [])
  const spokesOuter = useMemo(() => buildSpokes(16, 440, 460), [])
  const spokesMid = useMemo(() => buildSpokes(12, 300, 340), [])
  const spokesInner = useMemo(() => buildSpokes(24, 190, 210), [])

  return (
    <div className="orbital-background" aria-hidden="true">
      <svg viewBox="0 0 1920 1080" preserveAspectRatio="xMidYMid slice">
        <defs>
          <radialGradient id="orbitalCoreGlow" cx="50%" cy="50%" r="50%">
            <stop offset="0%" stopColor="#3fd4e8" stopOpacity="0.18" />
            <stop offset="100%" stopColor="#3fd4e8" stopOpacity="0" />
          </radialGradient>
        </defs>

        <rect width="1920" height="1080" fill="#0a0e16" />

        <g>
          {stars.map((star) => (
            <circle
              key={star.id}
              className="orbital-star"
              cx={star.cx}
              cy={star.cy}
              r={star.r}
              fill="#e7ecf4"
              style={{ animationDelay: `${star.delay}s` }}
            />
          ))}
        </g>

        <circle cx={CENTER_X} cy={CENTER_Y} r="260" fill="url(#orbitalCoreGlow)" />

        <g className="orbital-ring orbital-ring-outer" stroke="#3fd4e8" strokeWidth="0.5" fill="none">
          <circle cx={CENTER_X} cy={CENTER_Y} r="460" />
          <circle cx={CENTER_X} cy={CENTER_Y} r="440" />
          {spokesOuter.map((s) => (
            <line key={s.id} x1={s.x1} y1={s.y1} x2={s.x2} y2={s.y2} />
          ))}
        </g>

        <g className="orbital-ring orbital-ring-mid" stroke="#3fd4e8" strokeWidth="0.5" fill="none">
          <circle cx={CENTER_X} cy={CENTER_Y} r="340" />
          {spokesMid.map((s) => (
            <line key={s.id} x1={s.x1} y1={s.y1} x2={s.x2} y2={s.y2} />
          ))}
        </g>

        <g className="orbital-ring orbital-ring-inner" stroke="#d9b872" strokeWidth="0.5" fill="none">
          <circle cx={CENTER_X} cy={CENTER_Y} r="210" />
          {spokesInner.map((s) => (
            <line key={s.id} x1={s.x1} y1={s.y1} x2={s.x2} y2={s.y2} />
          ))}
        </g>

        <circle
          cx={CENTER_X}
          cy={CENTER_Y}
          r="120"
          stroke="#ff6a2c"
          strokeWidth="0.5"
          fill="none"
          opacity="0.25"
        />
      </svg>
    </div>
  )
}

export default OrbitalBackground
