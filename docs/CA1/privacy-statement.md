# Mino: Data-Use and Privacy Statement (v0.4.0, versionCode 4)
Mino collects no personal data. It makes no network requests, contains no ads, analytics or third-party SDKs, and stores only game progress, settings and a local gameplay log on the device.

## Data collected
None. The app does not collect, transmit or share any user or device data.

## Data stored on the device
Game progress and player settings, stored with Unity `PlayerPrefs` (Android SharedPreferences, private to the app):
| Key | Type | Purpose |
|-----|------|---------|
| haptics | int (0/1) | Vibration on or off |
| textScale | float | UI text size (0.85 / 1.0 / 1.25) |
| highscore | int | Best score, shown on the Result screen |
| wallet.coins | int | Soft-currency balance earned in play |
| wallet.level | int | Upgrade level bought with coins |
| perf_mode | int (0/1) | Balanced or High FPS setting |

None of these values identify the player or the device.

**Local telemetry log.** Gameplay events (session start, run start/end, upgrades, stub purchases) are appended to `telemetry.log` in `Application.persistentDataPath` (`/storage/emulated/0/Android/data/com.airidasbalkus.mino/files/`). Each line holds a timestamp, a random 8-character session id generated at launch (not linked to the player or device identity), the app version, the device model and Android version, and game numbers (distance, time, coins, score). The file is never transmitted; it is read only by the developer over USB (`adb`) for balancing. It grows over time and is removed by uninstalling or clearing storage. See `docs/CA2/Economy_Telemetry_Map.md` for the full event list.

**To remove all stored data:** uninstall the app, or use Settings > Apps > Mino > Storage > Clear data.

## Network activity
None. No code uses UnityWebRequest, sockets or any online service (verified by searching the source for network APIs).

## Third-party SDKs
None. No ads, analytics, crash reporting, in-app purchase or social SDKs are included (verified in Package Manager).

## In-app purchases
None. The "Buy 500 coins" button on the Result screen is a stub (`FakePurchaseProvider`) behind the `IPurchaseProvider` interface. It does not contact any store, take payment or collect any data; it only adds 500 coins locally and writes an `[iap-stub]` log line and a `purchase_stub` telemetry event.

## Permissions requested
From adb shell dumpsys package com.airidasbalkus.mino:
| Permission | Justification |
|------------|---------------|
| `android.permission.VIBRATE` | One haptic pulse when the player hits an obstacle; can be switched off in the pause menu. |
| `android.permission.INTERNET` | Declared by Unity's *Internet Access = Require* build setting; no code in this version makes network requests. Will be set to *Auto* in the next release so the permission is dropped. |
| `com.airidasbalkus.mino.DYNAMIC_RECEIVER_NOT_EXPORTED_PERMISSION` | Added automatically by AndroidX; private to this app and grants no access to user data. |

## How this would be declared on Google Play
- **Data collection:** "No data collected". Play counts data as *collected* only when it is transmitted off the device; progress, settings and the telemetry log kept locally are not collected.
- **Data sharing:** "No data shared with third parties".
- **In-app purchases:** none; the coin pack is a non-functional stub, so the store listing would not carry the "In-app purchases" label.
- **Security practices:** no data is transmitted, so encryption in transit does not apply. Users can delete all local data by uninstalling or clearing storage.
- **Privacy policy URL:** required before release on the closed, open or production tracks; this statement would be published at a public URL for that purpose.
- **Internal testing track:** apps available only on the internal testing track are exempt from the Data safety section.