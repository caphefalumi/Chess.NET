# Chess.NET

<div align="center">
  <a href="https://apps.microsoft.com/detail/9NSGRLLH9K7Z">
    <img src="https://get.microsoft.com/images/en-us%20dark.svg" alt="Get it from Microsoft" width="200"/>
  </a>
</div>

## Screenshots

<div align="center">
  <img src="Resources/Main%20Menu.png" alt="Main Menu" width="320" />
  <br><sub>Main Menu</sub>
  <br><br>
  <img src="Resources/LAN%20Discovery.png" alt="LAN Game Discovery" width="320" />
  <br><sub>LAN Game Discovery</sub>
  <br><br>
  <img src="Resources/AI%20Thinking.png" alt="AI Opponent in Action" width="320" />
  <br><sub>AI Opponent in Action</sub>
</div>

## Overview

Chess.NET is a C# chess application built with SplashKit. It implements standard chess rules and supports multiple game modes: local 2-player, AI opponent, custom board setup, and networked multiplayer. The project is intended both as an educational object-oriented programming demonstration and as a playable chess application.


## Table of Contents

- [Chess.NET](#chessnet)
  - [Screenshots](#screenshots)
  - [Overview](#overview)
  - [Table of Contents](#table-of-contents)
  - [Features](#features)
  - [Requirements](#requirements)
  - [Quick start (PowerShell)](#quick-start-powershell)
  - [Game modes and behavior](#game-modes-and-behavior)
  - [Troubleshooting](#troubleshooting)
  - [Contributing](#contributing)

## Features

- Local 2-player on the same machine
- Play against AI (remote API with local Stockfish fallback)
- Custom board setup screen
- Networked play using UDP discovery + TCP move transport
- Full chess rules: castling, en passant, promotion, check/checkmate, draw conditions
- Time controls, timers, save/load, and autosave

## Requirements

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

## Game modes and behavior

- Local: two players on one machine with full rule enforcement
- AI: attempts to use a configured remote API (`ChessApiBot`); falls back to the included Stockfish binary (`StockfishBot`)
- Custom setup: drag-and-drop piece placement and validation
- Network: discovery via UDP, game transport via TCP, moves encoded as algebraic coordinates (e.g. `e2e4`)

## Troubleshooting

- Missing SplashKit: copy `SplashKitSDK.dll` (from `bin/Release/net9.0/`) into the same folder as the executable or install SplashKit.
- Stockfish not found: ensure the binary exists at `Resources/Scripts/` and is executable.
- If the UI fails to render or the app exits, run the `Chess.exe` from `bin/Release/net9.0/` to view `myeasylog.log` for runtime logging.

## Contributing

All contributions are welcome.
