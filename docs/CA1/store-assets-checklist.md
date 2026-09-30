# Mino: Store-asset checklist
| Asset | Spec | Status | Notes |
|-------|------|--------|-------|
| App icon | 512x512, 32-bit PNG with alpha, max 1024 KB | planned | Full square artwork, no rounded corners or shadow (Play applies the mask); no text suggesting rank or price |
| Feature graphic | 1024x500, JPEG or 24-bit PNG, no alpha | planned | Key elements centred, away from the cut-off edges; vibrant colours (not pure white, black or dark grey); no "Best", "Free", "New"; add alt text |
| Phone screenshots | 2 to 8; JPEG or 24-bit PNG, no alpha; each side 320 to 3840 px; longest side at most 2x the shortest | planned | Portrait 9:16 at 1080x1920 minimum so the game is eligible for large-format recommendations (needs at least three gameplay shots) |
| Preview video | YouTube URL, public or unlisted, ads off | optional, not planned | Would need the feature graphic as its cover |

**Aspect-ratio note:** the OPPO Find X5 Pro screen is about 20:9, over Play's 2:1 limit, so raw screencaps cannot be uploaded as-is. Each shot is cropped to 9:16 (1080x1920, or 1440x2560 at native width) before export.

## Screenshot plan (portrait 9:16)
| No of shot (#) | Working caption | What must be visible |
|---|-----------------|----------------------|
| 1 | Swipe between three lanes | Mid-run, player changing lanes, obstacles ahead in all three lanes; no UI overlapping the camera cutout |
| 2 | Read the road, react fast | Denser obstacle pattern, player threading a gap |
| 3 | Every hit counts | Moment of impact with an obstacle (haptic pulse event) |
| 4 | Pause any time | Pause panel over the frozen run: Resume, haptics toggle, text-size buttons |
| 5 | Play your way | Large text size applied, haptics toggle visible |
| 6 | (CA2 slice) | Final CA2 core-loop screen, e.g. run end or score: added only if it exists in the slice - this is subject to change |

## Asset sources (Week 10)
- **Screenshots:** captured in-game on the OPPO with `adb shell screencap` from the release build, then cropped to 9:16.
- **App icon:** redrawn from the MDA key art at 512x512 and exported as 32-bit PNG.
- **Feature graphic:** built from the same key art and a gameplay capture at 1024x500, exported as 24-bit PNG with no alpha.