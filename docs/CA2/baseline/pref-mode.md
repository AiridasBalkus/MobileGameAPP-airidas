# Mino: performance modes (Samsung XCover Pro SM-G715FN, Android 13, release build v0.7.0)

| Mode | targetFrameRate | Render scale | Frame time avg (ms) | 99th (ms) | Bottleneck | Panel Hz |
|------|-----------------|--------------|---------------------|-----------|------------|----------|
| Balanced (default) | 60 | 1.0 | 27.0 | 50.0 | GPU | 60 |
| High FPS | 120 | 0.8 | 23.1 | 33.4 | GPU | 60 |

- Method: FrameTimeSampler (600-frame windows, ~10 s each) in logcat, release build, phone unplugged
  (wireless adb), gameplay only; first window (scene load) and the window spanning the toggle excluded.
- The panel is 60 Hz, so High FPS cannot exceed 60 fps here; the gain comes from render scale 0.8.
  It cuts avg frame time ~15% and p99 from 50 ms (3 vsync intervals) to 33.4 ms (2).
- Bottleneck stays GPU in both modes (`[AP] bottleneck=GPU`), consistent with a weaker Mali GPU vs the
  OPPO, which was CPU-bound in Week 3.
- Phone was already warm (AP temperature level 0.80–0.87, WarningLevel NoWarning). Week 5 measured
  16.7 ms on the same phone, so this may be thermal; to be checked from a cool start in the Lab B soak.