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