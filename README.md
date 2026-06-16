# FPS-Learning-Project
This project is a personal sandbox environment focused on mastering FPS mechanics, character control, and combat systems in Unity. The goal is to build a robust, maintainable, and modular foundation.

🎯 Learning Objectives
Character Controller: Implementing smooth movement, crouching, and sprinting.

Weapon Systems: Creating a modular system for shooting, reloading, and weapon switching.

AI Behaviors: Developing simple enemy AI using NavMesh and state machines.

Performance: Optimizing raycasting and object pooling for projectiles.

🛠 Tech Stack
Engine: Unity 2022.3

Language: C#

Core Systems:

Input System (old Unity Input System)

Raycasting for combat

ScriptableObjects for weapon data (stats, ammo, fire rate)

📂 Project Structure
Assets/Scripts/Player/: Logic for movement and camera control.

Assets/Scripts/Combat/: Weapon, damage, and projectile handling.

Assets/Scripts/AI/: Enemy logic and state behaviors.

📝 Current Status (Learning Log)
[x] Basic First-Person Movement.

[x] Shooting mechanics with Raycasts.

[x] Enemy AI patrolling.

[x] Weapon reloading animation logic.

💡 Engineering Principles applied
This project is built using:

Composition over Inheritance: Using small, focused components instead of massive scripts.

Dependency Injection: Keeping scripts decoupled to improve testability.

Clean Code: Following C# conventions for readability.
