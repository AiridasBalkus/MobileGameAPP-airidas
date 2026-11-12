## KI-01: First chunk can block the start lane right after Retry
- **Status:** Fixed in code on branch w09-showcase, ships in the next build (0.9.0). Not yet verified on device.
- **Scenario:** Found in Week 6 telemetry; reproduced in both Week 7 soaks and during the Week 8 low-RAM test (screens/05-low-ram.jpg, obstacle ahead at 25 m)
- **Build / device:** 0.8.0 (08279012), also 0.4.0; Samsung Galaxy S24 Ultra (SM-S928U1), Android 17; also XCover Pro
- **Steps to reproduce:** Start a run (or Retry) and don't swipe. Repeat ~4 times.
- **Expected:** The first seconds of a run are safe; the start (middle) lane is clear so a new player can learn to swipe.
- **Actual:** About half of runs have an obstacle in the middle lane at 20 m (Single chunk) or 26 m (PickupLine block), killing the player at ~4-5 s.
- **Logcat:** `[telemetry] 570.88 31ffabee level_fail level_id=1 time_s=3.8 cause=obstacle distance=19 coins_earned=0` and `[telemetry] 955.11 af66d58a level_fail level_id=1 time_s=4.9 cause=obstacle distance=25 coins_earned=0`
- **Cause:** `ObstacleSpawner.OnEnable` sets the first chunk at player + 20 m; `SpawnChunk` picks a random lane, so Single puts a block in lane 0 with p = 1/3 and PickupLine puts its block in lane 0 with p = 2/3 (total p = 1/2).
- **Impact:** High. An unfair death in the first 5 s, made worse because playtesters didn't know to swipe (Week 5 playtest).
- **Planned fix (next build):** Make the first chunk always a PickupLine with coins in the middle lane (block goes to a side lane). That keeps the start lane safe and teaches coin collecting, the second playtest confusion.

- **Fix (w09-showcase):** `ObstacleSpawner` has a `_first` flag reset in `OnEnable`; the first chunk of every run is a PickupLine with coins in lane 0, so its block always lands in a side lane.

## KI-02: Android navigation bar covers the bottom distance score
- **Status:** Fixed in code on branch w09-showcase, ships in the next build (0.9.0). Not yet verified on device.
- **Scenario:** Week 8 test plan, scenario 3 (rotation) and scenario 2 screenshot
- **Build / device:** 0.8.0 (08279012); Samsung Galaxy S24 Ultra (SM-S928U1), Android 17
- **Steps to reproduce:** Start a run, swipe up from the bottom edge so the navigation bar appears.
- **Expected:** The distance score stays readable above the system bar.
- **Actual:** The bar overlaps the score (screens/02-no-network.jpg).
- **Cause:** The HUD sits inside `SafeArea`, but with swipe-in navigation Android reports no bottom inset in `Screen.safeArea`, so the score at the bottom edge of the safe area is under the bar when it appears.
- **Impact:** Low. Cosmetic, but the score is the main feedback during a run.
- **Fix (w09-showcase):** `ScoreLabel.Start` lifts the score by 48 dp (Android nav bar height) on top of the safe area.

## KI-03: HUD pause button doesn't pause the run
- **Status:** Fixed in code on branch w09-showcase, ships in the next build (0.9.0). Not yet verified on device.
- **Scenario:** Peer survey for 0.8.0, response 1 ("Pause doesn't pause", written in the "one thing to keep" field)
- **Build / device:** 0.8.0 (08279012); Samsung Galaxy S25 FE (peer), Android 16
- **Steps to reproduce:** Start a run, tap the pause button (top right).
- **Expected:** Pause panel opens, the run, score and audio stop; Resume continues in place.
- **Actual (from code):** The pause panel opens but the run keeps going behind it.
- **Cause:** The Pause button's OnClick only called `PausePanel.SetActive(true)`; it never called `LifecycleGuard.SetPaused(true)`, so `Time.timeScale` stayed 1. The Android Back key (Escape in `PauseMenu.Update`) and app backgrounding did call `SetPaused(true)`, which is why test plan scenario 1 (Home + resume, 11 cycles) passed: it never used the HUD button.
- **Impact:** High for the showcase: a visible control that doesn't work, and a peer reported it.
- **Fix (w09-showcase):** `PauseMenu` finds `Canvas/SafeArea/Pause` at runtime and adds `SetPaused(true)` to its OnClick (no pausing after the run has ended). With timeScale 0 the runner, score, spawner (no player movement, so no spawns), coin bob and hint all stop; `ThermalOverlay` keeps updating on unscaled time by design. `tapSwipeInput` now ignores touches that start on an interactable button, so tapping Pause or Upgrade is never read as a lane swipe.
