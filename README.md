# 🕹️ Pac-Man Game Recreation

A fully functional, desktop-targeted recreation of the classic arcade game **Pac-Man**, built from scratch using **C#** and the **Uno Platform**.

This project was developed to demonstrate proficiency in game loop architecture, state management, static/dynamic collision detection, and UI rendering optimization without relying on heavy game engines like Unity or Godot.

## 🚀 Tech Stack
* **Language:** C# 12
* **Framework:** Uno Platform / .NET MAUI ecosystem concepts
* **Target Environment:** .NET 10 (Desktop / Windows)
* **UI Markup:** XAML

## 🧠 Core Technical Features

* **Custom Game Loop Engine:** Implemented a robust 60 FPS rendering cycle utilizing `DispatcherTimer`, separating the rendering logic from the state updates.
* **Hybrid Collision Detection:**
    * *Static Collision:* Matrix-based grid calculation for walls, pellets, and power-ups.
    * *Dynamic Collision:* Real-time absolute distance calculation for Player vs. Ghost encounters.
* **Separation of Concerns (MVC Pattern):** Strict architectural boundary between `Models` (Entities like Player and Ghosts), `Views` (XAML and Canvas rendering), and the Controller/GameManager logic.
* **Event-Driven Audio System:** Integrated Windows native `System.Media.SoundPlayer` with custom cooldown logic (DateTime calculations) to prevent sound overlapping and audio-thread locking, keeping the application extremely lightweight.
* **Local Data Persistence:** High score tracking, reading, writing, and sorting via local file I/O combined with **LINQ** queries.

## ⚙️ How to Run Locally

1. Ensure you have the [.NET 10 SDK](https://dotnet.microsoft.com/download) installed on your machine.
2. Clone this repository:
```bash
   git clone https://github.com/eniomartinst/pacman-game-recreation.git
   ```
3. Navigate to the project root directory.
4. Run the application via the .NET CLI:
```bash
   dotnet run --framework net10.0-desktop
   ```

> **Note on .NET Versions:** If you are running a different version of the .NET SDK (e.g., .NET 8 or .NET 9), simply open the `pacman.csproj` file, locate the `<TargetFrameworks>` tag, and change `net10.0-desktop` to your installed version before running the command.
