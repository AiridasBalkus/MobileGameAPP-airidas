| Field | Row 1 |
|-------|-------|
| Date, commit, versionName / versionCode, Release or Dev, Unity version | 23 Sep 2026, e5813ea, 0.1.4 / 4, Release (memory peak and GC from Dev), Unity 6000.6.0f1 |
| Device model, Android version, SoC / GPU, graphics API, refresh rate | OPPO Find X5 Pro (CPH2305), Android 15, Snapdragon 8 Gen 1 / Adreno 730, Vulkan, 120 Hz |
| Target fps | 60 (Optimized Frame Pacing on) |
| Menu: avg ms / p99 ms | 16.64 / 16.65 (pause panel) |
| Steady gameplay: avg ms / p99 ms | 16.64 / 16.65 |
| Worst case: avg ms / p99 ms | 16.64 / 16.65 (spawn interval 0.05 s, unchanged after ~77 s) |
| GC allocated per frame (bytes), allocating markers | 51 B; NativeInputSystem.NotifyBeforeUpdate (Input System); [HIT] Debug.Log on hit frames; GC.Collect 2.82 ms once in ~42 s |
| Peak Total Reserved (MB) / TOTAL PSS (MB) | 363.0 (Dev, stress, 60 s) / 176.6 (release, normal play, 2 min) |
| Worst case: SetPass / batches / triangles | 25 / n/a (SRP Batcher; 1 SRP Batch for gameplay per Frame Debugger) / 6.1k |
| Cold start ms (median of 3), first interactive s | 260 ms (295, 260, 257) / no loading screen, not timed separately |
| APK size (MB) | 52.9 |
| Thermal delta, throttling (Week 7) | |
| Load time (Week 5) | |
| Field | Row 2 (Week 5) |
|-------|----------------|
| Date, commit, build | 5 Oct 2026, v0.3.0-skeleton, Dev build |
| Device | Samsung Galaxy XCover Pro (SM-G715FN), Android 13, Exynos 9611 / Mali-G72 (OPPO broken; not directly comparable with row 1) |
| Steady gameplay with spawning: avg / p99 | 16.70 / 16.71 ms (Profiler detached, ~5 min run); 21-24 / 33.4 ms with Profiler attached |
| GC per frame | 51 B, all from NativeInputSystem.NotifyBeforeUpdate (Input System); 0 B from ObstacleSpawner / ObstaclePool / Obstacle |
| Change | Obstacles pooled (24 prewarmed) and recycled relative to the player; spawner converted to an Awaitable loop |
| Field | Row 3 (Week 5 Lab B) |
|-------|----------------------|
| Date, build | 12 Oct 2026, v0.3.1, Release |
| Device | Samsung Galaxy XCover Pro (SM-G715FN), Android 13 |
| Cold start TotalTime (3 runs, median) | 324 / 349 / 405 ms, median 349 ms |
| Time to interactive (Menu first frame) | 2.51 s median (2.45-2.58 s); includes Unity splash. Budget < 5 s: pass |
| Scene load: Boot>Menu / Menu>Game / Game>Result | 266 ms / 110 ms (46 ms on Retry) / 64-85 ms |
| APK size | 31.2 MB (budget < 100 MB) |

| Thermal delta, throttling (Week 7) | Not measured on this phone (OPPO broken in Week 5); see Row 4 (S24 Ultra) |
| Field | Row 4 (Week 7 Lab B, thermal soak) |
|-------|------------------------------------|
| Date, commit, build | 3 Nov 2026, 4dc1695, v0.4.0 / 4, Dev build (Development Build on for Profiler) |
| Device | Samsung Galaxy S24 Ultra (SM-S928U1), Android 17 / API 37, Snapdragon 8 Gen 3 for Galaxy, Vulkan, 120 Hz (new test phone; not directly comparable with rows 1-3) |
| Mode | Balanced (60 fps), Game scene, fresh run from Retry, crashes retried immediately |
| Start temperature (C) | 27.8 battery / 29.6 skin, Thermal Status 0, 18:18:21 |
| End temperature (C) and 10-minute delta | 28.2 battery / 30.9 skin at 18:30:42 (19 s after end); delta +0.4 C battery, +1.3 C skin |
| Overlay level at minute 0 / 5 / 10 | 0.46 / 0.51 / 0.52, NoWarning at all three (16.6 / 16.7 / 16.6 ms) |
| First ThrottlingImminent (minute, logcat time) | Not observed in 10 min; max AP level 0.53 (logcat 18:25:28) |
| Throttling observed (yes/no, minute) | No; Android Thermal Status 0 throughout |
| Frame time before: mean / 99th (ms) | 16.64 / 18.72 |
| Frame time after: mean / 99th (ms) | 16.64 / 18.59 (Compare: mean -0.01, median -0.02, max 25.42 -> 22.00) |
| Conditions | Unplugged (wireless adb), 70% battery, brightness ~50% (adaptive off), case off, other apps closed, ambient ~18C |
| High FPS soak (extra) | 120 fps (8.3 ms) held 10 min; AP level 0.50 -> 0.60, NoWarning, no throttling; 27.7 -> 29.4 C battery (read 5.5 min after end, so delta of at least +1.7 C is understated) |
| Method notes | Temperature from `adb shell dumpsys thermalservice` (BAT/SKIN, HAL current): `dumpsys battery` shows only history on Samsung and /sys is permission-denied on Android 17. Profiler CPU + Rendering only, 2000-frame buffer (last ~33 s of each 60 s capture). p99 calculated from Profile Analyzer "Single Frame Times" CSV export (no p99 in Frame Summary). Skin throttle thresholds start at 38 C. |
| Sustained budget | 16.7 ms at 60 fps, same as burst; ~5.4 ms real work per frame (PlayerLoop 16.58 - WaitForSignal 11.20), ~11 ms headroom, no throttling in 10 min |
| Evidence | docs/CA2/baseline/thermal-soak/: before/after .data + .pdata + frameTime.csv, compare.png, logcat-soak.txt, logcat-soak-highfps.txt, minute-00/05/10.jpg (phone screenshots, kept as JPG), highfps-minute-00.png + highfps-minute-05/10.jpg |
| Field | Row 5 (Week 8 Lab B, start-up and memory) |
|-------|-------------------------------------------|
| Date, build | 4 Nov 2026, v0.8.0 / 5, build 08279012, Release |
| Device | Samsung Galaxy S24 Ultra (SM-S928U1), Android 17 / API 37 |
| Launch activity | com.unity3d.player.UnityPlayerGameActivity (GameActivity, not UnityPlayerActivity) |
| Cold start TotalTime (3 runs, median) | 118 / 125 / 107 ms, median 118 ms (first launch after install excluded: LaunchState UNKNOWN, WaitTime 3021 ms) |
| Warm start TotalTime (Android: HOT, 3 runs) | 77 / 31 / 34 ms, median 34 ms |
| Time to interactive (Menu, [perf] interactive) | 2.24-2.33 s, median ~2.29 s (mostly Unity splash). Budget < 5 s: pass |
| TOTAL PSS after ~5 min play | 325,924 KB (~318 MB); RSS 442,916 KB; swap PSS 75 KB. Budget < 600 MB: pass. Not comparable with row 1 (OPPO, 176.6 MB) |

| Field | Row 6 (Week 9 Lab A, art pass and device tiers) |
|-------|-------------------------------------------------|
| Date, build | 12 Nov 2026, v0.9.0 / 6, build d573bf52, Development Build |
| Device, tier | Samsung Galaxy S24 Ultra (SM-S928U1), Android 17; DeviceTier = High (11084 MB, 8 cores), URP-High, 4x MSAA, render scale 1.0 |
| Frame time: mean / p99 (ms) | 16.64 / 19.00 (p95 18.10, max 27.99, 8 of 2000 frames > 20 ms); w09-frameTime.csv |
| vs Week 7 (S24, before art) | 16.64 / 18.72: art pass (fog, ~400 static props, particles, audio, shadows, 4x MSAA) costs ~0.4 ms real work (5.4 -> 5.8 ms) and +0.3 ms p99; still 60 fps with ~10 ms headroom |
| Rendering (gameplay frame) | 32 SetPass, 29.1k triangles, draw calls n/a (SRP Batcher), 20 RT changes. Week 3 (OPPO, greybox): 23-25 SetPass, 6.1k triangles; increase is the scenery props and the shadow pass |
| GC per frame | 99 B median (Input System + overlay), unchanged |

| Category | Budget (MB) | Measured (MB) | Inside? |
|----------|-------------|---------------|---------|
| Textures (Texture2D incl. atlases) | 100 | 5.9 (High) / 4.1 (Low) | Yes (ASTC 6x6 / 4x4) |
| Render targets (RenderTexture) | 64 | High: 134.8 -> 113.9 after HDR off; Low: 11.2 | High: **No**, named offender = 4x MSAA at 1080x2340. HDR off saved 20.9 MB; next: MSAA 2x on URP-High before CA3. Low/Mid: Yes |
| Audio (AudioClip + AudioManager) | 20 | ~1.3 + under 1.0 | Yes (music streamed) |
| Managed heap | 32 | 26.0-29.2 | Yes |
| Total PSS (dumpsys meminfo, 60 s play) | 600 | 367.5 tier High; Week 8: 318 | Yes |

Note: Memory Profiler "Total Resident" varied 284-397 MB between captures and the Low capture moved ~170 MB into "Untracked" (Vulkan driver memory it can't attribute), so PSS from dumpsys is used for the total.