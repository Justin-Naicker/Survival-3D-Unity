## 3D-Survival — Copilot instructions (concise)

This repository is a Unity project (URP template) containing gameplay scripts under `Assets/`.
These notes are intentionally focused and actionable so an AI coding agent can be productive immediately.

### Big picture
- Unity project using Universal Render Pipeline (see `Packages/manifest.json` — `com.unity.render-pipelines.universal`).
- Gameplay code lives under `Assets/Scripts` (eg. `Movement.cs`). Scenes, settings and assets are in `Assets/Scenes`, `ProjectSettings/` and `Assets/`.
- Typical runtime pattern: MonoBehaviours with serialized private fields, inspector wiring for references (Camera, CharacterController), and use of Unity lifecycle methods: `Start`, `Update`, `OnDisable`, `OnDrawGizmosSelected`.

### Major components & examples
- Player movement: `Assets/Scripts/Movement.cs` is the canonical implementation. Key patterns:
  - Uses `[RequireComponent(typeof(CharacterController))]` and a cached `CharacterController` via `GetComponent`.
  - Input is currently read with built-in Input API (`Input.GetAxis`, `Input.GetButtonDown`) even though `Input System` package is present (`Assets/InputSystem_Actions.inputactions`). Be careful: there is a mix of input approaches.
  - Ground check uses a short downward `Physics.Raycast` from `transform.position + Vector3.up * 0.03f` with `groundCheckDistance` and visualized in `OnDrawGizmosSelected`.
  - Jump velocity is calculated with `Mathf.Sqrt(jumpForce * -2f * gravity)` and vertical velocity integrated each frame.

### Project-specific conventions
- Favor private fields with `[SerializeField]` and grouped `Header("...")` attributes (see `Movement.cs`).
- Use `Cursor.lockState`/`Cursor.visible` in `Start` and `OnDisable` to manage cursor during Play mode.
- Use `CharacterController.Move(...)` with movement and gravity applied separately and multiplied by `Time.deltaTime`.
- Add editor-only debugging visuals inside `#if UNITY_EDITOR` + `OnDrawGizmosSelected`.

### Integration points & dependencies
- Packages: `Packages/manifest.json` includes `com.unity.inputsystem`, `com.unity.ai.navigation`, `com.unity.multiplayer.center`, `com.unity.test-framework`, etc. Expect assets or code to assume these packages exist.
- Scenes and prefabs in `Assets/Scenes` may reference serialized fields in scripts; prefer inspector wiring over searching by name at runtime.

### Common pitfalls for edits
- Watch for mixed input approaches: do not unilaterally switch to the new Input System without updating `InputSystem_Actions.inputactions` bindings and wiring player input components.
- Inspect `Assets/PlayerController.cs` before editing — it currently appears to be a stub and contains a syntax issue (malformed attribute line). Fixes here may be required before compilation.
- Preserve serialized field names and types when refactoring — changing a serialized field name will break existing inspector assignments.

### Developer workflows (what an engineer/agent will do)
- Play / iterate inside Unity Editor (recommended): open the project in Unity Hub or via `Unity.exe` and press Play.
- Edit C# in Visual Studio / Rider using the generated `.sln`/`.csproj` files in the repo root.
- Command-line builds (replace Unity path and target as needed):
```powershell
# example: run from PowerShell, replace <UnityEditorPath> and <projectPath>
& "C:\Program Files\Unity\Hub\Editor\<VERSION>\Editor\Unity.exe" -projectPath "C:\path\to\3D-Survival" -quit -batchmode -buildWindows64Player "C:\builds\3d-survival.exe"
```
- Tests: `com.unity.test-framework` is included, but there are no test folders present. If you add tests, follow Unity Test Framework conventions and place them under `Assets/Tests`.

### How to change gameplay safely
- When adjusting movement values (speed, gravity, jump), prefer editing `SerializeField` defaults in `Movement.cs` or tuning via the Inspector on prefab/scene instances.
- Keep physics-related changes consistent: `IsGrounded()` uses a raycast offset — if you change character pivot/origin, adjust `groundCheckDistance` and the offset vector.

### What to inspect for PRs
- Run the project in Editor Play mode after any code change to catch compile-time and runtime Unity errors.
- Check `Console` for serialization warnings when renaming serialized fields.
- If you modify input handling, search for `Input.GetAxis`, `Input.GetButtonDown`, and `InputSystem_Actions.inputactions` to update all usages.

### Files and locations to reference quickly
- `Assets/Scripts/Movement.cs` — canonical movement & look implementation
- `Assets/PlayerController.cs` — currently a stub; review before edits
- `Assets/InputSystem_Actions.inputactions` — input action asset (may be unused)
- `Packages/manifest.json` — package dependencies
- `ProjectSettings/` — project-wide settings that affect builds and editor behavior

If anything above is unclear or you want this tuned to prefer a specific workflow (Rider vs Visual Studio, exclusive Input System migration, or CI build commands), tell me which and I will update this file.
