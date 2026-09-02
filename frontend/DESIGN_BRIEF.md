# Grimoires of Sol — Design Brief

## Overview

This is a chat interface for a Destiny lore assistant. The visual identity is a
Destiny 2 menu-inspired **glassmorphism**: a near-black navy void, orbital
rings turning slowly behind the content, and panels of blurred glass with
diagonal-cut corners — the Grimoire card as a HUD element rather than a page
of parchment.

---

## Project structure

The app lives in `frontend/src/` and, as of the redesign, includes reusable
pieces shared across pages instead of one monolithic stylesheet:

```
src/
├── theme.css                    → color/font design tokens (:root custom properties)
├── components/
│   ├── OrbitalBackground.jsx    → fixed, animated SVG rings + starfield behind all pages
│   ├── GlassPanel.jsx           → the reusable "Grimoire card" glass shell
│   ├── GhostLoader.jsx          → the pixel-art Ghost sprite (static or animated)
│   ├── Header.jsx               → chat page title bar
│   ├── ChatWindow.jsx           → scrollable message history
│   ├── Message.jsx              → individual message (user or loremaster)
│   └── ChatInput.jsx            → text input + send button
├── pages/
│   ├── LandingPage.jsx          → glassmorphism-redesigned
│   └── ChatPage.jsx
├── App.jsx                      → router shell, mounts OrbitalBackground once
└── App.css                      → page/component styles that consume the tokens
```

`OrbitalBackground` is mounted once in `App.jsx`, above the routes, so it
persists behind every page without being duplicated.

---

## Color tokens (`theme.css`)

| Token | Hex / value | Usage |
|---|---|---|
| `--color-bg-base` | `#0a0e16` | Base background — near-black navy, not pure black |
| `--color-glass-surface` | `rgba(20, 27, 42, 0.55)` | Glass panel fill, paired with `--blur-glass` |
| `--blur-glass` | `18px` | `backdrop-filter: blur(...)` on glass panels |
| `--color-accent-solar` | `#ff6a2c` | Primary accent — used sparingly, for active states/CTAs |
| `--color-accent-arc` | `#3fd4e8` | Secondary accent — hairline borders, linework, orbital rings |
| `--color-accent-gold` | `#d9b872` | Reserved for lore/"exotic" highlights only |
| `--color-text-primary` | `#e7ecf4` | Main readable text |
| `--color-text-secondary` | `#7a8699` | Muted/meta text, labels, placeholders |

## Typography

- `--font-ui`: `'Space Grotesk', 'Rajdhani', system-ui, sans-serif` — nav,
  labels, stats, buttons, anything that reads as UI chrome.
- `--font-lore`: `'Spectral', 'Source Serif 4', Georgia, serif` — long-form
  lore/body text and headings that carry the "archive" feel (e.g. the
  Loremaster's answers, panel titles, the landing page description).

Fonts load via Google Fonts `<link>` tags in `index.html`.

---

## Panel rules (the key visual signature)

- **No `border-radius` on panels.** Corners are diagonal cuts via
  `clip-path: polygon(...)`, parameterized by a `cornerSize` — this is
  the defining shape language of the UI, not a decorative afterthought.
  `GlassPanel` implements this once; consume it rather than hand-rolling
  new clip-path math per component.
- **Borders are 1px hairlines** in the arc or solar accent color at low
  alpha — never a filled/solid border, never rounded.
- **Glass panels** (`GlassPanel`) combine `--color-glass-surface` +
  `backdrop-filter: blur(--blur-glass)` + a hairline border + the clip-path
  corners. Small solid-fill elements (e.g. the landing CTA button) reuse the
  same clip-path corner language without the blur, so a nested blur-on-blur
  never happens.

## Orbital background

`OrbitalBackground` renders concentric rings (hairline strokes, `--color-accent-arc`
for the outer/mid rings, `--color-accent-gold` for the inner ring) rotating
slowly — 90 to 180 seconds per rotation, alternating direction per ring — over
a faint twinkling starfield and a soft radial glow at the center. It sits
`position: fixed`, `pointer-events: none`, at a low `z-index`, and stays in
the 0.2–0.3 opacity range so it never competes with foreground content. Pages
that sit on top of it need their own stacking context (`position: relative`
+ a `z-index` of at least `1`) or they'll render underneath it.

## The "one glow at a time" principle

Only a single element carries visual emphasis (a stronger border and/or a
soft glow) at once — typically the active/primary-action element, e.g. the
landing page's "Enter the Archives" CTA, or an active chat state. Every other
panel, border, and divider stays quiet: hairline only, no glow, no heavy
blur layered on top of another blur. `GlassPanel`'s `emphasized` prop is the
single mechanism allowed to add glow — don't add `box-shadow`/`filter` glow
ad hoc elsewhere.

---

## What to avoid

- No `border-radius` on panels — diagonal clip-path corners only.
- No glow/blur applied uniformly across every panel — reserve it for the
  one emphasized element per view.
- No nested `backdrop-filter` blur (a glass panel inside a glass panel).
- No colors outside the token table above — especially no use of
  `--color-accent-gold` outside lore/"exotic" highlights.

---

## Status

Both the landing page (`LandingPage.jsx`) and the chat interface (`Header.jsx`,
`Message.jsx`, `ChatInput.jsx`, `ChatWindow.jsx`) are on this system.

Notes specific to the chat interface:
- The header and the chat input bar are translucent glass bars (blur + a
  single hairline border) with **no** clip-path corners — they're edge-to-edge
  chrome that touches the viewport bounds, not a floating card, so a diagonal
  cut there would read as a rendering gap rather than a design flourish.
- The Loremaster's message is the ornate "Grimoire card": an actual
  `GlassPanel` with `accent="gold"` (lore content, per the token table). The
  Guardian's (user) message stays a quiet, plain bordered bubble — arc
  hairline, no blur — per the "one glow at a time" principle.
- The pending "Loremaster is answering" placeholder and the send button both
  use `emphasized`, but never at the same time in practice: the send button's
  glow only turns on once there's text ready to send, and it's cleared the
  moment a request goes out — so exactly one thing glows at once, and it
  tracks the actual active state rather than being static.
