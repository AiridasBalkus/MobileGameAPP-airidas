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