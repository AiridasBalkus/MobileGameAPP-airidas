# Resume tests (Week 11 Lab A), 25 Nov 2026

Build: 0.9.1 Development Build with ResumeGuard pattern (branch w11-resume), Galaxy S24 Ultra, Android 17. Logcat: `adb logcat -s Unity`.
Checks per case: state intact, audio and timers resume once, no exception in Logcat.
Pattern check first: ResumeGuard sample, Home at ~3 s, back at ~10 s, Resume: one `Boss spawned` (15:56:28.914); second Home/return: none. Pass.

| # | Case | Expected | Observed | Pass/Fail | Logcat line if failed |
|---|------|----------|----------|-----------|-----------------------|
| 1 | Home, reopen from Recents after 30 s | Pause panel shown, coins/hint unchanged, no new obstacle wave; Resume continues once | coins 412 before and after, 30 s away | Pass | - |
| 2 | Interruption by a call remote call | Pauses and goes silent; after return, Resume continues once; if it happened right after a crash, Result loads once | Phone call from roommate | Pass | - |
| 3 | Notification shade down and up | `[Pause] True` (focus loss pauses, same as Home, by design); hint and timers stop and continue from where they were after Resume | `[Pause] True` (focus loss pauses, same as Home, by design); hint and timers stop and continue from where they were after Resume | Pass | - |
| 4 | Screen lock 1 min, unlock | One `[Pause] True`; coins kept; one spawn loop after Resume (no doubled obstacles) | One `[Pause] True`; coins kept; one spawn loop after Resume (no doubled obstacles) | Pass | - |
| 5 | Rotation | Portrait lock holds; no restart, no UI drift | auto-rotate on, phone turned to landscape, stayed portrait | Pass | - |
| 6 | Process death: coins, Home, `adb shell am kill com.airidasbalkus.mino`, relaunch | Wallet loads the coins held before Home; lands on Menu |  coins stayed the same 412 before and after | Pass | - |

**Decision on focus loss:** the notification shade (OnApplicationFocus(false)) pauses the run, the same as Home and calls, so every interruption behaves the same and no timer keeps running behind the shade.