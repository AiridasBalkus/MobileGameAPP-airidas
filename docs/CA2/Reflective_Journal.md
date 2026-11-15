Mino: Reflective Development Journal (CA2)

Airidas Balkus (20107228) · Mobile Game Development (A12581) · Weeks 5 to 9 · Full weekly log with every measurement: docs/dev-journal.md

Weekly bullets

Week 5 (5 to 12 Oct). Boot > Menu > Game > Result scene flow; spawner converted to an Awaitable loop; ObstaclePool (24 prewarmed) fixes the Week 3 obstacle leak. Greybox lane: 5 chunk types, speed ramp, finish at 1200 m. SceneLoader with an IsLoading guard and try/finally so the loading screen can never hang. Playtest: testers did not know to swipe or to collect coins. The OPPO test phone broke; moved to a Galaxy XCover Pro.

Week 6 (19 to 20 Oct). Coin economy (5 coins per pickup, lane-change speed upgrade costing 100 × 1.5^level) and a stubbed IPurchaseProvider. Six local telemetry events. Save/resume kill tests passed. Telemetry showed 2 of 6 runs ending at 19 m after 3.8 s.

Week 7 (20 Oct to 3 Nov). Adaptive Performance with a Balanced / High FPS toggle; the XCover was GPU-bound. 10-minute thermal soak on my new Galaxy S24 Ultra: +0.4 °C, no throttling, p99 18.72 ms before and 18.59 ms after.

Week 8 (4 to 9 Nov). Release 0.8.0 with a build stamp, QR distribution and an anonymous feedback form; two peers (Galaxy S25 FE, Galaxy A15) installed it first time. Six-scenario test plan: five pass, battery saver not run. Worst failure filed as KI-01.

Week 9 (9 to 12 Nov). Survey-driven fixes, underwater art pass, Low / Mid / High device tiers, asset budgets measured; showcase on Wed 11 Nov.

Structured reflection

This section follows Gibbs' (1988) cycle, condensed to Description, Evaluation, Analysis and Action.

1. What changed in scope or design, and why

Description. Three things changed after the scope lock on 16 Sep. The Week 6 lab added a soft-currency economy and a purchase stub. The test phone changed twice (OPPO to XCover in Week 5, XCover to S24 Ultra in Week 7). In Week 9 I pulled audio, particles and the underwater theme forward from the MDA's CA3 cut list (Hunicke, LeBlanc and Zubek, 2004), and raised the start speed from 5 to 7.5 units/s.

Evaluation. The economy and the art pass improved the slice. The device changes were the biggest disruption: my MDA budgets were written for a Snapdragon 8 Gen 1, and rows 1 to 6 of the Baseline Sheet come from three different phones.

Analysis. The art pass was justified by data, not taste. The Week 7 soak showed about 11 ms of idle time per 16.7 ms frame on the S24, so I could afford it. After the art pass, real work rose only from 5.4 to 5.8 ms per frame and p99 from 18.72 to 19.00 ms. The speed change came from the peer survey: one tester asked to "run faster" and scored Control 1/5 and "One more go" 1/5. The device changes taught me that a number without a device and build attached is worthless, which is why the Week 8 build stamp now puts the build ID on the About screen, in every session_start and in the survey.

2. Key problems: problem, approach, outcome

Performance: the obstacle leak. In Week 3 the Profiler showed real main-thread work growing from 4.3 to 6.1 ms over 33 seconds, physics from 0.62 to 1.15 ms and object count from 3.4k to 5.6k. Halving render scale changed frame time by less than 1%, so the game was CPU-bound (Unity Technologies, 2026). The approach was to despawn relative to the player and recycle obstacles through a pool instead of Instantiate. Outcome: spawner and pool allocate 0 B per frame, and frame time stayed flat at 16.70 / 16.71 ms (avg / p99) over a five-minute run. Two side effects appeared and were fixed: obstacles vanished in front of the player because the prefab kept an old serialized value, and the spawner hit an uninitialised pool because an async method runs synchronously until its first await.

Design: the unfair start. Telemetry logged level_fail at 19 m / 3.8 s twice in Week 6, and twice more during the Week 7 soaks. Reading ObstacleSpawner showed why: the first chunk spawns 20 m ahead in a random lane, so the start lane is blocked in about half of all runs. I filed it as KI-01 with that analysis in Week 8, and in Week 9 made the first chunk always a coin line in the middle lane with its obstacle to the side. This removes the cause by construction rather than lowering its probability, and it also teaches coin collecting, the second Week 5 playtest confusion. The test for it is simple and repeatable: five Retries without swiping must never end in the first five seconds, and level_fail with distance under 30 m should disappear from telemetry.

Quality: the test plan missed a real bug. My Week 8 plan passed 11 background/resume cycles, yet a peer wrote "Pause doesn't pause". The HUD button only showed the panel and never called LifecycleGuard.SetPaused, so Time.timeScale stayed at 1. My test exercised the operating-system path, not the player's path.

3. Evaluation of my process

Evidence-first decisions worked: every Week 9 change traces to a telemetry line, survey answer or Known Issue. My measurement discipline was weaker. The first thermal soak ran in High FPS by mistake, so before and after were not comparable and I repeated it. The logcat recorder stopped mid-test, and the gap was only recovered from the phone's buffer. The Memory Profiler was misleading on Vulkan: after a settings change, RenderTexture fell from 134.8 to 11.2 MB while total resident memory rose, because driver memory moved into "Untracked". I now use dumpsys meminfo PSS (367.5 MB on High, within the 600 MB budget) for totals.

4. Analysis: what the data says

Mino is not thermally limited on a modern phone: Balanced held a mean of 16.64 ms and p99 of 18.59 ms after 10 minutes, with Android Thermal Status 0. The real costs are memory on the High tier and design friction. Textures are only 5.9 MB thanks to ASTC, but render targets on High are 113.9 MB even after turning HDR off, because of 4x MSAA at 1080 × 2340. Start-up is fine: cold start median 118 ms and interactive in about 2.3 s, most of it the Unity splash (Google, 2026a). The pause bug also shows the limit of my own testing: I knew how the game was meant to be played, so I never tapped the control a stranger would reach for first. Peer testing on other phones found what eleven passing test cycles on mine did not.

5. Action for CA3 (each tied to evidence)
Render targets: MSAA 2x on URP-High; target RenderTexture at most 64 MB (measured 113.9 MB).
Resilience: verify KI-01 to KI-03 on device and mark them verified in KnownIssues.md; add HUD pause ×10 to the test plan; run battery saver unplugged over wireless adb (blocked while USB-charging); all six scenarios on the S24 and the A15 (Google, 2026b).
Players: reach at least 10 Pack 3 survey responses by 29 Nov (2 so far) and measure frame time on the low-tier A15.
GC: get from 99 B per frame back towards 0 B; Debug.Log spam is stripped from release builds, and the remaining 51 B comes from the Input System.
References

Anthropic (2026) Claude [AI assistant]. Used as a step-by-step lab guide and to draft this journal from my own logs, which I checked and edited (see README, AI assistance). Available at: https://claude.ai (Accessed: 15 November 2026).

Gibbs, G. (1988) Learning by Doing: A Guide to Teaching and Learning Methods. Oxford: Further Education Unit, Oxford Polytechnic.

Google (2026a) App startup time. Android Developers. Available at: https://developer.android.com/topic/performance/vitals/launch-time (Accessed: 4 November 2026).

Google (2026b) ANRs. Android Developers. Available at: https://developer.android.com/topic/performance/vitals/anr (Accessed: 4 November 2026).

Hunicke, R., LeBlanc, M. and Zubek, R. (2004) 'MDA: A formal approach to game design and game research', Proceedings of the AAAI Workshop on Challenges in Game AI. San Jose, CA: AAAI Press.

Unity Technologies (2026) Unity 6.6 Manual: Profiler. Available at: https://docs.unity3d.com/6000.6/Documentation/Manual/Profiler.html (Accessed: 3 November 2026).