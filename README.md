# The Shadowstone Curse

A 2D action RPG dungeon crawler built with Unity, featuring procedurally generated levels, skill-based combat, and progressive character development.

## 🎮 Game Overview

The Shadowstone Curse is an action-packed 2D adventure where players navigate through procedurally generated dungeons, battle various enemies, and collect powerful skills to enhance their abilities. The game features a card-based skill acquisition system and challenging boss encounters across multiple floors.

## 🌟 Key Features

### 🏰 Procedural Level Generation
- **Multi-floor dungeon system** with configurable room layouts
- **Dynamic level assembly** using start rooms, normal rooms, boss rooms, and connecting corridors
- **Progressive difficulty** scaling across floors
- **Exit portals** for seamless floor transitions

### ⚔️ Combat System
- **Real-time action combat** with various enemy types
- **Skill-based abilities** with cooldown management
- **Level-up mechanics** for skills with scaling damage and effects
- **Mana system** for resource management

### 🃏 Card-Based Skill System
- **Skill card selection** after defeating enemies or opening chests
- **Random skill distribution** from a configurable pool
- **Progressive skill enhancement** with level-based improvements
- **Visual card flip animations** and effects

### 👹 Enemy AI Types
- **Rat AI**: Ground-based melee enemies with patrol and chase behaviors
- **Flying Eye AI**: Aerial enemies with dive attack patterns
- **Vampire AI**: Life-stealing enemies with self-healing abilities
- **Skeleton AI**: Basic melee combatants with patrol patterns

### 🎯 Interactive Elements
- **Treasure chests** with reward systems
- **Room triggers** for dynamic enemy spawning
- **Portal systems** for level progression
- **UI integration** for health, mana, and skill management

## 🛠️ Technical Architecture

### Core Systems
- **Character Stats System**: Health, mana, damage, and speed management
- **Level Generator**: Procedural room assembly and player positioning
- **UI Manager**: Canvas management for rewards and game states
- **Room Trigger System**: Automatic level activation and enemy spawning

### Asset Organization
```
Assets/
├── Scripts/
│   ├── Controllers/        # Game logic controllers
│   ├── Core/              # Core game systems
│   ├── EnemyControllers/  # AI behavior scripts
│   └── UI/                # User interface components
├── TextMesh Pro/          # Text rendering system
└── [Additional Assets]    # Sprites, animations, sounds
```

## 🎯 Gameplay Mechanics

### Character Progression
- **Skill Data System**: ScriptableObject-based skill configuration
- **Level-based scaling**: Damage, range, and cooldown improvements
- **Resource management**: Mana consumption and regeneration

### Enemy Behaviors
- **Detection ranges**: Dynamic player tracking within specified distances
- **Attack patterns**: Unique combat styles for each enemy type
- **Patrol systems**: Autonomous movement when player is not detected
- **Health and damage systems**: Integrated with character stats

### Level Design
- **Room-based structure**: Modular level construction
- **Connector system**: Seamless room transitions
- **Spawn point management**: Strategic player and enemy positioning

## 🚀 Getting Started

### Prerequisites
- Unity 2022.3 LTS or later
- TextMesh Pro package (included)

### Installation
1. Clone the repository
2. Open the project in Unity
3. Ensure all required packages are installed
4. Build and run the game

## 🎮 Controls
- **Movement**: WASD or Arrow Keys
- **Skills**: Various key bindings (configurable)
- **Interaction**: Automatic trigger-based interactions

## 🔧 Configuration

### Skill System
Skills are configured using ScriptableObjects with the following parameters:
- Base damage and damage per level
- Cooldown timers and range scaling
- Visual effects and audio clips
- Dash distances and effect scaling

### Level Generation
Floors are configured with:
- Room prefab collections (start, normal, boss, corridor)
- Number of rooms per floor
- Difficulty progression settings

## 🎨 Art & Audio
- 2D sprite-based graphics
- Animator-driven character animations
- Audio system integration for skills and combat
- TextMesh Pro for UI text rendering

## 📝 Development Status

This project appears to be in active development with a solid foundation of core systems implemented. The modular architecture allows for easy expansion of content and features.

## 🤝 Contributing

This appears to be a team project under "The-Shadow-Force" organization. For contributions, please follow the established code structure and naming conventions.

## 📄 License

Please refer to the repository settings for license information.

---

*The Shadowstone Curse - Venture into the depths, master your skills, and break the ancient curse!*
