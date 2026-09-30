### This the README for my mobile game MINO
## Device Info
Device Model OPPO Find X5 Pro
Android Version: Android OS 15
Graphics API: VULKAN
[Boot] OPPO CPH2305 | Android OS 15 / API-35 (AP3A.240617.008/S.205cac3-124d3-124d5) | Vulkan | 2412x1080 @ 480 dpi

## Key INFO
key name: mygame-release   Location: C:\\Users\\jeffe\\Documents\\keystore, alias name: mygame-release, validity period: 50 years

## Difference between the two build profiles
Android Dev
Development Build: ON
Autoconnect Profiler: ON
Used for everyday iteration this is what you use when you're testing code changes, watching Logcat, or capturing Profiler sessions It includes debugging hooks and a connection back to the Unity Editor, which makes it slower and larger, but lets the Profiler and Logcat actually see what's happening inside the app.

Android Release
Development Build: OFF
Signed with your custom release keystore (not the debug key)
Used to produce the real, distributable build what you'd actually sideload as a finished APK (Part B). No debug overhead, no Profiler connection, smaller and faster, and it's signed the way a real published app would be.

## Signing

Release builds are signed with a private release keystore that is **not** stored in this repository 
| Field | Value |
|-------|-------|
| Keystore location | Outside the repo: %USERPROFILE%\Documents\keystore\mygame-release.keystore|
| Backups | Password manager attachment + 2FA-protected cloud folder |
| Alias | mygame |
| Key | 2048-bit RSA |
| Valid from / until | 15 Sep 2026 / 02 Sep 2076 |
| SHA-256 fingerprint | 60:DB:B7:22:B7:FA:F3:F4:36:36:1D:56:BE:C3:85:7E:5D:94:73:08:4A:95:0B:3D:67:76:62:65:74:DF:5B:1B |

## Build steps (clone to phone)
1. git clone <https://github.com/AiridasBalkus/MobileGameAPP-airidas> and open the folder in **Unity 6.6 (6000.6.0f1)** with Android Build Support installed.
2. **File > Build Profiles**: select **Android Release** (switch platform to Android if asked).
3. Check **Edit > Project Settings > Player > Android > Other Settings**:
   - Package Name com.airidasbalkus.mino
   - Version 0.2.0, Bundle Version Code 2
   - Scripting Backend **IL2CPP**, Target Architectures **ARM64**
   - Minimum API Level 26, Target API Level Automatic (highest installed)
4. **Publishing Settings**: select the release keystore (see Signing) and enter both passwords.
5. In the build profile, Development Build **off**, Build App Bundle **off**.
6. **Build** to releases/Mygame-0.2.0-arm64.apk.
7. With the phone connected (USB debugging on):
   `adb install -r releases/Mygame-0.2.0-arm64.apk`
   `adb shell dumpsys package com.airidasbalkus.mino | findstr version` → expect `versionCode=2`, `versionName=0.2.0`.

Earlier development builds (versionCodes 1–4) were sideloaded for testing only; v0.2.0 / versionCode 2 is the first tagged release. If a phone still has one of those installed, run `adb uninstall com.airidasbalkus.mino` first.

## Device targets
- Minimum API: 26 (Android 8.0); target API: 36 (Automatic)
- Current Play target API policy: https://support.google.com/googleplay/android-developer/answer/11926878
- ABI: arm64-v8a only
- Tested: see docs/CA1/device-matrix.md (OPPO Find X5 Pro, Android 15: runs; x86_64 emulator: installs, does not run under ARM64 translation)

## AI assistance
Claude (Anthropic) was used as a step-by-step guide through the lab sheets: explaining settings and error logs, which I reviewed and edited. All builds, installs and measurements were run by me on my own device.