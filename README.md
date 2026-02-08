# Unity Scene Bootstrap

[![GitHub](https://img.shields.io/github/license/SBUplakankus/unity-scene-bootstrapper)](LICENSE)
[![GitHub release](https://img.shields.io/github/v/release/SBUplakankus/unity-scene-bootstrapper)](https://github.com/SBUplakankus/unity-scene-bootstrapper/releases)
[![GitHub issues](https://img.shields.io/github/issues/SBUplakankus/unity-scene-bootstrapper)](https://github.com/SBUplakankus/unity-scene-bootstrapper/issues)
[![GitHub stars](https://img.shields.io/github/stars/SBUplakankus/unity-scene-bootstrapper?style=social)](https://github.com/SBUplakankus/unity-scene-bootstrapper/stargazers)

A lightweight Unity Editor tool that automates creation of organized scene hierarchy structures with one click.

The tool is editor-only, adds no runtime dependencies, and creates only empty GameObjects.

## Installation

### Via Unity Package Manager (From Git URL)

1. Open Unity Editor
2. Go to **Window → Package Manager**
3. Click the **+** button in the top-left corner
4. Select **Add package from git URL...**
5. Enter: `https://github.com/SBUplakankus/unity-scene-bootstrapper.git`
6. Click **Add**


### Via GitHub Release (From File)
1. Download the `SceneBootstrap.unitypackage` from the Release section
2. In Unity: `Assets > Import Package > Custom Package`
3. Select the package and import all files

---

## Features

- **One-click scene setup** – Create organized hierarchies instantly
- **Customizable groups** – Toggle which groups to create
- **Nested child objects** – Optional child hierarchies per group
- **Visual organization** – Dashed formatting for clear hierarchy separation
- **Persistent preferences** – User settings saved via EditorPrefs
- **Smart duplication prevention** – Won't create duplicate objects
- **Full Undo support** – All creations can be undone
- **Selection control** – Option to select created objects after generation
- **Editor-only** – No runtime code or dependencies

---

## Example Output

```
--- SCENE ROOT ---
├── -------- PLAYER --------
│    ├── Camera
│    ├── Player Controller
│    └── Character Model
├── ------ GAMEPLAY ------
│    ├── Interactables
│    ├── Enemies
│    ├── Pickups
│    └── Spawn Points
├── --------- UI ----------
│    ├── Overlay Canvas
│    ├── World Canvases
│    ├── Event System
│    └── UI Managers
├── ----- ENVIRONMENT -----
│    ├── Props
│    ├── Foliage
│    ├── Architecture
│    └── Detail Meshes
├── ----- SYSTEMS ------
│    ├── Audio Controller
│    ├── UI Controller
│    ├── Save Controller
│    └── Game Manager
├── -------- AUDIO --------
│    ├── Emitters
│    ├── Ambient Zones
│    ├── Music System
│    └── Audio Mixer
├── --------- MAP ---------
│    ├── Lighting
│    ├── Level Geometry
│    ├── Navigation
│    └── Triggers
└── ---------- FX ----------
├── Particles
├── Post Processing
├── Decals
└── Visual Effects
```

*Note: All objects are empty GameObjects with no components added.*

---

## Usage

1. Open Unity
2. Navigate to `Tools > Scene Bootstrap`
3. In the window that appears:
   - **Select groups** – Check which hierarchy groups to create
   - **Configure options** – Toggle root creation, dashed formatting, and selection
   - **Click "Create Scene Structure"** – The hierarchy will be created instantly

### Available Groups
- **PLAYER** – Camera, Player Controller, Character Model
- **GAMEPLAY** – Interactables, Enemies, Pickups, Spawn Points
- **UI** – Overlay Canvas, World Canvases, Event System, UI Managers
- **SYSTEMS** – Audio, UI, Save, and Game Managers
- **AUDIO** – Emitters, Ambient Zones, Music System, Audio Mixer
- **MAP** – Lighting, Level Geometry, Navigation, Triggers
- **ENVIRONMENT** – Props, Foliage, Architecture, Detail Meshes
- **FX** – Particles, Post Processing, Decals, Visual Effects

### Options
- **Create Scene Root** – Creates a root parent object for the entire structure
- **Dashed Group Names** – Formats group names with dashes for visual clarity
- **Select Created Objects** – Automatically selects all created objects after generation

---

## Customization

The tool can be customized by modifying the source code:

### Adding/Editing Groups
Edit the `_groupPrefKeys` dictionary in `SceneBootstrapWindow.cs` to add new groups.

### Modifying Child Objects
Edit the `GetChildNames()` method to change the default child objects for each group.

### Changing Title Formatting
Adjust the `TargetTitleLength` constant to change the width of dashed titles.

**Note:** User preferences are stored using `EditorPrefs` and are per-user, not per-project.

---

## Technical Details

- **Editor-Only** – All code is in `Editor` folder, no runtime impact
- **No Dependencies** – Uses only Unity's built-in APIs
- **Lightweight** – Creates only empty GameObjects
- **Undo Support** – All operations are registered with Unity's Undo system
- **Error Prevention** – Checks for existing objects before creation

---

## Contributing

This is a simple utility tool designed to be easily customizable. Feel free to modify it to suit your project's needs. Pull requests are welcome for bug fixes and minor enhancements.

---

## License

MIT License. See the `LICENSE` file for details.

---

## Support

For issues or questions:
1. Check the source code comments for documentation
2. Ensure you're using a supported Unity version
3. Verify the script is in an `Editor` folder

---

**Note:** This tool creates organizational structures only. It does not enforce any architectural patterns or add components to GameObjects.
