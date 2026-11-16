# Mino: store asset checklist (Week 10 Lab A)

Specs checked against the Play *Add preview assets* help page on 16 Nov 2026: https://support.google.com/googleplay/android-developer/answer/9866151

| File | Required spec | Actual | OK |
|------|---------------|--------|----|
| icon-512.png | 512 x 512, 32-bit PNG, under 1024 KB, subject in the central 80%, no rounded corners | 512 x 512, RGBA PNG, 69 KB; orange diver with goggles and snorkel on a deep-water gradient, no text | [x] |
| feature-1024x500.png | 1024 x 500, JPEG or 24-bit PNG, no alpha, focal point towards the centre, no device frame | 1024 x 500, RGB PNG (no alpha), 136 KB; title centred top, hero, coins and coral in the middle third, only kelp and water at the edges | [x] |
| screenshots/shot01 to shot06 | 9:16 portrait, each side 1080 to 3840 px, long side at most 2x the short, JPEG or 24-bit PNG, no HUD, no "Development Build" watermark | 6 shots captured on the S24 Ultra (release 0.9.1, so no watermark), cropped from 1080 x 2340 to 1080 x 1920 PNG. **HUD still visible** (coin counter, upgrade button, score): re-capture shots 1 to 3 with StoreShot for the final set | [~] |
| Screenshot count | Up to 8 per device type, at least 2; games need at least three 9:16 shots of 1080 x 1920 or more for promotion | 6 at 1080 x 1920 | [x] |

- **Icon silhouette:** a single orange capsule diver (the in-game player) with round goggles and a snorkel. One focal colour (orange) against teal. Reads at 48 px.
- **Feature graphic:** extends the icon's palette into a full reef scene rather than repeating the icon.
- **Capture note:** the S24 Ultra screen is 1080 x 2340, which breaks Play's 2:1 limit (2340 / 1080 = 2.17). Each shot is cropped to 1080 x 1920 (210 px off the top and bottom), which also removes the thermal overlay and the bottom score.
