using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SceneBootstrap.Editor
{
    public class SceneBootstrapWindow : EditorWindow
    {
        private const string RootPrefKey = "SB_Root";
        private const string DashedPrefKey = "SB_Dashed";
        private const string SelectPrefKey = "SB_Select";
        private const int TargetTitleLength = 24;

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

        private readonly Dictionary<string, bool> _enabledGroups = new();
        private readonly Dictionary<string, bool> _enabledChildren = new();
        private bool _shouldCreateRoot;
        private bool _useDashedNames;
        private bool _shouldSelectCreated;

        [MenuItem("Tools/Scene Bootstrap")]
        public static void Open()
        {
            GetWindow<SceneBootstrapWindow>("Scene Bootstrap");
        }

        private void OnEnable()
        {
            LoadPreferences();
        }

        private void OnGUI()
        {
            DrawHeader();
            DrawGroupToggles();
            DrawOptions();
            DrawCreateButton();
        }

        private void DrawHeader()
        {
            GUILayout.Label("Scene Bootstrap", EditorStyles.boldLabel);
            GUILayout.Label("Create organized scene hierarchy structures", EditorStyles.helpBox);
            GUILayout.Space(10);
        }

        private void DrawGroupToggles()
        {
            GUILayout.Label("Core Groups", EditorStyles.boldLabel);

            foreach (var groupName in _groupPrefKeys.Keys)
            {
                DrawGroupToggle(groupName);
            }
        }

        private void DrawGroupToggle(string groupName)
        {
            EditorGUILayout.BeginHorizontal();
            {
                _enabledGroups[groupName] = EditorGUILayout.Toggle(groupName, _enabledGroups[groupName]);

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

        private void DrawOptions()
        {
            GUILayout.Space(8);
            GUILayout.Label("Options", EditorStyles.boldLabel);

            _shouldCreateRoot = EditorGUILayout.Toggle("Create Scene Root", _shouldCreateRoot);
            _useDashedNames = EditorGUILayout.Toggle("Dashed Group Names", _useDashedNames);
            _shouldSelectCreated = EditorGUILayout.Toggle("Select Created Objects", _shouldSelectCreated);
        }

        private void DrawCreateButton()
        {
            GUILayout.Space(14);
            if (!GUILayout.Button("Create Scene Structure", GUILayout.Height(32))) return;
            SavePreferences();
            CreateSceneStructure();
        }

        private void CreateSceneStructure()
        {
            Transform rootTransform = null;
            var createdObjects = new List<GameObject>();

            if (_shouldCreateRoot)
            {
                rootTransform = CreateGameObject(FormatRootName(), null).transform;
                createdObjects.Add(rootTransform.gameObject);
            }

            var activeGroups = GetActiveGroups();

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

            if (_shouldSelectCreated && createdObjects.Count > 0)
            {
                Selection.objects = createdObjects.ConvertAll(go => (Object)go).ToArray();
            }
        }

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

        private GameObject CreateGameObject(string objectName, Transform parent)
        {
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

        private string FormatRootName()
        {
            // Ensure root name length matches the expected dashed format if dashing is on
            int targetLength = _useDashedNames ? TargetTitleLength : 0;
            if (_useDashedNames)
            {
                return CreateDashedName("SCENE ROOT", targetLength);
            }
            return "SCENE ROOT"; // Fixed length for non-dashed root
        }

        private string FormatGroupName(string groupName, List<string> activeGroups)
        {
            if (!_useDashedNames) return groupName;

            var maxLength = 0;
            foreach (var group in activeGroups)
            {
                if (group.Length > maxLength)
                {
                    maxLength = group.Length;
                }
            }

            // Use the maximum of TargetTitleLength or the calculated required length for consistency
            var targetLength = Mathf.Max(TargetTitleLength, maxLength + 4); 
            return CreateDashedName(groupName, targetLength);
        }

        private static string CreateDashedName(string name, int targetLength)
        {
            var totalDashesNeeded = targetLength - name.Length;
            if (totalDashesNeeded <= 0) return name;

            var leftDashes = totalDashesNeeded / 2;
            var rightDashes = totalDashesNeeded - leftDashes;

            return $"{new string('-', leftDashes)} {name} {new string('-', rightDashes)}";
        }

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

        private void LoadPreferences()
        {
            _shouldCreateRoot = EditorPrefs.GetBool(RootPrefKey, true);
            _useDashedNames = EditorPrefs.GetBool(DashedPrefKey, true);
            _shouldSelectCreated = EditorPrefs.GetBool(SelectPrefKey, true);

            foreach (var kvp in _groupPrefKeys)
            {
                _enabledGroups[kvp.Key] = EditorPrefs.GetBool(kvp.Value, true);
                _enabledChildren[kvp.Key] = EditorPrefs.GetBool($"{kvp.Value}_Children", true);
            }
        }

        private void SavePreferences()
        {
            EditorPrefs.SetBool(RootPrefKey, _shouldCreateRoot);
            EditorPrefs.SetBool(DashedPrefKey, _useDashedNames);
            EditorPrefs.SetBool(SelectPrefKey, _shouldSelectCreated);

            foreach (var kvp in _groupPrefKeys)
            {
                EditorPrefs.SetBool(kvp.Value, _enabledGroups[kvp.Key]);
                EditorPrefs.SetBool($"{kvp.Value}_Children", _enabledChildren[kvp.Key]);
            }
        }
    }
}
