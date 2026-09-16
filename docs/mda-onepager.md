# MDA One-Pager - Mino

**Module:** Mobile Game Development (A12581) · **Student:** Airidas Balkus · **Project option:** 2 (Endless Runner) · **Due:** Wed 16 Sep 2026 (Week 2 Lab B)

Keep this to **one page**. It is your scope contract for the semester and is submitted again with CA1.

## One-line pitch
A lane-based endless runner where swiping shifts you between three lanes to dodge obstacles and collect pickups, rewarding quick reflexes and clean timing. Underwater theme (preferable) if have time.

## Aesthetics (what the player feels)
- Flow and mastery: the run speeds up gradually, so basically success feels like the player's reactions genuinely improving
- Tension and relief: a near-miss dodge feels good; a clean multi-obstacle sequence feels great
- "Just one more run": short sessions and a visible high score pull the player back in

## Core mechanics (3 to 5 verbs or systems)
1. Lane change (swipe left/right to shift between 3 lanes)
2. Auto-forward movement (constant speed, gradually increasing over time)
3. Obstacle avoidance (static and moving obstacles placed per lane)
4. Pickup collection (coins/gems in-lane, add to score)
5. Score-on-distance (score increments with distance survived, plus 

## Dynamics (what emerges when the mechanics meet the player)
- Players start reading obstacle patterns ahead of time rather than reacting late, once speed increases enough to punish late swipes
- Risk/reward tension emerges around pickups placed in riskier lanes next to obstacles
- Players develop a personal "panic threshold" speed where they start making mistakes, creating a natural difficulty curve without explicit levels

## Progression & content
- **Session length:** 1 to 3 minutes per run (death ends the run, immediate restart)
- **Content in the vertical slice (by Week 6):** 1 lane track (3 lanes), 4 to 6 obstacle types, 1 pickup type, speed-ramp difficulty curve, score + high score display
- **Content by CA3:** additional obstacle variety, a second pickup type (e.g. shield/temporary invincibility), simple particle/juice polish on collect and collide, basic audio (music + SFX)

## Platform features (Android)
- **Touch model:** full-screen swipe detection (left/right) via Enhanced Touch, no on-screen buttons needed
- **Safe areas and orientation:** portrait only; HUD (score, high score) anchored inside Screen.safeArea; background/track extends full-bleed under any notch
- **Haptics:** short vibration pulse on collision (game-over feedback) and on pickup collection; none elsewhere to avoid overuse
- **Lifecycle:** pause/resume and focus loss handled from Week 2 (pause the run and mute audio on OnApplicationPause)
- **Store / testing tracks:** awareness only, no uploads

## Performance budget (your device)
- **Device:** OPPO Find X5 Pro (CPH2305), Adreno 730 / Snapdragon 8 Gen 1, Android 15
- **Target frame time:** 16.7 ms at 60 fps; High FPS toggle: no (60 fps is the fixed target via Application.targetFrameRate)
- **Memory ceiling:** under 600 MB
- **Cold start:** under 5s to interactive (menu/first playable frame).
- **APK size:** under 800 MB

## Monetisation (if any) & ethics notes
- No monetisation planned for this module's scope. If published later, a one-time purchase or optional cosmetic-only IAP would be the model considered.
- Explicitly will not use: loot boxes, pressure timers, non-stop ads.

## Risks & cuts list (in the order they get cut, 4 would get cut first)
1. Scoring/combo multiplier system
2. Second pickup type (shield/invincibility)
3. Audio/music and particle polish
4. Extra obstacle variety beyond the minimum 4 to 6 types

## Scope lock
- **Locked on:** Wed 16 Sep 2026
- **Changes after lock** require a note in the development journal explaining what changed and why.

Reference: 
- Hunicke, LeBlanc and Zubek (2004), *MDA: A Formal Approach to Game Design and Game Research*.
- Fink, D. (2025). Optimizing Cold, Warm, and Hot Starts: A Developer’s Guide to Faster App Launches. [online] Bugsee. Available at: https://bugsee.com/blog/cold-start-vs-warm-start/ [Accessed 16 Sept. 2026].

