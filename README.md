# Scene Bootstrap for Unity

Scene Bootstrap is a lightweight Unity Editor tool that automates the creation of organized scene hierarchy structures. It removes the repetitive setup work involved in creating empty GameObjects for new scenes while remaining flexible and non-opinionated.

The tool is editor-only, adds no runtime dependencies, and creates only empty GameObjects.

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

## Installation

### Method 1: Unity Package (Recommended)
1. Download the `SceneBootstrap.unitypackage`
2. In Unity: `Assets > Import Package > Custom Package`
3. Select the package and import all files

### Method 2: Manual Installation
Copy the `SceneBootstrap` folder into your Unity project's `Assets` directory:

```
Assets/
└── SceneBootstrap/
└── Editor/
└── SceneBootstrapWindow.cs
```

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

## Requirements

- **Unity 2020.3 LTS or newer**
- No additional packages required

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