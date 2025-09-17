# Chess.NET

## Overview

Chess.NET is a C# chess application built with SplashKit. It implements standard chess rules and supports multiple game modes: local 2-player, AI opponent, custom board setup, and networked multiplayer. The project is intended both as an educational object-oriented programming demonstration and as a playable chess application.

## Table of contents

- [Features](#features)
- [Requirements](#requirements)
- [Quick start (PowerShell)](#quick-start-powershell)
- [Runtime resources](#runtime-resources)
- [Project structure (important files)](#project-structure-important-files)
- [Game modes and behavior](#game-modes-and-behavior)
- [Save files](#save-files)
- [AI notes](#ai-notes)
- [Network multiplayer](#network-multiplayer)
- [Troubleshooting](#troubleshooting)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

## Features

- Local 2-player on the same machine
- Play against AI (remote API with local Stockfish fallback)
- Custom board setup screen
- Networked play using UDP discovery + TCP move transport
- Full chess rules: castling, en passant, promotion, check/checkmate, draw conditions
- Time controls, timers, save/load, and autosave

## Requirements

- Windows 10/11
- .NET SDK 7.0+ (project contains outputs targeting .NET 9.0; install the latest SDK)
- SplashKit runtime (the repository contains `SplashKitSDK.dll` in `bin/*/net9.0/`)

## Quick start (PowerShell)

Open PowerShell (pwsh.exe) in the project root and run:

Restore and build (Release):

```powershell
dotnet restore
dotnet build -c Release
```

Run from source (debug):

```powershell
dotnet run --project . -c Debug
```

Publish and run the published executable (Windows x64):

```powershell
# Publish
dotnet publish -c Release -r win-x64 --self-contained false /p:PublishSingleFile=false

# Run the published executable
& .\bin\Release\net9.0\Chess.exe
```

Notes: if you see SplashKit errors when running the built executable, copy `SplashKitSDK.dll` to the executable folder or install the SplashKit runtime.

## Runtime resources

Ensure these resources are present in the `Resources/` folder when distributing or running the published build:

- `Resources/Pieces/` — piece sprites (png files)
- `Resources/Scripts/stockfish-windows-x86-64-avx2.exe` — local Stockfish engine (used for offline AI)
- `Resources/Sounds/` — sound assets

The `bin/Release/net9.0/` folder in this repository already contains `SplashKitSDK.dll` and `Newtonsoft.Json.dll` for convenience.

## Project structure (important files)

- `Program.cs` — application entry point
- `Game.cs`, `Board.cs`, `BoardRenderer.cs` — core game and rendering logic
- `Piece*.cs` (Pawn, Rook, Knight, Bishop, Queen, King) — piece implementations
- `IBot.cs`, `ChessBot.cs`, `StockfishBot.cs`, `ChessApiBot.cs` — AI interfaces and implementations
- `NetworkManager.cs`, `NetworkSelectionScreen.cs` — network multiplayer
- `GameSaver.cs` — save/load logic
- `Resources/` — images, sounds, and Stockfish binary

## Game modes and behavior

- Local: two players on one machine with full rule enforcement
- AI: attempts to use a configured remote API (`ChessApiBot`); falls back to the included Stockfish binary (`StockfishBot`)
- Custom setup: drag-and-drop piece placement and validation
- Network: discovery via UDP, game transport via TCP, moves encoded as algebraic coordinates (e.g. `e2e4`)

## Save files

Saves are managed by `GameSaver.cs`. By default save files are written to a local application data folder (see `GameSaver` for exact path). An autosave is created during gameplay.

## AI notes

- The project uses a remote API bot if configured. If the API is unavailable, the local Stockfish binary is used as a fallback.

## Network multiplayer

- `NetworkManager` exposes hosting and client functionality.
- Discovery uses UDP broadcasts on the local network; active games exchange moves over TCP.
- Move format is simple coordinate notation (for example `e2e4`). Promotion is encoded where applicable.

## Troubleshooting

- Missing SplashKit: copy `SplashKitSDK.dll` (from `bin/Release/net9.0/`) into the same folder as the executable or install SplashKit.
- Stockfish not found: ensure the binary exists at `Resources/Scripts/` and is executable.
- If the UI fails to render or the app exits, run the `Chess.exe` from `bin/Release/net9.0/` to view `myeasylog.log` for runtime logging.

## Contributing

All contributions are welcome.

## Contact

For questions about running the project locally, open an issue in the repository or inspect `Documentation.docx` in the project root for additional author notes.
