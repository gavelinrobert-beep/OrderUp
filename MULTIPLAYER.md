# OrderUp Multiplayer Setup Guide

This guide explains how to set up and test the multiplayer functionality in OrderUp using Mirror networking.

## Overview

OrderUp uses [Mirror](https://mirror-networking.com/) for multiplayer networking. Sprint 1 has implemented the foundation for multiplayer gameplay, including:

- Network Manager configuration
- Player spawning and movement
- Basic warehouse environment
- Host/client connection system

## Quick Start

### Testing Multiplayer Locally

1. **Open the Project in Unity**
   - Open Unity Hub
   - Select the OrderUp project
   - Wait for Unity to load the project

2. **Open the Main Scene**
   - Navigate to `Assets/Scenes/Main.unity`
   - Double-click to open the scene

3. **Start as Host**
   - Press Play (▶️) in Unity Editor
   - Click the "Host Game" button in the top-left corner
   - You should see your player spawn in the warehouse

4. **Test with a Second Client (Build)**
   - Go to `File > Build Settings`
   - Click "Build and Run"
   - In the built game window, click "Join Game"
   - Both players should now be visible in the warehouse

### Testing with Multiple Instances

For more thorough testing, you can run multiple instances:

1. **Build the Game**
   - Go to `File > Build Settings`
   - Select your platform
   - Click "Build" and save to a folder

2. **Run Multiple Instances**
   - Run the built executable
   - Click "Host Game" in the first instance
   - Run another instance of the executable
   - Click "Join Game" in the second instance
   - Repeat for up to 4 players total

## Network Components

### OrderUpNetworkManager

The custom NetworkManager handles:
- Player connection/disconnection
- Maximum player limit (2-4 players)
- Server/client lifecycle

**Configuration:**
- Located in the Main scene
- Max Players: 4 (configurable)
- Player Prefab: Set to the Player prefab in the inspector

### Player Prefab

The Player prefab includes:
- **PlayerController**: Handles WASD movement and rotation
- **NetworkIdentity**: Required for networked objects
- **NetworkTransform**: Synchronizes position/rotation across network
- **CharacterController**: Provides character movement and collision

**Controls:**
- WASD or Arrow Keys: Move the player
- Player automatically rotates in movement direction

### Camera System

The CameraFollow component:
- Follows the local player smoothly
- Maintains an isometric/top-down view
- Only tracks the local player (not other players)

## Scene Setup

The Main scene includes:

1. **WarehouseBuilder**
   - Creates floor, walls, picking zone, and packing zone
   - Runs automatically on scene start
   - Configurable dimensions and colors

2. **NetworkManager**
   - OrderUpNetworkManager component
   - Network HUD for testing (optional)
   - Transport settings

3. **Camera**
   - Main camera with CameraFollow component
   - Automatically assigned to local player on spawn

4. **UI**
   - NetworkUI component for host/join buttons
   - Simple IMGUI interface for testing

## Network Settings

Default network settings:
- **Transport**: KCP Transport (default Mirror transport)
- **Port**: 7777
- **Max Connections**: 4
- **Network Address**: localhost (for joining)

To connect to a remote server:
1. Update the Network Address in the NetworkManager inspector
2. Or modify it in the NetworkUI script

## Troubleshooting

### Players can't connect
- Check that the host has started before clients try to join
- Verify the network address is correct (localhost for local testing)
- Check firewall settings if testing across network

### Player doesn't spawn
- Verify the Player prefab is assigned in NetworkManager
- Check that NetworkIdentity is on the Player prefab
- Ensure the scene has spawn points or uses default spawn

### Camera doesn't follow player
- Check that the Main Camera has the CameraFollow component
- Verify PlayerController calls SetTarget on spawn
- Check that there's only one Main Camera in the scene

### Movement feels laggy
- This is expected for remote players due to network latency
- Local player should feel responsive
- NetworkTransform interpolation can be adjusted for smoother remote player movement

## Next Steps

Sprint 1 establishes the networking foundation. Future sprints will add:
- Role selection (Picker/Packer)
- Cart and item interaction
- Synchronized order system
- Score synchronization
- Lobby system with ready-up

## Additional Resources

- [Mirror Documentation](https://mirror-networking.gitbook.io/docs/)
- [Mirror GitHub](https://github.com/MirrorNetworking/Mirror)
- [Mirror Discord](https://discord.gg/N9QVxbM)
