# 🌌 Galactic Arena Backend

A headless, distributed multiplayer game backend built with **Microsoft Orleans** and **.NET**. 

This project serves as a foundational architecture for real-time multiplayer games, demonstrating how to use the Virtual Actor Model to handle player state, matchmaking concurrency, and lock-free game logic at scale.

## 🏗️ Architecture

The solution follows a Clean Architecture approach, separating game contracts, business logic, and the hosting environment.

* **`GalacticArena.Interfaces`**: The shared contracts. Contains the Grain interfaces (`IPlayerGrain`, `IMatchGrain`) and state POCOs marked with `[GenerateSerializer]`. This project has zero dependencies on databases or game logic.
* **`GalacticArena.Grains`**: The core game logic. Implements the interfaces. Handles state mutations, XP distribution, and Grain-to-Grain communication (e.g., a Match checking a Player's status).
* **`GalacticArena.Silo`**: The Orleans Host. Currently configured for local development using Localhost Clustering and In-Memory grain storage. 
* **`GalacticArena.Web`** *(Planned)*: The API Gateway and SignalR Hub to act as the bridge between the Orleans cluster and game clients (Unity, Unreal, etc.).

## 🧠 Core Orleans Concepts Demonstrated

1. **State Persistence (`IPersistentState<T>`)**: The `PlayerGrain` manages player stats (Level, XP, Health) entirely in memory and flushes changes to storage without writing raw SQL or managing an Entity Framework `DbContext`.
2. **Lock-Free Concurrency**: The `MatchGrain` manages multiple players joining a 1v1 arena. Because Orleans guarantees single-threaded execution per Grain, there are no race conditions when two players attempt to join the same match slot simultaneously.
3. **Grain-to-Grain RPC**: Demonstrates complex routing where a Match acts as an orchestrator, looking up Player Grains via `IGrainFactory` to verify their status before allowing them to join a game.

## 🚀 Getting Started

### Prerequisites
* [.NET 8.0 SDK](https://dotnet.microsoft.com/download) (or later)

### Running the Server Locally

1. Clone the repository.
2. Navigate to the root directory.
3. Start the Orleans Silo:

```bash
dotnet run --project src/GalacticArena.Silo
```

Upon startup, the Silo runs a simulation in `Program.cs` that:
1. Activates two players ("CommanderShepard" and "MasterChief").
2. Creates a new Match instance.
3. Successfully adds both players to the match.
4. Rejects a third player ("Doomguy") because the room is full.
5. Resolves the match and awards 50 XP to the winner, demonstrating state updates.

## 🛣️ Roadmap

- [x] **Phase 1:** Player identity and state management.
- [x] **Phase 2:** Match instances and Grain-to-Grain communication.
- [x] **Phase 3:** Silo configuration and local testing.
- [ ] **Phase 4:** Build the API Gateway (`GalacticArena.Web`) and integrate **SignalR** to push real-time state changes to connected clients.
- [ ] **Phase 5:** Swap In-Memory storage for **PostgreSQL** and Localhost clustering for **Redis**.




















