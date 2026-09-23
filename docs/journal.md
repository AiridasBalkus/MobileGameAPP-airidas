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