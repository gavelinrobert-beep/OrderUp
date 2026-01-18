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

- **Engine**: Unity 2021.3 LTS or newer
- **Networking**: Mirror (https://mirror-networking.com/)
- **Language**: C#
- **Version Control**: Git/GitHub
- **Target Platform**: PC (Windows, Mac, Linux)

## 🚀 Getting Started

### Prerequisites
- Unity Hub
- Unity 2021.3 LTS or newer
- Git

### Installation
1. Clone the repository
   ```bash
   git clone https://github.com/gavelinrobert-beep/OrderUp.git
   ```
2. Open Unity Hub
3. Add the project (select the OrderUp folder)
4. Open the project in Unity
5. Install Mirror package via Package Manager or Asset Store

### Running the Game
1. Open the main scene in Unity
2. Press Play to test in editor
3. For multiplayer testing, build and run multiple instances

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