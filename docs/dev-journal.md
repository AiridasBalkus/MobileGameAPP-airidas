Week 1 - Project shortlist

Shortlisting the Runner option. Core verb: lane change (swipe left/right
to move between 3 lanes while auto-running forward). First thing I'd cut
if time ran out: any scoring/combo multiplier system — the bare lane-change
loop with obstacles is the minimum viable version.

Week 2 - 
- CPU: 33.84ms
- SetPass calls: 1
- GC allocated per frame: 3
- swipe dp: 60f
- tapmax: 0.3f

Week 3 - Lab A, Part B (stress test, 20 obstacles/s, targetFrameRate 60)
- Frame time locked at ~16.9 ms (vsync cap) throughout
- Real main-thread work (PlayerLoop minus Gfx.WaitForPresentOnGfxThread):
  frame 1436: ~4.3 ms, frame 3425 (~33 s later): ~6.1 ms
- Tallest marker: PostLateUpdate.FinishFrameRendering 12.31 ms (mostly vsync wait)
- Top real work (frame 3433): FixedUpdate.PhysicsFixedUpdate 1.15 ms,
  Update.ScriptRunBehaviourUpdate 0.61 ms, PostLateUpdate.UpdateAllRenderers 0.08 ms
- Early vs late (frames with a physics step):
  ~1436: Physics 0.62 ms, Scripts 0.41 ms
  3433:  Physics 1.15 ms, Scripts 0.61 ms
- Zoomed frame 3435: Physics.Simulate 0.68 ms; Instantiate 0.34 ms inside ObstacleSpawner.Update
- Obstacles never despawn (Obstacle.cs compares to world z -10), so script
  and physics work grow over the run
- GC.Collect: one spike ~1.06 ms around frame 1910; GC alloc ~51 B/frame

Week 3 - Lab A, Part C (frame 3433)
- Rendering: 23 SetPass, 6.1k tris, 11.7k verts (SRP Batcher, draw calls read 0)
- Memory: Total Reserved 364.0 MB (195.2 MB in use), textures 44.9 MB,
  meshes 128.7 KB, audio 1.1 MB, graphics/driver 70.1 MB, managed heap 3.7/4.8 MB
- Object Count rises all run: ~3.4k (frame 1910) -> ~5.6k (frame 3433)
- Rendering stays flat (off-screen cubes culled); the leak costs CPU, not GPU

Week 3 Lab A, Part D (RenderScaleProbe, stress test on)
- Probe toggle visible in capture: texture memory 44.9 MB -> 34.5 MB (render targets halved)
- Full scale: frame 16.57 ms, WaitForLastPresentation 11.10 ms (~5.5 ms real work),
  Gfx.WaitForPresentOnGfxThread 0.00 ms
- Half scale: frame 16.46 ms, WaitForLastPresentation 9.97 ms (~6.5 ms real work),
  Gfx.WaitForPresentOnGfxThread 0.81 ms
- Verdict: CPU-bound. Frame time barely moved (<1%) at half resolution, and the
  CPU never waits on the GPU. Real work rose during the run because obstacles
  keep accumulating (object count ~3.2k -> ~5.6k), not because of render scale.
- Candidate fix: stop the obstacle leak. Despawn relative to the player and pool
  obstacles instead of Instantiate/Destroy (Physics + ScriptRunBehaviourUpdate
  are the growing markers)

  Part E (Frame Debugger, AGI not used)
- 25 events per frame; all gameplay geometry in 1 SRP Batch (RenderLoop.DrawSRPBatcher)
- Busiest pass: BloomDownsample (10 draws) + BloomUpsample (5) = 17/25 events are bloom
- No GPU timings on this route; GPU not the limit per Part D, bloom noted for later

Week 3 Lab B, Part B (release 0.1.2, frame pacing on, targetFrameRate 60)
- Menu (pause panel): avg 16.64 ms / p99 16.65 ms
- Steady gameplay: avg 16.64 ms / p99 16.65 ms
- Worst case (spawn interval 0.05 s): avg 16.64 ms / p99 16.65 ms, unchanged after ~77 s
- No missed frames in any state; [HIT] Debug.Log spams during worst case
- Phone refresh rate = 120 Hz

Week 3 Lab B, Part C (Dev build, normal play, spawn interval 1.5)
- Steady-state GC Allocated in Frame: 51 B (3 x 17 B)
- Allocating marker: NativeInputSystem.NotifyBeforeUpdate() (Input System package, not project code)
- Hit frames: extra allocation from Debug.Log in PlayerCollision ([HIT] message)  <- confirm
- GC.Collect: one in ~42 s capture (frame 1784, 2.82 ms, 16% of that frame)
- Target 0 B: remove [HIT] Debug.Log; Input System allocation to investigate in Week 5

Week 3 Lab B, Part D (Dev build, stress test, 60 s, frame 2733)
- Peak Total Reserved: 363.0 MB (194.3 MB in use); textures 44.9 MB (104)
- Worst case rendering: SetPass 25 / batches n/a (SRP Batcher; counter reads 0,
  Frame Debugger shows 1 SRP Batch for gameplay, 25 events) / triangles 6.1k
- Peak Total Reserved 363.0 MB (Dev, stress, 60 s) / TOTAL PSS 176.6 MB (release, normal play, 2 min)
  PSS < Reserved: reserved address space isn't all resident; Dev build adds ~37 MB profiler
- Cold start median 260 ms; APK 52.9 MB
- Throttling: none observed

5 Oct 2026: Week 5 Lab A (skeleton, Awaitable, pooling)
- Scene flow: Boot (App: Bootstrap + FrameTimeSampler, DontDestroyOnLoad) > Menu > Game > Result > Retry. GameManager states Playing / Paused / Won / Lost; Won unused by design (endless runner). Feedback on the core loop: live distance score label (SetText, no per-change string allocation).
- Coroutine hunt: 0 in project code; 15 TMP sample scripts only. Converted ObstacleSpawner's Update() timer to an Awaitable loop with a token linked to Application.exitCancellationToken.
- Pooling: ObstaclePool (Stack, 24 prewarmed); obstacles recycle 10 units behind the player. Fixes bottleneck-01 (Week 3 leak).
- Bug: obstacles vanished in front of the player. The new field reused the old name despawnDistanceZ, so the prefab kept its saved -10 and the check became "10 units ahead". Lesson: serialized prefab values override code defaults; renamed the field to despawnBehind.
- Device: OPPO broken, profiled on Samsung XCover Pro (Android 13). Profiler connection needed adb forward and Enter IP 127.0.0.1:34999 as default ports didn't match.
- GC: 51 B/frame, all Input System; spawner/pool 0 B. Frame time 16.70/16.71 ms without Profiler; Profiler overhead causes 33 ms frames on this phone.

12 Oct 2026: Week 5 Lab B (greybox lane, SceneLoader)
**Lane:** one greybox biome, 5 chunk types (Single, Pickup line, Wall, Zigzag, Corridor), speed ramp 5 to 12 units/s, finish at 1200 m. Full run ~2.5 min; crash ends it early. Chunk types unlock by distance (150 m, 400 m).
- Roles by flat colour: floor grey, hazard red, pickup yellow, goal green.
- Feedback: click on lane change, higher click + score +10 on pickup, camera shake + haptic on hit (Awaitable, no coroutine; click generated in code as a stub).
- Bugs: (1) spawner hit an uninitialised pool. An async method runs synchronously until its first await, and the spawner's OnEnable ran before the pool's Awake; fixed with one NextFrameAsync. (2) Fire-and-forget Awaitables swallow exceptions; added catch (Exception e) { Debug.LogException(e); } to every loop.
- SceneLoader: persistent on App in Boot, static Instance, IsLoading guard (double taps, back button ignored), try/finally so the loading screen always hides. Every scene change goes through it.
- allowSceneActivation trap: with while (!op.isDone) the bar sat full forever. isDone can't become true until activation, which that loop never reaches. Reverted to progress < 0.9f.
- Resilience: Home mid-load (5 s debug hold, dev builds only) completed and the loading screen hid; back button ignored while loading.
- Perf (release, Samsung): cold start median 349 ms; interactive 2.51 s (mostly the Unity splash; candidate fix: disable splash); Menu into Game 110 ms; APK 31.2 MB. All within MDA budgets.
- Playtest confusions: 1 Not knowing that they needed to swipe 2 Didn't know to collect the coins
- First change: Not knowing that they needed to swipe

19 Oct 2026: Week 6 Lab A (Economy)
- Run 1 (wallet reset, no purchases): L1 0:36, L2 0:56, L3 2:17, finished 2:22
- Run 2: L4 at 2:20, finished 2:22. ~812+ coins earned in ~4:40 of play (~175/min)
- Projected L5 at ~8 min of play: both targets (first < 2:00, fifth by 10:00) met at 5 coins/pickup
- Scope change (after 16 Sep lock): added a soft-currency economy (coins from pickups,
  lane-change speed upgrade) and a stubbed "Buy 500 coins" button, as required by Week 6 Lab A.
  The coin pack is a non-functional IAP stub behind IPurchaseProvider; it is not planned
  for any release (see MDA monetisation note).
- Earn amount: before 5 coins/pickup into after 5 (unchanged; first upgrade at 0:36 already beat the 2:00 target)
- Upgrades bought in the first 3 min of play: 3 (run 1 ended at 2:22, before the 3:00 mark)
- Upgrade: lane-change speed +35%/level. Players feel it because lane change is Mino's only verb;
  faster switches make the late-run walls and zigzags dodgeable.

20 Oct 2026: Week 6 Lab B (Telemetry)
- Events: session_start, level_start, level_complete, level_fail, upgrade_purchased (+ purchase_stub). Local only: logcat + telemetry.log in persistentDataPath. Map: docs/CA2/Economy_Telemetry_Map.md
- Verified: upgrade_purchased logs the price paid (cost=100, 150, 225, 337); coins in level_start match previous coins_earned + stub purchases − upgrades; exactly one level_fail/level_complete per run.
- Cold start to first script: ~2.2–2.4 s on the XCover (session_start timestamp).

20 Oct 2026: Week 7 Lab A
- Home + resume (monkey): PASS, coins unchanged, app returned paused
- Force-stop (outside run): PASS, 163 → loaded 163
- am kill while backgrounded: PASS, 163 → loaded 163
- Log pulled to docs/CA2/telemetry/telemetry.log: 5+ sessions with distinct ids.
- Finding: 2 of 6 logged runs failed at 19 m / 3.8 s, one with no upgrade taps. The first chunk spawns at 20 m and can block the start (middle) lane right after Retry. Week 9 fix candidate: first chunk always a pickup line, or start it further away.
- Async Pack 1 (Monetisation Models and Ethics) due Sun 1 Nov 2026, 23:59 (end of Reading Week, 26–30 Oct)

3 Nov 2026: Week 7 Lab B (thermal soak), done late on the new phone
- Device change (after the 16 Sep lock): OPPO broken since Week 5, XCover used Weeks 5 to 7 Lab A. From Week 7 Lab B my own Galaxy S24 Ultra (SM-S928U1, Android 17) is the test phone. MDA budget was written for the OPPO; CA2 numbers from now are from the S24.
- ThermalOverlay.cs on the HUD (ms, WarningLevel, TemperatureLevel, trend); label refreshes 4x/s to avoid per-frame string allocation.
- Wireless adb kept the phone unplugged. Two entries for one phone gave "more than one device"; fixed with set ANDROID_SERIAL=ip:port (port changes after drops).
- Temperature from dumpsys thermalservice (BAT/SKIN); dumpsys battery only shows history on Samsung, /sys is permission-denied.
- First soak accidentally ran at 120 fps (High FPS left on), so before (60) and after weren't comparable. Kept it as the High FPS soak and redid Balanced after cooling back to 27.8 C.
- Balanced: +0.4 C in 10 min, NoWarning throughout, max AP level 0.53. Frame time before 16.64 / 18.72 ms, after 16.64 / 18.59 ms: no change.
- High FPS: 8.3 ms held 10 min, AP level 0.50 -> 0.60, at least +1.7 C, no throttling.
- First-chunk bug again: died at 19 m / 3.8 s (Balanced) and 25 m / 4.9 s (High FPS). 4 times total including Week 6.
- GC now ~99 B/frame (was 51 B), 51 LogStringToConsole calls per capture: Debug.Log spam ([AP], [Baseline], telemetry).
- Sustained budget: on the S24 it equals the burst budget, a locked 16.7 ms at 60 fps with ~11 ms headroom and no throttling after 10 min.
- Change because of it: no need to cut quality for thermals, so Balanced stays default. Spend the time on the first-chunk fix (first chunk always a pickup line, or spawn it further away) and stripping Debug.Log from release builds to get GC back to 0 B.
- Reading Week 26-30 Oct (no classes); back Mon 2 Nov (Week 8 Lab A, peer distribution with a release build). Async Pack 1 due Sun 1 Nov: submitted, CA2 due Sun 15 Nov 2026, 23:59: draft reflective bullets for Weeks 5-7.

4 Nov 2026: Week 8 Lab A (peer distribution)
- Version 0.8.0, versionCode 4 -> 5. Release-signed, IL2CPP, ARM64, Development Build off; Adaptive Performance logging turned off ("logging quiet").
- About panel in Menu with BuildStamp.cs (version, 8-char buildGUID, device, Android, RAM, resolution, Hz, form link) and a "Give feedback" button (Application.OpenURL). Android back closes the panel (Escape in the Input System).
- session_start now logs version, build and device. Verified: About shows 08279012, logcat session_start build=08279012, dumpsys versionName=0.8.0 / versionCode=5.
- First build (375b9535) had a duplicated resolution line; rebuilt before sharing. Uploading with the same file name and choosing Replace on OneDrive kept the same share link, so the QR didn't change.
- Stamp reports 60 Hz on a 120 Hz panel: Screen.currentResolution gives the current mode, and Samsung drops to 60 Hz while Mino runs at 60 fps.
- Created the form before building so the link is in the APK; did Part B before Part A to avoid building and uploading twice.
- Feedback form live (anonymous, 12 fields). Peer installs (Part D) on 5 Nov.
- Async Pack 3 (Peer APK Distribution & Survey) live until Sun 29 Nov 2026: 10+ testers, CSV export, 5 to 8 insights in docs/CA3/qa/survey/insights.md.

4 Nov 2026: Week 8 Lab B (QA evidence)
- Build uses GameActivity: launch activity is UnityPlayerGameActivity, not UnityPlayerActivity ("Error type 3" until fixed).
- Cold start median 118 ms, warm (HOT) 34 ms, interactive ~2.29 s, PSS ~318 MB after 5 min (baseline Row 5).
- Test plan: resume (11 cycles), no network, rotation, low RAM and cold/warm start passed; battery saver not run (Samsung won't enable Power saving while USB-charging at 100%).
- USB cable instead of wireless adb so airplane mode wouldn't drop adb.
- Recorder stopped mid-lab (16:44-17:00); recovered from the phone's log buffer. Lesson: don't stop the recorder to share the file.
- Worst failure: KI-01. The first chunk blocks the start lane in ~50% of runs (Single p=1/3 at 20 m, PickupLine block p=2/3 at 26 m). Filed with cause and fix; fix ships in the next build. KI-02: nav bar covers the score when swiped in.

5-9 Nov 2026: Week 8 Lab A, Part D (peer installs)
- Two peers installed 0.8.0 (08279012) by QR: Galaxy S25 FE (Android 16, high tier) and Galaxy A15 (Android 14, 4 GB, low tier). Both worked first time, no crashes, both copied the correct build ID.
- Peer 1 (5 min): smoothness 2, control 1, "one more go" 1; asked to "run faster" and "what does upgrade do?" into the start feels slow and the upgrade button is unexplained.
- Peer 1 wrote "Pause doesn't pause" in the keep field: the HUD pause button was never covered by the test plan (only lifecycle pause). To verify.
- Peer 2 (10 min, low-tier): mid scores (3-4) across the board, no comments.
- Form fixes: Likert options exported as "Option 1-5" (renamed to 1-5); "Control responsiveness" split into two statements by mistake.
- Pack 3 needs 10+ testers by Sun 29 Nov; 2 so far.

11 Nov 2026: Week 9 Lab A + showcase fixes
- Branch w09-showcase. Code changes made with Claude Code, reviewed by me; nothing below is verified on device yet (0.9.0 not built).
- KI-01 fix: first chunk of every run is a PickupLine with coins in the middle lane, block in a side lane. Evidence: KnownIssues KI-01, level_fail at 19 m / 3.8 s and 25 m / 4.9 s (Weeks 6-8).
- KI-03 (new): "Pause doesn't pause" (survey response 1, S25 FE). The HUD Pause button only showed the panel and never called LifecycleGuard.SetPaused(true), so timeScale stayed 1. The test plan missed it because it only tested Home/Back pause. Fixed by wiring the button at runtime; swipes that start on a button are ignored.
- Upgrade label now says what it does ("Faster lane change  Lv N / cost coins") and dims when unaffordable. Evidence: survey response 1, "what does upgrade do?".
- Start speed 5 -> 7.5, acceleration 0.05 -> 0.2 (top speed 12 reached in ~22 s instead of ~140 s); second chunk still arrives after ~5.7 s. Evidence: survey response 1, "Run faster", Control 1, One more go 1.
- Onboarding hint ("Swipe left / right", then "Collect coins to upgrade") for the first 3 runs. Evidence: Week 5 playtest, testers didn't know to swipe or collect coins.
- KI-02 fix: distance score lifted 48 dp above the safe area. Evidence: test plan scenario 3 note, screens/02-no-network.jpg.
- About screen: blank line before "Feedback:" (builds.md 0.8.0 known cosmetic issue).
- DeviceTier: Low / Mid / High from RAM and cores, thresholds 7000 / 3500 MB instead of the lab's 8192 / 4096 because phones report less than nominal RAM (S24 Ultra 11084, S25 FE 7204, A15 3638 MB). Tier override in Settings (applies next launch); session_start now logs tier. Low stays at 30 fps unless High FPS is chosen. See docs/CA2/quality-tiers.md.
- Underwater art pass: one AudioService (music, swipe/coin/hit/click/new best/upgrade/run start, Sound toggle) replacing the Week 5 generated click; UI sprites, logo, coin icon; ambient bubbles skipped on Low tier.
- Polish: coin bob/spin, coin counter pop with "+5", red hit flash, "New best!" on Result, bigger Play/Retry/Resume, 48 dp touch targets.
- Release logs: [Baseline], [Loader], [Wallet], [AP], [iap-stub], [Probe] now only in Editor/Development builds (Week 7: GC ~99 B/frame from Debug.Log spam). [telemetry], [perf], build= and [Pause] kept.
- Next: Editor steps (Quality levels Low/Mid/High and URP assets, tier dropdown, Always Included particle shader), build 0.9.0 / versionCode 6, re-run the pause test x10 with the HUD button, measure Week 9 memory budget.
