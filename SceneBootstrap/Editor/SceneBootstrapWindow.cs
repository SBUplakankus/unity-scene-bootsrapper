using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SceneBootstrap.Editor
{
    /// <summary>
    /// Scene Bootstrap Window - A Unity Editor tool for quickly creating scene hierarchy structures.
    /// This tool helps developers scaffold organized scene hierarchies with consistent naming patterns.
    /// Preferences are saved in Unity's EditorPrefs for persistence.
    /// </summary>
    public class SceneBootstrapWindow : EditorWindow
    {
        #region Constants and Static Fields
    
        /// <summary>EditorPrefs key for the root object toggle</summary>
        private const string RootPrefKey = "SB_Root";
    
        /// <summary>EditorPrefs key for the dashed names toggle</summary>
        private const string DashedPrefKey = "SB_Dashed";
    
        /// <summary>EditorPrefs key for the select created toggle</summary>
        private const string SelectPrefKey = "SB_Select";
    
        /// <summary>Target total character length for justified titles</summary>
        private const int TargetTitleLength = 24;
    
        #endregion
    
        #region Private Fields
    
        /// <summary>Dictionary mapping group names to their EditorPrefs keys</summary>
        private readonly Dictionary<string, string> _groupPrefKeys = new()
        {
            { "PLAYER", "SB_Player" },
            { "GAMEPLAY", "SB_Gameplay" },
            { "MAP", "SB_Map" },
            { "ENVIRONMENT", "SB_Environment" },
            { "UI", "SB_UI" },
            { "FX", "SB_FX" },
            { "AUDIO", "SB_Audio" },
            { "SYSTEMS", "SB_Systems" },
        };
    
        /// <summary>Stores which groups are enabled for creation</summary>
        private readonly Dictionary<string, bool> _enabledGroups = new();
    
        /// <summary>Stores which groups should have children created</summary>
        private readonly Dictionary<string, bool> _enabledChildren = new();
    
        /// <summary>Whether to create a root object for the scene</summary>
        private bool _shouldCreateRoot;
    
        /// <summary>Whether to use dashed formatting for group names</summary>
        private bool _useDashedNames;
    
        /// <summary>Whether to select created objects after generation</summary>
        private bool _shouldSelectCreated;
    
        #endregion
    
        #region Editor Window Methods
    
        /// <summary>
        /// Opens the Scene Bootstrap window from the Unity menu
        /// </summary>
        [MenuItem("Tools/Scene Bootstrap")]
        public static void Open()
        {
            GetWindow<SceneBootstrapWindow>("Scene Bootstrap");
        }
    
        /// <summary>
        /// Initializes window state and loads saved preferences
        /// </summary>
        private void OnEnable()
        {
            LoadPreferences();
        }
    
        /// <summary>
        /// Renders the window GUI
        /// </summary>
        private void OnGUI()
        {
            DrawHeader();
            DrawGroupToggles();
            DrawOptions();
            DrawCreateButton();
        }
    
        #endregion
    
        #region GUI Drawing Methods
    
        /// <summary>
        /// Draws the window header section
        /// </summary>
        private void DrawHeader()
        {
            GUILayout.Label("Scene Bootstrap", EditorStyles.boldLabel);
            GUILayout.Label("Create organized scene hierarchy structures", EditorStyles.helpBox);
            GUILayout.Space(10);
        }
    
        /// <summary>
        /// Draws the group toggle section with child toggles
        /// </summary>
        private void DrawGroupToggles()
        {
            GUILayout.Label("Core Groups", EditorStyles.boldLabel);
        
            // Use cached group keys to avoid allocations
            foreach (var groupName in _groupPrefKeys.Keys)
            {
                DrawGroupToggle(groupName);
            }
        }
    
        /// <summary>
        /// Draws a single group toggle row with child toggle
        /// </summary>
        /// <param name="groupName">Name of the group to draw</param>
        private void DrawGroupToggle(string groupName)
        {
            EditorGUILayout.BeginHorizontal();
            {
                // Main group toggle
                _enabledGroups[groupName] = EditorGUILayout.Toggle(groupName, _enabledGroups[groupName]);
            
                // Child toggle (enabled only if group is enabled)
                using (new EditorGUI.DisabledScope(!_enabledGroups[groupName]))
                {
                    _enabledChildren[groupName] = EditorGUILayout.ToggleLeft(
                        "Add children", 
                        _enabledChildren[groupName], 
                        GUILayout.Width(110)
                    );
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    
        /// <summary>
        /// Draws the options section (root, dashed names, selection)
        /// </summary>
        private void DrawOptions()
        {
            GUILayout.Space(8);
            GUILayout.Label("Options", EditorStyles.boldLabel);
        
            _shouldCreateRoot = EditorGUILayout.Toggle("Create Scene Root", _shouldCreateRoot);
            _useDashedNames = EditorGUILayout.Toggle("Dashed Group Names", _useDashedNames);
            _shouldSelectCreated = EditorGUILayout.Toggle("Select Created Objects", _shouldSelectCreated);
        }
    
        /// <summary>
        /// Draws the main create button
        /// </summary>
        private void DrawCreateButton()
        {
            GUILayout.Space(14);

            if (!GUILayout.Button("Create Scene Structure", GUILayout.Height(32))) return;
            SavePreferences();
            CreateSceneStructure();
        }
    
        #endregion
    
        #region Scene Creation Methods
    
        /// <summary>
        /// Creates the complete scene structure based on current settings
        /// </summary>
        private void CreateSceneStructure()
        {
            Transform rootTransform = null;
            var createdObjects = new List<GameObject>();
        
            // Create root object if enabled
            if (_shouldCreateRoot)
            {
                rootTransform = CreateGameObject(FormatRootName(), null).transform;
                createdObjects.Add(rootTransform.gameObject);
            }
        
            // Get active groups for justification calculation
            var activeGroups = GetActiveGroups();
        
            // Create each enabled group
            foreach (var groupName in activeGroups)
            {
                CreateGroup(
                    groupName, 
                    rootTransform, 
                    _enabledChildren[groupName] ? GetChildNames(groupName) : null, 
                    createdObjects, 
                    activeGroups
                );
            }
        
            // Select created objects if enabled - FIXED: Convert to Object[] to avoid covariant issues
            if (_shouldSelectCreated && createdObjects.Count > 0)
            {
                Selection.objects = createdObjects.ConvertAll(go => (Object)go).ToArray();
            }
        }

        /// <summary>
        /// Gets the list of active groups based on user selection
        /// </summary>
        private List<string> GetActiveGroups()
        {
            var activeGroups = new List<string>();
            
            foreach (var groupName in _groupPrefKeys.Keys)
            {
                if (_enabledGroups[groupName])
                {
                    activeGroups.Add(groupName);
                }
            }
            
            return activeGroups;
        }
    
        /// <summary>
        /// Creates a group GameObject with optional children
        /// </summary>
        /// <param name="groupName">Name of the group to create</param>
        /// <param name="parent">Parent transform (null for root-level)</param>
        /// <param name="childNames">Array of child names to create (null for no children)</param>
        /// <param name="createdObjects">List to track created objects</param>
        /// <param name="activeGroups">List of all active groups for justification</param>
        private void CreateGroup(
            string groupName, 
            Transform parent, 
            string[] childNames, 
            List<GameObject> createdObjects,
            List<string> activeGroups)
        {
            var groupObject = CreateGameObject(FormatGroupName(groupName, activeGroups), parent);
            if (groupObject == null) return;
        
            createdObjects.Add(groupObject);
        
            // Create children if specified
            if (childNames == null || childNames.Length == 0) return;
        
            foreach (var childName in childNames)
            {
                var childObject = CreateGameObject(childName, groupObject.transform);
                if (childObject != null)
                {
                    createdObjects.Add(childObject);
                }
            }
        }
    
        /// <summary>
        /// Creates a GameObject with the specified name and parent
        /// </summary>
        /// <param name="objectName">Name of the GameObject</param>
        /// <param name="parent">Parent transform (null for no parent)</param>
        /// <returns>The created GameObject, or existing one if found</returns>
        private GameObject CreateGameObject(string objectName, Transform parent)
        {
            // Check for existing object first
            var existingObject = GameObject.Find(objectName);
            if (existingObject != null) return existingObject;
        
            var gameObject = new GameObject(objectName);
            Undo.RegisterCreatedObjectUndo(gameObject, $"Create {objectName}");
        
            if (parent != null)
            {
                gameObject.transform.SetParent(parent);
            }
        
            return gameObject;
        }
    
        #endregion
    
        #region Name Formatting Methods
    
        /// <summary>
        /// Formats the root object name with dashes if enabled
        /// </summary>
        /// <returns>Formatted root name</returns>
        private string FormatRootName()
        {
            return !_useDashedNames ? "SCENE ROOT" : CreateDashedName("SCENE ROOT", TargetTitleLength);
        }
    
        /// <summary>
        /// Formats a group name with justified dashes if enabled
        /// </summary>
        /// <param name="groupName">Original group name</param>
        /// <param name="activeGroups">List of active groups for length calculation</param>
        /// <returns>Formatted group name</returns>
        private string FormatGroupName(string groupName, List<string> activeGroups)
        {
            if (!_useDashedNames) return groupName;
        
            // Use the longest active group name for consistent formatting
            var maxLength = 0;
            foreach (var group in activeGroups)
            {
                if (group.Length > maxLength)
                {
                    maxLength = group.Length;
                }
            }
            
            var targetLength = Mathf.Max(TargetTitleLength, maxLength + 4); // Ensure minimum padding
        
            return CreateDashedName(groupName, targetLength);
        }
    
        /// <summary>
        /// Creates a centered, dashed name with the target length
        /// </summary>
        /// <param name="name">Base name to center</param>
        /// <param name="targetLength">Total target length including dashes</param>
        /// <returns>Formatted dashed name</returns>
        private static string CreateDashedName(string name, int targetLength)
        {
            var totalDashesNeeded = targetLength - name.Length;
            if (totalDashesNeeded <= 0) return name;
        
            var leftDashes = totalDashesNeeded / 2;
            var rightDashes = totalDashesNeeded - leftDashes;
        
            return $"{new string('-', leftDashes)} {name} {new string('-', rightDashes)}";
        }
    
        #endregion
    
        #region Child Definitions
    
        /// <summary>
        /// Gets the default child names for a given group
        /// </summary>
        /// <param name="groupName">Name of the parent group</param>
        /// <returns>Array of child names, or null if group has no children</returns>
        private static string[] GetChildNames(string groupName)
        {
            return groupName switch
            {
                "PLAYER" => new[] { "Camera", "Player Controller", "Character Model" },
                "GAMEPLAY" => new[] { "Interactables", "Enemies", "Pickups", "Spawn Points" },
                "MAP" => new[] { "Lighting", "Level Geometry", "Navigation", "Triggers" },
                "ENVIRONMENT" => new[] { "Props", "Foliage", "Architecture", "Detail Meshes" },
                "UI" => new[] { "Overlay Canvas", "World Canvases", "Event System", "UI Managers" },
                "FX" => new[] { "Particles", "Post Processing", "Decals", "Visual Effects" },
                "AUDIO" => new[] { "Emitters", "Ambient Zones", "Music System", "Audio Mixer" },
                "SYSTEMS" => new[] { "Audio Controller", "UI Controller", "Save Controller", "Game Manager" },
                _ => null
            };
        }
    
        #endregion
    
        #region Preferences Management
    
        /// <summary>
        /// Loads all saved preferences from EditorPrefs
        /// </summary>
        private void LoadPreferences()
        {
            _shouldCreateRoot = EditorPrefs.GetBool(RootPrefKey, true);
            _useDashedNames = EditorPrefs.GetBool(DashedPrefKey, true);
            _shouldSelectCreated = EditorPrefs.GetBool(SelectPrefKey, true);
        
            // Initialize dictionaries with saved values
            foreach (var kvp in _groupPrefKeys)
            {
                _enabledGroups[kvp.Key] = EditorPrefs.GetBool(kvp.Value, true);
                _enabledChildren[kvp.Key] = EditorPrefs.GetBool($"{kvp.Value}_Children", true);
            }
        }
    
        /// <summary>
        /// Saves all current preferences to EditorPrefs
        /// </summary>
        private void SavePreferences()
        {
            EditorPrefs.SetBool(RootPrefKey, _shouldCreateRoot);
            EditorPrefs.SetBool(DashedPrefKey, _useDashedNames);
            EditorPrefs.SetBool(SelectPrefKey, _shouldSelectCreated);
        
            // Save group states
            foreach (var kvp in _groupPrefKeys)
            {
                EditorPrefs.SetBool(kvp.Value, _enabledGroups[kvp.Key]);
                EditorPrefs.SetBool($"{kvp.Value}_Children", _enabledChildren[kvp.Key]);
            }
        }
    
        #endregion
    }
}