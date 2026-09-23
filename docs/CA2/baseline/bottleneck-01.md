# Bottleneck 01: Obstacles never despawn

**What:** Obstacles are never destroyed, so object count, physics and script work grow for as long as the run lasts.

**Where:** `FixedUpdate.PhysicsFixedUpdate` (Physics.Simulate) and `Update.ScriptRunBehaviourUpdate` (Obstacle.Update, ObstacleSpawner.Update with Instantiate at 0.34 ms); cause is `Obstacle.cs` comparing against world z -10 instead of the player's position.

**Numbers:**
- Main thread: frame locked at ~16.9 ms (60 fps cap); real work ~4.3 ms (frame 1436) -> ~6.1 ms (frame 3425, ~33 s later)
- Physics 0.62 -> 1.15 ms, scripts 0.41 -> 0.61 ms over the same window
- Object count ~3.4k -> ~5.6k
- SetPass calls: 23 (all obstacles in 1 SRP Batch)
- GC alloc: ~51 B/frame; one GC.Collect spike ~1.06 ms around frame 1910
- Frame time full scale 16.57 ms, half scale 16.46 ms

**Verdict:** CPU-bound. Halving render scale changed frame time by less than 1%, Gfx.WaitForPresentOnGfxThread stays under 1 ms, and the growing markers are physics and scripts, not rendering.

**Fix to try:** Despawn obstacles relative to the player's position and return them to an object pool instead of calling Instantiate/Destroy.

## Evidence
- Profiler capture: [w03-profile.zip](w03-profile.zip) (contains w03-profile.data; unzip and load)
- Bad frame: ![bad frame](w03-bad-frame.png)
- GPU pass (Frame Debugger): ![gpu pass](w03-gpu-pass.png)