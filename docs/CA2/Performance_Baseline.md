# Mino: Performance Baseline (CA2, one page)

**Device:** Samsung Galaxy S24 Ultra (SM-S928U1) · Snapdragon 8 Gen 3 for Galaxy · 12 GB RAM (11,084 MB reported) · Android 17 / API 37 · Vulkan · 1080 × 2340, 120 Hz panel
**Battery health:** Good (`dumpsys battery` health = 2); capacity estimate 93% of design (Samsung ASOC), BSOH 100
**Submitted build:** v0.9.1 (4c05bd73), release-signed, IL2CPP, ARM64 · Mode: Balanced (60 fps) · Tier: High (DeviceTier, auto)
Full history (6 rows, 3 devices, raw captures): `docs/CA2/baseline/baseline-sheet.md`

| Metric | Value | Build and method |
|--------|-------|------------------|
| Frame time avg / 99th percentile | **16.64 / 19.00 ms** (p95 18.10, 8 of 2000 frames > 20 ms) | 0.9.0 dev (d573bf52), after the art pass; Profiler, 2000 frames; p99 from Profile Analyzer frame-time CSV |
| CPU vs GPU time | **CPU 5.41 ms, GPU 5.38 ms** per frame (median of 744 samples); bottleneck = *TargetFrameRate* in 99% of samples, so neither CPU nor GPU limits the 16.7 ms frame | 0.4.0 dev, Adaptive Performance logs over the 10-min Balanced soak. After the art pass, Profiler main-thread work is 5.8 ms (PlayerLoop 16.36 − WaitForSignal 10.52) |
| GC allocs / frame | **99 B** median (Input System 51 B + Debug.Log) | 0.9.0 dev, Profiler CSV. Debug.Log spam stripped from release builds in Week 9 |
| Peak memory | **Total PSS 367.5 MB** (budget 600). Textures 5.9 MB, render targets 113.9 MB, managed heap 26–29 MB, audio ~2 MB | `dumpsys meminfo` after 60 s play; category split from Memory Profiler (High tier) |
| Draw calls / batches | **32 SetPass, 29.1k triangles**; draw calls batched by the SRP Batcher (counter reads 0) | 0.9.0 dev, Profiler Rendering module, gameplay frame |
| Resolution / scale | 1080 × 2340; High: render scale 1.0, MSAA 4x, HDR off. Mid 0.9 / 2x. Low 0.75 / off, 30 fps | URP-Low / Mid / High, chosen by DeviceTier |
| Thermals | Start **27.8 °C** battery (29.6 skin). After 10 min: **+0.4 °C** battery (+1.3 skin). **No throttling**: NoWarning throughout, Android Thermal Status 0, max AP level 0.53. p99 18.72 → 18.59 ms | 0.4.0 dev, Balanced, unplugged; `dumpsys thermalservice` + ThermalOverlay + logcat |
| Cold start | **118 ms** to first frame (median of 107 / 118 / 125); interactive **~2.3 s** (mostly Unity splash); warm 34 ms | 0.8.0 release, `am start -W` |
| APK size | **34.7 MB** (35,528 KB), budget 100 MB | 0.9.1 release |

**Bottleneck fixed since Week 3.** Obstacles were never despawned, so main-thread work grew from 4.3 to 6.1 ms within 33 s (physics 0.62 → 1.15 ms, objects 3.4k → 5.6k), while halving render scale changed frame time by under 1%: CPU-bound. In Week 5 obstacles were despawned relative to the player and recycled through a 24-object pool fed by an `Awaitable` spawner. Afterwards the spawner and pool allocate 0 B and frame time stays flat at 16.70 / 16.71 ms (avg / p99) over a five-minute run; two side effects (a stale serialized despawn value and a spawner-before-pool start-up order) were found and fixed. *(Before: OPPO Find X5 Pro; after: Galaxy XCover Pro, as the OPPO broke in Week 5.)*

**Second fix (Week 9, memory).** Render targets on the High tier were the largest category at 134.8 MB (4x MSAA + HDR at 1080 × 2340). HDR off brought them to 113.9 MB (−20.9 MB); the Low tier uses 11.2 MB. Next: MSAA 2x on High to reach the 64 MB render-target budget.
