# space-shooter-prototype
AGALAG

AGALAG is a 2D space shooter inspired by classic arcade titles like Galaga and Space Invaders. You control a spaceship defending your home planet from waves of alien invaders. Only five enemy ships can slip through before your planet is overwhelmed — let more pass, and it’s game over.

The game features smooth movement, animated backgrounds, increasing difficulty, and high-score tracking. As you progress, enemies move faster and more aggressively, testing your reflexes and strategy.

🎯 Project Purpose

AGALAG demonstrates object-oriented game development using C# and XAML (UWP). It applies principles like inheritance, encapsulation, and class design while showcasing event handling, UI management, and a structured game loop.

🕹️ Features

Smooth horizontal player movement

Shooting and collision detection

Animated scrolling background

Difficulty and game mode selection

Warp mode (loop around screen edges)

Sound effects for feedback

Dynamic enemy spawning and scaling difficulty

Local high-score saving (highscores.txt)

Pause and main menu navigation

Error handling for stability

🎮 Controls

Move Left	- ← / A

Move Right - → / D

Shoot - Spacebar

Pause / Return to Menu - ESC

⚙️ Gameplay Overview

Defend your planet by shooting down alien ships.

Only 5 enemies can reach the bottom of the screen.

Survive for a set duration or as long as possible (depending on mode).

Each destroyed enemy slightly increases game speed and difficulty.

A “You Win” or “Game Over” message appears at the end of each round.

🧭 Game Modes

Difficulty:

Easy – Survive 30 seconds

Normal – Survive 1 minute

Hard – Survive 3 minutes

Modes:

Casual – Survive for a set time

Endless – Play until you lose

Additional Setting:

Warp Mode – Teleport from one screen edge to the opposite

🧩 Class Overview

GamePiece – Base class for all objects (player, enemies, bullets). Handles positioning, movement, and visibility.
PlayerPiece – Inherits from GamePiece; manages horizontal movement and warp mode.
EnemyPiece – Inherits from GamePiece; controls enemy descent and respawn logic.
Bullet – Represents player projectiles; handles firing, motion, and deactivation.
HighScore – Manages saving, loading, and displaying high scores.
LandingPage – Main menu UI; allows game configuration and navigation.
MainPage – Core gameplay logic; handles game loop, timers, collisions, and sound.
HighScorePage – Displays top 10 scores and manages file I/O.
GameSettings – Stores selected difficulty, mode, and warp options.

🖥️ Platform

Developed in C# and XAML

Built for Windows (UWP framework)

Requires keyboard input

🧱 Change Log

Added functional difficulty and game modes

Implemented scrolling background and dynamic visuals

Redesigned user interface and menus

Added local high-score saving via text file

Introduced Warp / No Warp toggle

Enabled ESC key to quit to menu

🐞 Known Issues

Win message delay: A short 2–3 second delay before the victory message appears (does not affect gameplay).

👩‍💻 Author

Created by: Misheel Azjargalbayar
