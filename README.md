# Ministrike-BotMod

Ministrike 2.0 with bots!

## Unity Editor compatibility

**This checkout is not compatible with Unity Editor 2.5. Do not open or save this project with that editor expecting a working project.** The checked-in exported project identifies itself as Unity **5.5.3f1** (`decomp/ExportedProject/ProjectSettings/ProjectVersion.txt`). Unity 2.5 predates the project format and APIs used here; changing the version marker or editing serialized assets by hand would not make it compatible and could damage the project.

### Confirmed blockers

- Project settings and serialized assets were produced for Unity 5.5.3. Unity 2.5 cannot reliably read or preserve these newer serialized formats and asset types.
- Scripts use APIs and namespaces introduced after Unity 2.5, including `UnityEngine.SceneManagement`, `UnityEngine.Networking`, `RectTransform`, and the newer UI/Animator systems. For example, `GameLogic.cs` uses `SceneManager.LoadScene`, and `PlayerController.cs` uses `RectTransform` and `HedgehogTeam.EasyTouch`/ETC UI components.
- The project includes Unity 5-era networking and plug-in assemblies. These are not available in Unity 2.5 and would require substantial replacement, not a mechanical API rename.
- The repository contains a decompiled Unity project and an APK, not a clean, reproducible source project with a verified Unity 2.5 build. The checked-in code also does not by itself establish that the advertised bot functionality is implemented.

### Safe next steps

1. Preserve this checkout unchanged and make a separate working copy before attempting any migration.
2. If the goal is to develop the existing game, use the matching Unity **5.5.3f1** editor and verify the project and platform dependencies there. This is the version recorded by the project, not a guarantee that every dependency is available or builds today.
3. If Unity **2.5 is a hard requirement**, treat this as a port/reconstruction: start a separate Unity 2.5 project, recover or recreate compatible assets and scripts, replace the networking/UI/animation systems with versions available in 2.5, and test incrementally in that editor. Do not copy the newer `ProjectSettings` or assume the decompiled assets will import correctly.
4. Keep the original APK and exported project as reference artifacts; record asset provenance and ensure you have rights to use the game and third-party assets before redistribution.

No project version, settings, or scripts have been changed to claim Unity 2.5 support. Such a claim would be unsafe without a real Unity 2.5 editor build and validation, neither of which is available in this repository workflow.
