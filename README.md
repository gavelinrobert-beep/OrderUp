# Order Up

A party co-op Unity game for 2-4 players where teams work together in a warehouse to pick and pack orders before time runs out!

## 🎮 Game Overview

**Order Up** is a fast-paced, cooperative multiplayer game where players take on roles in a warehouse to fulfill customer orders. Work together as Pickers and Packers to complete Standard and Express orders within a 5-minute time limit. Communication and coordination are key to achieving high scores!

## 🎯 Project Goals

- Create an engaging, accessible party game that encourages teamwork
- Deliver a polished MVP that can be expanded with additional features
- Implement smooth multiplayer gameplay using Unity Mirror
- Design intuitive role-based mechanics that are easy to learn but challenging to master
- Build a foundation for future content updates (new maps, items, roles, and game modes)

## 📦 MVP Scope

### Core Features

#### Game Setup
- **Players**: 2-4 players in cooperative multiplayer
- **Duration**: 5-minute rounds
- **Map**: 1 warehouse map with picking and packing zones

#### Player Roles
- **Picker**: Collects items from shelves and places them in carts
- **Packer**: Takes items from carts and packs them at packing stations

#### Order System
- **Order Types**:
  - **Standard Orders**: Normal priority, standard points
  - **Express Orders**: High priority, bonus points, time-limited
- Orders appear on a shared board visible to all players
- Clear visual indicators for order priority and time remaining

#### Gameplay Mechanics
- **Picking System**: 
  - Pick items from warehouse shelves
  - Place items in mobile carts
  - Move carts between picking and packing areas
- **Packing Station**:
  - Take items from carts
  - Place items in boxes
  - Apply correct shipping labels
  - Complete and submit orders

#### Scoring & Feedback
- **Team Score**: Shared score for all players
- **Round Summary**: End-of-round screen showing:
  - Total orders completed
  - Standard vs Express orders
  - Final team score
  - Performance breakdown

#### Technical Implementation
- **Multiplayer**: Networked gameplay using Unity Mirror
- **Platform**: PC (Windows/Mac/Linux)
- **Unity Version**: 2021.3 LTS or newer

### Out of Scope for MVP
- Multiple maps
- Additional roles beyond Picker/Packer
- Single-player mode
- Competitive modes
- Customization options
- Advanced statistics tracking

## 📅 Sprint Plan

### Sprint 1: Project Foundation (Week 1-2)
- [ ] Set up Unity project with Mirror networking
- [ ] Create basic warehouse environment
- [ ] Implement player movement and controls
- [ ] Set up multiplayer connection system

### Sprint 2: Core Mechanics (Week 3-4)
- [ ] Implement Picker role mechanics (pick items, use cart)
- [ ] Implement Packer role mechanics (pack boxes, apply labels)
- [ ] Create order generation system
- [ ] Implement Standard and Express order types

### Sprint 3: Game Loop (Week 5-6)
- [ ] Implement 5-minute round timer
- [ ] Create scoring system
- [ ] Build round summary screen
- [ ] Add order board UI

### Sprint 4: Polish & Testing (Week 7-8)
- [ ] Visual polish and UI improvements
- [ ] Sound effects and feedback
- [ ] Multiplayer testing and bug fixes
- [ ] Performance optimization
- [ ] Final playtesting and balancing

## 🛠️ Technical Stack

- **Engine**: Unity 2022.3 LTS
- **Networking**: Mirror (https://mirror-networking.com/) - To be integrated
- **Language**: C#
- **Version Control**: Git/GitHub
- **Target Platform**: PC (Windows, Mac, Linux)

## 🚀 Getting Started

### Prerequisites
- Unity Hub (latest version)
- Unity 2022.3 LTS (2022.3.17f1 or newer)
- Git

### Installation & Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/gavelinrobert-beep/OrderUp.git
   cd OrderUp
   ```

2. **Open in Unity Hub**
   - Open Unity Hub
   - Click "Add" or "Open"
   - Navigate to and select the `OrderUp` folder (root directory)
   - If prompted about Unity version, select Unity 2022.3 LTS
   - Unity Hub will download the correct version if not already installed

3. **Open the Project**
   - Once added, click on the project in Unity Hub to open it
   - Unity will import all assets (this may take a few minutes on first load)

### Running the MVP Prototype

#### In Unity Editor (Recommended for Testing)
1. In Unity, navigate to `Assets/Scenes/`
2. Double-click `Main.unity` to open the main scene
3. Press the **Play** button (▶️) at the top of the editor
4. The game will auto-start a 5-minute round

**MVP Gameplay Loop:**
- **Round Timer**: A 5-minute countdown timer runs automatically
- **Score Tracking**: Team score is displayed in the top-left corner
- **Order Spawning**: Orders automatically spawn every 30 seconds (max 5 active orders)
- **Game End**: When the timer reaches 0:00, the round ends and stats are logged to console

**UI Elements:**
- **Timer** (Top Center): Shows remaining time (MM:SS format)
  - White when > 2 minutes remaining
  - Yellow when < 2 minutes remaining
  - Red when < 1 minute remaining
- **Score** (Top Left): Displays current team score
- **Round Counter** (Top Left): Displays current round (requires a round text reference)
- **Order List** (Right Side): Container for active orders (placeholder for now)
- **IMGUI HUD** (Optional): Immediate-mode overlay showing score, round, timer, and summary

#### Testing the Managers
The MVP includes core manager scripts that coordinate the game loop:
- **GameManager**: Controls round timer and game state
- **ScoreManager**: Tracks team score and order completions
- **OrderManager**: Spawns and manages active orders
- **GameStateManager**: Tracks round count, state transitions, and exposes score updates

All managers log their activity to Unity's Console window. Open the Console (Window > General > Console) to see:
- Round start/end events
- Order spawn notifications
- Score updates
- Timer events

#### Building the Game
1. Go to **File > Build Settings**
2. Ensure `Assets/Scenes/Main.unity` is in the "Scenes in Build" list
3. Select your target platform (PC, Mac, Linux)
4. Click **Build** or **Build and Run**
5. Choose an output folder and wait for the build to complete

**Note**: Multiplayer networking with Mirror is not yet integrated. The current MVP focuses on the single-player game loop foundation.

### Project Structure

```
OrderUp/
├── Assets/
│   ├── Scenes/           # Unity scenes (Main.unity)
│   ├── Scripts/          # C# gameplay scripts
│   │   ├── GameManager.cs
│   │   ├── ScoreManager.cs
│   │   ├── OrderManager.cs
│   │   ├── GameUIManager.cs
│   │   ├── ProductData.cs
│   │   └── OrderData.cs
│   ├── Prefabs/          # Reusable game objects (to be populated)
│   ├── ScriptableObjects/ # Data assets
│   │   ├── Products/     # Product definitions
│   │   └── Orders/       # Order definitions
│   ├── UI/               # UI assets (to be populated)
│   ├── Audio/            # Sound effects and music (to be populated)
│   └── Art/              # Visual assets (to be populated)
├── ProjectSettings/      # Unity project configuration
├── Packages/             # Unity package dependencies
└── README.md
```

### Extending the MVP

The codebase includes TODO comments marking areas for future development:

**ScriptableObjects (Data Layer)**
- Add product icons/sprites
- Add 3D prefab references for products
- Add customer info to orders
- Implement order difficulty levels

**Game Managers**
- Implement pause/resume functionality
- Add round summary UI
- Implement express order timing and expiration
- Add order validation logic

**UI System**
- Create order list item prefabs
- Add visual feedback for order completion
- Implement round summary screen
- Add main menu and scene transitions

**Gameplay Mechanics (Future)**
- Player movement and controls
- Picker/Packer role mechanics
- Warehouse environment
- Cart and item interaction systems
- Mirror networking integration

## 🤝 Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct, development workflow, and the process for submitting pull requests.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👥 Team

Developed by the Order Up team

## 📞 Contact

For questions or feedback, please open an issue on GitHub.

---

**Status**: 🚧 In Development - MVP Phase
