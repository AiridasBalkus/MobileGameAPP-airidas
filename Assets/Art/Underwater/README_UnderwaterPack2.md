# Mino underwater pack 2 (scenery, icons, FX, extra audio, font)

Unzip over pack 1. It replaces `Editor/MinoUnderwaterSetup.cs` with v2, which adds scenery and keeps everything from v1.

| File | Use | Android import (automatic) |
|------|-----|----------------------------|
| Models/Rock_A/B/C.obj (80 tris each) | Low-poly rocks beside the track | No materials imported, no colliders |
| Models/Kelp_Cross.obj, CoralFan_Cross.obj (4 tris) | Crossed-quad plants | as above |
| Textures/T_Rock.png (512) | Rock material | ASTC 6x6, mipmaps + streaming |
| Textures/S_Kelp.png (512x1024), S_CoralFan.png (512) | Alpha-cutout plants | ASTC 6x6, clamp, mipmaps + streaming |
| Resources/MinoUW/UI/Icon_Pause, Play, Settings, SoundOn, SoundOff, Vibrate, Upgrade | HUD and settings icons (white, tintable) | Sprite, ASTC 4x4, max 128 |
| Resources/MinoUW/FX/FX_Sparkle, FX_SandPuff, FX_Bubble | Particle textures: coin pickup, obstacle hit, ambient bubbles | ASTC 4x4, max 256, no mipmaps |
| Resources/MinoUW/Audio/SFX_Upgrade (0.8 s), SFX_RunStart (0.7 s) | Upgrade bought, run begins | Decompress On Load, ADPCM, 22050 Hz mono |
| Resources/MinoUW/Audio/AMB_Underwater_Loop (8 s, seamless) | Quiet ambience under the music | Compressed In Memory, Vorbis 70 |
| Fonts/Poppins-Bold.ttf | UI font (SIL OFL 1.1, see FONT_LICENSE.txt) | Create a TMP font asset from it |

## Setup
1. Unzip into the project root and let Unity import.
2. **Mino > Underwater > 1. Create Materials** again (adds M_Rock, M_Kelp, M_CoralFan, M_Player).
3. Open the **Game** scene, then **Mino > Underwater > 3. Spawn Scenery + Player Material**. Places about 400 rocks, kelp and coral fans along both sides of the 1200 m track (static batched, no shadows, hidden by fog beyond 95 m) and turns the player capsule into a glossy orange "diver". If they float or sink, move the `UW_Scenery` object on Y. **Ctrl+S**.
4. Font: **Window > TextMeshPro > Font Asset Creator**, Source Font `Poppins-Bold`, Atlas 512x512, Character Set ASCII, **Generate Font Atlas**, **Save** as `Poppins-Bold SDF` in `Assets/Art/Underwater/Fonts/`.
