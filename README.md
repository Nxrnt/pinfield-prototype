# Pinfield

**Pinfield** is a one-input, gravity-driven plinko-style incremental engine builder.  
Instead of controlling the ball, the player designs the field and watches the system execute.

> Build the machine. Drop the ball. Watch the chaos pay off.

---

## 🎯 High Concept

Pinfield is a hands-off engine-building game where balls fall through a dense pinfield, generating score through collisions and synergies. Score converts into arcade-style **tickets**, which are used to modify the board and strengthen future runs.

The player never controls the ball directly.  
All decisions happen between drops.

---

## 🕹 Core Gameplay Loop

1. Drop a ball (Click / Space)
2. Ball collides with pins and builds score
3. Ball exits the field
4. Score converts into **tickets**
5. Spend tickets to modify the engine
6. Drop the next ball and observe the result

Each ball is a test run of the system you’ve built.

---

## 🧠 Design Philosophy

- **One Input Only** — No aiming, no flippers, no mid-ball decisions
- **Engine Building > Reflex Skill**
- **Readable Chaos** — High collision density with visual clarity
- **Rules Over Raw Scaling**
- **Short, Expressive Runs**

Pinfield treats the board like a deck in a roguelike.  
Pins are your cards. Traits are your synergies.

---

## 🧩 Core Systems

### Physics-Driven Pinfield
- Gravity-dominant movement
- High-energy bounces
- Ball always exits downward
- Single screen, fixed camera

### Score & Tickets
- **Score**: Temporary per ball
- **Tickets**: Persistent currency

Conversion rule:
```
tickets = floor(score / 10)
```

Tickets are whole numbers and framed as physical arcade tickets.

---

## 🔩 MVP Features

- Single vertical pinfield
- Two pin types:
  - **Value Pin** — +1 score
  - **Multiplier Pin** — ×2 score
- Score resets each ball
- Ticket conversion at ball end
- One upgrade: Starting score +1 (stackable)
- Session-only progression

MVP goal:
> Earn tickets → Buy upgrade → Feel difference next ball.

---

## 🧪 Post-MVP Direction

Planned expansions introduce engine-building depth through:

- **Pin Traits** (e.g. Echo, Charge, Convert)
- Run structure with limited between-ball choices
- Field mutations and global rule modifiers
- Prestige systems and additional ticket tiers

All new systems must respect:
- One input
- No mid-ball intervention
- System design over player dexterity

---

## 🛠 Technical Notes

- Built in Unity
- 2D physics-based interactions
- Component-driven pin architecture
- Designed with strict scope control

---

## 🚧 Project Status

Currently in active development.  
MVP focused on proving the core loop and game feel before expanding engine-building systems.

---

## 📜 License

TBD
