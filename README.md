## D&D Mobile-Controlled Unity Project: Sara, Zsofie, Ivan

---

## System & Architecture Overview

This project features a cross-platform pipeline designed for a mobile-controlled D&D-style multiplayer game.

* **Game Engine:** Unity 6.3 (Built-in Render Pipeline).
* **Version Control Workflow:** Feature-branch isolation merging directly into `main`. Remotes are pruned dynamically (`git fetch --prune`) to maintain clean development tracks.
* **Target Control Platforms:** iOS Ecosystem (iPhones utilized as handheld controllers via custom network wrappers).
* **Cross-Platform Architecture Workspace:**
* **Windows 11 Environments:** Utilized for heavy scene compilation, physics design, environmental layouts, and particle prototyping.


---

## Character Manifest & Group Assignments

To prevent code redundancy, team members used different branches. The project tracking hierarchy divides active assets among three primary character setups:

| Character | Development Owner | Current Implementation Status |
| --- | --- | --- |
| **Mano** | Zsofi | Core input mapping and locomotion controller integration are complete. |
| **Ashe** | Sara | Rigged using clean League of Legends (Khada) mesh topologies; animated via custom Mecanim Humanoid retargeting configurations. |
| **Katarina** | Ivan | Rigged using clean League of Legends Mobile version mesh topologies; animated via custom Mecanim Humanoid retargeting configurations. |
| **Dragon** | Sara / Ivan | Environmental asset placeholder and target integration pending script deployment. |

> **Note:** *Kataria has been officially deprecated and completely stripped from the active runtime hierarchy with the potential of future scaling.*

---

## Core Mechanics Blueprint (Desktop Implementation Strategy)

### 1. Motion Capture & Retargeting Pipeline

* **Animation Rigging:** Characters utilize the **Mecanim Humanoid System**.
* **Mocap Integration:** Motion capture sequences are imported via decoupled FBX files and mapped directly onto character avatars using custom retargeting profiles.
* **Loop Triggers:** Essential looping logic (e.g., `Loop Time` checked on `Run` and `Idle` states) is explicitly handled at the asset import configuration layer to prevent runtime animation locking.

### 2. Attack Actions & Runtime Particle Logic

To isolate performance profiling on non-Mac hardware, the attack pipeline uses a modular visual design backed by an event-driven control script containing local debugger overrides.

* **Particle Systems:** Attached directly as child transforms to character root nodes. Set to local execution grids with `Looping` disabled and standard `Emission Bursts` scaled between 30 and 50 particles over a 1.0-second execution cycle.
* **Script Interface:** Exposes public `Play()` and `Stop()` event sequences that can be bound directly to Zsofi's network interface scripts or tested directly via manual keyboard/Inspector handlers in the editor.

### 3. Locomotion & Physics Obstacle Filtering

To ensure players cannot pass through static world geometry (such as fences or houses), environmental constraints are handled through the native 3D physics matrix.

* **Kinematic Obstacles:** All environmental static meshes are batch-processed under a consolidated root parent transform named `Props`.
* **Collider Arrays:** Iterative `Box Colliders` and `Mesh Colliders` are explicitly generated across all child nodes under `Props`.
* **Player Physics:** Actor objects feature active `Capsule Colliders` driven by calculated `Rigidbody` components. Rigidbodies use frozen rotation profiles across the $X$, $Y$, and $Z$ axes to lock angular displacement during terrain collision events.

