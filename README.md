# Pinfield Prototype

![Build Status](https://img.shields.io/badge/build-pending-brightgreen)
![Unity Version](https://img.shields.io/badge/unity-2026.1-5C6AC4)
![License](https://img.shields.io/badge/license-MIT-blue)

**Pinfield** is an early, one-input, gravity‑driven Plinko‑style incremental engine builder. In this *prototype* the player designs the field and watches the system execute without directly controlling the ball.

> **Prototype** – Work in progress, demo focused.

---

## 🎯 High Concept

Pinfield is a hands‑off engine‑building game where balls fall through a dense pinfield, generating score through collisions and synergies. Score converts into arcade‑style **tickets**, which are used to modify the board and strengthen future runs.

### Quick Overview
1. Drop a ball (Click / Space)
2. Ball collides with pins, building score
3. Ball exits the field
4. Score converts into **tickets**
5. Spend tickets to modify the engine
6. Drop the next ball and observe the result

---

## 🧠 Design Philosophy

- **One Input Only** – No aiming, no flippers, no mid‑ball decisions
- **Engine Building > Reflex Skill**
- **Readable Chaos** – High collision density with visual clarity
- **Rules Over Raw Scaling**
- **Short, Expressive Runs**

*Pinfield* treats the board like a deck in a roguelike. Pins are your cards, traits are your synergies.

---

## 🧩 Core Systems

### Physics‑Driven Pinfield
- Gravity‑dominant movement
- High‑energy bounces
- Ball always exits downward
- Single screen, fixed camera

### Score & Tickets
- **Score**: Temporary per ball
- **Tickets**: Persistent currency
- Conversion rule: `tickets = floor(score / 10)`
- Tickets are whole numbers, presented as physical arcade tickets.

---

## 🔩 MVP Features

- Single vertical pinfield
- Two pin types:
  - **Value Pin** – +1 score
  - **Multiplier Pin** – ×2 score
- Score resets each ball
- Ticket conversion at ball end
- One upgrade: Starting score +1 (stackable)
- Session‑only progression

**MVP goal:** Earn tickets → Buy upgrade → Feel the difference on the next ball.

---

## 🧪 Post‑MVP Direction

Planned expansions will introduce engine‑building depth through:
- **Pin Traits** (Echo, Charge, Convert, etc.)
- Limited between‑ball choices
- Field mutations and global rule modifiers
- Prestige systems and additional ticket tiers

All new systems must respect the core principles: one input, no mid‑ball intervention, system design over player dexterity.

---

## 🛠 Technical Notes

- Built in Unity
- 2D physics‑based interactions
- Component‑driven pin architecture
- Designed with strict scope control

---

## 🚧 Project Status

- **Prototype / Work in progress**
- MVP focused on proving the core loop and game feel before expanding engine‑building systems.

---

## 📜 License

TBD
