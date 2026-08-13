# Fishdle.io

A desktop-themed idle fishing game built in Unity. Cast into the ocean to catch fish, sell them into a market that changes over time, invest in an automated fishing fleet, and prestige for permanent multipliers — all wrapped in a retro pixel/desktop-OS aesthetic.

## Gameplay

1. **Fish** — click the ocean to catch fish by hand.
2. **Sell** — dump your catch onto the market for money. The price per fish drifts within a min/max band and rerolls on a timer, so *when* you sell matters.
3. **Invest** — buy boats that generate fish per second automatically (idle income), and upgrade them.
4. **Upgrade tapping** — a small, finite upgrade track that boosts fish-per-tap.
5. **Prestige** — reset your run for a permanent earnings multiplier. Each prestige also unlocks another boat slot (boats are effectively infinite).
6. **Fossils** — after a few prestiges, fossils wash up on the beach. Walk over, play a short dig minigame, and collect fossils with randomized stats. Equip up to 5 for stacking bonuses (fish multiplier, sell bonus, spawn rate, minigame slowdown, price modifier).
7. **Time of day** — the beach background swaps between day / sunset / night based on your clock.

### How to Play

| Action | Input |
|---|---|
| Move along the beach | `WASD` or arrow keys |
| Catch a fish | **Left-click the ocean** |
| Open a menu | Click a **desktop folder** (bottom-right: Sell, Boats, Fossils, Tapping, Prestige) |
| Play a fossil minigame | Walk up to a fossil and click it |

---

## Technical Highlights

System wiring in general, as this project is more of a showcase in the internal system.

- **Config-driven balance.** Every tunable number lives in a single [`GameConfig`](Assets/Scripts/Core/GameConfig.cs) `ScriptableObject`. A config asset is thus created to rebalance the entire game from the Inspector without touching code. Progression is expressed as formulas (`GetBoatCost`, `GetPrestigeCost`, …) rather than hard-coded tables.

- **Push-model stats hub.** [`StatsManager`](Assets/Scripts/Managers/StatsManager.cs) is the single source of derived stats (fish/sec, multipliers, fossil bonuses). It recomputes and raises an event on change; UI subscribes instead of polling every frame.

- **Deliberate multiplier design.**
  - `boatCostMultiplier` equals `boatGenerationMultiplier`, so every boat tier is the *same value-per-dollar* — no trap purchases.
  - `prestigeCostMultiplier` is tuned against the boat/prestige multipliers to keep **run length roughly constant** as you climb.
  - The prestige bonus is applied once, at the point of sale — not baked into per-second generation — which keeps the generation numbers clean and makes the multiplier legible at the money layer.
  - Fossil bonuses are multiplicative — an earlier additive fish/sec design was overpowered and flattened progression.

- **Finite vs. infinite tracks on purpose.** Tap upgrades are capped; boats and prestige are unbounded. The tap track is an intentional early-game boost which becomes stale to make way for the future game.

- **Save system.** JSON persistence via `JsonUtility` to `Application.persistentDataPath`, storing money, fish, prestige/click levels, owned boats, and collected fossils. Do not worry about your save progress.
---


---

## How to Play It

**Download the latest Windows build from the [Releases page](../../releases).**

1. Download `Fishdle_io-Windows.zip` and **unzip the whole folder** (keep every file together).
2. Run `Fishdle_io.exe`.

Save data is written to `%USERPROFILE%/AppData/LocalLow/DefaultCompany/Fishdle_io/`. Delete that folder to start fresh.

### Run from source (requires Unity)

1. Open the project in **Unity `6000.5.3f1`** (Unity 6).
2. Open `Assets/Scenes/Game.unity`.
3. Press **Play**.

---


## Built With

- Unity 6 (`6000.5.3f1`), C#
- Unity Input System, Universal Render Pipeline (2D)
- TextMeshPro
