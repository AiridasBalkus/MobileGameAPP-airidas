# Final-sprint targets (set Wed 11 Nov 2026)

## Thermals
Balanced mode holds <= 19.5 ms at the 99th percentile at minute 10 of a 10-minute soak on the Galaxy S24 Ultra (SM-S928U1), on the art-passed 0.9.x build at tier High.
Baseline: 18.59 ms at minute 10 before the art pass (0.4.0, Week 7); 19.00 ms after the art pass from a cool start (0.9.0, Week 9).
Method: if `ThrottlingImminent` appears, drop render scale by 0.1 per warning (down to 0.75), or enable the Adaptive Resolution scaler; repeat the soak on the low-tier Galaxy A15 and record its p99.

## Resilience
All six test-plan scenarios pass on my phone (S24 Ultra, Android 17) and on a peer phone (Galaxy A15, Android 14), including battery saver run unplugged over wireless adb and the HUD pause button x10.
Crash-free sessions: 20 of 20, counted from `session_start` against a new `session_end` event (fired on quit or pause) by Sun 29 Nov.

## Store assets (awareness only)
Icon 512x512, feature graphic 1024x500, 5 screenshots at 1080x1920, short and long description,
privacy statement matching runtime data use, reviewed in the Week 10 Lab B critique.
