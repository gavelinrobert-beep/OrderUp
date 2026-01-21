# Crash Root-Cause Analysis Report

## Scope
- Repo: `/home/runner/work/OrderUp/OrderUp`
- Audit focus: project settings, core gameplay scripts, network/bootstrap systems, asset instantiation, scene loading, and runtime lifecycle patterns.
- No tests/builds run (per request).

## Project settings (crash-relevant audit)
- Player settings:
  - `ProjectSettings/ProjectSettings.asset`:
    - `actionOnDotNetUnhandledException: 1` (Unity will abort on unhandled exceptions; crashes likely if null refs escape).
    - `enableCrashReportAPI: 0` (built-in crash report disabled, reduces diagnostics in player).
    - `usePlayerLog: 1` (player log enabled; use for crash traces).
    - `gcIncremental: 1` (incremental GC enabled; lower GC spikes, but still watch allocations).
    - `scriptingRuntimeVersion: 1` (modern runtime; default).
- Physics:
  - `ProjectSettings/DynamicsManager.asset`:
    - `m_AutoSimulation: 1` (physics auto-simulated; no custom step issues).
    - `m_AutoSyncTransforms: 0` (manual transform sync; can cause missed collider updates if scripts move transforms directly without physics sync).
- Audio:
  - `ProjectSettings/AudioManager.asset`:
    - `m_DSPBufferSize: 1024` and `m_RealVoiceCount: 32` (large buffers can hide XR stutters; not crash-prone by itself).
- Quality/graphics:
  - `ProjectSettings/QualitySettings.asset` and `ProjectSettings/GraphicsSettings.asset` show default built-in render pipeline; no custom pipeline.

## Code audit (high-level)
- Core singleton managers with `DontDestroyOnLoad`:
  - `Assets/Scripts/Core/GameManager.cs`
  - `Assets/Scripts/Core/OrderManager.cs`
  - `Assets/Scripts/Core/ScoreManager.cs`
  - `Assets/Scripts/Core/GameStateManager.cs`
  - `Assets/Scripts/Core/PerformanceOptimizer.cs`
  - `Assets/Scripts/Audio/AudioManager.cs`
  - `Assets/Scripts/Gameplay/VisualFeedbackManager.cs`
- Scene loading:
  - `Assets/Scripts/Core/MainMenuManager.cs` (loads `Main` scene).
  - `Assets/Scripts/Core/GameUIManager.cs` (loads `MainMenu`).
- Runtime instantiation and cleanup:
  - `WarehouseBuilder`, `ShelfManager`, `WarehouseEquipmentManager`, `PerformanceOptimizer`, `GameUIManager`, `VisualFeedbackManager`, and `PackingStation` create/destroy objects at runtime.
- Networking:
  - `OrderUpNetworkManager` manages Mirror sessions; `NetworkUI` uses IMGUI.

## Crash-risk patterns found (with evidence)
1. **Singleton duplicates across scenes + `DontDestroyOnLoad`**
   - Evidence: multiple managers use singleton pattern and `DontDestroyOnLoad` (`GameManager`, `OrderManager`, `ScoreManager`, `GameStateManager`, `PerformanceOptimizer`, `AudioManager`, `VisualFeedbackManager`).
   - Risk: if scenes include prefabs with these managers, scene transitions can spawn duplicates; duplicates are destroyed in `Awake`, but timing + cross-scene dependencies can cause nulls if destroyed before other components subscribe.
   - Affected files: `Assets/Scripts/Core/*.cs`, `Assets/Scripts/Audio/AudioManager.cs`, `Assets/Scripts/Gameplay/VisualFeedbackManager.cs`.

2. **Runtime object creation without hard null guards on required references**
   - Evidence:
     - `PackingStation.CreateNewBox()` builds a `Box` if `boxPosition` not assigned; if `boxPosition` is null it uses station transform (safe), but other systems assume assigned order/box.
     - `GameUIManager` creates runtime UI objects if `orderItemPrefab` missing; relies on `OrderItemUI.Initialize` to be safe.
     - `WarehouseEquipmentManager` and `ShelfManager` instantiate runtime primitives if prefabs missing.
   - Risk: if prefabs are missing essential components or ScriptableObjects (e.g., `ProductData.prefab`), gameplay can degrade to null refs, especially in `PickableItem` interactions.
   - Affected files: `Assets/Scripts/Environment/WarehouseEquipmentManager.cs`, `Assets/Scripts/Environment/ShelfManager.cs`, `Assets/Scripts/Gameplay/PackingStation.cs`, `Assets/Scripts/Core/GameUIManager.cs`.

3. **Order lifecycle edge cases (expired order cleanup & lookup)**
   - Evidence: `OrderManager.UpdateOrderExpirations` expires orders with missing spawn times and removes them; `CompleteOrder` uses instance ID lookup.
   - Risk: race between expiration and completion can remove active orders before a packer completes; in UI, `GameUIManager` may attempt to remove UI for an order already removed (safe, but can lead to null item destruction order).
   - Affected files: `Assets/Scripts/Core/OrderManager.cs`, `Assets/Scripts/Core/GameUIManager.cs`, `Assets/Scripts/Gameplay/PackingStation.cs`.

4. **Physics usage with `AutoSyncTransforms` disabled**
   - Evidence: `ProjectSettings/DynamicsManager.asset` has `m_AutoSyncTransforms: 0`.
   - Risk: scripts that move transforms directly (e.g., procedural environment objects) might not sync collision until next physics step; if other code queries physics immediately after move, could see stale collider state. Not a direct crash, but can produce invalid interactions that lead to null refs in follow-on logic.
   - Affected files: `Assets/Scripts/Environment/WarehouseBuilder.cs`, `Assets/Scripts/Environment/WarehouseEquipmentManager.cs`, `Assets/Scripts/Environment/ShelfManager.cs`.

5. **Mirror network manager dependency in IMGUI**
   - Evidence: `NetworkUI` logs error if `NetworkManager.singleton` is missing.
   - Risk: if `NetworkUI` exists in a scene without a `NetworkManager`, UI interactions may throw nulls if used outside Start check (currently guarded). Low crash risk.
   - Affected files: `Assets/Scripts/Networking/NetworkUI.cs`.

## Assets/import pipeline considerations
- Runtime primitive creation uses default materials; no custom shader references (reduces shader import crash risk).
- ScriptableObjects for orders/products (`Assets/ScriptableObjects`) are expected by `OrderManager` and `ShelfManager`. Missing or partially configured assets can cause nulls in gameplay (order/product data fields used without deep validation).
- Mirror package (`Packages/manifest.json`) is present; ensure matching transport settings in scenes to avoid runtime network errors.

## Logs / diagnostics / repro workflow
- Player logs enabled (`usePlayerLog: 1`):
  - Windows: `%USERPROFILE%\AppData\LocalLow\OrderUpTeam\Order Up\Player.log`
  - macOS: `~/Library/Logs/Unity/Player.log`
  - Linux: `~/.config/unity3d/OrderUpTeam/Order Up/Player.log`
- Suggested repro steps for crash investigation:
  1. Launch `MainMenu` scene, start game, then return to main menu (scene load/unload sequence).
  2. Repeat multiple scene transitions to verify singleton lifecycle and duplicate destruction.
  3. Stress-test order spawning: leave orders to expire while packing another order.
  4. Spawn shelves/carts without prefabs to validate runtime generation path.
  5. Multiplayer host/client connect/disconnect cycles to observe network object lifecycle.
- Capture stack traces in Player.log; correlate with `actionOnDotNetUnhandledException: 1` (crash on unhandled exceptions).

## Prioritized suspected causes (evidence, affected files, risk, fix)
1. **Singleton duplication across scenes with `DontDestroyOnLoad`**
   - Evidence: multiple managers use `DontDestroyOnLoad` and singleton `Awake` destruction pattern.
   - Affected files: `Assets/Scripts/Core/GameManager.cs`, `OrderManager.cs`, `ScoreManager.cs`, `GameStateManager.cs`, `PerformanceOptimizer.cs`, `Assets/Scripts/Audio/AudioManager.cs`, `Assets/Scripts/Gameplay/VisualFeedbackManager.cs`.
   - Risk: **High** if scenes contain multiple manager prefabs or additive loads; can cause null refs in Start/OnEnable order.
   - Fix: ensure only one instance exists via bootstrap scene or `RuntimeInitializeOnLoadMethod`, or mark manager prefabs in one scene only; add defensive null checks before subscribing.

2. **Order lifecycle race (expiration vs completion)**
   - Evidence: `OrderManager.UpdateOrderExpirations` can expire orders while UI or packer logic interacts; `GameUIManager` removes UI items from lists and destroys objects; `PackingStation.TryCompleteOrder` uses current `Box` assigned order.
   - Affected files: `Assets/Scripts/Core/OrderManager.cs`, `Assets/Scripts/Core/GameUIManager.cs`, `Assets/Scripts/Gameplay/PackingStation.cs`, `Assets/Scripts/Player/PlayerInteractionController.cs`.
   - Risk: **Medium**; not a hard crash, but null/destroyed object access can occur if scene logic assumes order still active.
   - Fix: add state checks before completion; validate `order` still active in `PackingStation` or `PlayerInteractionController` before applying label/complete.

3. **Runtime generated objects missing required components**
   - Evidence: `WarehouseEquipmentManager`, `ShelfManager` create primitives when prefabs missing; `GameUIManager` creates UI with only `RectTransform` and adds `OrderItemUI` (depends on `Initialize`).
   - Affected files: `Assets/Scripts/Environment/WarehouseEquipmentManager.cs`, `Assets/Scripts/Environment/ShelfManager.cs`, `Assets/Scripts/Core/GameUIManager.cs`.
   - Risk: **Medium**; if `OrderItemUI` expects child `TextMeshPro` components or `PickableItem` expects colliders/rigidbodies, null refs can crash if not guarded.
   - Fix: ensure prefabs are assigned in scenes; add validation in `Start` for required components and disable script if missing.

4. **Physics AutoSyncTransforms disabled with runtime transform mutation**
   - Evidence: `ProjectSettings/DynamicsManager.asset` sets `m_AutoSyncTransforms: 0` and scripts move/instantiate objects at runtime.
   - Affected files: `Assets/Scripts/Environment/WarehouseBuilder.cs`, `WarehouseEquipmentManager.cs`, `ShelfManager.cs`.
   - Risk: **Low**; more likely to cause interaction misses than crashes, but can lead to follow-on null flows.
   - Fix: call `Physics.SyncTransforms()` after runtime repositioning when immediate physics queries depend on new transforms.

5. **Networking UI without manager**
   - Evidence: `NetworkUI` logs error if `NetworkManager.singleton` missing; OnGUI uses it if found.
   - Affected files: `Assets/Scripts/Networking/NetworkUI.cs`.
   - Risk: **Low**; guard exists, but scene misconfiguration could still allow UI access; keep scene prefabs consistent.
   - Fix: ensure `OrderUpNetworkManager` exists in scenes where `NetworkUI` appears.

## Notes
- No `Resources.Load` or Addressables usage found in scripts (lower asset load crash risk).
- No unmanaged/unsafe code patterns found (lower native crash risk).
- Tests use `DestroyImmediate` in editor only; not runtime risk.

