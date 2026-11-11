# Mino underwater art pack (Week 9 Lab A art pass)

All assets were generated for Mino (procedural textures, synthesised audio; the logo font is Poppins Bold, SIL Open Font License). No third-party assets.

## Contents
| File | Use | Android import (set automatically by Editor/MinoUnderwaterSetup.cs) |
|------|-----|---------------------------------------------------------------------|
| Textures/T_Track_Sand.png (1024) | Track surface, rippled sand with baked caustics, tileable | ASTC 6x6, max 1024, mipmaps + streaming |
| Textures/T_Seabed.png (1024) | Ground outside the track, tileable | ASTC 6x6, max 1024, mipmaps + streaming |
| Textures/T_Obstacle_Coral.png (512) | Obstacles: coral red (keeps red = hazard) | ASTC 6x6, max 512, mipmaps + streaming |
| Textures/T_Coin_Gold.png (512) | Coins: gold (keeps yellow = pickup) | ASTC 6x6, max 512, mipmaps + streaming |
| Textures/T_Sky_Underwater_Panorama.png (2048x1024) | Skybox: deep water gradient with light shafts (the lane's hero texture) | ASTC 6x6, max 2048, no mipmaps |
| Resources/MinoUW/UI/UI_Logo_Mino.png | Menu title | Sprite, ASTC 4x4, max 1024, no mipmaps |
| Resources/MinoUW/UI/UI_Button_Primary.png / _Secondary.png | Buttons (9-slice, border 48) | Sprite, ASTC 4x4, max 512 |
| Resources/MinoUW/UI/UI_Panel.png | Pause / About / Result panels (9-slice, border 60) | Sprite, ASTC 4x4, max 512 |
| Resources/MinoUW/UI/UI_Icon_Coin.png | Coin icon next to the coin counter | Sprite, ASTC 4x4, max 512 |
| Resources/MinoUW/UI/UI_AppIcon_512.png | Player Settings > Icon, and the store-asset checklist | Sprite, ASTC 4x4, max 1024 |
| Resources/MinoUW/UI/T_Bubble.png (128) | Ambient bubble particles | Default, alpha, ASTC 4x4, no mipmaps |
| Resources/MinoUW/Audio/SFX_Swipe, SFX_Coin, SFX_Hit, SFX_Click (under 1 s) | Lane change, pickup, obstacle hit, UI | Decompress On Load, ADPCM, mono, 22050 Hz |
| Resources/MinoUW/Audio/SFX_NewBest (1.4 s) | New high score | Compressed In Memory, Vorbis 70, mono, 22050 Hz |
| Resources/MinoUW/Audio/MUS_Underwater_Loop (19.2 s, seamless) | Music | Streaming, Vorbis 70, mono, load in background |

UI sprites and audio live in a Resources folder so scripts can load them by path (`Resources.Load<AudioClip>("MinoUW/Audio/SFX_Coin")`) without Inspector wiring.

## Setup (about 10 minutes)
1. Unzip so this folder sits at `Assets/Art/Underwater/` (the zip already contains that path). Unity imports everything with the Android overrides above.
2. **Mino > Underwater > 1. Create Materials**. Creates the materials in `Materials/` and puts coral on every prefab with an `Obstacle` component and gold on every `Pickup` prefab.
3. Open the **Game** scene, then **Mino > Underwater > 2. Apply Lighting To Open Scene**: skybox, teal linear fog (25 to 95 m, hides the far end of the track), trilight ambient, cyan-tinted sun. **Ctrl+S**. Repeat for Menu and Result if they show the 3D scene.
4. Drag **M_Track_Sand** onto the track objects and **M_Seabed** onto the ground. If the sand looks stretched, change the material's Tiling Y (the track is long in Z: roughly track length / track width).
5. **Player Settings > Icon**: set UI_AppIcon_512 as the default icon.
6. The lane separators can use M_Seabed (darker) so the three lanes stay readable.

Hazard red, pickup yellow and floor contrast are kept from the greybox so the lanes still read at speed.
