# Unity Scene Setup Guide for Sprint 1

This guide explains how to configure the Main.unity scene to use the Sprint 1 networking and gameplay features.

## Scene Setup Steps

### 1. Create Network Manager GameObject

1. In the Unity Hierarchy, right-click and select "Create Empty"
2. Rename it to "NetworkManager"
3. Add the following components:
   - **OrderUpNetworkManager** (Component > Scripts > OrderUp.Networking > OrderUpNetworkManager)
   - **NetworkUI** (Component > Scripts > OrderUp.Networking > NetworkUI)

4. Configure the OrderUpNetworkManager:
   - **Network Address**: `localhost`
   - **Max Players**: `4`
   - **Transport**: Should auto-detect KCP Transport
   - **Player Prefab**: Drag the `Assets/Prefabs/Player.prefab` into this field
   - **Spawn Info > Default Spawn Location**: Set to `(0, 0, 0)` or your preferred spawn point

### 2. Configure Main Camera

1. Select the Main Camera in the Hierarchy
2. Add the **CameraFollow** component (Component > Scripts > OrderUp.Player > CameraFollow)
3. Configure CameraFollow settings:
   - **Offset**: `(0, 10, -8)` for top-down isometric view
   - **Smooth Speed**: `0.125` for smooth camera movement
   - **Look At Height**: `1` to focus on player center

### 3. Add Warehouse Builder

1. In the Unity Hierarchy, right-click and select "Create Empty"
2. Rename it to "WarehouseBuilder"
3. Add the **WarehouseBuilder** component (Component > Scripts > OrderUp.Environment > WarehouseBuilder)
4. Configure settings (optional):
   - **Warehouse Width**: `20` (default)
   - **Warehouse Length**: `30` (default)
   - **Wall Height**: `4` (default)
   - **Picking Zone Color**: Light green with transparency
   - **Packing Zone Color**: Light blue with transparency

The WarehouseBuilder will automatically create the warehouse environment when the scene starts.

### 4. Keep Existing Game Managers

The following existing GameObjects should remain in the scene:
- **GameManager**: Manages round timer and game state
- **ScoreManager**: Tracks team score
- **OrderManager**: Manages order spawning
- **GameStateManager**: Tracks overall game state
- **GameUIManager**: Manages UI elements
- **ImGuiHudManager**: Debug HUD overlay

These managers are already configured and will work alongside the new networking features.

## Scene Hierarchy Example

After setup, your scene hierarchy should look like:

```
Main (Scene)
├── NetworkManager
│   └── OrderUpNetworkManager
│   └── NetworkUI
├── Main Camera
│   └── CameraFollow
├── WarehouseBuilder
│   └── WarehouseBuilder
├── GameManager (existing)
├── ScoreManager (existing)
├── OrderManager (existing)
├── GameStateManager (existing)
├── GameUIManager (existing)
├── ImGuiHudManager (existing)
└── Canvas (existing UI)
```

## Testing the Setup

1. **Press Play in Unity Editor**
   - You should see the warehouse environment generated
   - Network UI buttons should appear in the top-left
   - Click "Host Game" to start hosting

2. **Build and Test Client**
   - Go to File > Build Settings
   - Click "Build and Run"
   - In the built game, click "Join Game"
   - Both players should spawn and be able to move with WASD

3. **Verify Functionality**
   - Both players should be visible
   - WASD controls should work for the local player
   - Camera should follow the local player
   - Remote players should be visible and their movement should sync
   - Warehouse zones (picking and packing) should be visible

## Troubleshooting

### Player prefab not spawning
- Verify the Player prefab is assigned in NetworkManager's "Player Prefab" field
- Check that the Player prefab has NetworkIdentity component

### Camera not following player
- Ensure CameraFollow component is on the Main Camera
- Verify there's only one Main Camera tagged as "MainCamera"

### Warehouse not appearing
- Check that WarehouseBuilder component is added to a GameObject
- Verify the GameObject is active in the scene
- Check Console for any errors

### Network buttons not appearing
- Ensure NetworkUI component is on the NetworkManager GameObject
- Check that the GameObject is active

## Input Setup

Unity's Input System should be configured with:
- **Horizontal**: A/D or Left/Right arrows
- **Vertical**: W/S or Up/Down arrows

These are typically configured by default in Unity projects.

## Next Steps

After completing Sprint 1 setup:
- Test multiplayer with 2-4 players
- Verify all networking features work as expected
- Move to Sprint 2: Core Mechanics (Picker/Packer roles, cart interaction)

## Additional Notes

- The Player prefab uses a simple capsule mesh for visualization
- You can customize the player appearance by modifying the Player prefab
- Network synchronization runs at 10Hz (configurable in NetworkTransform)
- KCP Transport runs on port 7777 by default
