# 🧩 Arrows Puzzle Escape – Level Generator

A procedural **level generator** for the puzzle game **“Arrows Puzzle Escape”**, designed to automatically create solvable and scalable arrow-based puzzle levels.

> 🎯 Goal: Generate playable levels for *Arrows Puzzle Escape* to support level design, testing.

---

## 📖 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Project Structure](#project-structure)
- [Demo Video](#demo-video)
- [Environment & Dependencies](#environment--dependencies)
- [Parameters](#parameters)
- [Author](#author)
- [License](#license)

---

## 🧠 Overview

This project provides an **automated level generation tool** for the puzzle game *Arrows Puzzle Escape*.  
It focuses on generating playable levels filled with arrows that follow logical movement rules.

With this generator, you can:
- Produce playable levels of different sizes;
- Automatically ensure puzzle solvability and variation;
- Integrate generated results directly into Unity;
- Support designers by automating the tedious level-creation process.

---

## ⚙️ Features

✅ Procedural level generation  
✅ Arrow logic and path validation  
✅ Adjustable map size and arrow parameters  

---

## 📁 Project Structure

```
ArrowsPuzzleEscape-LevelGenerator/
├── Assets/
│   ├── Scripts/                 # Core C# logic scripts
│   │   ├── Arrow/               # Arrow-related scripts
│   │   │   ├── Arrow.cs               # Arrow data structure
│   │   │   ├── ArrowClickHandler.cs   # Handles arrow click events
│   │   │   └── ArrowGenerator.cs      # Arrow visualization and spawning
│   │   ├── Debugger/            # Debugging tools
│   │   │   └── ClickDebugger.cs       # Debug helper for input testing
│   │   ├── Level/               # Level generation and management
│   │   │   └── LevelGenerator.cs      # Core level generation logic
│   │   └── Tile/                # Tile and grid system
│   │       └── Tile.cs               # Grid tile definition
│   ├── Prefabs/                 # Arrow and tile prefabs
│   └── Scenes/                  # Example Unity scenes
├── ProjectSettings/
├── Packages/
├── README.md
└── .gitignore
```

---

## 📽️ Demo Video

> 📺 [Click here to watch the gameplay demo](https://drive.google.com/file/d/1DFxqxDP7hEUZzpdIrl15shHpdgS1_J2i/view?usp=sharing)

---

## 💻 Environment & Dependencies

| Component | Version | Notes |
|------------|----------|-------|
| Unity | 6000.2.7f2 or newer | Recommended |
| C# | 8.0+ | Core language |
| IDE | Visual Studio | Suggested for debugging |

---

## ⚖️ Parameters

| Parameter | Type | Default | Description |
|------------|------|----------|-------------|
| `Level Width` | int | 8 | Number of tiles horizontally |
| `Level Height` | int | 8 | Number of tiles vertically |
| `Arrow Min Length` | int | 5 | Minimum arrow path length |
| `Arrow Max Length` | int | 10 | Maximum arrow path length |
| `Change Direction Chance` | float | 0.3 | Probability for an arrow to change direction during generation |

---

## 👤 Author

**Xiaoyu Zhang**  
 
🔗 [GitHub](https://github.com/PanAkatsuki)

---

## 📜 License

MIT License

Copyright (c) 2025 PanAkatsuki

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files...

---