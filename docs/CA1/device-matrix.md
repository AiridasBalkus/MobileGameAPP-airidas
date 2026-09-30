Device matrix: v0.2.0 (versionCode 2)

| Device | Android | Serial (last 4) | Install result | Notes |
|--------|---------|-----------------|----------------|-------|
| OPPO Find X5 Pro (CPH2305), Snapdragon 8 Gen 1 / Adreno 730 | 15 | 9cfb | Success, versionCode 2, versionName 0.2.0 | Runs normally at 60 fps; safe area correct |
| Android Emulator "Medium Phone" (sdk_gphone16k_x86_64), 16 KB pages, ARM64 translated | 15 | 5554 | Success, versionCode 2, versionName 0.2.0 | Installs and starts (Displayed +351 ms), then aborts during Unity window init: berberis: Cannot process signal 11. The x86_64 emulator's ARM64 translator (libndk_translation) cannot handle a SIGSEGV raised in libunity.so. Emulator limitation with an ARM64-only APK; real ARM64 devices are unaffected.