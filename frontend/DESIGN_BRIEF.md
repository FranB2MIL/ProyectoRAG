# Destiny Loremaster — Design Brief

## Overview

This is a chat interface for a Destiny lore assistant. The visual identity should feel like
interacting with an ancient archive — warm blacks, golden accents, and a sense of weight and
history. Think Grimoire cards from the original Destiny: dark parchment, gold lettering,
the feeling that every word costs something to hold.

---

## Project structure

The app lives in `frontend/src/` and has the following components:

```
src/
├── components/
│   ├── Header.jsx        → app title and subtitle
│   ├── ChatWindow.jsx    → scrollable message history
│   ├── Message.jsx       → individual message (user or loremaster)
│   └── ChatInput.jsx     → text input + send button
├── App.jsx               → layout shell, state, API call
└── App.css               → all styles live here
```

The layout is a vertical flexbox column taking the full viewport height:
- Header: fixed at top, `flex-shrink: 0`
- ChatWindow: `flex: 1`, `overflow-y: auto`
- ChatInput: fixed at bottom, `flex-shrink: 0`

**Do not change the component structure or JavaScript logic.** Only modify `App.css` and,
if needed, add `className` attributes to existing JSX elements. Do not add new components
or restructure the layout.

---

## Visual identity

### Mood
Ancient archive. Warm darkness. Gold as a signal of importance, not decoration.
The interface should feel like it belongs in the game's lore UI — heavy, deliberate, quiet.
No playfulness, no brightness, no modern "clean SaaS" feel.

### Color palette

| Token | Hex | Usage |
|---|---|---|
| `--bg-base` | `#0e0c0a` | Page background |
| `--bg-surface` | `#161210` | Header, input bar, message backgrounds |
| `--bg-deep` | `#120f0d` | Loremaster message background |
| `--border` | `#2a2218` | All borders and dividers |
| `--gold` | `#c9a84c` | Primary accent: loremaster role label, send button, active border |
| `--gold-dim` | `#7a6a4a` | Secondary accent: user role label, placeholder text |
| `--text-primary` | `#d4b896` | Main readable text |
| `--text-secondary` | `#c4a882` | Loremaster message body text |
| `--text-muted` | `#7a6a4a` | Role labels, placeholder, empty state |

### Typography
- Font stack: `'Georgia', 'Times New Roman', serif` for the Loremaster's messages — this gives
  the feel of reading an ancient document.
- Font stack: `system-ui, sans-serif` for the Guardian's messages and UI chrome (header,
  input, buttons).
- Body text size: `0.95rem`, line-height `1.7`
- Role labels: `0.7rem`, `letter-spacing: 0.1em`, `text-transform: uppercase`
- Header title: `1.4rem`, serif, `letter-spacing: 0.05em`
- Header subtitle: `0.8rem`, sans-serif, muted gold

### Borders and dividers
- All borders: `1px solid #2a2218`
- Loremaster message: add a `border-left: 2px solid #c9a84c` to distinguish from user messages
- Header bottom border: `1px solid #2a2218`
- Input bar top border: `1px solid #2a2218`

### Buttons and inputs
- Send button: `background: #c9a84c`, `color: #0e0c0a`, `font-weight: 600`,
  no border-radius (use `border-radius: 3px` for a slightly angular feel)
- Send button disabled: `opacity: 0.4`
- Input field: dark background (`#120f0d`), `1px solid #2a2218` border, gold text color on focus
  border `#c9a84c`
- Input placeholder: `#7a6a4a`

### Message layout
- User messages: aligned to the right (`align-self: flex-end`), sans-serif,
  background `#161210`, border `1px solid #2a2218`
- Loremaster messages: aligned to the left (`align-self: flex-start`), serif font,
  background `#120f0d`, border `1px solid #2a2218`, `border-left: 2px solid #c9a84c`
- Both: `border-radius: 4px` (slightly angular, not fully rounded)
- Max width: `72%`
- Role label above each message bubble

### Loading state
- "Consulting the archives..." in the Loremaster style (serif, muted gold, italic)

### Empty state
- "The archives await, Guardian. Ask your question."
- Centered, serif, `#7a6a4a`, `font-style: italic`
- Add a subtle decorative separator above and below: a short `1px solid #2a2218` horizontal line,
  centered, `width: 60px`

---

## What to avoid
- No gradients
- No glow effects or box-shadows (except a subtle `inset` on the input field focus if needed)
- No rounded corners beyond `4px` — this should feel architectural, not soft
- No bright whites — the lightest color in the palette is `#d4b896`
- No blue, purple, or green anywhere
- Do not change font sizes drastically — keep them close to the current values
- Do not add animations or transitions beyond simple `color` transitions on hover/focus

---

## Reference feeling
If you need a visual reference for the mood: the Destiny Grimoire card UI, the Book of Sorrow
lore entries, the aesthetic of the Tower's archive rooms. Heavy, dark, gold-touched.
Every element should feel like it has been there for centuries.