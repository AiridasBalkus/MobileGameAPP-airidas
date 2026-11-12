# Mino: CA2 checklist

Ticked only where the repo already holds the evidence.

- [x] **Release-signed APK installs without ADB.** 0.8.0 (08279012), release-signed with mygame-release (docs/CA3/qa/builds.md); two peers installed it by QR link, "Worked first time" (docs/CA3/qa/survey/responses-0.8.0.csv).
- [x] **Pause and resume x10.** 11 Home + resume cycles on 0.8.0, each `[Pause] True, timeScale 0` then `[Pause] False, timeScale 1` (docs/CA3/qa/test-plan.md, scenario 1). Note: this covered app backgrounding only; the HUD pause button was broken (KI-03) and needs its own x10 re-test on 0.9.0.
- [x] **Baseline Sheet complete.** Rows 1-5 filled (docs/CA2/baseline/baseline-sheet.md); the Week 9 memory budget table is still blank.
- [x] **Telemetry events and event map.** docs/CA2/Economy_Telemetry_Map.md (6 events, `tier` added to session_start in Week 9) and a pulled log in docs/CA2/telemetry/telemetry.log.
- [x] **README.** README.md exists (device, signing, build steps, AI assistance) but the build steps still say 0.2.0
- [ ] **Reflective journal, 1,000 to 1,500 words.** docs/dev-journal.md is a weekly log, not the reflective journal.
- [ ] **3 to 5 minute gameplay capture.** No capture in the repo yet.
