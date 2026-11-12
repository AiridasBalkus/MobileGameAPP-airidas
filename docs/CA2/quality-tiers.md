# Mino: quality tiers (Week 9 Lab A)

Three Quality levels picked at launch by `Assets/scripts/Performance/DeviceTier.cs`, then adjusted by the player's Performance Mode (`PerformanceMode.cs`). Nothing in this file has been measured yet; it records the configuration and the reasons for it.

## Tier table

| Setting | Low | Mid | High |
|---------|-----|-----|------|
| Render Scale (URP asset) | 0.75 | 0.9 | 1.0 |
| MSAA (URP asset) | Disabled | 2x | 4x |
| Main Light Shadows (URP asset) | Off | On, 20 m, 1024 | On, 40 m, 2048 |
| Post-processing | Off | Bloom only | On |
| Global Mipmap Limit (Quality) | Half Resolution | Full Resolution | Full Resolution |
| Texture Streaming (Quality) | On | On | On |
| Anisotropic Textures (Quality) | Disabled | Per Texture | Forced On |
| Target fps (code) | 30 | 60 | 60 |

Performance Mode on top of the tier:
- **Balanced** (default): the tier's fps (30 / 60 / 60) and the tier's render scale.
- **High FPS** (player opt-in): 120 fps and render scale capped at 0.8, on every tier. A Low phone stays at 30 fps unless the player chooses High FPS.
- Until the Low / Mid / High levels exist, `DeviceTier` logs a warning, keeps the current level (Mobile), and Balanced keeps the pre-Week 9 full render scale (1.0).

Order at start-up: `DeviceTier` (before the first scene, and again from `Bootstrap.Awake`, idempotent) then `Bootstrap` logs `session_start ... tier=<name>` then `PerformanceMode.Start` applies the mode. `Bootstrap` no longer sets `targetFrameRate = 60` itself.

## Detection thresholds and why they differ from the lab

| Tier | Lab sheet | Mino |
|------|-----------|------|
| High | RAM >= 8192 MB and >= 8 cores | RAM >= 7000 MB and >= 8 cores |
| Mid | RAM >= 4096 MB and >= 6 cores | RAM >= 3500 MB and >= 6 cores |
| Low | everything else | everything else |

`SystemInfo.systemMemorySize` reports less than the marketed RAM because the kernel, GPU and modem reserve memory. Values from the Week 8 peer matrix and my test phone:

| Phone | Marketed | Reported | Lab tier | Mino tier |
|-------|----------|----------|----------|-----------|
| Galaxy S24 Ultra | 12 GB | 11084 MB | High | High |
| Galaxy S25 FE | 8 GB | 7204 MB | Mid (below 8192) | High |
| Galaxy A15 | 4 GB | 3638 MB | Low (below 4096) | Mid (if it reports >= 6 cores) |

With the lab thresholds an 8 GB phone can never reach High and a 4 GB phone can never reach Mid, so the thresholds sit just below the reported values. Core counts were not recorded in the peer matrix; the `[perf] tier=... ram=... cores=...` logcat line from the next build will confirm each phone's tier.

Player override: Settings > Quality dropdown (Auto, Low, Mid, High) writes PlayerPrefs `tier_override` (0 Low, 1 Mid, 2 High; Auto deletes it). It applies on the next launch.

## Editor steps (done by hand in Unity)

### 1. URP assets
1. Project window, `Assets/Settings`: select `Mobile_RPAsset`, **Ctrl+D** three times. Rename the copies `URP-Low`, `URP-Mid`, `URP-High`.
2. For each, in the Inspector:

| Field | URP-Low | URP-Mid | URP-High |
|-------|---------|---------|----------|
| Quality > Render Scale | 0.75 | 0.9 | 1.0 |
| Quality > Anti Aliasing (MSAA) | Disabled | 2x | 4x |
| Lighting > Main Light > Cast Shadows | off | on | on |
| Shadows > Max Distance | (n/a) | 20 | 40 |
| Lighting > Main Light > Shadow Resolution | (n/a) | 1024 | 2048 |

3. Post-processing:
   - Low: select `Mobile_Renderer`, Ctrl+D, rename `Renderer-Low`, untick **Post-processing**. In `URP-Low` > Rendering > Renderer List, replace the renderer with `Renderer-Low`.
   - Mid and High keep `Mobile_Renderer` (post-processing on). For "Bloom only" on Mid, the Game scene's `Global Volume` profile should contain only Bloom; if it has more effects, Mid shows them too. Simplest for the showcase: leave Mid as "On" and note the difference.

### 2. Quality levels
**Edit > Project Settings > Quality**:
1. Click the `Mobile` level name and rename it **Low**. Rename `PC` to **Mid**. Click **Add Quality Level** and name the new one **High**. This keeps the order Low, Mid, High. (`DeviceTier` finds levels by name, so the order is for the lab write-up; the names must match exactly.)
2. In the level matrix, tick the **Android** column for Low, Mid and High. In the **Default** row, set Android's default to **Mid** (green tick).
3. Select each level and set:

| Field | Low | Mid | High |
|-------|-----|-----|------|
| Rendering > Render Pipeline Asset | URP-Low | URP-Mid | URP-High |
| Rendering > VSync Count | Don't Sync | Don't Sync | Don't Sync |
| Textures > Global Mipmap Limit | Half Resolution | Full Resolution | Full Resolution |
| Textures > Anisotropic Textures | Disabled | Per Texture | Forced On |
| Textures > Mipmap Streaming | on | on | on |

4. Save the project (**File > Save Project**).

### 3. Script Execution Order (optional, the code already enforces the order)
**Edit > Project Settings > Script Execution Order**: `+` **DeviceTier** = -100, `+` **PerformanceMode** = -50, Apply.

### 4. Check in Play mode / on device
Logcat filter `[perf]`: expect `tier=<name> ... quality=<same name>`, then `perf_mode=Balanced ... tier=<name> fps=60 scale=0.90 asset=URP-Mid` (values per tier), and `[telemetry] ... session_start ... tier=<name>`.
