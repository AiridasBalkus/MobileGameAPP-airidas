# Mino: Economy Telemetry Map

| Event | Parameters (type) | Fires when | Question it answers |
|-------|-------------------|------------|---------------------|
| `session_start` | `app_version` (string), `device_model` (string), `android` (string) | `Bootstrap.Awake`, once per launch | Which build and device produced this log |
| `level_start` | `level_id` (int), `attempt` (int), `coins` (int) | `GameManager.Start`, lane becomes playable | How many retries per session; how much wealth players bring in |
| `level_complete` | `level_id` (int), `time_s` (float), `coins_earned` (int), `score` (int) | Win (finish line at 1200 m) | Run length and earn rate on success |
| `level_fail` | `level_id` (int), `time_s` (float), `cause` (id), `distance` (int, m), `coins_earned` (int) | Lose (obstacle hit) | Where players die (distance) and earn rate on failure |
| `upgrade_purchased` | `upgrade_id` (id), `level` (int), `cost` (int), `coins_left` (int) | `Wallet.TryUpgrade` on success | When the first upgrade lands; spend pacing on the 100 * 1.5^level curve |
| `purchase_stub` | `product_id` (id) | `FakePurchaseProvider.PurchaseAsync` | How often the stubbed shop is used vs earned coins |

`time_s` uses scaled time, so pauses are excluded. `cause` is always `obstacle` (Mino has one fail state), so `distance` is used to locate failures. `coins_earned` counts pickups in that run only, not wallet change, so mid-run upgrades do not distort it.

Data is written to logcat and appended to `telemetry.log` in `Application.persistentDataPath` (`/storage/emulated/0/Android/data/com.airidasbalkus.mino/files/`). Nothing leaves the device; the same statement appears in `docs/CA1/privacy-statement.md`.
