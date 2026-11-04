## KI-01: First chunk can block the start lane right after Retry
- **Scenario:** Found in Week 6 telemetry; reproduced in both Week 7 soaks and during the Week 8 low-RAM test (screens/05-low-ram.jpg, obstacle ahead at 25 m)
- **Build / device:** 0.8.0 (08279012), also 0.4.0; Samsung Galaxy S24 Ultra (SM-S928U1), Android 17; also XCover Pro
- **Steps to reproduce:** Start a run (or Retry) and don't swipe. Repeat ~4 times.
- **Expected:** The first seconds of a run are safe; the start (middle) lane is clear so a new player can learn to swipe.
- **Actual:** About half of runs have an obstacle in the middle lane at 20 m (Single chunk) or 26 m (PickupLine block), killing the player at ~4-5 s.
- **Logcat:** `[telemetry] 570.88 31ffabee level_fail level_id=1 time_s=3.8 cause=obstacle distance=19 coins_earned=0` and `[telemetry] 955.11 af66d58a level_fail level_id=1 time_s=4.9 cause=obstacle distance=25 coins_earned=0`
- **Cause:** `ObstacleSpawner.OnEnable` sets the first chunk at player + 20 m; `SpawnChunk` picks a random lane, so Single puts a block in lane 0 with p = 1/3 and PickupLine puts its block in lane 0 with p = 2/3 (total p = 1/2).
- **Impact:** High. An unfair death in the first 5 s, made worse because playtesters didn't know to swipe (Week 5 playtest).
- **Planned fix (next build):** Make the first chunk always a PickupLine with coins in the middle lane (block goes to a side lane). That keeps the start lane safe and teaches coin collecting, the second playtest confusion.