# Dragon Arena Battle — Unity Developer Technical Test

> A 2.5D top-down dragon combat arena developed for the **DexHigh Unity Developer Technical Test**. Features responsive character controls, distinct abilities with sweetspot mechanics and damage falloff, intelligent enemy AI, and high-impact visual feedback and combat "juice."

---

## 📋 Project Specifications

- **Unity Version:** `Unity 6 (6000.3.24f1)`
- **Render Pipeline:** Universal Render Pipeline (URP)
- **Target Platform:** PC / Standalone (Windows, macOS, Linux)
- **Primary Scene:** [`Assets/_Project/Scenes/DragonArenaBattle.unity`](Assets/_Project/Scenes/DragonArenaBattle.unity)
- **Repository:** [https://github.com/Sub0dh23/DexHigh-Task.git](https://github.com/Sub0dh23/DexHigh-Task.git)
- **Deliverables & Links:**
  - 📂 **Google Drive Folder (Build, Video & Docs):** [DexHigh Task Google Drive](https://drive.google.com/drive/u/0/folders/1hSZMfNlij99pf4cEUKhaSvNYupK40m5S)
  - 🎮 **Playable Windows Build (64-bit):** Available in Google Drive folder (`DragonArena_Windows_x64.zip`)
  - 🎥 **Gameplay Video:** Available in Google Drive (`Gameplay Recording/`) and directly in repo at [`Recordings/Gameplay_002.webm`](Recordings/Gameplay_002.webm)

---

## 🎮 Game Controls

| Action | Primary Input | Secondary / Alternative | Description |
| :--- | :--- | :--- | :--- |
| **Move** | <kbd>W</kbd> <kbd>A</kbd> <kbd>S</kbd> <kbd>D</kbd> | Arrow Keys | Smooth directional movement with acceleration and turning arcs |
| **Aim / Face Direction** | **Mouse Cursor** | — | Directional cursor aiming on ground plane (with deadzone threshold) |
| **Fire Breath** | <kbd>Q</kbd> | <kbd>1</kbd> | Channeled cone AoE; applies Burn DoT; 1.4x center sweetspot |
| **Tail Whip** | <kbd>V</kbd> | <kbd>2</kbd> | Melee sweep from tail bone; strong knockback; 1.35x sweetspot |
| **Sky Dive Slam** | <kbd>E</kbd> | <kbd>3</kbd> / <kbd>Space</kbd> | Aerial ascent and ground slam; 3.5m epicenter + 8m radial falloff |

---

## ⚔️ Combat Abilities & Mechanics

Each ability is designed with distinct tactical utility, hit confirmation, and skill expression:

### 1. 🔥 Fire Breath (`Q` / `1`)
- **Type:** Channeled Ranged Cone AoE
- **Mechanics:** Emits a continuous high-temperature flame stream across a 45° frontal arc up to 8.0 meters.
- **Sweetspot:** Enemies caught directly in the center 15° take **1.4x sweetspot critical damage**.
- **Status Effect:** Applies a burning **Damage-over-Time (DoT)** status ticking every 0.5s for 2.0s.
- **Visuals & Feedback:** URP particle stream with burning embers, flame tongues, camera rumble, and orange floating combat numbers.

### 2. 🐉 Tail Whip (`V` / `2`)
- **Type:** Melee Arc Sweep
- **Mechanics:** Sweeps a 120° physical arc centered behind the dragon originating directly from the `tail5` bone transform.
- **Physics Knockback:** Inflicts heavy impulse force pushing targets backward relative to the swing vector.
- **Sweetspot:** Impacts at the outer edge of the tail whip deal **1.35x bonus damage**.
- **Visuals & Feedback:** Slash arc ribbon trail, radial hit sparks, and camera impulse shake.

### 3. ☄️ Sky Dive Slam (`E` / `3` / `Space`)
- **Type:** High-Impact Aerial Dive Bomb
- **Mechanics:** 
  1. **Ascent & Pinning:** The dragon leaps into the sky; the ground impact point is locked and pinned with a telegraphed indicator to provide clear counterplay.
  2. **Aerial Hover:** Brief hover pause at apex altitude.
  3. **High-Velocity Slam:** Rapid plunge descent toward the locked coordinates.
- **Radial Damage Falloff:**
  - **Epicenter (`<= 3.5m`):** Full critical damage (100%–125%) + massive directional knockback + landing squash animation.
  - **Outer Blast (`3.5m – 8.0m`):** Smooth linear falloff damage (75% down to 15%).
  - **Safe Zone (`> 8.0m`):** Strictly **0 damage** (rewards players and AI for dodging out of the zone).
- **Visuals & Feedback:** Pinned ground reticle, crater decal, expanding shockwave ring, procedural landing squash, and camera screen shake.

---

## 🧠 Enemy AI System

The AI Dragon ([`AIDragonController.cs`](Assets/_Project/Scripts/AI/AIDragonController.cs)) features a state-machine driven decision system:

- **Distance Evaluation:**
  - **Far Range (> 10m):** Enters `Chase` state, turning smoothly toward the player and closing the gap.
  - **Mid Range (4m – 10m):** Evaluates `Fire Breath` when frontal alignment is met, or initiates `Sky Dive Slam` when the player is mobile.
  - **Close Range (< 4m):** Maneuvers for a `Tail Whip` sweep or repositions.
- **Cooldown & Resource Parity:** AI abilities share identical cooldowns, damage stats, and telegraph timings as the player dragon to maintain balance and fairness.
- **Interruptible & Telegraphed:** AI dive slams show the same pinned ground indicator, allowing the player to react and dash away.

---

## ✨ Visual Polish, UI & Game Feel ("Juice")

- **Roman Colosseum Arena & Atmosphere:** Multi-tiered gladiatorial amphitheater featuring 28 arched arcade bays, columns, spectator seating tiers, inner flagstone rings, and perimeter rock rubble.
- **Golden-Hour Lighting & Volumetric God Rays:** Warm golden directional key light (46° angle, intensity 2.85) with soft contact shadows, warm terracotta ground bounce fill light, volumetric sun shafts slicing through upper arches, and suspended drifting golden dust motes.
- **Cinematic Color Grading & Post-Processing:** ACES filmic tonemapping, warm golden-hour white balance (+24°), warm split toning (deep umber shadows, earthy bronze midtones, radiant gold highlights), subtle atmospheric bloom, and sepia edge vignette.
- **Dota 2-Inspired Combat HUD:** Clean bottom-bar HUD displaying player status, health bar with smooth lerping, and active cooldown sweeps for all 3 ability slots.
- **Tactical Reticle & Ground Indicators:** Custom circular telegraph indicators for area attacks, preventing cursor drift during ability execution.
- **Dynamic Combat Camera:** [`DynamicCombatCamera.cs`](Assets/_Project/Scripts/Camera/DynamicCombatCamera.cs) provides smooth lerped following with subtle look-ahead based on player movement, coupled with a 6-degree impulse shake system on heavy impacts.
- **Floating Combat Numbers:** [`DamageNumberPopup.cs`](Assets/_Project/Scripts/UI/DamageNumberPopup.cs) displays floating damage values with color differentiation (orange for burn DoT, gold for critical sweetspots, red for regular hits) and clean punctuation.
- **Procedural Body Squash:** Procedural scale deformation on ground impact gives physical weight and impact to the dragon's landing.
- **Victory & Defeat Flow:** Complete game loop with victory/defeat banners and match restart handling via [`BattleGameManager.cs`](Assets/_Project/Scripts/Core/BattleGameManager.cs).

---

## 🏗️ Architecture & Project Structure

The project strictly follows modular, component-based Unity best practices:

```
Assets/_Project/
├── Animations/           # Dragon Animator Controller and animation clips
├── Art/
│   ├── UI/               # Combat HUD frames, reticles, victory banners
│   └── VFX/              # Custom particle textures (crater, slash, sparks, smoke)
├── Materials/            # URP PBR materials for dragons, arena, and VFX
├── Models/               # Dragon FBX meshes and optimized rigs
├── Prefabs/
│   ├── Prefab_PlayerDragon.prefab    # Player dragon setup
│   ├── Prefab_AIDragon.prefab        # AI dragon setup
│   ├── UI/                           # HUD and damage number popup prefabs
│   └── VFX/                          # Fire breath, slam impact, and tail whip prefabs
├── Scenes/
│   └── DragonArenaBattle.unity       # Main battle arena scene
├── ScriptableObjects/    # Ability data definitions (damage, ranges, cooldowns)
└── Scripts/
    ├── AI/               # AIDragonController state machine
    ├── Audio/            # AudioManager sound trigger hooks
    ├── Camera/           # DynamicCombatCamera with shake & follow
    ├── Characters/       # DragonMotor, DragonHealth, PlayerDragonController
    ├── Combat/           # DragonCombat, AbilityData, DamageInfo, IDamageable
    ├── Core/             # BattleGameManager, ArenaBounds
    └── UI/               # CombatHUD, AbilitySlotUI, DamageNumberPopup, Reticles
```

### Key Script Roles:
- **`DragonMotor.cs`:** Manages character physics, kinematic velocity, acceleration, ground alignment, and smooth rotation.
- **`DragonCombat.cs`:** Coordinates ability execution, cooldown timers, sweetspot detection, cone checks, and radial falloff formulas.
- **`DragonHealth.cs`:** Implements `IDamageable`, handles health tracking, damage numbers, DoT coroutines, and death events.
- **`PlayerDragonController.cs`:** Gathers input and performs mathematical ground-plane cursor projection.
- **`AIDragonController.cs`:** Autonomous state machine evaluating engagement distances and ability priorities.

---

## 📦 Third-Party Assets & Attribution

- **Dragon Model & Rig:** Free PBR Red Dragon Model with skeleton rig and animations.
- **Render Pipeline & Shaders:** Universal Render Pipeline (URP) Lit and Particles/Unlit shaders.
- **Typography:** TextMeshPro with LiberationSans SDF asset for crisp in-game text and popups.
- **VFX Textures:** Procedural and CC0 particle textures (Shockwave ring, smoke puff, ground crater, slash arc).

---

## 🤖 Section 7: AI Usage Note

In accordance with Section 7 of the technical assessment, below is the transparent disclosure of AI tool usage during development:

### 1. Tools Used
- **Google Antigravity Agent (Gemini):** Used as an intelligent pair programmer for rapid code scaffolding, mathematical modeling, shader/particle setup, and real-time debugging.

### 2. Where AI Helped Most
- **Rapid Prototyping:** Quick generation of component foundations (`DragonMotor`, `DragonCombat`, `DamageInfo` structs) and UI layouts.
- **Vector Mathematics & Combat Geometry:** Formulating precise cone raycasts for Fire Breath sweetspots and multi-tier radial damage curves for the Sky Dive Slam.
- **Shader & Material Verification:** Automating material asset configurations for URP Particle Lit/Unlit blends.

### 3. Mistakes Made by AI & How They Were Diagnosed and Fixed
- **Issue 1: Cursor Jitter & 180° Snapback**
  - *Symptom:* The dragon would randomly snap 180° backward when aiming near itself.
  - *Diagnosis:* The cursor raycast was hitting the dragon's own character collider rather than the ground, resulting in raycast distance ≈ 0 and erratic normal reflection.
  - *Fix:* Replaced raycasts with a clean mathematical ground-plane intersection (`Plane(Vector3.up, Vector3.zero)`), ignoring character colliders entirely and adding a 1.2m center deadzone.
- **Issue 2: Map-Wide Dive Bomb Blast Radius**
  - *Symptom:* Sky Dive Slam damaged the enemy dragon even when on the complete opposite side of the arena.
  - *Diagnosis:* The AI code originally passed `ability.EffectiveRange` (`22m` targeting distance) into the splash damage check instead of the blast radius (`3.5m`).
  - *Fix:* Decoupled targeting range from explosion radius and implemented a strict 3-tier falloff: `<= 3.5m` epicenter, `3.5m–8.0m` linear falloff, and strictly `0 damage` beyond `8.0m`.
- **Issue 3: Reticle Drift During Ascent**
  - *Symptom:* During the dive slam ascent, moving the mouse caused the ground indicator to slide, misleading the player.
  - *Fix:* Pinned both the ground indicator and the combat impact coordinates to the locked target location upon ability trigger, unpinning only after landing completion.

### 4. Workflow & Efficiency Reflection
Leveraging AI significantly expedited boilerplate generation and complex geometric calculations, freeing up focus for core gameplay feel ("juice"), camera tuning, ability telegraphs, and gameplay balance. The iterative human-in-the-loop review was essential for catching subtle physics and targeting bugs.

---

## 🎵 Audio & Polish Highlights

- **Dynamic Combat Soundscape:** Custom trimmed audio effects integrated for all dragon abilities (Fire Breath flame burst, Tail Whip crack, and Heavy Impact Sky Dive slam).
- **Stinger Fanfares & Atmosphere:** Dedicated Victory & Defeat music cues and ambient colosseum music.
- **Main Menu UI:** Dota 2 dark fantasy aesthetic gateway with Play Game and Exit buttons, full hotkey briefing, and seamless arena escape/return handling.
